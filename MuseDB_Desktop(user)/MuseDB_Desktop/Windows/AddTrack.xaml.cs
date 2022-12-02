using MuseDB_Desktop.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MuseDB_Desktop.Windows
{
    /// <summary>
    /// Interaction logic for AddTrack.xaml
    /// </summary>
    public partial class AddTrack : Window
    {
        private bool success = false;
        private string trackName = "";
        private string FilePath = "";
        private string Duration = "";
        private readonly bool isMiscellaneous = false;
        public bool Success => success;
        public string TrackName => trackName;
        public string AudioFilePath => FilePath;
        public string TrackDuration => Duration;

        public AddTrack(int TrackOrder, string ArtFileLocation, bool isMiscellaneous)
        {
            InitializeComponent();
            if (String.IsNullOrWhiteSpace(ArtFileLocation))
                this.Image_Background.Source = new BitmapImage(new Uri("/img/icon_albumloading.png", UriKind.Relative));
            else
                this.Image_Background.Source = new BitmapImage(new Uri(ArtFileLocation));
            this.isMiscellaneous = isMiscellaneous;
            this.TextBox_TrackOrder.Text = TrackOrder.ToString();
            this.TextBox_TrackOrder.IsEnabled = false;
        }

        private void Button_ConfirmOnClick(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(TextBox_TrackName.Text))
            {
                TextBlock_Error.Text = "Invalid Track Name.";
                return;
            }

            if (String.IsNullOrWhiteSpace(TextBox_TrackDuration.Text) || !TimeSpan.TryParse(TextBox_TrackDuration.Text, out _))
            {
                TextBlock_Error.Text = "Invalid Track Duration.";
                return;
            }
            if (String.IsNullOrWhiteSpace(TextBox_TrackOrder.Text))
            {
                TextBlock_Error.Text = "Invalid Track Name.";
                return;
            }
            if (String.IsNullOrWhiteSpace(FilePath))
            {
                TextBlock_Error.Text = "No track audio selected.";
                return;

            }
            trackName = TextBox_TrackName.Text;
            Duration = TextBox_TrackDuration.Text;

            success = true;
            if(isMiscellaneous)
                this.Close();
        }

        private void Button_SelectAudioOnClick(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog FilePicker = new Microsoft.Win32.OpenFileDialog()
            {
                DefaultExt = "MP3 Files (*.mp3)|*.mp3",
                Filter = "MP3 Files (*.mp3)|*.mp3"
            };

            Nullable<bool> result = FilePicker.ShowDialog();

            if (result == true)
            {
                FilePath = FilePicker.FileName;
                this.TextBox_TrackAudio.Text = FilePicker.SafeFileName;
            }
        }
    }
}
