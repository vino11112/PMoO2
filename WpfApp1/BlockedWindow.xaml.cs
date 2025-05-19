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
using System.Windows.Threading;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для BlockedWindow.xaml
    /// </summary>
    public partial class BlockedWindow : Window
    {
        private readonly DispatcherTimer _unlockTimer;
        private readonly DateTime _unlockTime;
        public BlockedWindow(TimeSpan blockDuration)
        {
            InitializeComponent();
            _unlockTime = DateTime.Now.Add(blockDuration);

            _unlockTimer = new DispatcherTimer();
            _unlockTimer.Interval = TimeSpan.FromSeconds(1);
            _unlockTimer.Tick += UnlockTimer_Tick;
            _unlockTimer.Start();
        }


        private void UnlockTimer_Tick(object sender, EventArgs e)
        {
            var timeLeft = _unlockTime - DateTime.Now;
            BlockTimeText.Text = $"До разблокировки: {timeLeft:mm\\:ss}";

            if (timeLeft <= TimeSpan.Zero)
            {
                _unlockTimer.Stop();
                this.Close();

                // Возвращаем пользователя на окно входа
               MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
            }
        }
    }
}