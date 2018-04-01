using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using scalodrom.scalodrom_classes;

namespace scalodrom.wrappers
{
    
    public class tr_pathWrapper : AddDeleteCommandSender, System.ComponentModel.INotifyPropertyChanged
    {
        private tr_path i_tr_path;

        public event PropertyChangedEventHandler PropertyChanged;

        public tr_pathWrapper()
        {
            i_tr_path = new tr_path();
        }

        public tr_pathWrapper(tr_path a_tr_path)
        {
            i_tr_path = a_tr_path;
        }

        public tr_pathWrapper(tr_path a_tr_path, tr_pathWrapper a_previous)
        {
            i_tr_path = a_tr_path;
            Previous = a_previous;
        }


        public long id { get { return i_tr_path.id; } set { i_tr_path.id = value; Notify("id"); } }
        public long order {
            get
            {
                return i_tr_path.order;
            }
            set
            {
                i_tr_path.order = value;
                if (Next != null) Next.order = order + 1;
                Notify("order");
            }
        }
        public long id_training { get { return i_tr_path.id_training; } set { i_tr_path.id_training = value; Notify("id_training"); } }
        public long duration { get { return i_tr_path.duration; } set { i_tr_path.duration = value; Notify("duration"); } }
        public long speed { get { return i_tr_path.speed; } set { i_tr_path.speed = value; Notify("speed"); } }
        public long num_path { get { return i_tr_path.num_path; } set { i_tr_path.num_path = value; Notify("num_path"); } }

        public virtual training training { get { return i_tr_path.training; } set { i_tr_path.training = value; Notify("training"); } }

        public tr_path tr_path { get { return i_tr_path; } }

        public tr_pathWrapper Next
        {
            get
            {
                return _next;
            }

            set
            {
                _next = value;
                if(_next != null)
                    if (_next.Previous != this) _next.Previous = this;
            }
        }

        public tr_pathWrapper Previous
        {
            get
            {
                return _previous;
            }

            set
            {
                _previous = value;
                if(_previous != null)
                    if (_previous.Next != this) _previous.Next = this;
            }
        }

        private void Notify(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this,
                    new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }

        public delegate int ActionTaken(tr_pathWrapper a_item);

        public event ActionTaken addButtonClicked;

        public event ActionTaken deleteButtonClicked;

        protected override void Add_Button_Click()
        {
            int l_res = 0;
            if (addButtonClicked != null)
                l_res = addButtonClicked.Invoke(this);
        }

        protected override void Delete_Button_Click()
        {
            int l_res = 0;
            if (deleteButtonClicked != null)
                l_res = deleteButtonClicked.Invoke(this);
            _next = null;
            _previous = null;
        }

        private tr_pathWrapper _next;
        private tr_pathWrapper _previous;
    }
}
