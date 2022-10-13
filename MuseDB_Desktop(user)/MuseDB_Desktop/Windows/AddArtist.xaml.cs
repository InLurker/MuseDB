using MuseDB_Desktop.Controls;
using MuseDB_Desktop.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
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
using System.Xml.Linq;
using static System.Net.WebRequestMethods;

namespace MuseDB_Desktop.Windows
{
    /// <summary>
    /// Interaction logic for AddArtist.xaml
    /// </summary>
    public partial class AddArtist : Window
    {
        private string FilePath = "";
        private bool success = false;
        public bool Success => success;

        public AddArtist()
        {
            InitializeComponent();
        }


        private void Button_SelectOnClick(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog FilePicker = new Microsoft.Win32.OpenFileDialog()
            {
                DefaultExt = "JPG Files (*.jpg)|*.jpg",
                Filter = "JPG Files (*.jpg)|*.jpg"
            };

            Nullable<bool> result = FilePicker.ShowDialog();

            if (result == true)
            {
                FilePath = FilePicker.FileName;
                this.Image_Background.Source = this.Image_Icon.Source = new BitmapImage(new Uri(FilePath));
            }
        }

        private void Button_ConfirmOnClick(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(this.TextBox_ArtistName.Text))
            {
                TextBlock_Error.Text = "Please provide the artist's name.";
                return;
            }
            if (String.IsNullOrWhiteSpace(FilePath))
            {
                TextBlock_Error.Text = "Please select an image as artist's profile.";
                return;
            }
            int NewID = 0;
            using (SqlConnection SQLConnection = new SqlConnection(SqlHelper.CnnVal("database")))
            {
                SQLConnection.Open();
                using (SqlCommand command = new SqlCommand($"INSERT INTO artist OUTPUT INSERTED.artist_id VALUES (N'{this.TextBox_ArtistName.Text.Replace("'", "''")}')", SQLConnection))
                {
                    NewID = (int)command.ExecuteScalar();
                    try
                    {
                        HttpHelper.UploadFile("http://192.168.0.120:4040/artist/", FilePath, NewID + ".jpg");
                    }
                    catch (Exception exception)
                    {
                        TextBlock_Error.Text = exception.Message;
                    }
                }
            }
            if(NewID > 0)
            {
                this.Hide();
                _ = new NotificationPopUp("Successfully Added Artist\n" +
                        $"{NewID} - {TextBox_ArtistName.Text}").ShowDialog();
                success = true;
            }
            this.Close();
        }
    }

}
