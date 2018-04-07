using NModbus;
using NModbus.Serial;
using scalodrom.scalodrom_classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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
    /// Interaction logic for HighlightTest.xaml
    /// </summary>
    public partial class HighlightTest : Page, System.ComponentModel.INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void Notify(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this,
                    new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }

        BackgroundWorker _bwFrameHighlight = null;
        BackgroundWorker _bwScalodrom = null;
        TcpClient _soc = null;
        SerialPort _slavePort = null;
        private int _x;
        private int _y;
        private int _speed;
        private int _pos;
        private string _dbg;
        private bool _isPlaying;

        public int X
        {
            get
            {
                return _x;
            }

            set
            {
                _x = value; Notify("X");
            }
        }

        public int Y
        {
            get
            {
                return _y;
            }

            set
            {
                _y = value; Notify("Y");
            }
        }

        public int Speed
        {
            get
            {
                return _speed;
            }

            set
            {
                _speed = value; Notify("Speed");
            }
        }

        public int Pos
        {
            get
            {
                return _pos;
            }

            set
            {
                _pos = value; Notify("Pos");
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
                _dbg = value; Notify("Dbg");
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
                _isPlaying = value; Notify("IsPlaying");
            }
        }

        bool _sendMessage = false;
        void ThreadWork(object sender, DoWorkEventArgs e)
        {
           while (true)
           {
                if (_sendMessage)
                {
                    _sendMessage = false;
                    IPAddress ipAddr = IPAddress.Parse("172.16.15.65");
                    IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 5005);

                    _soc = new TcpClient();
                    try
                    {
                        _soc.Connect(ipEndPoint);
                        NetworkStream tcpStream = _soc.GetStream();
                        int lx = _x;
                        int ly = _y;
                        Byte[] sendBytes = Encoding.ASCII.GetBytes(lx.ToString() + " " + ly.ToString());
                        Dbg = lx.ToString() + " " + ly.ToString();
                        tcpStream.Write(sendBytes, 0, sendBytes.Length);
                        _soc.Close();
                    }
                    catch
                    {
                        if (_soc.Connected) _soc.Close();
                    }
                }
           }
            
        }

        void ThreadScalWork(object s, DoWorkEventArgs e)
        {
            if (_slavePort == null)
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
                        if (args.StartingAddress == 9)
                        {
                            
                        }
                    }
                    else
                    {
                    //if (!adr.Contains(args.StartingAddress)) adr.Add(args.StartingAddress);

                    if (args.StartingAddress == 5)
                        {
                            float f = 0.0f;
                            f = Speed;
                            int m = (int)f;
                            byte[] b = BitConverter.GetBytes(m);
                            ushort high = BitConverter.ToUInt16(b, 0);
                            ushort low = BitConverter.ToUInt16(b, 2);
                            //dataStore.InputRegisters.WritePointsSilent(args.StartingAddress, new ushort[] { high, low, high, low, high, low });
                        }
                        else if (args.StartingAddress == 1)
                        {
                        //172.16.15.65
                            float f = 0.0f;
                            if (IsPlaying)
                            {
                                f = Speed;
                            }
                            else
                            {
                                f = 0.0f;
                            }

                            //Dbg = f.ToString();
                            byte[] b = BitConverter.GetBytes(f);
                            ushort high = BitConverter.ToUInt16(b, 0);
                            ushort low = BitConverter.ToUInt16(b, 2);
                            //_currentPoint
                            ushort nil = 0;
                            dataStore.InputRegisters.WritePointsSilent(args.StartingAddress, new ushort[] { high, low, high, low, high, low}); //{ high, low, nil, nil, nil, nil });
                            //Dbg = f.ToString();
                        }
                        else if (args.StartingAddress == 9)
                        {
                        //h = args.StartingAddress.ToString() + " " + args.Points.Length.ToString();
                            float f = 0.0f;
                        //if (_currentPoint != null) f = _currentPoint.Speed;
                        byte[] b = BitConverter.GetBytes(f);
                            ushort high = BitConverter.ToUInt16(b, 0);
                            ushort low = BitConverter.ToUInt16(b, 2);
                        //dataStore.InputRegisters.WritePointsSilent(args.StartingAddress, new ushort[] { high, low });
                        }
                        else
                        {
                        //h = args.ToString() + " zzzz";
                        }
                    }
                };

                //8.89 на отсчет
                //1280 40
                //1300 220
                int _prevPos = -1;
                dataStore.HoldingRegisters.StorageOperationOccurred += (sender, args) =>
                {
                    if (args.Operation == PointOperation.Write)
                    {
                        if (args.StartingAddress == 9)
                        {

                        }
                        else if (args.StartingAddress == 13)
                        {

                        }
                        else if (args.StartingAddress == 11)
                        {
                            Pos = args.Points[0];

                            if (args.Points[0] == 0)
                            {
                                IsPlaying = false;
                            }
                            X = (int)(1280 + args.Points[0] * 8.89);
                            Y = 220;
                            if (Pos != _prevPos) _sendMessage = true;
                            _prevPos = Pos;
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
                catch (Exception exc)
                {

                }
            }
        }

        public HighlightTest()
        {
            InitializeComponent();
            if (_bwFrameHighlight == null)
            {
                _bwFrameHighlight = new BackgroundWorker();
                _bwFrameHighlight.WorkerSupportsCancellation = true;
                _bwFrameHighlight.DoWork += ThreadWork;
                _bwFrameHighlight.RunWorkerAsync();
            }
            if (_bwScalodrom == null)
            {
                _bwScalodrom = new BackgroundWorker();
                _bwScalodrom.DoWork += ThreadScalWork;
                _bwScalodrom.RunWorkerAsync();
            }
            DataContext = this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            IsPlaying = !IsPlaying;
        }
    }
}
