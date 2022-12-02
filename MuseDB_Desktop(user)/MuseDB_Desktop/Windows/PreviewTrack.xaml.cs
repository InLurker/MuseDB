using MuseDB_Desktop.Helpers;
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
using System.Windows.Shapes;

namespace MuseDB_Desktop.Windows
{
    /// <summary>
    /// Interaction logic for PreviewTrack.xaml
    /// </summary>
    public partial class PreviewTrack : Window
    {
        private readonly string TrackID;
        private readonly string AlbumID;
        public PreviewTrack(string TrackID)
        {
            InitializeComponent();
            this.TrackID = TrackID;
            using (SqlConnection SQLConnection = new SqlConnection(SqlHelper.CnnVal("database")))
            {
                SQLConnection.Open();
                using (SqlCommand command = new SqlCommand($"SELECT track_name, track_duration, track_order, last_playback, album_id FROM track WHERE track_id = {TrackID}", SQLConnection))
                {
                    using (SqlDataReader SQLDataReader = command.ExecuteReader())
                    {
                        SQLDataReader.Read();
                        TimeSpan duration = TimeSpan.FromSeconds((short)SQLDataReader["track_duration"]);
                        this.TextBlock_TrackName.Text = SQLDataReader["track_name"].ToString();
                        this.TextBlock_TrackDuration.Text = (duration.Hours > 0 ? duration.Hours + (duration.Hours < 10 ? ":0" : ":") : "")
                                + duration.Minutes + (duration.Seconds < 10 ? ":0" : ":")
                                + duration.Seconds;
                        this.TextBlock_TrackOrderLastPlayed.Text = "Track " + SQLDataReader["track_order"].ToString() + " - " + (String.IsNullOrEmpty(SQLDataReader["last_playback"].ToString()) ? "not played yet." : "last played " + SQLDataReader["last_playback"].ToString());
                        AlbumID = SQLDataReader["album_id"].ToString();
                    }
                    command.CommandText = $"SELECT album_name FROM album WHERE album_id = {AlbumID}";
                    this.TextBlock_AlbumName.Text = (string)command.ExecuteScalar() + $" (#{AlbumID})";
                }
            }
            this.TextBlock_TrackID.Text = "Track #" + TrackID;
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            var uriSource = new Uri($"http://192.168.0.120:4040/album/{AlbumID}.jpg", UriKind.Absolute);
            var imgTemp = new BitmapImage();
            imgTemp.BeginInit();
            imgTemp.UriSource = uriSource;
            //Reduces memory usage
            imgTemp.DecodePixelWidth = 200;
            imgTemp.DecodePixelHeight = 200;
            imgTemp.CacheOption = BitmapCacheOption.OnLoad;
            imgTemp.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            imgTemp.EndInit();
            this.Image_Profile.Source = imgTemp;
            this.Image_Background.Source = imgTemp;

            using (SqlConnection SQLConnection = new SqlConnection(SqlHelper.CnnVal("database")))
            {
                SQLConnection.Open();
                using (SqlCommand command = new SqlCommand($"UPDATE track SET last_playback = GETDATE() WHERE track_id = {TrackID}", SQLConnection))
                {
                    command.ExecuteNonQuery();
                    command.CommandText = $"UPDATE album SET last_playback = GETDATE() WHERE album_id = {AlbumID}";
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
