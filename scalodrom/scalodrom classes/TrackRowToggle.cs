using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace scalodrom.scalodrom_classes
{
    public class TrackRowToggle : PropertyChangedNotifier
    {
        public const string cs_doneImg = "images/done.png";
        public const string cs_sunImg = "images/sun.png";
        #region udptest
        /*
        BackgroundWorker bw = null;
        void dowork()
        {
            TcpClient _soc;
            _soc = new TcpClient();
            IPAddress ipAddr = IPAddress.Parse("172.16.15.65");
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 5005);
            _soc.Connect(ipEndPoint);
            Byte[] sendBytes = Encoding.ASCII.GetBytes(x.ToString() + " " + y.ToString());
            NetworkStream tcpStream = _soc.GetStream();
             
            tcpStream.Write(sendBytes, 0, sendBytes.Length);
            //    tcpStream.Flush();
            
            _soc.Close();
        }

        static TrackRowToggle _trt;
        static long x;
        static long y;
        static public void countCoords() {
                    x = (82 - _trt.parentRow.index) * 41 + 21;
                    y = _trt.parentRow.track.index * (1080 / 3) + _trt.index * ((1080 / 3) / 4) + ((1080 / 3) / 8);
        }*/
        #endregion

        private ICommand _toggleCmd;
        public ICommand toggleCmd
        {
            get
            {
                if (_toggleCmd == null)
                {
                    _toggleCmd = new RelayCommand(
                        param => Button_Click(),
                        param => { return true; }
                    );
                }
                return _toggleCmd;
            }
        }

        private int _index = 0;
        public int index { get { return _index; } private set { _index = value; Notify("index"); } }

        private string i_currentImage = cs_sunImg;
        public string currentImage
        {
            get { return i_currentImage; }
            set
            {
                i_currentImage = value;
                pathToggleChanged?.Invoke(this, currentImage);
                Notify("currentImage");
            }
        }

        private void Button_Click()
        {
            
            #region test
            /*
            _trt = this;
            countCoords();
            dowork();
            */
            #endregion
            
            if (i_currentImage == cs_sunImg)
            {
                currentImage = cs_doneImg;
            }
            else
            {
                currentImage = cs_sunImg;
            }
        }
        
        #region test
        /*
        private void Bw_DoWork(object sender, DoWorkEventArgs e)
        {
            dowork();
        }*/
        #endregion
        private TrackRow _parentRow;
        public TrackRow parentRow { get { return _parentRow; } private set { _parentRow = value; } }

        public TrackRowToggle(TrackRow a_parentRow, int a_index)
        {
            _parentRow = a_parentRow;
            index = a_index;
        }

        public void setImageSilently(string a_image)
        {
            i_currentImage = a_image;
            Notify("currentImage");
        }

        public delegate void PathToggleChanged(object sender, string a_newState);

        public event PathToggleChanged pathToggleChanged;
    }
}
