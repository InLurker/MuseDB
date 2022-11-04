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
                using (SqlCommand command = new SqlCommand($"SELECT album_name, artist_id FROM album WHERE album_id = {AlbumID}", SQLConnection))
                {
                    string ArtistID;
                    using (SqlDataReader SQLDataReader = command.ExecuteReader())
                    {
                        SQLDataReader.Read();
                        this.AlbumName = SQLDataReader["album_name"].ToString();
                        ArtistID = SQLDataReader["artist_id"].ToString();
                    }
                    command.CommandText = $"SELECT artist_name FROM artist WHERE artist_id = {ArtistID}";
                    this.TextBlock_ArtistName.Text = (string)command.ExecuteScalar();
                    command.CommandText = $"SELECT artist_name FROM artist WHERE artist_id = {ArtistID}";
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
                            this.ListBox_Tracks.Items.Add(
                                new TrackListItem(
                                    SQLDataReader.GetByte(0),
                                    SQLDataReader.GetInt32(1).ToString(),
                                    SQLDataReader["track_name"].ToString(),
                                    SQLDataReader["track_duration"].ToString(),
                                    SQLDataReader.GetInt32(3).ToString(),
                                    false));
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

        }
    }
}
