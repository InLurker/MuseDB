﻿using System;
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
    /// Interaction logic for SuccessPopUp.xaml
    /// </summary>
    public partial class NotificationPopUp : Window
    {
        public NotificationPopUp(string DialogueText)
        {
            InitializeComponent();
            this.TextBlock_Text.Text = DialogueText;
        }

        private void Button_ConfirmOnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
