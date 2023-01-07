using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Controls;
using System.Windows.Input;
using System.Threading;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Win32;
using OpenCvSharp;
using ST4I.Vision.Controls;
using ST4I.Vision.UI;
using ST4I.Vision;
using ST4I.Vision.Component;
using System.Windows.Media.Imaging;

namespace VisionBuildComponent
{
    public class MainContext : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        private ImageBoxContext imageBoxView;
        private int selectedLanguageIndex = 0;
        private ReadImageUriNode readImageNode = new ReadImageUriNode();
        /// <summary>
        /// Node module
        /// </summary>
        public ReadBarcodeZXingNode testNode { get; set; } = new ReadBarcodeZXingNode();
        /// <summary>
        /// Context cho vision module
        /// </summary>
        public ReadBarcodeZXingContext VisionContext { get; set; } = new ReadBarcodeZXingContext();
        /// <summary>
        /// Image box data context
        /// </summary>
        public ImageBoxContext ImageBoxView
        {
            get
            {
                return imageBoxView;
            }
        }
        /// <summary>
        /// Lựa chọn ảnh từ file
        /// </summary>
        public RelayCommand BrowseImageCommand { get; set; }
        /// <summary>
        /// Lựa chọn ngôn ngữ
        /// </summary>
        public int SelectedLanguageIndex
        {
            get
            {
                return selectedLanguageIndex;
            }
            set
            {
                selectedLanguageIndex = value;
                if (value == 0)
                {
                    string language = "en";
                    System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(language);
                    System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(language);
                }
                else if (value == 1)
                {
                    string language = "vi-VN";
                    System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(language);
                    System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(language);
                }
                OnPropertyChanged();
            }
        }

        public MainContext()
        {
            imageBoxView = new ImageBoxContext()
            {
                MaxNumRoi = 1
            };
            ImageBoxView.RoiAdded += OnRoiAdded;
            ImageBoxView.RoiChanged += OnRoiChanged;
            ImageBoxView.RoiDeleted += OnRoiDeleted;
            BrowseImageCommand = new RelayCommand(OnBrowseImageCommand);

            #region Giả lập thông tin, tất cả các vision control đều được thực hiện gắn dữ liệu và gọi hàm dưới đây
            readImageNode.Name = "Image File";
            testNode.Name = "Read Barcode";
            // Tạo kết nối giả lập giữa module đọc ảnh từ file và module cần test. Khi readImageNode có ảnh nó sẽ tự động forwared tới node cần test
            var outputPort = NodeExtension.GetFirstPortType(readImageNode.Ports, PortType.Output);
            var inputPort = NodeExtension.GetFirstPortType(testNode.Ports, PortType.Input);
            var edge = new Edge()
            {
                SourcePort = outputPort,
                DestPort = inputPort
            };
            outputPort.AddEdge(edge);
            inputPort.AddEdge(edge);
            testNode.ImageSourceID = readImageNode.NodeID;
            if (VisionContext is IInteractImageBox)
            {
                VisionContext.ImageBox = ImageBoxView;
            }
            VisionContext.NodeLink = testNode;
            VisionContext.OnStartupControl();
            #endregion
        }
        /// <summary>
        /// Hàm xử lý sự kiện thêm một Roi mới
        /// </summary>
        /// <param name="roi"></param>
        public void OnRoiAdded(object o, IRoi roi)
        {
            VisionContext.OnRoiAdded(o, roi);
        }
        /// <summary>
        /// Hàm xử lý sự kiện chỉnh sửa Roi
        /// </summary>
        /// <param name="o"></param>
        /// <param name="roi"></param>
        public void OnRoiChanged(object o, IRoi roi)
        {
            VisionContext.OnRoiChanged(o, roi);
        }
        /// <summary>
        /// Hàm xử lý sự kiện xóa ROI
        /// </summary>
        /// <param name="o"></param>
        /// <param name="roi"></param>
        public void OnRoiDeleted(object o, IRoi roi)
        {
            VisionContext.OnRoiDeleted(o, roi);
        }
        /// <summary>
        /// Hàm xử lý yêu cầu chọn ảnh từ file
        /// </summary>
        /// <param name="o"></param>
        private void OnBrowseImageCommand(object o)
        {
            OpenFileDialog opf = new OpenFileDialog();
            opf.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.tif, *.png,*.bmp) | *.jpg; *.jpeg; *.jpe; *.tif; *.png; *.bmp";
            if (opf.ShowDialog() == true)
            {
                ImageBoxView.ImageBitmap = new BitmapImage(new Uri(opf.FileName));
                readImageNode.UriSource = opf.FileName;
                readImageNode.Excute();
                VisionContext.OnStartupControl();
            }
        }
    }
}
