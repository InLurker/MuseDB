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
    /// Interaction logic for Button_AlbumDetails.xaml
    /// </summary>
    public partial class Button_AlbumDetails : UserControl
    {
        public Button_AlbumDetails()
        {
            InitializeComponent();
        }

        public Button_AlbumDetails(string AlbumID, string AlbumName, string ArtistName, int AlbumCount)
        {
            InitializeComponent();
            this.TextBlock_AlbumName.Text = AlbumName;
            this.TextBlock_ArtistName.Text = ArtistName;
            this.TextBlock_TrackCount.Text = AlbumCount.ToString();
            this.AlbumIcon.AlbumID = AlbumID;
        }
        private void Delete_OnClick(object sender, MouseButtonEventArgs e)
        {
            var Confirmation = new ConfirmationPopUp($"#{AlbumIcon.AlbumID} - {TextBlock_AlbumName.Text}'s data, along with its tracks will be deleted.\nAre you sure?");
            Confirmation.ShowDialog();
            if (Confirmation.ConfirmResult)
            {
                using (SqlConnection SQLConnection = new SqlConnection(SqlHelper.CnnVal("database")))
                {
                    SQLConnection.Open();
                    using (SqlCommand command = new SqlCommand($"SELECT track_id FROM track WHERE album_id = {AlbumIcon.AlbumID}", SQLConnection))
                    {
                        using (SqlDataReader SQLDataReader = command.ExecuteReader())
                        {
                            while (SQLDataReader.Read())
                            {
                                HttpHelper.DeleteFile($"http://192.168.0.120:4040/track/{SQLDataReader.GetInt32(0)}.mp3");
                            }

                        }
                        command.CommandText = $"DELETE FROM album WHERE album_id = {AlbumIcon.AlbumID}";
                        int result = (int)command.ExecuteNonQuery();
                        if (result > 0)
                        {
                            HttpHelper.DeleteFile($"http://192.168.0.120:4040/album/{AlbumIcon.AlbumID}.jpg");
                            _ = new NotificationPopUp($"#{AlbumIcon.AlbumID} - {TextBlock_AlbumName.Text}'s data,\nalong with its tracks, has been deleted.").ShowDialog();
                        }
                    }
                }
                if (this.Parent is ListBox)
                    (this.Parent as ListBox).Items.Remove(this);
            }
        }

        public void DeleteButton(bool reveal)
        {
            this.Button_Delete.Visibility = (reveal) ? Visibility.Visible : Visibility.Collapsed;
        }

    }
}
