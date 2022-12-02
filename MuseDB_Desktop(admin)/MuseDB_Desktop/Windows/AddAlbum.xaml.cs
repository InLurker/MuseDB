using MuseDB_Desktop.Controls;
using MuseDB_Desktop.Helpers;
using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Reflection.Emit;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media.Imaging;

namespace MuseDB_Desktop.Windows
{
    /// <summary>
    /// Interaction logic for AddAlbum.xaml
    /// </summary>
    public partial class AddAlbum : Window
    {
        private string FilePath = "";
        private bool success = false;
        private readonly System.Collections.Generic.List<Track> TrackList = new System.Collections.Generic.List<Track>();
        private struct Track
        {
            public string TrackName;
            public string TrackDuration;
            public string TrackAudio;
            public Track(string track_name, string track_duration, string track_audio)
            {
                TrackName = track_name;
                TrackDuration = track_duration;
                TrackAudio = track_audio;
            }
        }
        public bool Success => success;
        public AddAlbum()
        {
            InitializeComponent();
            using (SqlConnection SQLConnection = new SqlConnection(SqlHelper.CnnVal("database")))
            {
                SQLConnection.Open();
                using (SqlCommand command = new SqlCommand("SELECT * FROM artist", SQLConnection))
                {
                    using (SqlDataReader SQLDataReader = command.ExecuteReader())
                    {
                        for (int i = 0; SQLDataReader.Read(); ++i)
                            this.ComboBox_Artist.Items.Add(SQLDataReader["artist_id"].ToString() + " - " + SQLDataReader["artist_name"].ToString());
                    }
                }
            }
        }


        private void Button_SelectOnClick(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog FilePicker = new Microsoft.Win32.OpenFileDialog()
            {
                DefaultExt = "JPG Files (*.jpg)|*.jpg",
                Filter = "JPG Files (*.jpg)|*.jpg"
            };

            Nullable<bool> result = FilePicker.ShowDialog();

            if (result == true)
            {
                FilePath = FilePicker.FileName;
                this.Image_Background.Source = this.Image_Icon.Source = new BitmapImage(new Uri(FilePath));
            }
        }

        private void Button_ConfirmOnClick(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(this.TextBox_AlbumName.Text))
            {
                TextBlock_Error.Text = "Please provide the album name.";
                return;
            }
            if (String.IsNullOrWhiteSpace(this.ComboBox_Artist.Text))
            {
                TextBlock_Error.Text = "Please select the album artist.";
                return;
            }
            if (String.IsNullOrWhiteSpace(FilePath))
            {
                TextBlock_Error.Text = "Please select an album art image.";
                return;
            }
            if(TrackList.Count == 0)
            {
                TextBlock_Error.Text = "Please assign tracks to album.";
                return;
            }
            int counter = 0;
            int NewID = 0;
            using (SqlConnection SQLConnection = new SqlConnection(SqlHelper.CnnVal("database")))
            {
                SQLConnection.Open();
                using (SqlCommand command = new SqlCommand($"INSERT INTO album (album_name, artist_id) OUTPUT INSERTED.album_id VALUES (N'{this.TextBox_AlbumName.Text.Replace("'", "''")}', {this.ComboBox_Artist.Text.Substring(0,5)})", SQLConnection))
                {
                    NewID = (int)command.ExecuteScalar();
                    try
                    {
                        _ = HttpHelper.UploadFile("http://192.168.0.120:4040/album/", FilePath, NewID + ".jpg");
                    }
                    catch (Exception exception)
                    {
                        TextBlock_Error.Text = exception.Message;
                        return;
                    }
                    int TrackID = 0;
                    string[] timeformats = { @"m\:ss", @"mm\:ss", @"h\:mm\:ss" };
                    TrackList.ForEach(track =>
                    {
                        command.CommandText = "INSERT INTO track (track_order, track_name, track_duration, album_id) OUTPUT INSERTED.track_id VALUES (" +
                                            $"{++counter}, " +
                                            $"N'{track.TrackName.Replace("'", "''")}', " +
                                            $"{TimeSpan.ParseExact(track.TrackDuration, timeformats, CultureInfo.InvariantCulture).TotalSeconds}, " +
                                            $"{NewID})";
                        TrackID = (int)command.ExecuteScalar();
                        if (TrackID > 0)
                            _ = HttpHelper.UploadFile("http://192.168.0.120:4040/track/", track.TrackAudio, TrackID + ".mp3");
                    }
                    );
                }
            }
            if (NewID > 0 && counter > 0)
            {
                success = true;
                this.Hide();
                _ = new NotificationPopUp("Successfully Added Album\n" +
                    $"{NewID} - {TextBox_AlbumName.Text}\n" +
                    $"({counter} " + ((counter == 1) ? "track" : "tracks") + ")").ShowDialog();
                this.Close();
            }
            else
            {
                TextBlock_Error.Text = "An unknown error occurred.";
            }
        }
        private void Add_OnHover(object sender, RoutedEventArgs e)
        {
            this.Button_Add.Opacity = 1;
        }

        private void Add_OnLeave(object sender, RoutedEventArgs e)
        {
            this.Button_Add.Opacity = 0.2;
        }

        private void Add_OnClick(object sender, RoutedEventArgs e)
        {
            string lastID = "";
            using (SqlConnection SQLConnection = new SqlConnection(SqlHelper.CnnVal("database")))
            {
                SQLConnection.Open();
                using (SqlCommand command = new SqlCommand($"SELECT IDENT_CURRENT('album') + {TrackList.Count + 1}", SQLConnection))
                {
                    command.ExecuteScalar();
                    lastID = command.ExecuteScalar().ToString();
                    if (lastID == "")
                        return;
                }
            }
            var track = new AddTrack(TrackList.Count + 1, lastID, FilePath, true);
            track.ShowDialog();
            if (track.Success == false)
                return;
            Track NewTrack = new Track(track.TrackName, track.TrackDuration, track.AudioFilePath);
            TrackList.Add(NewTrack);
            this.ListBox_Tracks.Items.Add(new TrackListItem(TrackList.Count, NewTrack.TrackName, NewTrack.TrackDuration, true));
        }
        private void Delete_OnHover(object sender, RoutedEventArgs e)
        {
            this.Button_Delete.Opacity = 1;
        }

        private void Delete_OnLeave(object sender, RoutedEventArgs e)
        {
            this.Button_Delete.Opacity = 0.2;
        }
        private void Delete_OnClick(object sender, RoutedEventArgs e)
        {
            if (TrackList.Count > 0)
            {
                TrackList.RemoveAt(TrackList.Count-1);
                this.ListBox_Tracks.Items.RemoveAt(TrackList.Count);
            }
        }
    }

}
