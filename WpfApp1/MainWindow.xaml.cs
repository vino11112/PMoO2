using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        private readonly CaptchaGenerator captchaGenerator = new CaptchaGenerator();
        public MainWindow()
        {
            InitializeComponent();
        }
        int a = 1;

        private void ShowCaptcha()
        {

            captchaPanel.Visibility = Visibility.Visible;

        }
        private void login_button_Click(object sender, RoutedEventArgs e)
        {

            using (var db = new labaEntities3())
            {
                var usern = db.users.FirstOrDefault(пользователи => пользователи.login == login.Text && пользователи.password == password.Password);
                if (usern == null)
                {
                    MessageBox.Show("!");
                }
                else
                {

                    date.id = usern.id;
                    date.name = usern.name;
                    date.ip = usern.ip;
                    date.services = usern.servicies;
                    date.id_role = usern.id_Role;
                    date.last = usern.lastenter;

                    ShowCaptcha();


                }
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (password.Visibility == Visibility.Visible)
            {
                TextBox.Text = password.Password;
                TextBox.Visibility = Visibility.Visible;
                password.Visibility = Visibility.Collapsed;
                button.Content = "🙈";
            }
            else
            {
                password.Password = TextBox.Text;
                password.Visibility = Visibility.Visible;
                TextBox.Visibility = Visibility.Collapsed;
                button.Content = "👁️";
            }
        }
        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
        }
        private void RefreshCaptcha()
        {
            captchaGenerator.GenerateNewCaptcha();
            captchaImage.Source = captchaGenerator.CaptchaImage;
            captchaTextBox.Clear();
            resultText.Text = "";
        }
        private void RefreshCaptcha_Click(object sender, RoutedEventArgs e) => RefreshCaptcha();
        private void VerifyCaptcha_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new labaEntities3())
            {
                var usern = db.users.FirstOrDefault(пользователи => пользователи.login == login.Text && пользователи.password == password.Password);
                if (captchaTextBox.Text.Equals(captchaGenerator.CurrentCaptchaText, StringComparison.OrdinalIgnoreCase))
                {
                    resultText.Text = "Правильно!";
                    resultText.Foreground = Brushes.Green;
                    if (usern.id_Role == 1)
                    {
                        laborant laborant = new laborant();
                        laborant.Show();
                        this.Close();

                    }
                    else
                    {
                        if (usern.id_Role == 2)
                        {
                            laborantiss laborantiss = new laborantiss();
                            laborantiss.Show();
                            this.Close();
                        }
                        else
                        {
                            if (usern.id_Role == 3)
                            {
                                buh buh = new buh();
                                buh.Show();
                                this.Close();
                            }
                            else if (usern.id_Role == 4)
                            {
                                admin admin = new admin();
                                admin.Show();
                                this.Close();
                            }
                        }
                    }
                }
                else
                {
                    resultText.Text = "Неверно, попробуйте снова";
                    resultText.Foreground = Brushes.Red;

                    a = 1;
                }

            }
        }
    }
}
