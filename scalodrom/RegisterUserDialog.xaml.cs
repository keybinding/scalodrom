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
using System.Windows.Shapes;

namespace scalodrom
{
    /// <summary>
    /// Interaction logic for RegisterUserDialog.xaml
    /// </summary>
    /// 

    public class RegisterUserModel
    {
        public string inputLogin { get; set; }
        public string inputPassword { get; set; }

        public void RegisterNewUser()
        {
            using (var db = new scalodromEntities3())
            {
                login l_new_login = new login() { login1 = inputLogin, password = (inputPassword == "" ? "12345" : inputPassword), approved = 1 };
                try
                {
                    db.login.Add(l_new_login);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    db.Dispose();
                }
                
            }
        }
    }
    
    public partial class RegisterUserDialog : Window
    {

        RegisterUserModel registerUserModel = new RegisterUserModel();

        public RegisterUserDialog()
        {
            InitializeComponent();
        }

        private void cb_ok_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                registerUserModel.RegisterNewUser();
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось создать нового пользователя!", "Сообщение пользователю");
            }
            finally
            {
                Close();
            }
        }

        private void tb_login_TextChanged(object sender, TextChangedEventArgs e)
        {
            registerUserModel.inputLogin = (sender as TextBox).Text;
        }

        private void tb_password_TextChanged(object sender, TextChangedEventArgs e)
        {
            registerUserModel.inputPassword = (sender as TextBox).Text;
        }
    }
}
