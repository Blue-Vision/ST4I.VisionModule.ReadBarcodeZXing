namespace ST4I.Vision.Core
{
    /// <summary>
    /// Loại barcode
    /// </summary>
    public enum BarcodeType
    {
        /// <summary>
        /// The type of barcode is unknown
        /// </summary>
        Unknown = -1,
        /// <summary>
        /// The barcode is of type Codabar
        /// </summary>
        Codabar = 1,
        /// <summary>
        /// The barcode is of type Code 39
        /// </summary>
        Code39 = 2,
        /// <summary>
        /// The barcode is of type Code 93
        /// </summary>
        Code93 = 4,
        /// <summary>
        /// The barcode is of type Code 128
        /// </summary>
        Code128 = 8,
        /// <summary>
        /// The barcode is of type EAN 8
        /// </summary>
        Ean8 = 16,
        /// <summary>
        /// The barcode is of type EAN 13
        /// </summary>
        Ean13 = 32,
        /// <summary>
        /// The barcode is of type Code 25
        /// </summary>
        I2Of5 = 64,
        /// <summary>
        /// The barcode is of type MSI code
        /// </summary>
        Msi = 128,
        /// <summary>
        /// The barcode is of type UPC A
        /// </summary>
        UpcA = 256,
        /// <summary>
        /// The barcode is of type UPC A
        /// </summary>
        UpcE = 512,
        /// <summary>
        /// The barcode is of type Pharmacode
        /// </summary>
        Pharmacode = 1000,
        /// <summary>
        /// The barcode is of type GS1 DataBar
        /// </summary>
        RssExpanded = 2000,
    }
}
