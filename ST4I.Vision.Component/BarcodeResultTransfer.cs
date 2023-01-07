using System;
using System.Collections.Generic;
using System.Text;
using ST4I.Vision.Core;

namespace ST4I.Vision.Component
{
    /// <summary>
    /// Cấu trúc dữ liệu thông tin kết quả đọc barcode
    /// </summary>
    [KeyAttribute(KeyAttributeType.Class, "Barcode Result Transfer")]
    public class BarcodeResultTransfer : BarcodeResult, INodeData
    {
        /// <summary>
        /// <inheritdoc cref="INodeData.NodeID"/>
        /// </summary>
        public string NodeID { get; set; }
        /// <summary>
        /// Hàm hủy
        /// </summary>
        ~BarcodeResultTransfer() => Dispose(false);

        private bool _disposedValue;
        /// <summary>
        /// Public implementation of Dispose pattern callable by consumers.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Protected implementation of Dispose pattern.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }
                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposedValue = true;
            }
        }
    }
}
