using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scalodrom.wrappers
{
    public class angle_seriesWrapper : System.ComponentModel.INotifyPropertyChanged
    {

        private angle_series i_angle_series;

        public angle_seriesWrapper()
        {
            i_angle_series = new angle_series();
        }

        public angle_seriesWrapper(angle_series a_angle_series)
        {
            i_angle_series = a_angle_series;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Notify(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this,
                    new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }

        public long id { get { return i_angle_series.id; } set { i_angle_series.id = value; Notify("id"); } }
        public long id_training { get { return i_angle_series.id_training; } set { i_angle_series.id_training = value; Notify("id_training"); } }
        public long order { get { return i_angle_series.order; } set { i_angle_series.order = value; Notify("order"); } }
        public long value { get { return i_angle_series.value; } set { i_angle_series.value = value; Notify("value"); } }
        public long duration { get { return i_angle_series.duration; } set { i_angle_series.duration = value; Notify("duration"); } }

        public virtual training training { get { return i_angle_series.training; } set { i_angle_series.training = value; Notify("training"); } }

        public angle_series angle_series { get { return i_angle_series; } }
    }

}
