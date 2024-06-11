using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

namespace ООО_Техносервис_2
{
    /// <summary>
    /// Логика взаимодействия для AuthWindow.xaml
    /// </summary>
    public partial class AuthWindow : Window
    {
        public AuthWindow()
        {
            InitializeComponent();
        }

        private void btnAuth_Click(object sender, RoutedEventArgs e)
        {
            DataBaseClass.USERS_ID = "sa";
            DataBaseClass.Password = "12345";
            DataBaseClass.ConnectionString = String.Format(DataBaseClass.ConnectionString, DataBaseClass.USERS_ID, DataBaseClass.Password);
            DataBaseClass dataBaseClass = new DataBaseClass();

            try
            {
                Visibility = Visibility.Hidden;

                SqlCommand sqlCommand = new SqlCommand("select [ROLE_ID] from [USER] where LOGIN_USER = @LOGIN AND PASSWORD_USER = @PASSWORD");
                sqlCommand.Parameters.AddWithValue("@LOGIN", tbxLogin.Text);
                sqlCommand.Parameters.AddWithValue("@PASSWORD", tbxPassword.Password);
                sqlCommand.Connection = dataBaseClass.connection;
                dataBaseClass.connection.Open();
                int role = (int)sqlCommand.ExecuteScalar();

                if (role != null)
                {
                    if(role == 1)
                    {
                        MainWindow window = new MainWindow();
                        window.Show();
                    }
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль");
                }

            }
            catch
            {
                MessageBox.Show("Неверный логин или пароль");
            }
            finally
            {
                dataBaseClass.connection.Close();
                tbxLogin.Clear();
                tbxPassword.Clear();
            }
        }
    }
}
