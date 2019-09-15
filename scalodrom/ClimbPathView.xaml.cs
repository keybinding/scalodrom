using scalodrom.scalodrom_classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace scalodrom
{
    /// <summary>
    /// Interaction logic for ClimbPathView.xaml
    /// </summary>
    /// 

    public class ClimbPathModel : PropertyChangedNotifier
    {
        training training = null;
        scalodromEntities3 db_context = null;
        private ObservableCollection<Track> _tracks = null;
        ObservableCollection<grapple_series> _grapples;
        public ObservableCollection<Track> tracks
        {
            get
            {
                return _tracks;
            }

            set
            {
                _tracks = value;
            }
        }

        public ObservableCollection<grapple_series> Grapples
        {
            get
            {
                return _grapples;
            }

            set
            {
                _grapples = value;
                FillToggles(value);
            }
        }

        private void FillToggles(ObservableCollection<grapple_series> a_grapples)
        {
            foreach(var g in a_grapples)
            {
                var track = tracks[(int)g.track_num];
                var row = track.rows[track.rows.Count - (int)g.row_num - 1];
                row.toggles[(int)g.grapple_num].setImageSilently(TrackRowToggle.cs_doneImg);
            }
        }

        public ClimbPathModel()
        {
            tracks = new ObservableCollection<Track>();
            tracks.Add(new Track(0));
            tracks.Add(new Track(1));
            tracks.Add(new Track(2));
            training = new training();
            subscribeToggles();
        }

        public ClimbPathModel(training a_tr, scalodromEntities3 a_db_context)
        {
            training = a_tr;
            db_context = a_db_context;
            tracks = new ObservableCollection<Track>();
            tracks.Add(new Track(0));
            tracks.Add(new Track(1));
            tracks.Add(new Track(2));
            subscribeToggles();
        }

        private void subscribeToggles()
        {
            foreach(var t in tracks) foreach (var r in t.rows) foreach(var tog in r.toggles) tog.pathToggleChanged += Tog_pathToggleChanged;
        }

        private void Tog_pathToggleChanged(object sender, string a_newState)
        {
            TrackRowToggle toggle = (sender as TrackRowToggle);
            if (a_newState == TrackRowToggle.cs_doneImg)
            {
                //add grapple
                grapple_series gs = new grapple_series() { grapple_num = toggle.index, row_num = toggle.parentRow.index - 1, track_num = toggle.parentRow.track.index, id_training = training.id};
                try
                {
                    db_context.grapple_series.Add(gs);
                    db_context.SaveChanges();
                }
                catch(Exception e)
                {
                    MessageBox.Show("Не удалось сохранить выбор", "Сообщение пользователю", MessageBoxButton.OK, MessageBoxImage.Error);
                    toggle.setImageSilently(TrackRowToggle.cs_sunImg);
                }
            }
            else
            {
                //delete grapple
                try
                {
                    grapple_series l_grapple = (from grapples in db_context.grapple_series where grapples.id_training == training.id && grapples.grapple_num == toggle.index
                                    && grapples.row_num == toggle.parentRow.index - 1 && grapples.track_num == toggle.parentRow.track.index select grapples).ToArray()[0];
                    db_context.grapple_series.Remove(l_grapple);
                    db_context.SaveChanges();
                }
                catch (Exception)
                {
                    MessageBox.Show("Не удалось отменить выбор", "Сообщение пользователю", MessageBoxButton.OK, MessageBoxImage.Error);
                    toggle.setImageSilently(TrackRowToggle.cs_doneImg);
                }
            }
        }
    }
    public partial class ClimbPathView : Page
    {

        ObservableCollection<grapple_series> _grapples;

        ClimbPathModel _model;
        public ClimbPathView()
        {
            _model = new ClimbPathModel();
            _grapples = new ObservableCollection<grapple_series>();
            InitializeComponent();
            DataContext = _model;
        }

        public ClimbPathView(training a_tr, scalodromEntities3 a_db_context)
        {
            _model = new ClimbPathModel(a_tr, a_db_context);
            _grapples = new ObservableCollection<grapple_series>((from grapplesList in a_db_context.grapple_series where grapplesList.id_training == a_tr.id select grapplesList));
            InitializeComponent();
            DataContext = _model;
            if (_grapples.Count > 0) _model.Grapples = _grapples;
        }
    }
}
