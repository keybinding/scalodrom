using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace scalodrom.scalodrom_classes
{
    public class TrackRow : PropertyChangedNotifier
    {
        private ObservableCollection<TrackRowToggle> _toggles;
        public ObservableCollection<TrackRowToggle> toggles { get { return _toggles; } set { _toggles = value; } }

        public int index { get { return _index; } private set { _index = value; Notify("index"); } }

        public Track track
        {
            get
            {
                return _track;
            }

            private set
            {
                _track = value;
            }
        }

        private int _index;

        private Track _track;

        public TrackRow(int a_index, int capacity, Track a_track)
        {
            index = a_index;
            track = a_track;
            toggles = new ObservableCollection<TrackRowToggle>();
            for (int i = 0; i < capacity; ++i)
            {
                toggles.Add(new TrackRowToggle(this, i));
            }
        }
    }
}
