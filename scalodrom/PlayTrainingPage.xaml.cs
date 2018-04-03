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
using scalodrom.scalodrom_classes;
using System.IO.Ports;
using NModbus.Serial;
using NModbus;
using System.ComponentModel;

namespace scalodrom
{
    /// <summary>
    /// Interaction logic for PlayTrainingPage.xaml
    /// </summary>
    /// 

    public class PlayTrainingModel : PropertyChangedNotifier
    {
        TrainingViewModel _initialModel = null;
        public TrainingViewModel initialModel { get { return _initialModel; } private set { _initialModel = value; } }

        string _currentPlayButtonImage;

        int _curPrgBarValue;
        int _maxPrgBarValue;
        bool _isPlaying;
        private int _currentTime;
        private int _maxTime;
        private string _dbg;
        Task<bool> finished;
        List<trPathNodeModel> series1 = new List<trPathNodeModel>();
        BackgroundWorker _bw;

        public const string c_playImage = "images/play_green.png";
        public const string c_pauseImage = "images/pause1.png";

        public int CurrentTime
        {
            get
            {
                return _currentTime;
            }

            set
            {
                _currentTime = value;
                Notify("CurrentTime");
            }
        }

        public string CurrentPlayButtonImage
        {
            get
            {
                return _currentPlayButtonImage;
            }

            set
            {
                _currentPlayButtonImage = value;
                Notify("CurrentPlayButtonImage");
            }
        }

        public int CurPrgBarValue
        {
            get
            {
                return _curPrgBarValue;
            }

            set
            {
                _curPrgBarValue = value;
                Notify("CurPrgBarValue");
            }
        }

        public int MaxPrgBarValue
        {
            get
            {
                return _maxPrgBarValue;
            }

            set
            {
                _maxPrgBarValue = value;
                Notify("MaxPrgBarValue");
            }
        }

        public bool IsPlaying
        {
            get
            {
                return _isPlaying;
            }

            set
            {
                _isPlaying = value;
                if (!_isPlaying)
                {
                    CurrentPlayButtonImage = c_playImage;
                    if (finished == null || finished.Status != TaskStatus.Running)
                    {
                        series1.Clear();
                        foreach (var v in initialModel.seriesCollection1[0].ActualValues)
                        {
                            series1.Add(new trPathNodeModel() { Speed = (v as trPathNodeModel).Speed, Start = (v as trPathNodeModel).Start });
                        }
                        _currentIndex = 0;
                        _currentPoint = series1[0];
                        MaxTime = (int) series1[series1.Count - 1].Start;
                        finished = Play();
                        finished.Start();
                        finished.GetAwaiter().OnCompleted(() => {
                            _currentIndex = 0;
                            series1.Clear();
                            CurrentTime = 0;
                            CurPrgBarValue = 0;
                            CurrentPlayButtonImage = c_playImage;
                            _isPlaying = false;
                        });
                    }
                }
                else CurrentPlayButtonImage = c_pauseImage;
            }
        }

        public Task<bool> Play()
        {
            return new Task<bool>(() =>
            {
                HiResTimer l_hrt = new HiResTimer();
                Int64 curTime = l_hrt.Value;
                Int64 timeElapsed = 0;
                while (CurrentTime <= MaxTime)
                {
                    Int64 newCurTime = l_hrt.Value;
                    if (IsPlaying)
                    {
                        timeElapsed += newCurTime - curTime;
                    }
                    double secondsPassed = (double)timeElapsed / (double)l_hrt.Frequency;
                    if (((int)secondsPassed) > CurrentTime)
                    {
                        CurrentTime = ((int)secondsPassed);
                        if (CurrentTime > (series1[_currentIndex + 1] as trPathNodeModel).Start)
                        {
                            _currentIndex += 1;
                            if (_currentIndex < series1.Count - 1)
                            {
                                _currentPoint = (series1[_currentIndex] as trPathNodeModel);
                                //switchAngle = true;
                            }
                        }
                    }
                    CurPrgBarValue = (int) ((secondsPassed / MaxTime) * MaxPrgBarValue);
                    curTime = newCurTime;
                }
                return true;
            });
        }



        public int MaxTime
        {
            get
            {
                return _maxTime;
            }

            set
            {
                _maxTime = value;
                Notify("MaxTime");
            }
        }

        public string Dbg
        {
            get
            {
                return _dbg;
            }

            set
            {
                _dbg = value;
                Notify("Dbg");
            }
        }

        trPathNodeModel _currentPoint;
        private int _currentIndex;
        private SerialPort _slavePort;

        public PlayTrainingModel(training a_tr, project_dbEntities1 a_db_context)
        {
            initialModel = new TrainingViewModel(a_tr, a_db_context);
            CurrentPlayButtonImage = c_playImage;
            IsPlaying = false;
            CurrentTime = 0;
            MaxPrgBarValue = CalculatePrgBarLength();
            if (_bw == null)
            {
                _bw = new BackgroundWorker();
                _bw.WorkerSupportsCancellation = true;
                _bw.DoWork += Bw_DoWork;
                _bw.RunWorkerAsync();
            }
        }

        private void Bw_DoWork(object sender, DoWorkEventArgs e)
        {
            ListenPort();
        }

        private int CalculatePrgBarLength()
        {
            return 600;
        }

        private int CalculateTrainingLength()
        {
            return 10;
        }

