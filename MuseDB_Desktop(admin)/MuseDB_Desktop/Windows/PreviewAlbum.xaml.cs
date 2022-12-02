using MuseDB_Desktop.Controls;
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
    /// Interaction logic for PreviewAlbum.xaml
    /// </summary>
    public partial class PreviewAlbum : Window
    {
        private bool DeleteButtonEnabled = false;

        private string SortParam = "track_order";
        private string SortOrder = "ASC";
        private string SearchQuery = "";
        private readonly string AlbumID = "";
        private readonly string AlbumName = "";

        public PreviewAlbum(string AlbumID)
        {
            InitializeComponent();
            this.AlbumID = AlbumID;
            using (SqlConnection SQLConnection = new SqlConnection(SqlHelper.CnnVal("database")))
            {
                SQLConnection.Open();
                using (SqlCommand command = new SqlCommand($"SELECT album_name, artist_id, last_playback FROM album WHERE album_id = {AlbumID}", SQLConnection))
                {
                    string ArtistID;
                    using (SqlDataReader SQLDataReader = command.ExecuteReader())
                    {
                        SQLDataReader.Read();
                        this.AlbumName = SQLDataReader["album_name"].ToString();
                        ArtistID = SQLDataReader["artist_id"].ToString();
                        this.TextBlock_LastPlayback.Text = (String.IsNullOrEmpty(SQLDataReader["last_playback"].ToString()) ? "Not played yet." : "Last played " + SQLDataReader["last_playback"].ToString());
                    }
                    command.CommandText = $"SELECT artist_name FROM artist WHERE artist_id = {ArtistID}";
                    this.TextBlock_ArtistName.Text = (string)command.ExecuteScalar();
                }
            }
            this.TextBlock_AlbumID.Text = "Album #" + AlbumID;
            this.TextBlock_AlbumName.Text = AlbumName;
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
            LoadTracks();
        }

        private void LoadTracks()
        {
            if (this.ListBox_Tracks == null)
                return;
            this.TextBlock_Loading.Text = "Loading...";
            this.ListBox_Tracks.Items.Clear();
            using (SqlConnection SQLConnection = new SqlConnection(SqlHelper.CnnVal("database")))
            {
                SQLConnection.Open();
                using (SqlCommand command = new SqlCommand("SELECT " +
                    "track_order, track_id, track_name, album_id, track_duration " +
                    "FROM track " +
                    $"WHERE album_id = {AlbumID} " +
                    SearchQuery +
                    $"ORDER BY {SortParam} {SortOrder}", SQLConnection))
                {
                    using (SqlDataReader SQLDataReader = command.ExecuteReader())
                    {
                        for (int i = 0; SQLDataReader.Read(); ++i)
                        {
                            TimeSpan duration = TimeSpan.FromSeconds((short)SQLDataReader["track_duration"]);
                            this.ListBox_Tracks.Items.Add(
                                new TrackListItem(
                                    SQLDataReader.GetByte(0),
                                    SQLDataReader.GetInt32(1).ToString(),
                                    SQLDataReader["track_name"].ToString(),
                                    (duration.Hours > 0 ? duration.Hours + (duration.Hours < 10 ? ":0" : ":") : "")
                                        + duration.Minutes + (duration.Seconds < 10 ? ":0" : ":")
                                        + duration.Seconds,
                                    SQLDataReader.GetInt32(3).ToString(),
                                    false));
                        }
                    }
                }
            }
            this.TextBlock_Loading.Text = "";
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*
            0 - Track Order
            1 - Recently Added
            2 - Alphabetical
            */
            switch (this.ComboBox_SortParam.SelectedIndex)
            {
                case 0:
                    SortParam = "track_order";
                    break;
                case 1:
                    SortParam = "track_id";
                    break;
                case 2:
                    SortParam = "track_name";
                    break;
                default:
                    SortParam = "track_order";
                    break;
            }
            LoadTracks();
        }

        private void Button_Sort_OnClick(object sender, RoutedEventArgs e)
        {
            //if initial is asc change value to desc, vice versa
            if (SortOrder == "ASC")
            {
                SortOrder = "DESC";
                this.Button_Sort_Image.RenderTransform = new ScaleTransform() { ScaleY = -1 };
                this.Button_Sort_Image.UpdateLayout();
            }
            else
            {
                SortOrder = "ASC";
                this.Button_Sort_Image.RenderTransform = new ScaleTransform() { ScaleY = 1 };
                this.Button_Sort_Image.UpdateLayout();
            }
            LoadTracks();
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
            var AddTrack = new AddTrack(AlbumID, $"http://192.168.0.120:4040/album/{AlbumID}.jpg", false);
            AddTrack.ShowDialog();
            if (AddTrack.Success)
                LoadTracks();
        }
        private void Delete_OnHover(object sender, RoutedEventArgs e)
        {
            this.Button_Delete.Opacity = 1;
        }

        private void Delete_OnLeave(object sender, RoutedEventArgs e)
        {
            this.Button_Delete.Opacity = (DeleteButtonEnabled) ? 1 : 0.2;
        }

        private void Delete_OnClick(object sender, RoutedEventArgs e)
        {
            DeleteButtonEnabled = !DeleteButtonEnabled;
            foreach (TrackListItem track in ListBox_Tracks.Items)
                track.DeleteButton(DeleteButtonEnabled);
        }
        private void Search_OnClick(object sender, RoutedEventArgs e)
        {
            SearchQuery = String.IsNullOrWhiteSpace(TextBox_SearchQuery.Text) ? "" : $"AND track_name LIKE N'%{TextBox_SearchQuery.Text.Replace("'", "''")}%' ";
            LoadTracks();
        }

        private void TextBox_SearchQuery_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(TextBox_SearchQuery.Text))
            {
                SearchQuery = "";
                LoadTracks();
            }
        }

        private void DeleteAlbum_OnClick(object sender, MouseButtonEventArgs e)
        {
            var Confirmation = new ConfirmationPopUp($"#{AlbumID} - {AlbumName}'s data, along with its tracks will be deleted.\nAre you sure?");
            Confirmation.ShowDialog();
            if (Confirmation.ConfirmResult)
            {
                using (SqlConnection SQLConnection = new SqlConnection(SqlHelper.CnnVal("database")))
                {
                    SQLConnection.Open();
                    using (SqlCommand command = new SqlCommand($"SELECT track_id FROM track WHERE album_id = {AlbumID}", SQLConnection))
                    {
                        using (SqlDataReader SQLDataReader = command.ExecuteReader())
                        {
                            while (SQLDataReader.Read())
                            {
                                HttpHelper.DeleteFile($"http://192.168.0.120:4040/track/{SQLDataReader.GetInt32(0)}.mp3");
                            }

                        }
                        command.CommandText = $"DELETE FROM album WHERE album_id = {AlbumID}";
                        int result = (int)command.ExecuteNonQuery();
                        if (result > 0)
                        {
                            HttpHelper.DeleteFile($"http://192.168.0.120:4040/album/{AlbumID}.jpg");
                            _ = new NotificationPopUp($"#{AlbumID} - {AlbumName}'s data,\nalong with its tracks, has been deleted.").ShowDialog();
                        }
                    }
                }
                this.Close();
            }
        }
    }
}
