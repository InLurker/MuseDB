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
        private readonly string TrackName;
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
                        this.TextBlock_TrackName.Text = TrackName = SQLDataReader["track_name"].ToString();
                        this.TextBlock_TrackDuration.Text = SQLDataReader["track_duration"].ToString();
                        this.TextBlock_TrackOrderLastPlayed.Text = "Track " + SQLDataReader["track_order"].ToString() + " - last played " + (String.IsNullOrEmpty(SQLDataReader["last_playback"].ToString()) ? "(null)" : SQLDataReader["last_playback"].ToString());
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
        }

        private void DeleteTrack_OnClick(object sender, MouseButtonEventArgs e)
        {
            var Confirmation = new ConfirmationPopUp($"#{TrackID} - {TrackName}'s data will be deleted.\nAre you sure?");
            Confirmation.ShowDialog();
            if (Confirmation.ConfirmResult)
            {
                using (SqlConnection SQLConnection = new SqlConnection(SqlHelper.CnnVal("database")))
                {
                    SQLConnection.Open();
                    using (SqlCommand command = new SqlCommand($"SELECT COUNT(track_id) FROM track WHERE album_id = {AlbumID}", SQLConnection))
                    {
                        int result = (int)command.ExecuteScalar();
                        if (result == 1)//if track the last in album, then delete album
                        {
                            command.CommandText = $"DELETE FROM album WHERE album_id = {AlbumID}";
                            HttpHelper.DeleteFile($"http://192.168.0.120:4040/album/{AlbumID}.jpg");
                        }
                        else //if track is not the last, only delete track
                            command.CommandText = $"DELETE FROM track WHERE track_id = {TrackID}";
                        result = command.ExecuteNonQuery();

                        if (result > 0)
                            _ = new NotificationPopUp($"#{TrackID} - {TrackName}'s data has been deleted.").ShowDialog();
                    }
                    HttpHelper.DeleteFile($"http://192.168.0.120:4040/track/{TrackID}.mp3");
                }
                this.Close();
            }
        }

    }
}
