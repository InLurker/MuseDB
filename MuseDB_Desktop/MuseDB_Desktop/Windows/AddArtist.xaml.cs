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

namespace MuseDB_Desktop.Windows
{
    /// <summary>
    /// Interaction logic for AddArtist.xaml
    /// </summary>
    public partial class AddArtist : Window
    {
        public AddArtist()
        {
            InitializeComponent();
        }


        private void Button_AddArtistOnClick(object sender, RoutedEventArgs e)
        {
            using (SqlConnection SQLConnection = new SqlConnection(SqlHelper.CnnVal("database")))
            {
                SQLConnection.Open();
                using (SqlCommand command = new SqlCommand($"INSERT INTO artist OUTPUT INSERTED.ID VALUES N'{this.TextBox_ArtistName.Text.Replace("'", "''")}' "))
                {
                    int NewID = (int)command.ExecuteScalar();
                    if (NewID > 0)
                    {
                        this.Label_Result.Content = "sucess";
                        using (var client = new HttpClient())
                        {
                            using (var s = client.GetStreamAsync(this.TextBox_ArtistPfp.Text))
                            {
                                using (var fs = new FileStream($"D:/大学/Projects/MuseDB/Data/artist/{NewID}.jpg", FileMode.OpenOrCreate))
                                {
                                    s.Result.CopyTo(fs);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

}
