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
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Configurations;
using scalodrom.wrappers;
using scalodrom.scalodrom_classes;

namespace scalodrom
{
    /// <summary>
    /// Interaction logic for TrainingView.xaml
    /// </summary>
    /// 
    public class trPathNodeModel
    {
        public long Start { get; set; }
        public float Speed { get; set; }
    }

    public class TrainingViewModel : PropertyChangedNotifier
    {
        training training = null;
        project_dbEntities1 db_context = null;
        private SeriesCollection i_seriesCollection1 = null;
        private SeriesCollection i_seriesCollection2 = null;
        private SeriesCollection i_seriesCollection3 = null;
        private SeriesCollection i_seriesCollectionAngles = null;

        private ObservableCollection<tr_pathWrapper> i_trPath1 = null;
        private ObservableCollection<tr_pathWrapper> i_trPath2 = null;
        private ObservableCollection<tr_pathWrapper> i_trPath3 = null;
        private ObservableCollection<angle_seriesWrapper> i_angles = null;

        public ObservableCollection<tr_pathWrapper> trPath1 { get { return i_trPath1; }  set { i_trPath1 = value; } }
        public ObservableCollection<tr_pathWrapper> trPath2 { get { return i_trPath2; } set { i_trPath2 = value; } }
        public ObservableCollection<tr_pathWrapper> trPath3 { get { return i_trPath3; } set { i_trPath3 = value; } }
        public ObservableCollection<angle_seriesWrapper> angles { get { return i_angles; } set { i_angles = value; } }

        public SeriesCollection seriesCollection1 { get { return i_seriesCollection1; } set { i_seriesCollection1 = value; } }
        public SeriesCollection seriesCollection2 { get { return i_seriesCollection2; } set { i_seriesCollection2 = value; } }
        public SeriesCollection seriesCollection3 { get { return i_seriesCollection3; } set { i_seriesCollection3 = value; } }
        public SeriesCollection seriesCollectionAngles { get { return i_seriesCollectionAngles; } set { i_seriesCollectionAngles = value; } }

        

        public TrainingViewModel()
        {
            i_trPath1 = new ObservableCollection<tr_pathWrapper>();
            i_trPath2 = new ObservableCollection<tr_pathWrapper>();
            i_trPath3 = new ObservableCollection<tr_pathWrapper>();
            i_angles = new ObservableCollection<angle_seriesWrapper>();
            
            training = new training();
        }

        public Dictionary<long, SeriesCollection> path_graph_dict = new Dictionary<long, SeriesCollection>();
        public Dictionary<long, ObservableCollection<tr_pathWrapper>> path_col_dict = new Dictionary<long, ObservableCollection<tr_pathWrapper>>();

        public TrainingViewModel(training a_tr, project_dbEntities1 a_db_context)
        {
            training = a_tr;
            db_context = a_db_context;
            i_trPath1 = collectionOfWrappers((from trPathList in a_db_context.tr_path where trPathList.id_training == training.id && trPathList.num_path == 1 select trPathList).ToList());
            i_trPath2 = collectionOfWrappers((from trPathList in a_db_context.tr_path where trPathList.id_training == training.id && trPathList.num_path == 2 select trPathList).ToList());
            i_trPath3 = collectionOfWrappers((from trPathList in a_db_context.tr_path where trPathList.id_training == training.id && trPathList.num_path == 3 select trPathList).ToList());
            i_angles = collectionOfAngleWrappers((from angles in a_db_context.angle_series where angles.id_training == training.id select angles).ToList());

            i_seriesCollection1 = ConfigureTrPathPlot(i_trPath1);
            i_seriesCollection2 = ConfigureTrPathPlot(i_trPath2);
            i_seriesCollection3 = ConfigureTrPathPlot(i_trPath3);
            i_seriesCollectionAngles = ConfigureAnglesPlot(i_angles);

            path_graph_dict.Add(1, i_seriesCollection1); path_graph_dict.Add(2, i_seriesCollection2); path_graph_dict.Add(3, i_seriesCollection3);
            path_col_dict.Add(1, i_trPath1); path_col_dict.Add(2, i_trPath2); path_col_dict.Add(3, i_trPath3);

            ConfigureCollbacksTrPath(i_trPath1, i_seriesCollection1, 1);
            ConfigureCollbacksTrPath(i_trPath2, i_seriesCollection2, 2);
            ConfigureCollbacksTrPath(i_trPath3, i_seriesCollection3, 3);
        }

