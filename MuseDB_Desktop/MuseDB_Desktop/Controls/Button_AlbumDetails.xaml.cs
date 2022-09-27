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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MuseDB_Desktop.Controls
{
    /// <summary>
    /// Interaction logic for Button_AlbumDetails.xaml
    /// </summary>
    public partial class Button_AlbumDetails : UserControl
    {
        public Button_AlbumDetails()
        {
            InitializeComponent();
        }

        public Button_AlbumDetails(string AlbumID, string AlbumName, string ArtistName, int AlbumCount)
        {
            InitializeComponent();
            this.TextBlock_AlbumName.Text = AlbumName;
            this.TextBlock_ArtistName.Text = ArtistName;
            this.TextBlock_TrackCount.Text = AlbumCount.ToString();
            this.AlbumIcon.AlbumID = AlbumID;
        }
        private void Delete_OnClick(object sender, MouseButtonEventArgs e)
        {
            if (this.Parent is ListBox)
                (this.Parent as ListBox).Items.Remove(this);
        }

        public void DeleteButton(bool reveal)
        {
            this.Button_Delete.Visibility = (reveal) ? Visibility.Visible : Visibility.Collapsed;
        }

    }
}
