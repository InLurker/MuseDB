using MuseDB_Desktop.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
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
    /// Interaction logic for AddTrack.xaml
    /// </summary>
    public partial class AddTrack : Window
    {
        public AddTrack()
        {
            InitializeComponent();
        }

        private void Button_AddTrackOnClick(object sender, RoutedEventArgs e)
        {
            using (SqlConnection SQLConnection = new SqlConnection(SqlHelper.CnnVal("database")))
            {
                SQLConnection.Open();
                using (SqlCommand command = new SqlCommand($"INSERT INTO track (track_name, track_order, track_duration, album_id) "
                                                + $"VALUES (N'{this.TextBox_TrackName.Text.Replace("'", "''")}')"))
                {
                    Console.WriteLine(command.CommandText);
                    int NewID = (int)command.ExecuteScalar();
                    if (NewID > 0)
                    {
                        this.Label_Result.Content = "sucess";
                        using (var client = new HttpClient())
                        {
                            
                        }
                    }
                }
            }
        }
    }
}
