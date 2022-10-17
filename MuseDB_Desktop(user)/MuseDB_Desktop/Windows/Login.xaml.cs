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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SQLite;
using System.IO;

namespace MuseDB_Desktop.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void Btn_Login_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidInput()) //if input invalid, return early
                return;

            using (SqlConnection SQLConnection = new SqlConnection(SqlHelper.CnnVal("database")))
            {
                string Query = $"SELECT COUNT(username) FROM users WHERE username COLLATE Latin1_General_CS_AS = '{TextBox_Username.Text}'";
                SqlCommand command = new SqlCommand(Query, SQLConnection);

                SQLConnection.Open();
                int count = (int)command.ExecuteScalar();
                if (count != 0)
                {
                    command.CommandText = $"SELECT COUNT(username) FROM users WHERE username COLLATE Latin1_General_CS_AS = '{TextBox_Username.Text}' AND password COLLATE Latin1_General_CS_AS = '{PasswordBox_Password.Password}'";
                    count = (int)command.ExecuteScalar();
                    if (count != 0)
                    {
                        Label_ErrorMsg.Content = "Successfully logged in.";
                        Label_ErrorMsg.Foreground = Brushes.Green;
                        MainMenu mainmenu = new MainMenu(TextBox_Username.Text);
                        this.Close();
                        mainmenu.ShowDialog();
                    } else
                    {
                        Label_ErrorMsg.Content = "Invalid password.";
                        Label_ErrorMsg.Foreground = Brushes.Red;
                    }
                }
                else
                {
                    Label_ErrorMsg.Content = "Account doesn't exist.";
                    Label_ErrorMsg.Foreground = Brushes.Red;
                }
            }

        }

        private void Btn_Register_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidInput()) //if input invalid, return early
                return;

            using (SqlConnection SQLConnection = new SqlConnection(SqlHelper.CnnVal("database")))
            {
                string Query = $"SELECT COUNT(username) FROM users WHERE username = '{TextBox_Username.Text}'";
                SqlCommand command = new SqlCommand(Query, SQLConnection);

                SQLConnection.Open();
                int count = (int)command.ExecuteScalar();
                if (count == 0)
                {
                    Query = $"INSERT INTO users (username, password) VALUES ('{TextBox_Username.Text}', '{PasswordBox_Password.Password}')";
                    command.CommandText = Query;
                    command.ExecuteNonQuery();

                    var SqliteConnection = new SQLiteConnection("Data Source=temp.db");
                    SqliteConnection.Open();
                    using (var SqliteCommand = new SQLiteCommand(
                        "CREATE TABLE IF NOT EXISTS album (" +
                            "album_id INT NOT NULL PRIMARY KEY)", SqliteConnection))
                    {
                        SqliteCommand.ExecuteNonQuery();
                    }
                    SqliteConnection.Close();
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    HttpHelper.UploadFileAndDelete("http://192.168.0.120:4040/user_library/", Environment.CurrentDirectory + "\\temp.db", TextBox_Username.Text + ".db");
                    Label_ErrorMsg.Content = "Successfully registered.";
                    Label_ErrorMsg.Foreground = Brushes.Green;
                }
                else
                {
                    Label_ErrorMsg.Content = "Username is already taken.";
                    Label_ErrorMsg.Foreground = Brushes.Red;
                }
            }
        }

        private bool ValidInput()
        {
            if (String.IsNullOrWhiteSpace(TextBox_Username.Text) || String.IsNullOrWhiteSpace(PasswordBox_Password.Password))
            {
                Label_ErrorMsg.Content = "Please enter your username and password.";
                Label_ErrorMsg.Foreground = Brushes.Red;
                return false;
            }
            return true;
        }
    }
}
