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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MuseDB_Desktop.Controls
{
    /// <summary>
    /// Interaction logic for UserComment.xaml
    /// </summary>
    public partial class UserComment : UserControl
    {
        public UserComment(string Username, string Comment, string CommentTime)
        {
            InitializeComponent();
            this.TextBlock_Username.Text = Username;
            this.TextBlock_Comment.Text = Comment;
            this.TextBlock_CommentTime.Text = CommentTime;
        }
    }
}