        public void PlayClicked()
        {

            if (CurrentTime == 0)
            {
                _currentPoint = (initialModel.seriesCollection1[0].ActualValues[0] as trPathNodeModel);
                _currentIndex = 0;
            }
            IsPlaying = !IsPlaying;
        }

        public void ListenPort()
        {
            _slavePort = new SerialPort("COM3");
            _slavePort.BaudRate = 115200;
            _slavePort.DataBits = 8;
            _slavePort.Parity = Parity.None;
            _slavePort.StopBits = StopBits.Two;
            _slavePort.Open();
            var adapter = new SerialPortAdapter(_slavePort);
            var factory = new ModbusFactory();
            var slaveNetwork = factory.CreateRtuSlaveNetwork(adapter);
            var dataStore = new SlaveStorage();
            var transport = factory.CreateRtuTransport(adapter);
            IModbusSlave slave1 = factory.CreateSlave(4, dataStore); //5 - у тестового, 4 - scalodrom
            dataStore.CoilDiscretes.StorageOperationOccurred += (sender, args) =>
            {
                ;
            };
            dataStore.CoilInputs.StorageOperationOccurred += (sender, args) =>
            {
                if (args.Operation == PointOperation.Read)
                {
                    //i = "c-i read addr = " + args.StartingAddress.ToString() + " cnt = " + args.Points.Length.ToString();
                    if (false)//switchAngle)
                    {
                        //dataStore.CoilInputs.WritePoints(args.StartingAddress, new bool[] { true });
                        //switchAngle = false;
                    }
                    else
                    {
                        dataStore.CoilInputs.WritePoints(args.StartingAddress, new bool[] { false });
                    }
                }
            };
            List<ushort> adr = new List<ushort>(); //5 addr - angle, 7 - flag
            dataStore.InputRegisters.StorageOperationOccurred += (sender, args) =>
            {
                if (args.Operation == PointOperation.Write)
                {
                    //h = "yes"; //Console.WriteLine($"Input registers: {args.Operation} starting at {args.StartingAddress} {args.Points.Length}");
                    //i = "yes";
                }
                else
                {
                    //if (!adr.Contains(args.StartingAddress)) adr.Add(args.StartingAddress);
                    
                    if (args.StartingAddress == 5)
                    {
                        float f = 0.0f;
                        if (_currentPoint != null) f = _currentPoint.Speed;
                        int m = (int)f;
                        byte[] b = BitConverter.GetBytes(m);
                        ushort high = BitConverter.ToUInt16(b, 0);
                        ushort low = BitConverter.ToUInt16(b, 2);
                        dataStore.InputRegisters.WritePointsSilent(args.StartingAddress, new ushort[] { high, low, high, low, high, low });
                    }
                    else if (args.StartingAddress == 1)
                    {
                        //172.16.15.65
                        float f = 0.0f;
                        if (IsPlaying)
                        {
                            if (_currentPoint != null) f = _currentPoint.Speed;
                            else f = 0.0f;
                        }
                        else
                        {
                            f = 0.0f;
                        }

                        Dbg = f.ToString();
                        byte[] b = BitConverter.GetBytes(f);
                        ushort high = BitConverter.ToUInt16(b, 0);
                        ushort low = BitConverter.ToUInt16(b, 2);
                        //_currentPoint

                        dataStore.InputRegisters.WritePointsSilent(args.StartingAddress, new ushort[] { high, low, high, low, high, low });
                    }
                    else if (args.StartingAddress == 9)
                    {
                        //h = args.StartingAddress.ToString() + " " + args.Points.Length.ToString();
                        Dbg += args.StartingAddress.ToString();
                        Dbg += " cnt = " + args.Points.Length.ToString();
                        float f = 0.0f;
                        if (_currentPoint != null) f = _currentPoint.Speed;
                        byte[] b = BitConverter.GetBytes(f);
                        ushort high = BitConverter.ToUInt16(b, 0);
                        ushort low = BitConverter.ToUInt16(b, 2);
                        dataStore.InputRegisters.WritePointsSilent(args.StartingAddress, new ushort[] { high, low });
                    }
                    else
                    {
                        //h = args.ToString() + " zzzz";
                    }
                }
            };


            dataStore.HoldingRegisters.StorageOperationOccurred += (sender, args) =>
            {
                //Console.WriteLine($"Holding registers: {args.Operation} starting at {args.StartingAddress}  {args.Points[0]}");
                if (args.Operation == PointOperation.Write)
                {

                }
                else
                {
                    //h = "zzzz";
                    string dbg = "";
                    //h = args.StartingAddress.ToString();
                }
            };
            slaveNetwork.AddSlave(slave1);

            try
            {
                slaveNetwork.ListenAsync();
            }
            catch (Exception exc){
                
            }
            
        }

        public void Unload()
        {
            if (_bw.IsBusy) _bw.CancelAsync();
            if (_slavePort.IsOpen) _slavePort.Close();
        }

    }

    public partial class PlayTrainingPage : Page
    {
        PlayTrainingModel model = null;
        public PlayTrainingPage(training a_tr, project_dbEntities1 a_db_context)
        {
            InitializeComponent();
            model = new PlayTrainingModel(a_tr, a_db_context);
            DataContext = model;
        }

        private void bt_play_Click(object sender, RoutedEventArgs e)
        {
            model.PlayClicked();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            model.Unload();
        }
    }
}
