using MuseDB_Desktop.Helpers;
using MuseDB_Desktop.Windows;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Interaction logic for SubmissionEntry.xaml
    /// </summary>
    public partial class SubmissionEntry : UserControl
    {
        private readonly string AlbumSubmissionID;
        private readonly string ArtistID;

        public SubmissionEntry(string Username, string AlbumSubmissionID, string AlbumName, string ArtistName, string ArtistID)
        {
            InitializeComponent();
            this.TextBlock_Username.Text = Username;
            this.TextBlock_AlbumName.Text = AlbumName;
            this.TextBlock_ArtistName.Text = ArtistName;
            this.ArtistID = ArtistID;
            this.AlbumSubmissionID = AlbumSubmissionID;
            using (SqlConnection SQLConnection = new SqlConnection(SqlHelper.CnnVal("database")))
            {
                SQLConnection.Open();
                using (SqlCommand command = new SqlCommand($"SELECT * FROM track_submission WHERE album_s_id = '{AlbumSubmissionID}' ORDER BY track_order ASC", SQLConnection))
                {
                    using (SqlDataReader SQLDataReader = command.ExecuteReader())
                    {
                        for (int i = 1; SQLDataReader.Read(); ++i)
                        {
                            TimeSpan duration = TimeSpan.FromSeconds((short)SQLDataReader["track_duration"]);
                            this.ListBox_Tracks.Items.Add(
                                new TrackListItem(
                                    (byte)SQLDataReader["track_order"],
                                    SQLDataReader["track_name"].ToString(),
                                    (duration.Hours > 0 ? duration.Hours + (duration.Hours < 10 ? ":0" : ":") : "")
                                        + duration.Minutes + (duration.Seconds < 10 ? ":0" : ":")
                                        + duration.Seconds,
                                    true
                                    ));
                        }
                    }
                }
            }
            var uriSource = new Uri($"http://192.168.0.120:4040/submissions/pending_album/{AlbumSubmissionID}.jpg", UriKind.Absolute);
            var imgTemp = new BitmapImage();
            imgTemp.BeginInit();
            //Reduces memory usage
            imgTemp.DecodePixelWidth = 200;
            imgTemp.DecodePixelHeight = 200;
            imgTemp.UriSource = uriSource;
            imgTemp.EndInit();
            this.Image_AlbumCover.Source = imgTemp;
        }

        private struct TrackSubmission
        {
            public string TrackOrder;
            public string TrackName;
            public string TrackDuration;
            public string TrackSID;
            public TrackSubmission(string track_order, string track_s_id, string track_name, string track_duration)
            {
                TrackOrder = track_order;
                TrackName = track_name;
                TrackDuration = track_duration;
                TrackSID = track_s_id;
            }
        }

        private void Button_ApproveSubmission_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection SQLConnection = new SqlConnection(SqlHelper.CnnVal("database")))
            {
                int NewID = 0;
                SQLConnection.Open();
                using (SqlCommand command = new SqlCommand($"INSERT INTO album (album_name, artist_id) OUTPUT INSERTED.album_id VALUES (N'{this.TextBlock_AlbumName.Text.Replace("'", "''")}', {ArtistID})", SQLConnection))
                {
                    NewID = (int)command.ExecuteScalar();
                    if (NewID == 0)
                    {
                        _ = new NotificationPopUp("An error occured.").ShowDialog();
                    }
                    using (var client = new WebClient())
                    {
                        client.DownloadFile("http://192.168.0.120:4040/submissions/pending_album/" + AlbumSubmissionID + ".jpg", "temp_albumart.jpg");
                    }

                    _ = HttpHelper.UploadFileAndDelete("http://192.168.0.120:4040/album/", "temp_albumart.jpg", NewID + ".jpg") ;

                    command.CommandText = $"SELECT * FROM track_submission WHERE album_s_id = '{AlbumSubmissionID}' ORDER BY track_order ASC";
                    List<TrackSubmission> TrackSubmissions = new List<TrackSubmission>();
                    using (SqlDataReader SQLDataReader = command.ExecuteReader())
                    {
                        while (SQLDataReader.Read())
                            TrackSubmissions.Add(
                                new TrackSubmission(
                                    SQLDataReader["track_order"].ToString(), 
                                    SQLDataReader["track_s_id"].ToString(), 
                                    SQLDataReader["track_name"].ToString(),
                                    SQLDataReader["track_duration"].ToString()));
                            
                    }

                    TrackSubmissions.ForEach(tracksubmission =>
                    {
                        int TrackID = 0;
                        command.CommandText = "INSERT INTO track (track_order, track_name, track_duration, album_id) OUTPUT INSERTED.track_id VALUES (" +
                                            $"{tracksubmission.TrackOrder}, " +
                                            $"N'{tracksubmission.TrackName.Replace("'", "''")}', " +
                                            $"{tracksubmission.TrackDuration}, " +
                                            $"{NewID})";

                        TrackID = (int)command.ExecuteScalar();
                        if (TrackID > 0)
                        {
                            using (var client = new WebClient())
                            {
                                client.DownloadFile("http://192.168.0.120:4040/submissions/pending_track/" + tracksubmission.TrackSID + ".mp3", tracksubmission.TrackSID + ".mp3");
                            }
                            _ = HttpHelper.UploadFileAndDelete("http://192.168.0.120:4040/track/", tracksubmission.TrackSID + ".mp3", TrackID + ".mp3");
                        }
                    });

                    command.CommandText = $"DELETE FROM album_submission WHERE album_s_id = {AlbumSubmissionID}";
                    int result = command.ExecuteNonQuery();
                    if (result > 0)
                    {
                        HttpHelper.DeleteFile("http://192.168.0.120:4040/submissions/pending_album/" + AlbumSubmissionID + ".jpg");
                        TrackSubmissions.ForEach(
                            tracksubmission => HttpHelper.DeleteFile("http://192.168.0.120:4040/submissions/pending_track/" + tracksubmission.TrackSID + ".mp3"));
                    }
                }
            }
            _ = new NotificationPopUp("Successfully added album submission.").ShowDialog();
        }

        private void Button_DenySubmission_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection SQLConnection = new SqlConnection(SqlHelper.CnnVal("database")))
            {
                SQLConnection.Open();
                using (SqlCommand command = new SqlCommand($"SELECT track_s_id FROM track_submission WHERE album_s_id = '{AlbumSubmissionID}' ORDER BY track_order ASC", SQLConnection))
                {
                    using (SqlDataReader SQLDataReader = command.ExecuteReader())
                    {
                        while (SQLDataReader.Read())
                            HttpHelper.DeleteFile("http://192.168.0.120:4040/submissions/pending_track/" + SQLDataReader["track_s_id"] + ".mp3");

                    }

                    command.CommandText = $"DELETE FROM album_submission WHERE album_s_id = {AlbumSubmissionID}";
                    int result = command.ExecuteNonQuery();
                    if (result > 0)
                    {
                        HttpHelper.DeleteFile("http://192.168.0.120:4040/submissions/pending_album/" + AlbumSubmissionID + ".jpg");
                    }
                }
            }
            _ = new NotificationPopUp("Successfully deleted album submission.").ShowDialog();
        }
    }
}
