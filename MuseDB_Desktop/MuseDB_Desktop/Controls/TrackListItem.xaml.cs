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
        public TrackListItem(int TrackOrder, string TrackName, string TrackDuration)
        {
            InitializeComponent();
            this.TextBlock_Order.Text = TrackOrder.ToString();
            this.TextBlock_TrackName.Text = TrackName;
            this.TextBlock_TrackDuration.Text = TrackDuration;
        }
    }
}
