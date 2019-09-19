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
using System.IO;
using System.Net.Sockets;

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
        trPathNodeModel _currentPoint1;
        private int _currentIndex1;
        List<trPathNodeModel> series2 = new List<trPathNodeModel>();
        trPathNodeModel _currentPoint2;
        private int _currentIndex2;
        List<trPathNodeModel> series3 = new List<trPathNodeModel>();
        trPathNodeModel _currentPoint3;
        private int _currentIndex3;

        List<trPathNodeModel> angles = new List<trPathNodeModel>();
        trPathNodeModel _currentAngle;
        float _realAngle = 0;
        private int _currentAngleIndex;

        bool switchAngle = false;


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
                        series1.Clear(); series2.Clear(); series3.Clear();angles.Clear();
                        foreach (var v in initialModel.seriesCollection1[0].ActualValues)
                        {
                            series1.Add(new trPathNodeModel() { Speed = (v as trPathNodeModel).Speed, Start = (v as trPathNodeModel).Start });
                        }
                        foreach (var v in initialModel.seriesCollection2[0].ActualValues)
                        {
                            series2.Add(new trPathNodeModel() { Speed = (v as trPathNodeModel).Speed, Start = (v as trPathNodeModel).Start });
                        }
                        foreach (var v in initialModel.seriesCollection3[0].ActualValues)
                        {
                            series3.Add(new trPathNodeModel() { Speed = (v as trPathNodeModel).Speed, Start = (v as trPathNodeModel).Start });
                        }
                        foreach (var v in initialModel.seriesCollectionAngles[0].ActualValues)
                        {
                            angles.Add(new trPathNodeModel() { Speed = (v as trPathNodeModel).Speed, Start = (v as trPathNodeModel).Start });
                        }
                        _currentIndex1 = 0; _currentIndex2 = 0; _currentIndex3 = 0; _currentAngleIndex = 0;
                        _currentPoint1 = series1[0]; _currentPoint2 = series2[0]; _currentPoint3 = series3[0]; _currentAngle = angles[0];

                        MaxTime = (new int[3]{(int) series1[series1.Count - 1].Start,
                                                   (int) series2[series2.Count - 1].Start,
                                                   (int) series3[series3.Count - 1].Start}).ToList().Max();
                        
                        finished = Play();
                        finished.Start();
                        finished.GetAwaiter().OnCompleted(() => {
                            _currentIndex1 = 0; _currentIndex2 = 0; _currentIndex3 = 0; _currentAngleIndex = 0;
                            series1.Clear(); series2.Clear(); series3.Clear(); angles.Clear();
                            CurrentTime = 0;
                            CurPrgBarValue = 0;
                            CurrentPlayButtonImage = c_playImage;
                            _isPlaying = false;
                            _justInit = false;
                            try
                            {
                                SendMusicCmd("-s");
                            }
                            catch (Exception)
                            {

                                //throw;
                            }
                            
                        });
                    }
                }
                else CurrentPlayButtonImage = c_pauseImage;
            }
        }

        bool _firstTime = true;
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
                        if (_firstTime) { _justInit = true; _firstTime = false; }
                        CurrentTime = ((int)secondsPassed);
                        
                        if (CurrentTime > (series1[_currentIndex1 + 1] as trPathNodeModel).Start)
                        {
                            _currentIndex1 += 1;
                            if (_currentIndex1 < series1.Count - 1)
                            {
                                _currentPoint1 = (series1[_currentIndex1] as trPathNodeModel);
                                //switchAngle = true;
                            }
                            else
                            {
                                _currentIndex1 = series1.Count - 2;
                                _currentPoint1 = null;
                            }
                        }                         

                        if (CurrentTime > (series2[_currentIndex2 + 1] as trPathNodeModel).Start)
                        {
                            _currentIndex2 += 1;
                            if (_currentIndex2 < series2.Count - 1)
                            {
                                _currentPoint2 = (series2[_currentIndex2] as trPathNodeModel);
                                //switchAngle = true;
                            }
                            else
                            {
                                _currentIndex2 = series2.Count - 2;
                                _currentPoint2 = null;
                            }
                        }

                        if (CurrentTime > (series3[_currentIndex3 + 1] as trPathNodeModel).Start)
                        {
                            _currentIndex3 += 1;
                            if (_currentIndex3 < series3.Count - 1)
                            {
                                _currentPoint3 = (series3[_currentIndex3] as trPathNodeModel);
                                //switchAngle = true;
                            }
                            else
                            {
                                _currentIndex3 = series3.Count - 2;
                                _currentPoint3 = null;
                            }
                        }

                        if (CurrentTime > (angles[_currentAngleIndex + 1] as trPathNodeModel).Start)
                        {
                            _currentAngleIndex += 1;
                            if (_currentAngleIndex < angles.Count - 1)
                            {
                                _currentAngle = (angles[_currentAngleIndex] as trPathNodeModel);
                                switchAngle = true;
                            }
                            else
                            {
                                _currentAngleIndex = angles.Count - 2;
                                switchAngle = false;
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
        
        
        private SerialPort _slavePort;

        public PlayTrainingModel(training a_tr, scalodromEntities3 a_db_context)
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
            OpenDMX.start();
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
                _currentPoint1 = (initialModel.seriesCollection1[0].ActualValues[0] as trPathNodeModel);
                _currentPoint2 = (initialModel.seriesCollection2[0].ActualValues[0] as trPathNodeModel);
                _currentPoint3 = (initialModel.seriesCollection3[0].ActualValues[0] as trPathNodeModel);
                _currentAngle = (initialModel.seriesCollectionAngles[0].ActualValues[0] as trPathNodeModel);
                _currentIndex1 = 0; _currentIndex2 = 0; _currentIndex3 = 0; _currentAngleIndex = 0;
                try
                {
                    SendMusicCmd("-c"); //clear
                    SendMusicCmd("-a /home/pi/Music"); //fill playlist from folder
                    SendMusicCmd("-p -t repeat, autonext"); //start paying
                }
                catch (Exception)
                {

                    
                }
                
            }
            else
            {
                try
                {
                    SendMusicCmd("-G");
                }
                catch (Exception)
                {

                    //throw;
                }
                
            }
            IsPlaying = !IsPlaying;            
        }

        public void SendMusicCmd(string a_cmd)
        {
            /*
            TcpClient client = new TcpClient("169.254.7.0", 8876);            
            NetworkStream ns = client.GetStream();
            Byte[] msg = new Byte[256];
            StreamWriter writer = new StreamWriter(ns, System.Text.Encoding.ASCII);
            writer.WriteLine(a_cmd);
            writer.Flush();
            */
        }

        bool _justInit = false;
        bool _switched = false;
        public void ListenPort()
        {
            _slavePort = new SerialPort("COM5");
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
                File.AppendAllLines("log.txt", new string[1] { $"CoilDiscretes: {args.Operation} starting at {args.StartingAddress} {args.Points.Length}" });
                //File.AppendAllLines("log.txt", new string[1] { $"CoilDiscretes: {args.Operation} starting at {args.StartingAddress} {args.Points.Length}" });
            };
            dataStore.CoilInputs.StorageOperationOccurred += (sender, args) =>
            {
                File.AppendAllLines("log.txt", new string[1] { $"CoilInputs: {args.Operation} starting at {args.StartingAddress} {args.Points.Length}" });
                if (args.Operation == PointOperation.Read)
                {

                    //i = "c-i read addr = " + args.StartingAddress.ToString() + " cnt = " + args.Points.Length.ToString();
                    //dataStore.CoilInputs.WritePoints(args.StartingAddress, new bool[] { true });
                    if ( Math.Abs(_currentAngle.Speed - _realAngle) > 1)
                    {
                        if (!switchAngle)
                        {
                            dataStore.CoilInputs.WritePoints(args.StartingAddress, new bool[] { true, false });
                            switchAngle = true;
                        }
                    }
                    else
                    {
                        dataStore.CoilInputs.WritePoints(args.StartingAddress, new bool[] { false, false });
                        switchAngle = false;
                    }

                }
                else
                {
                    //Dbg += $"Input registers: {args.Operation} starting at {args.StartingAddress} {args.Points.Length}\r\n";
                    //File.AppendAllLines("log.txt", new string[1] { $"CoilInputs: {args.Operation} starting at {args.StartingAddress} {args.Points.Length}" });

                }
            };
            List<ushort> adr = new List<ushort>(); //5 addr - angle, 7 - flag
            dataStore.InputRegisters.StorageOperationOccurred += (sender, args) =>
            {
                File.AppendAllLines("log.txt", new string[1] { $"InputRegisters: {args.Operation} starting at {args.StartingAddress} {args.Points.Length}" });
                if (args.Operation == PointOperation.Write)
                {
                    //h = "yes"; //Console.WriteLine($"Input registers: {args.Operation} starting at {args.StartingAddress} {args.Points.Length}");
                    //i = "yes";
                    //Dbg += $"Input registers: {args.Operation} starting at {args.StartingAddress} {args.Points.Length}" ;
                    //File.AppendAllLines("log.txt", new string[1] { $"InputRegisters: {args.Operation} starting at {args.StartingAddress} {args.Points.Length}" });
                    
                }
                else
                {
                    
                    if (args.StartingAddress == 1)
                    {
                        float curAngle = -15.0f;
                        byte[] ab = BitConverter.GetBytes(curAngle);
                        ushort high = BitConverter.ToUInt16(ab, 0);
                        ushort low = BitConverter.ToUInt16(ab, 2);
                        //dataStore.InputRegisters.WritePointsSilent(5, new ushort[] { high, low });
                        //172.16.15.65
                        float f1 = 0.0f; float f2 = 0.0f; float f3 = 0.0f;
                        if (IsPlaying)
                        {
                            if (_currentPoint1 != null) f1 = -1 *_currentPoint1.Speed;
                            else f1 = 0.0f;
                            if (_currentPoint2 != null) f2 = -1 * _currentPoint2.Speed;
                            else f2 = 0.0f;
                            if (_currentPoint3 != null) f3 = -1 * _currentPoint3.Speed;
                            else f3 = 0.0f;
                        }
                        else
                        {
                            f1 = 0.0f; f2 = 0.0f; f3 = 0.0f;
                        }
                        
                        byte[] b1 = BitConverter.GetBytes(f1); byte[] b2 = BitConverter.GetBytes(f2); byte[] b3 = BitConverter.GetBytes(f3);
                        ushort high1 = BitConverter.ToUInt16(b1, 0); ushort high2 = BitConverter.ToUInt16(b2, 0); ushort high3 = BitConverter.ToUInt16(b3, 0);
                        ushort low1 = BitConverter.ToUInt16(b1, 2); ushort low2 = BitConverter.ToUInt16(b2, 2); ushort low3 = BitConverter.ToUInt16(b3, 2);
                        //_currentPoint1

                        dataStore.InputRegisters.WritePointsSilent(args.StartingAddress, new ushort[] { high3, low3, high1, low1, high2, low2});
                    }
                    else if(args.StartingAddress == 21)
                    {
                        byte[] hl = BitConverter.GetBytes(_currentAngle.Speed);
                        ushort high = BitConverter.ToUInt16(hl, 0);
                        ushort low = BitConverter.ToUInt16(hl, 2);
                        dataStore.InputRegisters.WritePointsSilent(args.StartingAddress, new ushort[] { high, low });
                        //Dbg = _currentAngle.ToString();
                    }
                    else
                    {
                        Dbg += args.ToString() + " zzzz";
                    }
                }
            };


            dataStore.HoldingRegisters.StorageOperationOccurred += (sender, args) =>
            {
                
                //Console.WriteLine($"Holding registers: {args.Operation} starting at {args.StartingAddress}  {args.Points[0]}");
                if (args.Operation == PointOperation.Write)
                {
                    if (args.StartingAddress == 7)
                    {
                        ushort high = args.Points[1];
                        ushort low = args.Points[2];
                        byte[] bytes = new byte[4];
                        bytes[0] = (byte)(high & 0xFF);
                        bytes[1] = (byte)(high >> 8);
                        bytes[2] = (byte)(low & 0xFF);
                        bytes[3] = (byte)(low >> 8);
                        _realAngle = BitConverter.ToSingle(bytes, 0);
                        File.AppendAllLines("log.txt", new string[1] { $"HoldingRegisters: {args.Operation} starting at {args.StartingAddress} {args.Points.Length} angle = {_realAngle}" });
                        Dbg = _realAngle.ToString();
                    }
                }
                else
                {
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
            try
            {
                SendMusicCmd("-s");
            }
            catch (Exception)
            {

                //throw;
            }
            
            if (_bw.IsBusy) _bw.CancelAsync();
            if (_slavePort.IsOpen) _slavePort.Close();

        }

    }

    public partial class PlayTrainingPage : Page
    {
        PlayTrainingModel model = null;
        public PlayTrainingPage(training a_tr, scalodromEntities3 a_db_context)
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
