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

namespace MuseDB_Desktop.Windows
{
    /// <summary>
    /// Interaction logic for ConfirmationPopUp.xaml
    /// </summary>
    public partial class ConfirmationPopUp : Window
    {
        private bool Confirm = false;
        public bool ConfirmResult => Confirm;
        public ConfirmationPopUp(string DialogueText)
        {
            InitializeComponent();
            this.TextBlock_Text.Text = DialogueText;
        }

        private void Button_CancelOnClick(object sender, RoutedEventArgs e)
        {
            Confirm = false;
            this.Close();
        }
        private void Button_ConfirmOnClick(object sender, RoutedEventArgs e)
        {
            Confirm = true;
            this.Close();
        }
    }
}
