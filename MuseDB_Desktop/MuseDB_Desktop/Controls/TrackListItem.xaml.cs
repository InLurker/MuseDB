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
    /// Interaction logic for TrackListItem.xaml
    /// </summary>
    public partial class TrackListItem : UserControl
    {
        private readonly string TrackID;
        private readonly string TrackName;
        private readonly string AlbumID;
        private readonly bool isMiscellaneous = false;
        public TrackListItem(int TrackOrder, string TrackName, string TrackDuration, bool isMiscellenous)
        {
            InitializeComponent();
            this.isMiscellaneous = isMiscellenous;
            this.TextBlock_Order.Text = TrackOrder.ToString();
            this.TextBlock_TrackName.Text = TrackName;
            this.TextBlock_TrackDuration.Text = TrackDuration;
        }

        public TrackListItem(int TrackOrder, string TrackID, string TrackName, string TrackDuration, string AlbumID, bool isMiscellenous) :
            this(TrackOrder, TrackName, TrackDuration, isMiscellenous)
        {
            this.TrackID = TrackID;
            this.TrackName = TrackName;
            this.AlbumID = AlbumID;
        }

        public void DeleteButton(bool reveal)
        {
            this.Button_Delete.Visibility = (reveal) ? Visibility.Visible : Visibility.Collapsed;
        }


        private void Delete_OnClick(object sender, RoutedEventArgs e)
        {
            if (isMiscellaneous)
                return;
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
                if (this.Parent is ListBox)
                    (this.Parent as ListBox).Items.Remove(this);
            }
        }

        private void OnDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!isMiscellaneous)
                _ = new PreviewTrack(TrackID).ShowDialog();
        }
    }
}
