using MuseDB_Desktop.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
        private string AlbumID = "";
        private string Duration = "";
        private bool isMiscellaneous = false;
        public bool Success => success;
        public string TrackName => trackName;
        public string AudioFilePath => FilePath;
        public string TrackDuration => Duration;

        public AddTrack()
        {
            InitializeComponent();
            using (SqlConnection SQLConnection = new SqlConnection(SqlHelper.CnnVal("database")))
            {
                SQLConnection.Open();
                using (SqlCommand command = new SqlCommand("SELECT * FROM album", SQLConnection))
                {
                    using (SqlDataReader SQLDataReader = command.ExecuteReader())
                    {
                        for (int i = 0; SQLDataReader.Read(); ++i)
                            this.ComboBox_Album.Items.Add(SQLDataReader["album_id"].ToString() + " - " + SQLDataReader["album_name"].ToString());
                    }
                }
            }
        }
        public AddTrack(int TrackOrder, string AlbumID, string ArtFileLocation)
        {
            InitializeComponent();
            this.TextBox_TrackOrder.Text = TrackOrder.ToString();
            this.TextBox_TrackOrder.IsEnabled = false;
            this.ComboBox_Album.IsEditable = true;
            this.ComboBox_Album.Text = AlbumID;
            this.ComboBox_Album.IsEnabled = false;
            if (String.IsNullOrWhiteSpace(ArtFileLocation))
                this.Image_Background.Source = new BitmapImage(new Uri("/img/icon_albumloading.png", UriKind.Relative));
            else
                this.Image_Background.Source = new BitmapImage(new Uri(ArtFileLocation));
            isMiscellaneous = true;
        }

        private void Button_ConfirmOnClick(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(TextBox_TrackName.Text))
            {
                TextBlock_Error.Text = "Invalid Track Name.";
                return;
            }
            if (String.IsNullOrWhiteSpace(TextBox_TrackDuration.Text))
            {
                TextBlock_Error.Text = "Invalid Track Duration.";
                return;
            }
            if (String.IsNullOrWhiteSpace(TextBox_TrackOrder.Text))
            {
                TextBlock_Error.Text = "Invalid Track Name.";
                return;
            }
            if (String.IsNullOrWhiteSpace(ComboBox_Album.Text))
            {
                TextBlock_Error.Text = "No albums selected.";
                return;
            }
            if (String.IsNullOrWhiteSpace(FilePath))
            {
                TextBlock_Error.Text = "No track audio selected.";
                return;

            }
            trackName = TextBox_TrackName.Text;
            Duration = TextBox_TrackDuration.Text;
            AlbumID = ComboBox_Album.Text.Substring(0, 6);

            success = true;
            if(isMiscellaneous)
                this.Close();
            else
            {
                int NewID = 0;
                using (SqlConnection SQLConnection = new SqlConnection(SqlHelper.CnnVal("database")))
                {
                    SQLConnection.Open();
                    using (SqlCommand command = new SqlCommand("INSERT INTO track (track_order, track_name, track_duration, album_id) OUTPUT INSERTED.track_id VALUES (" +
                                                    $"{this.TextBox_TrackOrder.Text}, " +
                                                    $"N'{this.TextBox_TrackName.Text.Replace("'", "''")}', " +
                                                    $"'{this.TextBox_TrackDuration.Text}', " +
                                                    $"{this.ComboBox_Album.Text.Substring(0, 6)})",
                                                    SQLConnection))
                    {
                        NewID = (int)command.ExecuteScalar();
                        try
                        {
                            HttpHelper.UploadFile("http://192.168.0.120:4040/track/", FilePath, NewID + ".mp3");
                        }
                        catch (Exception exception)
                        {
                            TextBlock_Error.Text = exception.Message;
                        }
                    }
                }
                if (NewID > 0)
                {
                    success = true;
                    this.Hide();
                    _ = new NotificationPopUp($"Successfully Added Album\n{NewID} - {TextBox_TrackName.Text}").ShowDialog();
                    this.Close();
                }
                else
                {
                    TextBlock_Error.Text = "An unknown error occurred.";
                }
                this.Close();
            }
            return;
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
