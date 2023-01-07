using System.Windows.Controls;
using System.Reflection;

namespace ST4I.Vision.Controls
{
    /// <summary>
    /// Interaction logic for ReadBarcodeZXing.xaml
    /// </summary>
    public partial class ReadBarcodeZXingControl : UserControl
    {
        public ReadBarcodeZXingControl()
        {
            var languageResource = LanguageUtils.SelectFromGlobalLanguage(Assembly.GetExecutingAssembly().GetName().Name, ReadBarcodeZXingContext.LANGUAGE_PATH_RESOURCE);
            InitializeComponent();
            if (languageResource != null)
            {
                this.Resources.MergedDictionaries.Add(languageResource);
            }
        }
    }
}