        private void ConfigureCollbacksTrPath(ObservableCollection<tr_pathWrapper> a_trPath, SeriesCollection a_seriesCollection, int a_num_path)
        {
            for(int i = 0; i != a_trPath.Count; ++i)
            {
                a_trPath[i].deleteButtonClicked += (tr_pathWrapper s) => { DeleteTrPathItem(a_trPath, s, a_seriesCollection); return 1; };
                a_trPath[i].addButtonClicked += (tr_pathWrapper s) => { AddTrPathItem(a_trPath, s, a_seriesCollection, a_num_path); return 1; };
            }
        }

        //Plot configuration according to DatabaseModel
        SeriesCollection ConfigureTrPathPlot(ObservableCollection<tr_pathWrapper> a_trPath)
        {
            var plotConfig = Mappers.Xy<trPathNodeModel>().X(arg => arg.Start).Y(arg => arg.Speed);
            List<trPathNodeModel> plotValues = new List<trPathNodeModel>();
            if (a_trPath.Count > 0)
            {
                plotValues.Add(new trPathNodeModel() { Speed = a_trPath[0].speed, Start = 0 });
                for (int i = 1; i < a_trPath.Count; ++i)
                {
                    plotValues.Add(new trPathNodeModel() { Speed = a_trPath[i].speed, Start = plotValues[i - 1].Start + a_trPath[i - 1].duration });
                }
                plotValues.Add(new trPathNodeModel()
                {
                    Speed = plotValues[plotValues.Count - 1].Speed,
                    Start = plotValues[plotValues.Count - 1].Start + a_trPath[plotValues.Count - 1].duration
                });
            }

            SeriesCollection l_seriesCollection = new SeriesCollection(plotConfig)
            {
                new StepLineSeries { Values = new ChartValues<trPathNodeModel>(plotValues) }
            };

            return l_seriesCollection;
        }

        //Plot configuration according to DatabaseModel
        SeriesCollection ConfigureAnglesPlot(ObservableCollection<angle_seriesWrapper> a_trPath)
        {
            var plotConfig = Mappers.Xy<trPathNodeModel>().X(arg => arg.Start).Y(arg => arg.Speed);
            List<trPathNodeModel> plotValues = new List<trPathNodeModel>();
            if (a_trPath.Count > 0)
            {
                plotValues.Add(new trPathNodeModel() { Speed = a_trPath[0].value, Start = 0 });
                for (int i = 1; i < a_trPath.Count; ++i)
                {
                    plotValues.Add(new trPathNodeModel() { Speed = a_trPath[i].value, Start = plotValues[i - 1].Start + a_trPath[i - 1].duration });
                }
                plotValues.Add(new trPathNodeModel()
                {
                    Speed = plotValues[plotValues.Count - 1].Speed,
                    Start = plotValues[plotValues.Count - 1].Start + a_trPath[plotValues.Count - 1].duration
                });
            }

            SeriesCollection l_seriesCollection = new SeriesCollection(plotConfig)
            {
                new StepLineSeries { Values = new ChartValues<trPathNodeModel>(plotValues) }
            };

            return l_seriesCollection;
        }

        ObservableCollection<tr_pathWrapper> collectionOfWrappers(List<tr_path> a_col)
        {
            ObservableCollection<tr_pathWrapper> l_res = new ObservableCollection<tr_pathWrapper>();
            foreach (var v in a_col)
            {
                tr_pathWrapper l_w = new tr_pathWrapper(v, (l_res.Count > 0 ? l_res.Last() : null));
                l_res.Add(l_w);
            }
            return l_res;
        }

        ObservableCollection<angle_seriesWrapper> collectionOfAngleWrappers(List<angle_series> a_col)
        {
            ObservableCollection<angle_seriesWrapper> l_res = new ObservableCollection<angle_seriesWrapper>();
            foreach (var v in a_col)
            {
                l_res.Add(new angle_seriesWrapper(v));
            }
            return l_res;
        }

