using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
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
using Color = System.Windows.Media.Color;

namespace MuseDB_Desktop.Controls
{
    /// <summary>
    /// Interaction logic for Button_AlbumIcon.xaml
    /// </summary>
    public partial class Button_AlbumIcon : UserControl
    {
        public string AlbumID;

        public Button_AlbumIcon()
        {
            InitializeComponent();
        }

        public Button_AlbumIcon(string AlbumID)
        {
            InitializeComponent();
            this.AlbumID = AlbumID;
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            if(AlbumID == null)
            {
                this.Button_Image.Source = new BitmapImage(new Uri("/img/icon_albumloading.png", UriKind.Relative));
                return;
            }
            var uriSource = new Uri($"http://192.168.0.120:4040/album/{AlbumID}/cover.jpg", UriKind.Absolute);
            var imgTemp = new BitmapImage();
            imgTemp.BeginInit();
            //Reduces memory usage
            imgTemp.DecodePixelWidth = 200;
            imgTemp.DecodePixelHeight = 200;
            imgTemp.UriSource = uriSource;
            imgTemp.EndInit();
            this.Button_Image.Source = imgTemp;
        }
        public void OnClick(object sender, EventArgs e)
        {
            
        }
    }
}
