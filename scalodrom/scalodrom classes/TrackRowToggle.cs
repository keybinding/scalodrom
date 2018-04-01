using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace scalodrom.scalodrom_classes
{
    public class TrackRowToggle : PropertyChangedNotifier
    {
        public const string cs_doneImg = "images/done.png";
        public const string cs_sunImg = "images/sun.png";

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
            if (i_currentImage == cs_sunImg)
            {
                currentImage = cs_doneImg;
            }
            else
            {
                currentImage = cs_sunImg;
            }
        }

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
