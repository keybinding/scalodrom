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

namespace scalodrom
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public class MainWindowModel
    {
        public string loginEntered { get; set; }
        public string passwordEntered { get; set; }

        [Serializable]
        public class WrongPasssword : Exception
        {
            public WrongPasssword() { }
            public WrongPasssword(string message) : base(message) { }
            public WrongPasssword(string message, Exception inner) : base(message, inner) { }
            protected WrongPasssword(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }


        [Serializable]
        public class NoSuchUserException : Exception
        {
            public NoSuchUserException() { }
            public NoSuchUserException(string message) : base(message) { }
            public NoSuchUserException(string message, Exception inner) : base(message, inner) { }
            protected NoSuchUserException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }


        [Serializable]
        public class UserIsNotActiveException : Exception
        {
            public UserIsNotActiveException() { }
            public UserIsNotActiveException(string message) : base(message) { }
            public UserIsNotActiveException(string message, Exception inner) : base(message, inner) { }
            protected UserIsNotActiveException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }

        public login Login()
        {
            IList<login> logins = new List<login>();
            using (var db = new scalodromEntities3())
            {
                logins = (from loginlist in db.login where loginlist.login1 == loginEntered select loginlist).ToList();
            }
            if (logins.Count == 1)
            {
                if (logins[0].password == passwordEntered)
                {
                    if (logins[0].approved == 1)
                    {
                        return logins[0];
                    }
                    else
                    {
                        throw new UserIsNotActiveException("Учетная запись не активирована!");
                    }
                }
                else
                {
                    throw new WrongPasssword("Неверный пароль!");
                }
            }
            else
            {
                throw new NoSuchUserException("Пользователь не найден!");
            }
        }
    }

    public partial class MainWindow : Window
    {
        MainWindowModel i_model = new MainWindowModel();
        public static IList<login> login = new List<login>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                login l_login = i_model.Login();
                if (l_login != null)
                {
                    Window l_mainContentWindow = new MainContentWindow(l_login);
                    Hide();
                    l_mainContentWindow.Show();
                    Close();
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Сообщение пользователю", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            RegisterUserDialog l_dlg = new RegisterUserDialog();
            l_dlg.Owner = this;
            l_dlg.ShowDialog();
        }

        private void tb_login_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox l_tb = sender as TextBox;
            i_model.loginEntered = l_tb.Text;
        }

        private void tb_password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox l_tb = sender as PasswordBox;
            i_model.passwordEntered = l_tb.Password;
        }

        private void tb_login_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) tb_password.Focus();
        }

        private void tb_password_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) Button_Click(sender, new RoutedEventArgs());
        }
    }
}
