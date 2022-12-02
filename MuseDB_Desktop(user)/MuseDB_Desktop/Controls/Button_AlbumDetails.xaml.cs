using MuseDB_Desktop.Helpers;
using MuseDB_Desktop.Windows;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Net;
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
            var Confirmation = new ConfirmationPopUp($"#{AlbumIcon.AlbumID} - {TextBlock_AlbumName.Text} will be deleted from your collection.\nAre you sure?");
            Confirmation.ShowDialog();
            if (Confirmation.ConfirmResult)
            {
                using (var client = new WebClient())
                {
                    client.DownloadFile("http://192.168.0.120:4040/user_library/" + App.Current.Properties["username"] + ".db", "temp_userdata.db");
                }
                using (var SqliteConnection = new SQLiteConnection("Data Source=temp_userdata.db"))
                {
                    SqliteConnection.Open();
                    using (var SqliteCommand = new SQLiteCommand($"DELETE FROM album WHERE album_id = {AlbumIcon.AlbumID}", SqliteConnection))
                    {
                        int result = (int)SqliteCommand.ExecuteNonQuery();
                        if (result == 0)
                        {
                            _ = new NotificationPopUp($"Failed to remove #{AlbumIcon.AlbumID} - {TextBlock_AlbumName.Text}.").ShowDialog();
                            return;
                        }
                    }
                }
                _ = HttpHelper.ReplaceFileAndDelete("http://192.168.0.120:4040/user_library/", Environment.CurrentDirectory + "\\temp_userdata.db", App.Current.Properties["username"] + ".db");
                ((List<string>)App.Current.Properties["album_library"]).Remove(((List<string>)App.Current.Properties["album_library"]).Find(item => item == AlbumIcon.AlbumID));
                _ = new NotificationPopUp($"#{AlbumIcon.AlbumID} - {TextBlock_AlbumName.Text}\nhas been removed from your collection.").ShowDialog();
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
