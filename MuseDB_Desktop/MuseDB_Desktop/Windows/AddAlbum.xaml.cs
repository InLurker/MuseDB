using MuseDB_Desktop.Helpers;
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
        private string FilePath = "";
        public AddAlbum()
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
            using (SqlConnection SQLConnection = new SqlConnection(SqlHelper.CnnVal("database")))
            {
                SQLConnection.Open();
                using (SqlCommand command = new SqlCommand($"INSERT INTO artist OUTPUT INSERTED.album_id  VALUES N'{this.TextBox_ArtistName.Text.Replace("'", "''")}' "))
                    this.Close();
            }
        }
    }

    }