        public void DeleteTrPathItem(ObservableCollection<tr_pathWrapper> a_col, tr_pathWrapper item, SeriesCollection a_graf)
        {
            if (item == null) return;
            a_col.Remove(item);
            int idx = (int)item.order;
            a_graf[0].ActualValues.RemoveAt(idx);
            //if fake point left then remove it
            int l_pointCnt = a_graf[0].ActualValues.Count;
            if (l_pointCnt < 2)
            {
                a_graf[0].ActualValues.Clear();
            }
            else
            {
                for (int i = idx; i < l_pointCnt; ++i)
                {
                    (a_graf[0].ActualValues[i] as trPathNodeModel).Start -= item.duration;
                }
                (a_graf[0].ActualValues[l_pointCnt - 1] as trPathNodeModel).Speed = (a_graf[0].ActualValues[l_pointCnt - 2] as trPathNodeModel).Speed;
            }
            if (item.Next != null)
                item.Next.order--;
            if(db_context != null)
            {
                //db_context.tr_path.Remove(item.tr_path);
                try
                {
                    //db_context.SaveChanges();
                }
                catch(Exception e)
                {
                    MessageBox.Show(e.InnerException.ToString());
                }
            }
        }

        public void AddTrPathItem(ObservableCollection<tr_pathWrapper> a_col, tr_pathWrapper item, SeriesCollection a_graf, long a_num_path)
        {
            if (item == null) return;
            int idx = (int)item.order + 1;
            long l_midDuration = 10;
            tr_pathWrapper l_newPath = new tr_pathWrapper(new tr_path() { id_training = training.id, num_path = a_num_path, duration = l_midDuration, order = idx, speed = a_col[idx-1].speed });
            
            if (idx < a_col.Count) {
                a_col.Insert(idx, l_newPath);
            }
            else
            {
                a_col.Add(l_newPath);
            }

            l_newPath.Next = item.Next;
            if (item.Next != null) item.Next.Previous = l_newPath;
            l_newPath.Previous = item;
            item.Next = l_newPath;

            l_newPath.order = idx;

            l_newPath.deleteButtonClicked += (tr_pathWrapper s) => { DeleteTrPathItem(a_col, s, a_graf); return 1; };
            l_newPath.addButtonClicked += (tr_pathWrapper s) => { AddTrPathItem(a_col, s, a_graf, a_num_path); return 1; };

            //trPathNodeModel l_newPoint = new trPathNodeModel() { Speed = (a_graf[0].ActualValues[idx - 1] as trPathNodeModel).Speed, Start = (a_graf[0].ActualValues[idx - 1] as trPathNodeModel).Start + l_midDuration };

            //for (int i = idx; i < a_col.Count; ++i)
            //{
            //    (a_graf[0].ActualValues[idx] as trPathNodeModel).Start += l_midDuration;
            //}
            //a_graf[0].ActualValues.Insert(idx, l_newPoint);
            //int l_pointCnt = a_graf[0].ActualValues.Count;
            //if (idx == l_pointCnt - 2)
            //    (a_graf[0].ActualValues[l_pointCnt - 1] as trPathNodeModel).Speed = (a_graf[0].ActualValues[l_pointCnt - 2] as trPathNodeModel).Speed;

            if (db_context != null)
            {
                //db_context.tr_path.Remove(item.tr_path);
                try
                {
                    //db_context.SaveChanges();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.InnerException.ToString());
                }
            }
        }
    }
    
    public partial class TrainingView : Page
    {
        TrainingViewModel model = null;
        public TrainingView()
        {
            InitializeComponent();
            model = new TrainingViewModel();
            
            DataContext = model;
        }

        public TrainingView(training a_tr, project_dbEntities1 a_db_context)
        {
            InitializeComponent();
            model = new TrainingViewModel(a_tr, a_db_context);
            DataContext = model;
        }

        
        //public void RefreshButtonsPathDelete(ItemsControl a_tr_path, string a_cb_name)
        //{
        //    foreach (var item in a_tr_path.Items)
        //    {
        //        Button cb_del = FindItemControl(a_tr_path, a_cb_name, item) as Button;
                
