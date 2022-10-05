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
    /// Interaction logic for Button_ArtistDetails.xaml
    /// </summary>
    public partial class Button_ArtistDetails : UserControl
    {
        public Button_ArtistDetails()
        {
            InitializeComponent();
        }

        public Button_ArtistDetails(string ArtistID, string ArtistName, int AlbumCount)
        {
            InitializeComponent();
            this.TextBlock_ArtistName.Text = ArtistName;
            this.TextBlock_AlbumCount.Text = (AlbumCount == 1) ? "1 album" : AlbumCount + " albums";
            this.ArtistIcon.ArtistID = ArtistID;
        }

        private void Delete_OnClick(object sender, RoutedEventArgs e)
        {
            var Confirmation = new ConfirmationPopUp($"#{ArtistIcon.ArtistID} - {TextBlock_ArtistName.Text}'s profile,\nalong with their albums will be deleted.\nAre you sure?");
            Confirmation.ShowDialog();
            if (Confirmation.ConfirmResult == false)
                return;
            using (SqlConnection SQLConnection = new SqlConnection(SqlHelper.CnnVal("database")))
            {
                SQLConnection.Open();
                using (SqlCommand command = new SqlCommand($"SELECT album_id FROM album WHERE artist_id = {ArtistIcon.ArtistID}", SQLConnection))
                {
                    var AlbumList = new List<int>();
                    using (SqlDataReader SQLDataReader = command.ExecuteReader())
                    {
                        while (SQLDataReader.Read())
                        {
                            AlbumList.Add(SQLDataReader.GetInt32(0));
                        }

                    }
                    AlbumList.ForEach(Album =>
                    {
                        HttpHelper.DeleteFile($"http://192.168.0.120:4040/album/{Album}.jpg");
                        command.CommandText = $"SELECT track_id FROM track WHERE album_id = {Album}";
                        {
                            using (SqlDataReader TrackDataReader = command.ExecuteReader())
                            {
                                while (TrackDataReader.Read())
                                {
                                    HttpHelper.DeleteFile($"http://192.168.0.120:4040/track/{TrackDataReader.GetInt32(0)}.mp3");
                                }
                            }
                        }
                    });
                    command.CommandText = $"DELETE FROM artist WHERE artist_id = {ArtistIcon.ArtistID}";
                    int result = (int)command.ExecuteNonQuery();
                    if (result > 0)
                    {
                        _ = new NotificationPopUp($"#{ArtistIcon.ArtistID} - {TextBlock_ArtistName.Text}'s profile,\nalong with their albums, has been deleted.").ShowDialog();
                    }
                }
            }
            if (this.Parent is ListBox)
                (this.Parent as ListBox).Items.Remove(this);
        }

        public void DeleteButton(bool reveal)
        {
            this.Button_Delete.Visibility = (reveal) ? Visibility.Visible : Visibility.Collapsed;
        }


    }
}
