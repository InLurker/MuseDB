using MuseDB_Desktop.Controls;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
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
using Path = System.IO.Path;

namespace MuseDB_Desktop.Windows
{
    /// <summary>
    /// Interaction logic for AddAlbum.xaml
    /// </summary>
    public partial class AddAlbum : Window
    {
        public AddAlbum()
        {
            InitializeComponent();
        }


        private void Button_AddAlbumOnClick(object sender, RoutedEventArgs e)
        {
            using (SqlConnection SQLConnection = new SqlConnection(SqlHelper.CnnVal("database")))
            {
                SQLConnection.Open();
                using (SqlCommand command = new SqlCommand($"INSERT INTO album (album_name, artist_id)  OUTPUT inserted.album_id VALUES (N'{this.TextBox_AlbumName.Text.Replace("'", "''")}', { this.TextBox_ArtistID.Text})", SQLConnection))
                {
                    int NewID = (int)command.ExecuteScalar();
                    if (NewID > 0)
                    {
                        this.Label_Result.Content = "sucess";
                        using (var client = new HttpClient())
                        {
                            using (var stream = client.GetStreamAsync(this.TextBox_AlbumPfp.Text))
                            {
                                string filedir = $"D:/大学/Projects/MuseDB/Data/album/{NewID}/cover.jpg";
                                Directory.CreateDirectory(Path.GetDirectoryName(filedir));
                                using (var filestream = new FileStream(filedir, FileMode.OpenOrCreate))
                                {
                                    stream.Result.CopyTo(filestream);
                                }
                            }
                        }
                        this.TextBox_AlbumName.Text = NewID.ToString();
                        this.TextBox_AlbumPfp.Text = "";

                    } else this.Label_Result.Content = "fail";
                }
            }
        }

    }

}
