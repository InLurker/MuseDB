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

namespace MuseDB_Desktop.Pages
{
    /// <summary>
    /// Interaction logic for Page_Albums.xaml
    /// </summary>
    public partial class Page_Albums : Page
    {
        private string SortParam = "album_id";
        private string SortOrder = "DESC";
        private string SearchQuery = "";

        public Page_Albums()
        {
            InitializeComponent();
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            LoadAlbums();
        }

        private void LoadAlbums()
        {
            if (this.ListBox_Albums == null)
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
            SearchQuery = String.IsNullOrWhiteSpace(TextBox_SearchQuery.Text) ? "" : $"WHERE artist_name LIKE N'%{TextBox_SearchQuery.Text.Replace("'", "''")}%'";
            LoadAlbums();
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
