using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace scalodrom.scalodrom_classes
{
    public abstract class AddDeleteCommandSender
    {
        private ICommand _deleteCmd;
        public ICommand deleteCmd
        {
            get
            {
                if (_deleteCmd == null)
                {
                    _deleteCmd = new RelayCommand(
                        param => Delete_Button_Click(),
                        param => { return true; }
                    );
                }
                return _deleteCmd;
            }
        }

        private ICommand _addCmd;
        public ICommand addCmd
        {
            get
            {
                if (_addCmd == null)
                {
                    _addCmd = new RelayCommand(
                        param => Add_Button_Click(),
                        param => { return true; }
                    );
                }
                return _addCmd;
            }
        }

        protected abstract void Add_Button_Click();
        protected abstract void Delete_Button_Click();
    }
}
