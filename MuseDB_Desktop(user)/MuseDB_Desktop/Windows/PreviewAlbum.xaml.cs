using MuseDB_Desktop.Controls;
using MuseDB_Desktop.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Globalization;
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
using System.Windows.Shapes;

namespace MuseDB_Desktop.Windows
{
    /// <summary>
    /// Interaction logic for PreviewAlbum.xaml
    /// </summary>
    public partial class PreviewAlbum : Window
    {
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
            using (SqlConnection SQLConnection = new SqlConnection(SqlHelper.CnnVal("database")))
            {
                SQLConnection.Open();
                using (SqlCommand command = new SqlCommand($"SELECT * FROM album_comment WHERE album_id = {AlbumID} ORDER BY comment_id DESC", SQLConnection))
                {
                    using (SqlDataReader SQLDataReader = command.ExecuteReader())
                    {
                        while (SQLDataReader.Read())
                            ListBox_Comments.Items.Add(
                                new UserComment(
                                    SQLDataReader["username"].ToString(),
                                    SQLDataReader["comment_details"].ToString(),
                                    SQLDataReader["comment_time"].ToString()
                                ));
                    }
                }
            }
            if(ListBox_Comments.Items.Count > 0)
                this.TextBlock_NoComments.Visibility = Visibility.Collapsed;
        }

        private void LoadTracks()
        {
            if (this.ListBox_Tracks == null)
                return;
            if (((List<string>)App.Current.Properties["album_library"]).Contains(AlbumID))
                Button_AddAlbumToLibrary.Visibility = Visibility.Hidden;
            this.TextBlock_Loading.Text = "Loading...";
            this.ListBox_Tracks.Items.Clear();
            using (SqlConnection SQLConnection = new SqlConnection(SqlHelper.CnnVal("database")))
            {
                SQLConnection.Open();
                using (SqlCommand command = new SqlCommand("SELECT " +
                    "track_order, track_id, track_name, track_duration " +
                    "FROM track " +
                    $"WHERE album_id = {AlbumID} " +
                    SearchQuery +
                    $"ORDER BY {SortParam} {SortOrder}", SQLConnection))
                {
                    using (SqlDataReader SQLDataReader = command.ExecuteReader())
                    {
                        while (SQLDataReader.Read()) {
                            TimeSpan duration = TimeSpan.FromSeconds((short)SQLDataReader["track_duration"]);
                            this.ListBox_Tracks.Items.Add(
                                new TrackListItem(
                                    SQLDataReader.GetByte(0),
                                    SQLDataReader.GetInt32(1).ToString(),
                                    SQLDataReader["track_name"].ToString(),
                                    (duration.Hours > 0 ? duration.Hours + (duration.Hours < 10 ? ":0" : ":") : "")
                                        + duration.Minutes + (duration.Seconds < 10 ? ":0" : ":")
                                        + duration.Seconds,
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

        private void Button_PostComment_OnClick(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(TextBox_Comment.Text))
                return;

            using (SqlConnection SQLConnection = new SqlConnection(SqlHelper.CnnVal("database")))
            {
                SQLConnection.Open();
                using (SqlCommand command = new SqlCommand(
                    "INSERT INTO album_comment " +
                    "(album_id, username, comment_details) " +
                    "OUTPUT INSERTED.comment_time " +
                    $"VALUES ({AlbumID}, '{App.Current.Properties["username"].ToString().Replace("'", "''")}', N'{TextBox_Comment.Text.Replace("'", "''")}')",
                    SQLConnection))
                {
                    string CommentTime = command.ExecuteScalar().ToString();
                    if (String.IsNullOrEmpty(CommentTime))
                    {
                        _ = new NotificationPopUp("Failed to post comment.").ShowDialog();
                        return;
                    }
                    TextBlock_NoComments.Visibility = Visibility.Collapsed;
                    ListBox_Comments.Items.Insert(0,
                                        new UserComment(
                                            App.Current.Properties["username"].ToString(),
                                            TextBox_Comment.Text,
                                            CommentTime
                                        ));
                    TextBox_Comment.Text = "";
                }
            }
        }

        private void AddAlbumToLibrary_OnClick(object sender, MouseButtonEventArgs e)
        {
            using (var client = new WebClient())
            {
                client.DownloadFile("http://192.168.0.120:4040/user_library/" + App.Current.Properties["username"] + ".db", "temp_userdata.db");
            }
            using (var SqliteConnection = new SQLiteConnection("Data Source=temp_userdata.db"))
            {
                SqliteConnection.Open();
                using (var SqliteCommand = new SQLiteCommand($"INSERT INTO album VALUES ({AlbumID})", SqliteConnection))
                {
                    int result = SqliteCommand.ExecuteNonQuery();
                    if (result == 0)
                    {
                        _ = new NotificationPopUp("Failed to add album into library.").ShowDialog();
                        return;
                    }
                }
            }
            _ = HttpHelper.ReplaceFileAndDelete("http://192.168.0.120:4040/user_library/", Environment.CurrentDirectory + "\\temp_userdata.db", App.Current.Properties["username"] + ".db");
            ((List<string>)App.Current.Properties["album_library"]).Add(AlbumID);
            Button_AddAlbumToLibrary.Visibility = Visibility.Hidden;
            _ = new NotificationPopUp("Album successfully added to your library.").ShowDialog();
        }
    }
}
