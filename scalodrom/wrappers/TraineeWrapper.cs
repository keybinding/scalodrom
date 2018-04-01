using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scalodrom.wrappers
{
    public class TraineeWrapper
    {
        trainee i_trainee;
        public TraineeWrapper(trainee a_trainee)
        {
            i_trainee = a_trainee;
        }

        public TraineeWrapper()
        {
            i_trainee = new trainee();
        }

        public trainee trainee { get { return i_trainee; } }

        public Int64 id { get { return i_trainee.id; } }

        public string firstname {
            get { return i_trainee.firstname; }
            set {
                i_trainee.firstname = value;
                i_trainee.fullname = lastname + ' ' + value + ' ' + middlename;
                i_trainee.modified_date = DateTime.Today.ToString("yyyy-MM-dd");
            }
        }

        public string middlename {
            get { return i_trainee.middlename; }
            set {
                i_trainee.middlename = value;
                i_trainee.fullname = lastname + ' ' + firstname + ' ' + value;
                i_trainee.modified_date = DateTime.Today.ToString("yyyy-MM-dd");
            }
        }

        public string lastname
        {
            get {
                return i_trainee.lastname;
            }
            set { i_trainee.lastname = value;
                i_trainee.fullname = value + ' ' + firstname + ' ' + middlename;
                i_trainee.modified_date = DateTime.Today.ToString("yyyy-MM-dd");
            }
        }

        public DateTime birthdate {
            get {
                DateTime l_res = DateTime.Parse(i_trainee.birthdate);
                return l_res;
            }
            set {
                i_trainee.birthdate = value.ToString("yyyy-MM-dd");
                i_trainee.modified_date = DateTime.Today.ToString("yyyy-MM-dd");
            }
        }

        public DateTime create_date { get { return DateTime.Parse(i_trainee.create_date); } set { i_trainee.create_date = value.ToString("yyyy-MM-dd"); } }
        public long id_user_create { get { return i_trainee.id_user_create; } set { i_trainee.id_user_create = value; } }
        public Nullable<long> id_user_modified { get { return i_trainee.id_user_modified; } set { i_trainee.id_user_modified = value; } }
        public DateTime modified_date { get { return DateTime.Parse(i_trainee.modified_date); } }
        public string fullname { get { return i_trainee.fullname; } }

        public virtual login login { get { return i_trainee.login; } set { i_trainee.login = value; } }
        public virtual login login1 { get { return i_trainee.login1; } set { i_trainee.login1 = value; } }
    }
}
