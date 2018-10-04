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
using System.Data.Entity;

namespace scalodrom
{
    /// <summary>
    /// Interaction logic for UsersPage.xaml
    /// </summary>
    public partial class UsersPage : Page
    {
        project_dbEntities1 db_context = new project_dbEntities1();
        public UsersPage()
        {
            InitializeComponent();
        }
        

        private void page_user_Unloaded(object sender, RoutedEventArgs e)
        {
            db_context.Dispose();
            db_context = null;
        }

        private void page_user_Loaded(object sender, RoutedEventArgs e)
        {
            System.Windows.Data.CollectionViewSource categoryViewSource =
                    ((System.Windows.Data.CollectionViewSource)(this.FindResource("traineeViewSource")));
            db_context.trainee.Load();
            
            categoryViewSource.Source = db_context.trainee.Local.OrderBy(item => item.lastname);
        }
        

        private void traineeDataGrid_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            trainee l_trainee = traineeDataGrid.SelectedItem as trainee;
            if (l_trainee != null)
            {
                UserViewPage l_usrPage = new UserViewPage(l_trainee, db_context);
                l_usrPage.cb_save.Click += Cb_save_Click;
                userPageFrame.Content = l_usrPage;
            }
        }

        private void Cb_save_Click(object sender, RoutedEventArgs e)
        {
            traineeDataGrid.Items.Refresh();
        }

        private void btn_takePicture_Click(object sender, RoutedEventArgs e)
        {

            trainee l_trainee = traineeDataGrid.SelectedItem as trainee;
            if (l_trainee != null)
            {

            }
        }
    }
}
