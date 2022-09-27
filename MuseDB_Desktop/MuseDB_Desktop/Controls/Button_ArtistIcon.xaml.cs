using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;

namespace MuseDB_Desktop.Controls
{
    /// <summary>
    /// Interaction logic for Button_ArtistIcon.xaml
    /// </summary>
    public partial class Button_ArtistIcon : UserControl
    {
        public string ArtistID { get; set; }

        public Button_ArtistIcon()
        {
            InitializeComponent();
        }
        public Button_ArtistIcon(string ArtistID)
        {
            InitializeComponent();
            this.ArtistID = ArtistID;
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            if (ArtistID == null)
            {
                this.Button_Image.Source = new BitmapImage(new Uri("/img/profile_icon.png", UriKind.Relative));
                return;
            }
            var uriSource = new Uri($"http://192.168.0.120:4040/artist/{ArtistID}.jpg", UriKind.Absolute);
            var imgTemp = new BitmapImage();
            imgTemp.BeginInit();
            imgTemp.CacheOption = BitmapCacheOption.OnLoad;
            imgTemp.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            //Reduces memory usage
            imgTemp.DecodePixelWidth = 160;
            imgTemp.DecodePixelHeight = 160;
            imgTemp.UriSource = uriSource;
            imgTemp.EndInit();
            this.Button_Image.Source = imgTemp;

        }

        private void OnClick(object sender, EventArgs e)
        {

        }
    }
}
