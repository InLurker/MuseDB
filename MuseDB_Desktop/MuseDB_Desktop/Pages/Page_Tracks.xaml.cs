using MuseDB_Desktop.Helpers;
using MuseDB_Desktop.Controls;
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
using MuseDB_Desktop.Windows;

namespace MuseDB_Desktop.Pages
{
    /// <summary>
    /// Interaction logic for Page_Tracks.xaml
    /// </summary>
    public partial class Page_Tracks : Page
    {
        private bool DeleteButtonEnabled = false;

        private string SortParam = "album_name";
        private string SortOrder = "DESC";

        private string SearchQuery = "";

        public Page_Tracks()
        {
            InitializeComponent();
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            LoadTracks();
        }

        private void LoadTracks()
        {
            if (this.ListBox_Tracks == null)
                return;
            this.ListBox_Tracks.Items.Clear();
            using (SqlConnection SQLConnection = new SqlConnection(SqlHelper.CnnVal("database")))
            {
                SQLConnection.Open();
                using (SqlCommand command = new SqlCommand($"SELECT " +
                        "track_id, track_name, artist_name, track.album_id, album_name, track_duration " +
                        "FROM track " +
                        "INNER JOIN album ON track.album_id = album.album_id " +
                        "INNER JOIN artist ON album.artist_id = artist.artist_id " +
                        SearchQuery +
                        $"ORDER BY {SortParam} {SortOrder}", SQLConnection))
                {
                    using (SqlDataReader SQLDataReader = command.ExecuteReader())
                    {
                        for (int i = 0; SQLDataReader.Read(); ++i)
                            ListBox_Tracks.Items.Add(
                                new Button_TrackDetails(
                                    SQLDataReader["track_id"].ToString(),
                                    SQLDataReader["track_name"].ToString(),
                                    SQLDataReader["artist_name"].ToString(),
                                    SQLDataReader["album_id"].ToString(),
                                    SQLDataReader["album_name"].ToString(),
                                    SQLDataReader["track_duration"].ToString()
                                    ));
                    }
                }
            }
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*
            0 - Recently Added
            1 - Alphabetical
            2 - Album Name
            3 - Artist Name
            */
            switch (this.ComboBox_SortParam.SelectedIndex)
            {
                case 0:
                    SortParam = "track_id";
                    break;
                case 1:
                    SortParam = "track_name";
                    break;
                case 2:
                    SortParam = "album_name";
                    break;
                case 3:
                    SortParam = "artist_name";
                    break;
                default:
                    SortParam = "track_name";
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
            var AddPopUp = new AddTrack();
            AddPopUp.ShowDialog();
            if (AddPopUp.Success)
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
            foreach (Button_TrackDetails Track in ListBox_Tracks.Items)
                Track.DeleteButton(DeleteButtonEnabled);
        }

        private void Search_OnClick(object sender, RoutedEventArgs e)
        {
            SearchQuery = String.IsNullOrWhiteSpace(TextBox_SearchQuery.Text) ? "" : $"WHERE Track_name LIKE N'%{TextBox_SearchQuery.Text.Replace("'", "''")}%'";
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
    }
}
