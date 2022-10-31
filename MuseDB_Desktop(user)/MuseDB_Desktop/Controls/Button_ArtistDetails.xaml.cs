using MuseDB_Desktop.Helpers;
using MuseDB_Desktop.Windows;
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

namespace MuseDB_Desktop.Controls
{
    /// <summary>
    /// Interaction logic for Button_ArtistDetails.xaml
    /// </summary>
    public partial class Button_ArtistDetails : UserControl
    {
        public Button_ArtistDetails()
        {
            InitializeComponent();
        }

        public Button_ArtistDetails(string ArtistID, string ArtistName, int AlbumCount)
        {
            InitializeComponent();
            this.TextBlock_ArtistName.Text = ArtistName;
            this.TextBlock_AlbumCount.Text = (AlbumCount == 1) ? "1 album" : AlbumCount + " albums";
            this.ArtistIcon.ArtistID = ArtistID;
        }
    }
}
