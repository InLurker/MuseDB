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

namespace MuseDB_Desktop.Pages
{
    /// <summary>
    /// Interaction logic for Page_Home.xaml
    /// </summary>
    public partial class Page_Home : Page
    {
        public Page_Home()
        {
            InitializeComponent();
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            using (SqlConnection SQLConnection = new SqlConnection(SqlHelper.CnnVal("database")))
            {
                SQLConnection.Open();
                using (SqlCommand command = new SqlCommand("SELECT TOP 6 artist_id FROM artist ORDER BY artist_id DESC", SQLConnection))
                {
                    using (SqlDataReader SQLDataReader = command.ExecuteReader())
                    {
                        for (int i = 0; SQLDataReader.Read(); ++i)
                            DockPanel_Artists.Children.Add(new Button_ArtistIcon(SQLDataReader.GetInt32(0).ToString()));
                    }

                    command.CommandText = "SELECT TOP 5 album_id, artist_id FROM album ORDER BY album_id DESC";
                    using (SqlDataReader SQLDataReader = command.ExecuteReader())
                    {
                        for (int i = 0; SQLDataReader.Read(); ++i)
                            DockPanel_Albums.Children.Add(new Button_AlbumIcon(SQLDataReader["album_id"].ToString()));
                    }

                    command.CommandText = "SELECT TOP 5 track_id, track_name, artist_name, album_name, track_duration " +
                        "FROM track " +
                        "INNER JOIN album ON track.album_id = album.album_id " +
                        "INNER JOIN artist ON album.artist_id = artist.artist_id " +
                        "ORDER BY track_id DESC";
                    using (SqlDataReader SQLDataReader = command.ExecuteReader())
                    {
                        for (int i = 0; SQLDataReader.Read(); ++i)
                            StackPanel_Tracks.Children.Add(
                                new Button_Tracks(
                                    SQLDataReader["track_id"].ToString(),
                                    SQLDataReader["track_name"].ToString(),
                                    SQLDataReader["artist_name"].ToString(),
                                    SQLDataReader["album_name"].ToString(),
                                    SQLDataReader["track_duration"].ToString()
                                    ));
                    }
                }
            }
        }
    }
}
