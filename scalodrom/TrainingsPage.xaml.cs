using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace scalodrom
{
    /// <summary>
    /// Interaction logic for TrainingsPage.xaml
    /// </summary>
    public partial class TrainingsPage : Page, System.ComponentModel.INotifyPropertyChanged
    {
        scalodromEntities3 db_context = new scalodromEntities3();
        public const string c_plotsImage = "images/lines.png";
        public const string c_grapplesImage = "images/sun.png";
        private string _detailsImage;
        private BitmapImage _plotsBmi = new BitmapImage();
        private BitmapImage _grapplesBmi = new BitmapImage();
        private Label _detailsBtnLable = null;

        public string DetailsImage
        {
            get
            {
                return _detailsImage;
            }
            set
            {
                _detailsImage = value;
                Notify("DetailsImage");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public TrainingsPage()
        {
            InitializeComponent();
            DetailsImage = c_grapplesImage;
            _plotsBmi.BeginInit();
            _plotsBmi.UriSource = new Uri(c_plotsImage, UriKind.Relative);
            _plotsBmi.EndInit();
            _grapplesBmi.BeginInit();
            _grapplesBmi.UriSource = new Uri(c_grapplesImage, UriKind.Relative);
            _grapplesBmi.EndInit();
            _detailsBtnLable = FindName("detailsBtnLable") as Label;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            db_context.Dispose();
            db_context = null;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            System.Windows.Data.CollectionViewSource categoryViewSource =
                    ((System.Windows.Data.CollectionViewSource)(this.FindResource("trainingViewSource")));
            db_context.training.Load();
            categoryViewSource.Source = db_context.training.Local.OrderBy(item => item.name);
        }

        protected void Notify(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this,
                    new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }

        private void trainingDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            training l_training = trainingDataGrid.SelectedItem as training;
            if (l_training != null)
            {
                if (DetailsImage == c_grapplesImage)
                {
                    TrainingView l_usrPage = new TrainingView(l_training, db_context);
                    viewFrame.Content = l_usrPage;
                }
                else
                {
                    ClimbPathView l_usrPage = new ClimbPathView(l_training, db_context);
                    viewFrame.Content = l_usrPage;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            training l_training = trainingDataGrid.SelectedItem as training;
            if (l_training != null)
            {
                PlayTrainingPage l_usrPage = new PlayTrainingPage(l_training, db_context);
                viewFrame.Content = l_usrPage;
            }
        }

        private void addTraining_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void delTraining_Click(object sender, RoutedEventArgs e)
        {

        }

        private void climbPath_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (sender as Button);
            System.Windows.Controls.Image img = (btn.Content as Image);            
            training l_training = trainingDataGrid.SelectedItem as training;
            if (l_training != null)
            {
                if (DetailsImage == c_grapplesImage)
                {
                    ClimbPathView l_usrPage = new ClimbPathView(l_training, db_context);
                    viewFrame.Content = l_usrPage;
                    DetailsImage = c_plotsImage;
                    img.Source = _plotsBmi;
                    _detailsBtnLable.Content = "Полотна";
                }
                else
                {
                    TrainingView l_usrPage = new TrainingView(l_training, db_context);
                    viewFrame.Content = l_usrPage;
                    DetailsImage = c_grapplesImage;
                    img.Source = _grapplesBmi;
                    _detailsBtnLable.Content = "Зацепы";
                }
            }
        }

        private void strobos_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (sender as Button);
            System.Windows.Controls.Image img = (btn.Content as Image);
            training l_training = trainingDataGrid.SelectedItem as training;
            if (l_training != null)
            {
                ClimbPathView l_usrPage = new ClimbPathView(l_training, db_context);
                viewFrame.Content = l_usrPage;
            }
        }

    }
}
