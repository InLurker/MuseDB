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
    /// Interaction logic for TrackListItem.xaml
    /// </summary>
    public partial class TrackListItem : UserControl
    {
        private string TrackID;
        private bool isMiscellaneous = false;
        public TrackListItem(int TrackOrder, string TrackName, string TrackDuration, bool isMiscellenous)
        {
            InitializeComponent();
            this.isMiscellaneous = isMiscellenous;
            this.TextBlock_Order.Text = TrackOrder.ToString();
            this.TextBlock_TrackName.Text = TrackName;
            this.TextBlock_TrackDuration.Text = TrackDuration;
        }

        public TrackListItem(int TrackOrder, string TrackID, string TrackName, string TrackDuration, bool isMiscellenous) :
            this(TrackOrder, TrackName, TrackDuration, isMiscellenous)
        {
            this.TrackID = TrackID;
        }

        public void DeleteButton(bool reveal)
        {
            this.Button_Delete.Visibility = (reveal) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void Delete_OnClick(object sender, RoutedEventArgs e)
        {
            if (this.Parent is ListBox)
                (this.Parent as ListBox).Items.Remove(this);
        }

        private void OnDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!isMiscellaneous)
                _ = new PreviewTrack(TrackID).ShowDialog();
        }
    }
}
