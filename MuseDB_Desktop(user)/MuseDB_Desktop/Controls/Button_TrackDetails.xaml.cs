using MuseDB_Desktop.Helpers;
using MuseDB_Desktop.Windows;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
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

namespace MuseDB_Desktop.Controls
{
    /// <summary>
    /// Interaction logic for Button_TrackDetails.xaml
    /// </summary>
    public partial class Button_TrackDetails : UserControl
    {
        private readonly string TrackID;
        private readonly string TrackName;
        private readonly string AlbumID;

        public Button_TrackDetails(string TrackID, string TrackName, string TrackArtist, string AlbumID, string AlbumName, string TrackDuration)
        {
            InitializeComponent();
            this.Button_TrackPanel.TextBlock_TrackName.FontSize = 16;
            this.Button_TrackPanel.TextBlock_TrackArtist.FontSize = 13;
            this.Button_TrackPanel.TextBlock_TrackName.Foreground = new SolidColorBrush(Color.FromRgb(60, 60, 60));
            this.TrackID = TrackID;
            this.TrackName = TrackName;
            this.AlbumID = AlbumID;
            this.Button_TrackPanel.Background = Brushes.Transparent;
            this.Button_TrackPanel.Label_TrackDuration.FontSize = 15;
            this.Button_TrackPanel.TrackID = TrackID;
            this.Button_TrackPanel.TextBlock_TrackName.Text = TrackName;
            this.Button_TrackPanel.TextBlock_TrackArtist.Text = TrackArtist + " - " + AlbumName;
            this.Button_TrackPanel.Label_TrackDuration.Content = TrackDuration;
            LoadImage();
        }

        private void LoadImage()
        {
            if (AlbumID == null)
                return;
            var uriSource = new Uri($"http://192.168.0.120:4040/album/{AlbumID}.jpg", UriKind.Absolute);
            var imgTemp = new BitmapImage();
            imgTemp.BeginInit();
            //Reduces memory usage
            imgTemp.DecodePixelWidth = 200;
            imgTemp.DecodePixelHeight = 200;
            imgTemp.UriSource = uriSource;
            imgTemp.EndInit();
            this.Image_Background.Source = imgTemp;
            this.Image_AlbumIcon.Source = imgTemp;
        }

        private void OnDoubleClick(object sender, MouseButtonEventArgs e)
        {
            _ = new PreviewTrack(TrackID).ShowDialog();
        }
    }
}
