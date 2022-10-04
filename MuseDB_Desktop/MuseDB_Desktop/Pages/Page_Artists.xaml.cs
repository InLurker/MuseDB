using MuseDB_Desktop.Controls;
using MuseDB_Desktop.Windows;
using MuseDB_Desktop.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for Page_Artists.xaml
    /// </summary>
    public partial class Page_Artists : Page
    {
        private string SortParam = "artist_id";
        private string SortOrder = "DESC";
        private bool DeleteButtonEnabled = false;
        private string SearchQuery = "";

        public Page_Artists()
        {
            InitializeComponent();
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            LoadArtists();
        }

        private void LoadArtists()
        {
            if (this.ListBox_Artists == null)
                return;
            this.TextBlock_Loading.Text = "Loading...";
            this.ListBox_Artists.Items.Clear();
            using (SqlConnection SQLConnection = new SqlConnection(SqlHelper.CnnVal("database")))
            {
                SQLConnection.Open();
                using (SqlCommand command = new SqlCommand("SELECT " +
                    "COUNT(album_id) AS album_count, artist.artist_id, artist.artist_name " +
                    "FROM artist " +
                    "LEFT JOIN album ON artist.artist_id = album.artist_id " +
                    SearchQuery +
                    "GROUP BY artist.artist_id, artist.artist_name " +
                    $"ORDER BY {SortParam} {SortOrder}", SQLConnection))
                {
                    using (SqlDataReader SQLDataReader = command.ExecuteReader())
                    {
                        for (int i = 0; SQLDataReader.Read(); ++i)
                            this.ListBox_Artists.Items.Add(
                                new Button_ArtistDetails(
                                    SQLDataReader["artist_id"].ToString(),
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
                    SortParam = "artist_id";
                    break;
                case 1:
                    SortParam = "artist_name";
                    break;
                case 2:
                    SortParam = "COUNT(album_id)";
                    break;
                default:
                    SortParam = "artist_id";
                    break;
            }
            LoadArtists();
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
            LoadArtists();
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
            var AddPopUp = new AddArtist();
            AddPopUp.ShowDialog();
            if (AddPopUp.Success)
                LoadArtists();
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
            foreach (Button_ArtistDetails artist in ListBox_Artists.Items)
                artist.DeleteButton(DeleteButtonEnabled);
        }

        private void Search_OnClick(object sender, RoutedEventArgs e)
        {
            SearchQuery = String.IsNullOrWhiteSpace(TextBox_SearchQuery.Text) ? "" : $"WHERE artist_name LIKE N'%{TextBox_SearchQuery.Text.Replace("'", "''")}%'";
            LoadArtists();
        }

        private void TextBox_SearchQuery_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(TextBox_SearchQuery.Text))
            {
                SearchQuery = "";
                LoadArtists();
            }
        }
    }
}
