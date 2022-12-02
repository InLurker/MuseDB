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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MuseDB_Desktop.Controls
{
    /// <summary>
    /// Interaction logic for SubmissionEntry.xaml
    /// </summary>
    public partial class SubmissionEntry : UserControl
    {
        public SubmissionEntry(string AlbumSubmissionID, string AlbumName, string ArtistName)
        {
            InitializeComponent();
            this.TextBlock_Username.Text = App.Current.Properties["username"].ToString();
            this.TextBlock_AlbumName.Text = AlbumName;
            this.TextBlock_ArtistName.Text = ArtistName;
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
    }
}
