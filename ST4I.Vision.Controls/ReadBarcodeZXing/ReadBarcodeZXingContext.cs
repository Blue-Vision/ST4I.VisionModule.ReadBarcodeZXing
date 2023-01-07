using System.Collections.ObjectModel;
using System.Diagnostics;
using ST4I.Vision.Core;
using ST4I.Vision.Component;

namespace ST4I.Vision.Controls
{
    /// <summary>
    /// DataContext cho module ReadBarcodeZXing
    /// </summary>
    public class ReadBarcodeZXingContext : VisionModuleAdvancedContext
    {
        /// <summary>
        /// Kết quả đọc barcode hiển thị lên giao diện
        /// </summary>
        public class ResultReportView
        {
            private int index;
            /// <summary>
            /// Thứ tự
            /// </summary>
            public int Index
            {
                get { return index; }
                set { index = value; }
            }
            private string content;
            /// <summary>
            /// Nội dung barcode
            /// </summary>
            public string Content
            {
                get { return content; }
                set { content = value; }
            }
            private BarcodeType barCodeType;
            /// <summary>
            /// Kiểu barcode
            /// </summary>
            public BarcodeType BarCodeType
            {
                get { return barCodeType; }
                set { barCodeType = value; }
            }
        }

        public static string LANGUAGE_PATH_RESOURCE = "ReadBarcodeZXing/Language/Language.xaml";
        private ReadBarcodeZXingNode nodeLink = new ReadBarcodeZXingNode();
        private DisplayOverlaySettingContext overlaySettingContext;
        /// <summary>
        /// Tool đọc barcode
        /// </summary>
        public ReadBarcodeZXing VisionModule
        {
            get
            {
                return nodeLink.VisionModule;
            }
        }
        /// <summary>
        /// Giao diện cài đặt cho overlay setting
        /// </summary>
        public DisplayOverlaySettingContext OverlaySettingContext
        {
            get
            {
                return overlaySettingContext;
            }
            set
            {
                overlaySettingContext = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Chọn hình ảnh đơn sắc thuần túy ?
        /// </summary>
        public bool IsEnablePureMonochromeImage
        {
            get { return VisionModule.IsEnablePureMonochromeImage; }
            set
            {
                VisionModule.IsEnablePureMonochromeImage = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Chọn tối ưu độ chính xác ?
        /// </summary>
        public bool IsEnableOptimizeForAccuracy
        {
            get { return VisionModule.IsEnableOptimizeForAccuracy; }
            set
            {
                VisionModule.IsEnableOptimizeForAccuracy = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Chọn ảnh đảo ngược ?
        /// </summary>
        public bool IsEnableInvertedImage
        {
            get { return VisionModule.IsEnableInvertedImage; }
            set
            {
                VisionModule.IsEnableInvertedImage = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// <inheritdoc cref="BaseModuleContext.NodeLink"/>
        /// </summary>
        public override INode NodeLink
        {
            get
            {
                return nodeLink;
            }
            set
            {
                if (value is ReadBarcodeZXingNode)
                {
                    nodeLink?.Dispose();
                    nodeLink = (ReadBarcodeZXingNode)value;
                    OverlaySettingContext = new DisplayOverlaySettingContext(nodeLink.OverlaySetting);
                    OnPropertyChanged("RoiSelectedType");
                    OnPropertyChanged("VisionModule");
                }
            }
        }
        /// <summary>
        /// <inheritdoc cref="VisionModuleAdvancedContext.RoiSelectedType"/>
        /// </summary>
        public override RoiSetupMode RoiSelectedType
        {
            get => base.RoiSelectedType;
            set
            {
                base.RoiSelectedType = value;
                if (RoiSelectedType == RoiSetupMode.FullImage)
                {
                    VisionModule.SearchingRoi = null;
                }
            }
        }
        /// <summary>
        /// Kết quả tìm kiếm
        /// </summary>
        public ObservableCollection<ResultReportView> BarcodeResultViews { get; set; } = new ObservableCollection<ResultReportView>();
        /// <summary>
        /// Danh sách loại barcode chọn để thực hiện đọc
        /// </summary>
        public ObservableCollection<ReadBarcodeZXing.BarcodeTypeConfig> SelectedTypeBarcodes
        {
            get
            {
                return new ObservableCollection<ReadBarcodeZXing.BarcodeTypeConfig>(VisionModule.SelectedTypeBarcodes);
            }
        }
        /// <summary>
        /// Kiểm tra nội dung có chứa chuỗi con không ?
        /// </summary>
        public bool IsEnableCodeContains
        {
            get
            {
                return VisionModule.IsEnableCodeContains;
            }
            set
            {
                VisionModule.IsEnableCodeContains = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Nội dung chuỗi con
        /// </summary>
        public string TemplateCodeContains
        {
            get
            {
                return VisionModule.TemplateCodeContains;
            }
            set
            {
                VisionModule.TemplateCodeContains = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// So sánh với nội dung có sẵn không ?
        /// </summary>
        public bool IsEnableCompareContent
        {
            get
            {
                return VisionModule.IsEnableCompareContent;
            }
            set
            {
                VisionModule.IsEnableCompareContent = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Nội dung so sánh
        /// </summary>
        public string TemplateContent
        {
            get
            {
                return VisionModule.TemplateContent;
            }
            set
            {
                VisionModule.TemplateContent = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Lệnh chọn tất cả
        /// </summary>
        public RelayCommand SelectAllBarcodeTypeCommand { get; set; }
        /// <summary>
        /// <inheritdoc cref="VisionModuleContext.RoiTypeSupported"/>
        /// </summary>
        public override RoiType[] RoiTypeSupported => new RoiType[] { RoiType.Rectangle, RoiType.RotatedRectangle};
        /// <summary>
        /// <inheritdoc cref="ReadBarcodeZXingContext"/>
        /// </summary>
        public ReadBarcodeZXingContext() : base()
        {
            SelectAllBarcodeTypeCommand = new RelayCommand(OnSelectAllBarcodeTypeCommand);
        }
        /// <summary>
        /// <inheritdoc cref="VisionModuleContext.OnRoiAdded(object, IRoi)"/>
        /// </summary>
        /// <param name="o"></param>
        /// <param name="roi"></param>
        public override void OnRoiAdded(object o, IRoi roi)
        {
            VisionModule.SearchingRoi = roi;
            AutoExcuteModule();
        }
        /// <summary>
        /// <inheritdoc cref="VisionModuleContext.OnRoiChanged(object, IRoi)"/>
        /// </summary>
        /// <param name="o"></param>
        /// <param name="roi"></param>
        public override void OnRoiChanged(object o, IRoi roi)
        {
            VisionModule.SearchingRoi = roi;
            AutoExcuteModule();
        }
        /// <summary>
        /// <inheritdoc cref="VisionModuleContext.OnRoiDeleted(object, IRoi)"/>
        /// </summary>
        /// <param name="o"></param>
        /// <param name="roi"></param>
        public override void OnRoiDeleted(object o, IRoi roi)
        {
            VisionModule.SearchingRoi = null;
            AutoExcuteModule();
        }
        /// <summary>
        /// Thực hiện tính toán và hiển thị thông tin lên giao diện
        /// </summary>
        private void AutoExcuteModule()
        {
            if (AutoExcuteWhenPropertyChanged)
                ExcuteModule(InputImage);
        }
        /// <summary>
        /// Thực hiện tính toán và hiển thị thông tin lên giao diện
        /// </summary>
        /// <param name="img">Ảnh xử lý</param>
        private void ExcuteModule(object? img)
        {
            if (img != null)
            {
                // Thực hiện đọc barcode
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                var data = NodeLink.Excute() as BarcodeResultTransfer;
                stopWatch.Stop();
                ExcutionTime = (float)stopWatch.ElapsedMilliseconds;
                // Hiển thị kết quả đọc
                ViewResult(data);
            }
        }
        /// <summary>
        /// Hiển thị kết quả
        /// </summary>
        /// <param name="result"></param>
        private void ViewResult(BarcodeResultTransfer? result)
        {
            ImageBox.DeleteViewItems();
            BarcodeResultViews.Clear();
            if (result == null)
            {
                Status = false;
                return;
            }
            for (int i = 0; i < result.BarcodeItems.Count; i++)
            {
                var item = result.BarcodeItems[i];
                BarcodeResultViews.Add(new ResultReportView()
                {
                    Index = i + 1,
                    Content = item.Content,
                    BarCodeType = item.Type,
                });
            }
            var overlays = nodeLink.GetOverlays(result, true);
            if (overlays.Length > 0)
            {
                ImageBox.AddViewItems(overlays);
            }
            Status = result.Status;
        }
        /// <summary>
        /// <inheritdoc cref="VisionModuleContext.OnExcuteModuleCommand(object)"/>
        /// </summary>
        /// <param name="o"></param>
        public override void OnExcuteModuleCommand(object o)
        {
            ExcuteModule(InputImage);
        }
        /// <summary>
        /// <inheritdoc cref="SelectAllBarcodeTypeCommand"/>
        /// </summary>
        /// <param name="o"></param>
        private void OnSelectAllBarcodeTypeCommand(object o)
        {
            foreach (var item in VisionModule.SelectedTypeBarcodes)
            {
                item.IsEnable = true;
            }
            OnPropertyChanged("SelectedTypeBarcodes");
        }
        /// <summary>
        /// <inheritdoc cref="InputModuleContext.OnStartupControl"/>
        /// </summary>
        public override void OnStartupControl()
        {
            base.OnStartupControl();
            if (RoiSelectedType == RoiSetupMode.Constant)
            {
                ImageBox.SetRoi(VisionModule.SearchingRoi);
            }
            AutoExcuteModule();
        }
        /// <summary>
        /// <inheritdoc cref="InputModuleContext.RequestExcution"/>
        /// </summary>
        public override void RequestExcution()
        {
            ExcuteModule(InputImage);
        }
    }
}
