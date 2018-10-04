using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using scalodrom.wrappers;

namespace scalodrom
{
    /// <summary>
    /// Interaction logic for UserViewPage.xaml
    /// </summary>
    public partial class UserViewPage : Page, System.ComponentModel.INotifyPropertyChanged
    {

        public TraineeWrapper currentTrainee { get { return i_trainee; }
            set {
                i_trainee = value;
                Notify("currentTrainee");
            }
        }

        TraineeWrapper i_trainee;
        project_dbEntities1 db_context;

        public event PropertyChangedEventHandler PropertyChanged;

        private void Notify(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this,
                    new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }


        public UserViewPage()
        {
            InitializeComponent();
            DataContext = this;
            i_trainee = new TraineeWrapper();
        }

        public UserViewPage(trainee a_trainee, project_dbEntities1 a_db_context)
        {
            InitializeComponent();
            DataContext = this;
            currentTrainee = new TraineeWrapper(a_trainee);
            db_context = a_db_context;
        }

        async private void cb_save_Click(object sender, RoutedEventArgs e)
        {
            await db_context.SaveChangesAsync();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void tb_firstname_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