        //        cb_del.Click += new RoutedEventHandler((object s, RoutedEventArgs a) =>
        //        {
        //            // This is the item that you want.  Many assumptions about the types are made, but that is OK.
        //            tr_pathWrapper node = ((s as FrameworkElement).DataContext as tr_pathWrapper);
        //            model.DeleteTrPathItem(model.trPath1, node, model.seriesCollection1);
        //        });
        //    }
        //}

        //public void RefreshButtonsPathAdd(ItemsControl a_tr_path, string a_cb_name, long a_num_path, ItemsControl a_itemsCntrl, TrainingView a_trView)
        //{
        //    foreach (var item in a_tr_path.Items)
        //    {
        //        Button cb_del = FindItemControl(a_tr_path, a_cb_name, item) as Button;

        //        cb_del.Click += new RoutedEventHandler((object s, RoutedEventArgs a) =>
        //        {
        //            // This is the item that you want.  Many assumptions about the types are made, but that is OK.
        //            tr_pathWrapper node = ((s as FrameworkElement).DataContext as tr_pathWrapper);
        //            //model.AddTrPathItem(model.trPath1, node, model.seriesCollection1, a_num_path, a_itemsCntrl, a_trView);
        //        });
        //    }
        //}

        private object FindItemControl(ItemsControl itemsControl, string controlName, object item)
        {
            ContentPresenter container = itemsControl.ItemContainerGenerator.ContainerFromItem(item) as ContentPresenter;
            container.ApplyTemplate();
            return container.ContentTemplate.FindName(controlName, container);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //first path
            //foreach (var item in tr_path1.Items)
            //{
            //    Button cb_del = FindItemControl(tr_path1, "cb_delete", item) as Button;

            //    cb_del.Click += new RoutedEventHandler((object s, RoutedEventArgs a) =>
            //    {
            //        // This is the item that you want.  Many assumptions about the types are made, but that is OK.
            //        tr_pathWrapper node = ((s as FrameworkElement).DataContext as tr_pathWrapper);
            //        model.DeleteTrPathItem(model.trPath1, node, model.seriesCollection1);
            //    });
            //}
            //foreach (var item in tr_path1.Items)
            //{
            //    Button cb_del = FindItemControl(tr_path1, "cb_append", item) as Button;

            //    cb_del.Click += new RoutedEventHandler((object s, RoutedEventArgs a) =>
            //    {
            //        // This is the item that you want.  Many assumptions about the types are made, but that is OK.
            //        tr_pathWrapper node = ((s as FrameworkElement).DataContext as tr_pathWrapper);
            //        model.AddTrPathItem(model.trPath1, node, model.seriesCollection1, 1, tr_path1, this);
            //        //model.DeleteTrPathItem(model.trPath1, node, model.seriesCollection1);
            //    });
            //}
            //second path
            //foreach (var item in tr_path2.Items)
            //{
            //    Button cb_del = FindItemControl(tr_path2, "cb_delete", item) as Button;

            //    cb_del.Click += new RoutedEventHandler((object s, RoutedEventArgs a) =>
            //    {
            //        // This is the item that you want.  Many assumptions about the types are made, but that is OK.
            //        tr_pathWrapper node = ((s as FrameworkElement).DataContext as tr_pathWrapper);
            //        model.DeleteTrPathItem(model.trPath2, node, model.seriesCollection2);
            //    });
            //}
            //third path
            //foreach (var item in tr_path3.Items)
            //{
            //    Button cb_del = FindItemControl(tr_path3, "cb_delete", item) as Button;

            //    cb_del.Click += new RoutedEventHandler((object s, RoutedEventArgs a) =>
            //    {
            //        // This is the item that you want.  Many assumptions about the types are made, but that is OK.
            //        tr_pathWrapper node = ((s as FrameworkElement).DataContext as tr_pathWrapper);
            //        model.DeleteTrPathItem(model.trPath3, node, model.seriesCollection3);
            //    });
            //}
        }
        
    }
}
