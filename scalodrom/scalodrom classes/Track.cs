using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scalodrom.scalodrom_classes
{
    public class Track
    {
        public int index { get; set; }
        public ObservableCollection<TrackRow> rows { get { return _rows; } set { _rows = value; } }
        private ObservableCollection<TrackRow> _rows;

        public Track(int a_index)
        {
            index = a_index;
            rows = new ObservableCollection<TrackRow>();
            for (int i = 82; i > 0; --i)
            {
                rows.Add(new TrackRow(i, 4, this));
            }
        }
    }
}
