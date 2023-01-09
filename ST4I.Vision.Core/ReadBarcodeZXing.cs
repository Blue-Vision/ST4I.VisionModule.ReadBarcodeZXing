using System.Collections.Generic;
using OpenCvSharp;
using ZXing;
using ZXing.Common;
using static ZXing.RGBLuminanceSource;
using ZXing.Multi;
using ZXing.OneD;
using System.Linq;
using System;

namespace ST4I.Vision.Core
{
    /// <summary>
    /// Module đọc barcode
    /// </summary>
    public class ReadBarcodeZXing : IVisionModule
    {
        /// <summary>
        /// Cài đặt kiểu barcode
        /// </summary>
        public class BarcodeTypeConfig
        {
            private bool isEnable;
            private ST4I.Vision.Core.BarcodeType barCodeType;
            /// <summary>
            /// Sử dụng loại barocde
            /// </summary>
            public bool IsEnable
            {
                get { return isEnable; }
                set { isEnable = value; }
            }
            /// <summary>
            /// Kiểu barcode
            /// </summary>
            public ST4I.Vision.Core.BarcodeType BarCodeType
            {
                get { return barCodeType; }
                set { barCodeType = value; }
            }
        }
        /// <summary>
        /// Số lượng đối tượng cần tìm thấy, cài đặt thông số này giúp quá trình tìm kiếm có thể kết thúc sớm hơn, mặc định bằng 1
        /// </summary>
        public int NumOfBarcodeToRead { get; set; } = 1;
        /// <summary>
        /// Các kiểu barcode hỗ trợ
        /// </summary>

