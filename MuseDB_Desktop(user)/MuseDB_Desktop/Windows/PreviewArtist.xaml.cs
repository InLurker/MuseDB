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
    /// Interaction logic for PreviewArtist.xaml
    /// </summary>
    public partial class PreviewArtist : Window
    {
        private string SortParam = "album_id";
        private string SortOrder = "DESC";
        private string SearchQuery = "";
        private readonly string ArtistID = "";
        private readonly string ArtistName = "";

        public PreviewArtist(string ArtistID)
        {
            InitializeComponent();
            this.ArtistID = ArtistID;
            using (SqlConnection SQLConnection = new SqlConnection(SqlHelper.CnnVal("database")))
            {
                SQLConnection.Open();
                using (SqlCommand command = new SqlCommand($"SELECT artist_name FROM artist WHERE artist_id = {ArtistID}", SQLConnection))
                {
                    this.ArtistName = (string)command.ExecuteScalar();
                }
            }
            this.TextBlock_ArtistID.Text = "Artist #" + ArtistID;
            this.TextBlock_ArtistName.Text = ArtistName;
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            var uriSource = new Uri($"http://192.168.0.120:4040/artist/{ArtistID}.jpg", UriKind.Absolute);
            var imgTemp = new BitmapImage();
            imgTemp.BeginInit();
            imgTemp.UriSource = uriSource;
            //Reduces memory usage
            imgTemp.DecodePixelWidth = 184;
            imgTemp.DecodePixelHeight = 184;
            imgTemp.CacheOption = BitmapCacheOption.OnLoad;
            imgTemp.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            imgTemp.EndInit();
            this.Image_Profile.Source = imgTemp;
            this.Image_Background.Source = imgTemp;
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
                    "COUNT(track_id) AS track_count, album.album_id, album.album_name " +
                    "FROM album " +
                    "INNER JOIN track ON album.album_id = track.album_id " +
                    $"WHERE artist_id = {ArtistID} " + 
                    SearchQuery +
                    "GROUP BY album.album_id, album.album_name " +
                    $"ORDER BY {SortParam} {SortOrder}", SQLConnection))
                {
                    using (SqlDataReader SQLDataReader = command.ExecuteReader())
                    {
                        for (int i = 0; SQLDataReader.Read(); ++i)
                            this.ListBox_Albums.Items.Add(
                                new Button_AlbumDetails(
                                    SQLDataReader["album_id"].ToString(),
                                    SQLDataReader["album_name"].ToString(),
                                    ArtistName,
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
            SearchQuery = String.IsNullOrWhiteSpace(TextBox_SearchQuery.Text) ? "" : $"AND album_name LIKE N'%{TextBox_SearchQuery.Text.Replace("'", "''")}%' ";
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
