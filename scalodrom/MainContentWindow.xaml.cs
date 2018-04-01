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

namespace scalodrom
{
    /// <summary>
    /// Interaction logic for MainContentWindow.xaml
    /// </summary>
    public partial class MainContentWindow : Window
    {
        login i_user_login = null;
        public MainContentWindow(login a_login)
        {
            InitializeComponent();
            i_user_login = a_login;
        }

        private void cb_users_Click(object sender, RoutedEventArgs e)
        {
            frame_content.Content = new UsersPage();
        }

        private void cb_trainings_Click(object sender, RoutedEventArgs e)
        {
            frame_content.Content = new TrainingsPage();
        }

        private void cb_control_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
