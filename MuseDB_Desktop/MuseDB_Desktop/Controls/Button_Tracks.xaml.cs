using MuseDB_Desktop.Windows;
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
    /// Interaction logic for Button_Tracks.xaml
    /// </summary>
    public partial class Button_Tracks : UserControl
    {
        public string TrackID;

        public Button_Tracks()
        {
            InitializeComponent();
        }

        public Button_Tracks(string TrackID, string TrackName, string TrackArtist, string TrackAlbum, string TrackDuration)
        {
            InitializeComponent();
            this.TrackID = TrackID;
            TextBlock_TrackName.Text = TrackName;
            TextBlock_TrackArtist.Text = TrackArtist + " - " + TrackAlbum;
            Label_TrackDuration.Content = TrackDuration;
        }

        private void OnDoubleClick(object sender, MouseButtonEventArgs e)
        {
            _ = new PreviewTrack(TrackID).ShowDialog();
        }
    }
}
