using System;
using System.Collections.Generic;
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
        public Button_TrackDetails()
        {
            InitializeComponent();
            this.Button_TrackPanel.TextBlock_TrackName.FontSize = 16;
            this.Button_TrackPanel.TextBlock_TrackArtist.FontSize = 13;
            this.Button_TrackPanel.TextBlock_TrackName.Foreground = new SolidColorBrush(Color.FromRgb(60,60,60));
        }

        public Button_TrackDetails(string TrackID, string TrackName, string TrackArtist, string AlbumID, string AlbumName, string TrackDuration)
        {
            InitializeComponent();
            this.Button_TrackPanel.Background = Brushes.Transparent;
            this.Button_TrackPanel.TextBlock_TrackName.FontSize = 16;
            this.Button_TrackPanel.TextBlock_TrackArtist.FontSize = 13;
            this.Button_TrackPanel.Label_TrackDuration.FontSize = 15;
            this.Button_TrackPanel.TrackID = TrackID;
            this.Button_TrackPanel.TextBlock_TrackName.Foreground = new SolidColorBrush(Color.FromRgb(60,60,60));
            this.Button_TrackPanel.TextBlock_TrackName.Text = TrackName;
            this.Button_TrackPanel.TextBlock_TrackArtist.Text = TrackArtist + " - " + AlbumName;
            this.Button_TrackPanel.Label_TrackDuration.Content = TrackDuration;
            LoadImage(AlbumID);
        }

        private void LoadImage(string AlbumID)
        {
            if (AlbumID == null)
                return;
            var uriSource = new Uri($"http://192.168.0.120:4040/album/{AlbumID}/cover.jpg", UriKind.Absolute);
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
        public void DeleteButton(bool reveal)
        {
            this.Button_Delete.Visibility = (reveal) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void Delete_OnClick(object sender, MouseButtonEventArgs e)
        {
            if (this.Parent is ListBox)
                (this.Parent as ListBox).Items.Remove(this);
        }
    }
}
