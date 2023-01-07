using System;
using System.Collections.Generic;
using System.Text;
using OpenCvSharp;

namespace ST4I.Vision.Core
{
    /// <summary>
    /// Thông tin về barcode
    /// </summary>
    [KeyAttribute(KeyAttributeType.Class, "Barcode Report")]
    public class BarcodeReport
    {
        /// <summary>
        /// Kết quả barcode
        /// </summary>
        [KeyAttribute(KeyAttributeType.Undefined, "Content")]
        public string Content { get; set; } = string.Empty;
        /// <summary>
        /// Toạ độ các đỉnh của đối tượng được tìm thấy
        /// </summary>
        [KeyAttribute(KeyAttributeType.Collection, "4 Vertices")]
        public Point2d[] Vertices { get; set; }
        /// <summary>
        /// Loại barcode
        /// </summary>
        [KeyAttribute(KeyAttributeType.Undefined, "Barcode Type")]
        public BarcodeType Type { get; set; }
    }
    /// <summary>
    /// Thông tin về barcode
    /// </summary>
    public class BarcodeResult : IModuleData
    {
        /// <summary>
        /// Vùng roi thực tế xử lý
        /// </summary>
        public IRoi ProcessedRoi { get; set; }
        /// <summary>
        /// <inheritdoc cref="IModuleData.Status"/>
        /// </summary>
        public bool Status { get; set; }
        /// <summary>
        /// Danh sách kết qủa barcode
        /// </summary>
        public List<BarcodeReport> BarcodeItems { get; set; }
    }
}
