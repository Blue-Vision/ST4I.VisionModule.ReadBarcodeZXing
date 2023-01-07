using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using ST4I.Vision.Core;
using ST4I.Vision.Drawing;
using OpenCvSharp;

namespace ST4I.Vision.Component
{
    /// <summary>
    /// Node có chức năng đọc barcode
    /// </summary>
    public class ReadBarcodeZXingNode : NodeVisionModuleNI, INodeDisplayResult
    {
        /// <summary>
        /// <inheritdoc cref="INode.Module"/>
        /// </summary>
        public ReadBarcodeZXing VisionModule { get; set; } = new ReadBarcodeZXing();
        /// <summary>
        /// <inheritdoc cref="INode.Module"/>
        /// </summary>
        public override IVisionModule Module { get { return VisionModule; } }
        /// <summary>
        /// <inheritdoc cref="INode.DataResult"/>
        /// </summary>
        [JsonIgnore]
        public BarcodeResultTransfer Result = new BarcodeResultTransfer();
        /// <summary>
        /// <inheritdoc cref="INode.DataResult"/>
        /// </summary>
        public override INodeData DataResult => Result;
        /// <summary>
        /// <inheritdoc cref="INode.Type"/>
        /// </summary>
        public override NodeType Type { get { return NodeType.ReadBarcodeNI; } }
        /// <summary>
        /// <inheritdoc cref="INodeInput.ReceiveDataType"/>
        /// </summary>
        public override Type[] ReceiveDataType { get { return new Type[] { typeof(ImageTransfer) }; } }
        /// <summary>
        /// <inheritdoc cref="INodeOutput.TransferDataType"/>
        /// </summary>
        public override Type[] TransferDataType { get { return new Type[] { typeof(BarcodeResultTransfer) }; } }
        /// <summary>
        /// <inheritdoc cref="INodeDisplayResult.OverlaySetting"/>
        /// </summary>
        public DisplayOverlaySetting OverlaySetting { get; set; } = new DisplayOverlaySetting() { ObjectFeatureExisted = true, IsShowObjectRegion = true};
        /// <summary>
        /// <inheritdoc cref="INodeInputImage.ImageSupportedType"/>
        /// </summary>
        public override int[] ImageSupportedType { get { return new int[] { MatType.CV_8UC1, MatType.CV_8UC3 }; } }
        /// <summary>
        /// <inheritdoc cref="INode.Excute"/>
        /// </summary>
        /// <returns></returns>
        public override INodeData Excute()
        {
            Result = new BarcodeResultTransfer()
            {
                NodeID = NodeID,
                ProcessedRoi = VisionModule.SearchingRoi,
                BarcodeItems = new List<BarcodeReport>(),
                Status = false
            };
            var inputImage = GetInputImage()?.Image;
            if (inputImage != null)
            {
                var result = new BarcodeResult();
                if (RoiSelectedType == RoiSetupMode.Constant)
                {
                    var newRoi = IsEnableReposition ? EuclideanTransformRoi(VisionModule.SearchingRoi) : VisionModule.SearchingRoi;
                    result = VisionModule.Read(inputImage, newRoi);
                }
                else
                {
                    result = VisionModule.Read(inputImage);
                }
                Result.ProcessedRoi = result.ProcessedRoi;
                Result.BarcodeItems = result.BarcodeItems;
                Result.Status = result.Status;
            }
            FinishWork(Result);
            return Result;
        }
        /// <summary>
        /// <inheritdoc cref="INodeDisplayResult.GetOverlays"/>
        /// </summary>
        /// <param name="data">Dữ liệu được truyền vào</param>
        /// <param name="debugMode">Nếu debugMode bằng true thì sẽ không hiển thị vùng xử lý</param>
        /// <returns></returns>
        public OverlayItem[] GetOverlays(INodeData data, bool debugMode = false)
        {
            List<OverlayItem> overlays = new List<OverlayItem>();
            if (data != null)
            {
                if (data is BarcodeResultTransfer)
                {
                    var result = data as BarcodeResultTransfer;
                    if (debugMode == false)
                    {
                        OverlayItem overlay = OverlaySetting.RenderProcessingRegionOverlay(result.ProcessedRoi, result.Status);
                        if (overlay != null) overlays.Add(overlay);
                    }
                    var overlayContent = OverlaySetting.RenderProcessingTextOverlay(result.ProcessedRoi, Name, string.Empty, result.Status);
                    if (overlayContent != null) overlays.Add(overlayContent);

                    for (int i = 0; i < result.BarcodeItems.Count; i++)
                    {
                        string content = $"{i + 1}-{result.BarcodeItems[i].Content}";
                        OverlayItem overlay = OverlaySetting.RenderObjectRegionOverlay(result.BarcodeItems[i].Vertices, result.Status);
                        if (overlay != null) overlays.Add(overlay);
                        overlay = OverlaySetting.RenderObjectTextOverlay(result.BarcodeItems[i].Vertices, content, result.Status);
                        if (overlay != null) overlays.Add(overlay);
                    }
                }
            }
            return overlays.ToArray();
        }
    }
}
