using System;
using System.Collections.Generic;
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
using MuseDB_Desktop.Pages;

namespace MuseDB_Desktop.Windows
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Window
    {
        public MainMenu(string UserName)
        {
            InitializeComponent();
            this.TextBlock_UserName.Text = UserName;
            this.Frame_PageLoader.Content = new Page_YourCollection(UserName);
        }
        private void Load_Home(object sender, RoutedEventArgs e)
        {
            this.Label_PageTitle.Content = "Your Collection";
            Load_Page(new Page_YourCollection(TextBlock_UserName.Text));
        }

        private void Load_Artists(object sender, RoutedEventArgs e)
        {
            this.Label_PageTitle.Content = "Artists";
            Load_Page(new Page_Artists());
        }

        private void Load_Albums(object sender, RoutedEventArgs e)
        {
            this.Label_PageTitle.Content = "Albums";
            Load_Page(new Page_Albums());
        }

        private void Load_Tracks(object sender, RoutedEventArgs e)
        {
            this.Label_PageTitle.Content = "Tracks";
            Load_Page(new Page_Tracks());
        }

        private void Load_Submissions(object sender, RoutedEventArgs e)
        {

        }
        private void Load_Comments(object sender, RoutedEventArgs e)
        {

        }

        private void Load_Page(Page newpage)
        {
            this.Frame_PageLoader.Content = newpage;
            while (Frame_PageLoader.NavigationService.CanGoBack)
            {
                try
                {
                    Frame_PageLoader.NavigationService.RemoveBackEntry();
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    break;
                }
            }
        }
    }
}