        public readonly ST4I.Vision.Core.BarcodeType[] BarcodeSupportedType = new ST4I.Vision.Core.BarcodeType[] {
            ST4I.Vision.Core.BarcodeType.Codabar,
            ST4I.Vision.Core.BarcodeType.Code39,
            ST4I.Vision.Core.BarcodeType.Code93,
            ST4I.Vision.Core.BarcodeType.Code128,
            ST4I.Vision.Core.BarcodeType.Ean13,
            ST4I.Vision.Core.BarcodeType.Ean8,
            ST4I.Vision.Core.BarcodeType.Pharmacode,
            ST4I.Vision.Core.BarcodeType.UpcA,
            ST4I.Vision.Core.BarcodeType.UpcE,
            ST4I.Vision.Core.BarcodeType.I2Of5,
            ST4I.Vision.Core.BarcodeType.Msi,
            ST4I.Vision.Core.BarcodeType.RssExpanded
        };
        /// <summary>
        /// <inheritdoc cref="IVisionModule.ModuleType"/>
        /// </summary>
        public VisionModuleType ModuleType { get { return VisionModuleType.ReadBarcode; } }
        /// <summary>
        /// Vùng đo đạc
        /// </summary>
        public IRoi SearchingRoi { get; set; }
        /// <summary>
        /// So sánh nội dung không ?
        /// </summary>
        public bool IsEnableCompareContent { get; set; }
        /// <summary>
        /// Nội dung compare
        /// </summary>
        public string TemplateContent { get; set; }
        /// <summary>
        /// Kiểm tra có chứa chuỗi con không ?
        /// </summary>
        public bool IsEnableCodeContains { get; set; }
        /// <summary>
        /// Chọn hình ảnh đơn sắc thuần túy ?
        /// </summary>
        public bool IsEnablePureMonochromeImage { get; set; } = false;
        /// <summary>
        /// Chọn tối ưu độ chính xác ?
        /// </summary>
        public bool IsEnableOptimizeForAccuracy { get; set; } = false;
        /// <summary>
        /// Chọn hình ảnh đảo ngược ?
        /// </summary>
        public bool IsEnableInvertedImage { get; set; } = false;
        /// <summary>
        /// Nội dung chuỗi string con
        /// </summary>
        public string TemplateCodeContains { get; set; }
        /// <summary>
        /// Danh sách cài đặt các loại barcode
        /// </summary>
        public List<BarcodeTypeConfig> SelectedTypeBarcodes { get; set; }
        /// <summary>
        /// <inheritdoc cref="ReadBarcodeZXing"/>
        /// </summary>
        public ReadBarcodeZXing()
        {
            SelectedTypeBarcodes = GenerateListTypeBarcode();
        }
        /// <summary>
        /// Đọc barcode của ảnh
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public BarcodeResult Read(Mat image)
        {
            return Read(image, SearchingRoi);
        }
        /// <summary>
        /// Đọc barcode của ảnh
        /// </summary>
        /// <param name="image">Ảnh đầu vào</param>
        /// <param name="roi">Vùng ROI tìm kiếm</param>
        /// <returns></returns>
        public BarcodeResult Read(Mat image, IRoi roi)
        {
            
            var searchingRoi = roi != null ? roi : new RectangleRoi() { X = 0, Y = 0, Angle = 0, Height = image.Height, Width = image.Width };
            Rect rect = searchingRoi.MinRectangle();
            BarcodeResult barcodeResult = new BarcodeResult()
            {
                ProcessedRoi = searchingRoi,
            };
            
            bool needDispose = false;
            Mat imgGray = new Mat();
            if (roi == null)
            {
                if (image.Channels() > 1)
                {
                    Cv2.CvtColor(image, imgGray, ColorConversionCodes.BGR2GRAY);
                    needDispose = true;
                }
                else
                {
                    imgGray = image;
                }
            }
            else
            {
                Mat img = ImageUtils.ExtractImageByRoi(image, searchingRoi);
                if (image.Channels() > 1)
                {
                    Cv2.CvtColor(img, imgGray, ColorConversionCodes.BGR2GRAY);
                }
                else
                {
                    imgGray = img;
                }
                needDispose = true;
            }
            imgGray.GetArray(out byte[] data);
            LuminanceSource source = new RGBLuminanceSource(data, imgGray.Width, imgGray.Height, BitmapFormat.Gray8);
            BinaryBitmap binaryBitmap = new BinaryBitmap(new HybridBinarizer(source));
            var hints = new Dictionary<DecodeHintType, object>{};
            if (IsEnableOptimizeForAccuracy)
            {
                hints.Add(DecodeHintType.TRY_HARDER, true );
            }
            if (IsEnablePureMonochromeImage)
            {
                hints.Add(DecodeHintType.PURE_BARCODE, true);
            }
            if (IsEnableInvertedImage)
            {
                hints.Add(DecodeHintType.ALSO_INVERTED, true);
            }
            List<BarcodeFormat> formats = new List<BarcodeFormat>{};
            foreach (var item in SelectedTypeBarcodes)
            {
                if (item.IsEnable)
                {
                    formats.Add(ConvertToNIBarcodeType(item.BarCodeType));
                }
            }
            if (formats.Count == Enum.GetNames(typeof(BarcodeType)).Length - 1)
            {
                formats = new List<BarcodeFormat>{BarcodeFormat.All_1D};
            }
            hints.Add(DecodeHintType.POSSIBLE_FORMATS, formats);
            MultiFormatOneDReader multiFormatOneDReader = new MultiFormatOneDReader(hints);
            GenericMultipleBarcodeReader genericMultipleBarcodeReader = new GenericMultipleBarcodeReader(multiFormatOneDReader);
            Result[] results = genericMultipleBarcodeReader.decodeMultiple(binaryBitmap, hints);
            barcodeResult.BarcodeItems = new List<BarcodeReport>();
            if (results != null)
            {
                int numOfbarcode = NumOfBarcodeToRead < results.Count()? NumOfBarcodeToRead: results.Count();
                for (int i = 0; i < numOfbarcode; i++)
                {
                    BarcodeReport result = new BarcodeReport();
                    List<Point2d> points = new List<Point2d>();
                    foreach (var p in results[i].ResultPoints)
                    {
                        points.Add(new Point2d((int)p.X + rect.X, (int)p.Y + rect.Y));
                        points.Add(new Point2d((int)p.X + rect.X, (int)p.Y + rect.Y));
                    }
                    result.Vertices = points.ToArray();
                    result.Content = results[i].Text;
                    result.Type = ConvertToStandardBarcodeType(results[i].BarcodeFormat);
                    barcodeResult.BarcodeItems.Add(result);
                }
            }
            barcodeResult.Status = VerifyResult(barcodeResult);
            if (needDispose)
            {
                imgGray.Dispose();
            }
            return barcodeResult;
        }
        /// <summary>
        /// Hàm kiểm tra compare
        /// </summary>
        /// <param name="listResult"></param>
        /// <returns></returns>
        public bool VerifyResult(BarcodeResult listResult)
        {
            bool check = true;
            if (listResult == null)
            {
                return false;
            }
            if (listResult.BarcodeItems != null)
            {
                if (listResult.BarcodeItems.Count == 0)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            bool compareContent = true;
            if (IsEnableCompareContent)
            {
                foreach (var item in listResult.BarcodeItems)
                {
                    if (item.Content != TemplateContent)
                    {
                        compareContent = false;
                    }
                }
            }
            bool codeContain = true;
            if (IsEnableCodeContains)
            {
                if (TemplateCodeContains != null)
                {
                    foreach (var item in listResult.BarcodeItems)
                    {
                        if (!item.Content.Contains(TemplateCodeContains))
                        {
                            codeContain = false;
                        }
                    }
                }
            }
            if (compareContent && codeContain)
            {
                check = true;
            }
            else
            {
                check = false;
            }
            return check;
        }
        /// <summary>
        /// Chuyển đổi loại barcode tiêu chuẩn sang barcode của NI
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private BarcodeFormat ConvertToNIBarcodeType(ST4I.Vision.Core.BarcodeType type)
        {
            switch (type)
            {
                case BarcodeType.Codabar:
                    return BarcodeFormat.CODABAR;
                case BarcodeType.Code39:
                    return BarcodeFormat.CODE_39;
                case BarcodeType.Code93:
                    return BarcodeFormat.CODE_93;
                case BarcodeType.Code128:
                    return BarcodeFormat.CODE_128;
                case BarcodeType.Ean8:
                    return BarcodeFormat.EAN_8;
                case BarcodeType.Ean13:
                    return BarcodeFormat.EAN_13;
                case BarcodeType.I2Of5:
                    return BarcodeFormat.ITF;
                case BarcodeType.Msi:
                    return BarcodeFormat.MSI;
                case BarcodeType.UpcA:
                    return BarcodeFormat.UPC_A;
                case BarcodeType.UpcE:
                    return BarcodeFormat.UPC_E;
                case BarcodeType.Pharmacode:
                    return BarcodeFormat.PHARMA_CODE;
                case BarcodeType.RssExpanded:
                    return BarcodeFormat.RSS_EXPANDED;
                case BarcodeType.Unknown:
                    return BarcodeFormat.All_1D;
                default:
                    return BarcodeFormat.All_1D;
            }
        }
        /// <summary>
        /// Chuyển đổi loại barcode của NI sang loại barcode tiêu chuẩn
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private ST4I.Vision.Core.BarcodeType ConvertToStandardBarcodeType(BarcodeFormat type)
        {
            switch (type)
            {

                case BarcodeFormat.CODABAR:
                    return ST4I.Vision.Core.BarcodeType.Codabar;
                case BarcodeFormat.CODE_39:
                    return ST4I.Vision.Core.BarcodeType.Code39;
                case BarcodeFormat.CODE_93:
                    return ST4I.Vision.Core.BarcodeType.Code93;
                case BarcodeFormat.CODE_128:
                    return ST4I.Vision.Core.BarcodeType.Code128;
                case BarcodeFormat.EAN_8:
                    return ST4I.Vision.Core.BarcodeType.Ean8;
                case BarcodeFormat.EAN_13:
                    return ST4I.Vision.Core.BarcodeType.Ean13;
                case BarcodeFormat.ITF:
                    return ST4I.Vision.Core.BarcodeType.I2Of5;
                case BarcodeFormat.MSI:
                    return ST4I.Vision.Core.BarcodeType.Msi;
                case BarcodeFormat.UPC_A:
                    return ST4I.Vision.Core.BarcodeType.UpcA;
                case BarcodeFormat.UPC_E:
                    return ST4I.Vision.Core.BarcodeType.UpcE;
                case BarcodeFormat.PHARMA_CODE:
                    return ST4I.Vision.Core.BarcodeType.Pharmacode;
                case BarcodeFormat.RSS_EXPANDED:
                    return ST4I.Vision.Core.BarcodeType.RssExpanded;
                default:
                    return ST4I.Vision.Core.BarcodeType.Unknown;
            }
        }
        /// <summary>
        /// Tạo ra danh sách cài đặt barcode
        /// </summary>
        /// <returns></returns>
        private List<BarcodeTypeConfig> GenerateListTypeBarcode()
        {
            var listTypeBarcodes = new List<BarcodeTypeConfig>();
            foreach (var item in BarcodeSupportedType)
            {
                listTypeBarcodes.Add(new BarcodeTypeConfig() { BarCodeType = item, IsEnable = true });
            }
            return listTypeBarcodes;
        }
    }
}
