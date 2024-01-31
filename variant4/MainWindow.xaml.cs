using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace variant4
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int captchaAttempts = 0;
        private const int maxCaptchaAttempts = 3;
        private int secondsRemaining;

        public MainWindow()
        {
            InitializeComponent();
            GenerateCaptcha();

        }

        private void GuestLogin_Click(object sender, RoutedEventArgs e)
        {

        }

        private void GenerateCaptcha()
        {

            Random random = new Random();
            string captcha = new string(Enumerable.Range(0, 5).Select(_ => (char)('A' + random.Next(26))).ToArray());

            captchaText.Text = captcha;

        }
        
        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            

            string connect = "data source=stud-mssql.sttec.yar.ru,38325;user id=user53_db;password=user53;MultipleActiveResultSets=True;App=EntityFramework";
            SqlConnection myConnection = new SqlConnection(connect);
            myConnection.Open();
            string enteredCaptcha = captchaInput.Text;
            string currentCaptcha = captchaText.Text;

            if (enteredCaptcha.Equals(currentCaptcha, StringComparison.OrdinalIgnoreCase))
            {
                captchaAttempts = 0;
                string command = "Select * from [YP07User] where UserLogin='" + txtLogin.Text + "' and UserPassword='" + txtPassword.Password + "'";
                string Login = "null";
                string Password = "null";
                SqlCommand Auth = new SqlCommand(command, myConnection);
                SqlDataReader rd = Auth.ExecuteReader();
                while (rd.Read())
                {
                    Login = rd.GetString(1);
                    Password = rd.GetString(2);
                }
                if (Password == "null" && Login == "null")

                {
                    MessageBox.Show("Вы ввели неверный логин или пароль!");
                }
                else
                {
                    string command2 = "Select UserRole from [YP07User] where UserLogin='" + txtLogin.Text + "' and UserPassword='" + txtPassword.Password + "'";
                    SqlCommand UserRole = new SqlCommand(command2, myConnection);
                    int idrole = (int)UserRole.ExecuteScalar();
                    if (idrole == 1)
                    {
                        MessageBox.Show("Вы успешно вошли как администратор!");
                    }
                    else if (idrole == 2)
                    {
                        MessageBox.Show("Вы успешно вошли как менеджер!");
                    }
                    else if (idrole == 3)
                    {
                        MessageBox.Show("Вы успешно вошли как пользователь!");
                    }
                    else
                    {

                    }
                }
            }
            else
            {
                
                MessageBox.Show("Неверная капча. Пожалуйста, повторите попытку.");
                GenerateCaptcha();
                captchaAttempts++;
            }
            if (captchaAttempts >= maxCaptchaAttempts)
            {
                Login.IsEnabled = false;
                MessageBox.Show("Превышено максимальное количество попыток ввода капчи. Кнопки заблокированы на 10 секунд");
                LockButtonForSeconds(10);
                await Task.Delay(secondsRemaining * 1000);

                UnlockButton();
            }
            myConnection.Close();

        }
        private void LockButtonForSeconds(int seconds)
        {
            secondsRemaining = seconds;
        }

        private void UnlockButton()
        {
            Login.IsEnabled = true;
        }
    }

}
