using MuseDB_Desktop.Controls;
using MuseDB_Desktop.Windows;
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
using System.Net;
using System.Data.SQLite;
using System.IO;
using System.Runtime.CompilerServices;
using System.Collections;

namespace MuseDB_Desktop.Pages
{
    /// <summary>
    /// Interaction logic for Page_YourCollection.xaml
    /// </summary>
    public partial class Page_YourCollection : Page
    {
        private string SortParam = "album_id";
        private string SortOrder = "DESC";
        private string SearchQuery = "";
        private readonly string UserName = "";
        private bool DeleteButtonEnabled = false;

        public Page_YourCollection(string username)
        {
            InitializeComponent();
            UserName = username;
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            LoadUserCollection();
            LoadAlbums();
        }

        private void LoadUserCollection()
        {
            using (var client = new WebClient())
            {
                client.DownloadFile("http://192.168.0.120:4040/user_library/" + UserName + ".db", "temp_userdata.db");
            }
            List<string> AlbumCollection = new List<string>();
            using (var SqliteConnection = new SQLiteConnection("Data Source=temp_userdata.db"))
            {
                SqliteConnection.Open();
                using (var SqliteCommand = new SQLiteCommand("SELECT * FROM album", SqliteConnection))
                {
                    using (var reader = SqliteCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            AlbumCollection.Add(reader.GetInt32(0).ToString());
                        }
                    }
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            File.Delete("temp_userdata.db");
            App.Current.Properties["album_library"] = AlbumCollection;
        }

        private void LoadAlbums()
        {
            if (this.ListBox_Albums == null)
                return;
            if (((List<string>)App.Current.Properties["album_library"]).Count < 1)
                return;
            this.TextBlock_Loading.Text = "Loading...";
            this.ListBox_Albums.Items.Clear();
            using (SqlConnection SQLConnection = new SqlConnection(SqlHelper.CnnVal("database")))
            {
                SQLConnection.Open();
                using (SqlCommand command = new SqlCommand("SELECT " +
                    "COUNT(track_id) AS track_count, album.album_id, album.album_name, artist.artist_name " +
                    "FROM album " +
                    "INNER JOIN artist ON album.artist_id = artist.artist_id " +
                    "LEFT JOIN track ON album.album_id = track.album_id " +
                    $"WHERE album.album_id IN ({string.Join(",", ((List<string>)App.Current.Properties["album_library"]))}) " + 
                    SearchQuery +
                    "GROUP BY album.album_id, album.album_name, artist.artist_name " +
                    $"ORDER BY {SortParam} {SortOrder}", SQLConnection))
                {
                    using (SqlDataReader SQLDataReader = command.ExecuteReader())
                    {
                        while (SQLDataReader.Read())
                            this.ListBox_Albums.Items.Add(
                                new Button_AlbumDetails(
                                    SQLDataReader["album_id"].ToString(),
                                    SQLDataReader["album_name"].ToString(),
                                    SQLDataReader["artist_name"].ToString(),
                                    SQLDataReader.GetInt32(0)));
                    }
                }
            }
            this.TextBlock_Loading.Text = "";
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*
            0 - Recently Added
            1 - Alphabetical
            2 - Album Count
            */
            switch (this.ComboBox_SortParam.SelectedIndex)
            {
                case 0:
                    SortParam = "album.album_id";
                    break;
                case 1:
                    SortParam = "album.album_name";
                    break;
                case 2:
                    SortParam = "artist.artist_name";
                    break;
                case 3:
                    SortParam = "COUNT(track.track_id)";
                    break;
                default:
                    SortParam = "album.album_id";
                    break;
            }
            LoadAlbums();
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
            LoadAlbums();
        }
        
        private void Search_OnClick(object sender, RoutedEventArgs e)
        {
            SearchQuery = String.IsNullOrWhiteSpace(TextBox_SearchQuery.Text) ? "" : $"AND artist_name LIKE N'%{TextBox_SearchQuery.Text.Replace("'", "''")}%' ";
            LoadAlbums();
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
            foreach (Button_AlbumDetails album in ListBox_Albums.Items)
                album.DeleteButton(DeleteButtonEnabled);
        }

        private void TextBox_SearchQuery_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(TextBox_SearchQuery.Text))
            {
                SearchQuery = "";
                LoadAlbums();
            }
        }
    }
}
