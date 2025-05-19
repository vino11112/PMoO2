using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace WpfApp1
{
    public static class SessionManager
    {
        // Публичные константы для настроек времени
        public static readonly TimeSpan SessionDuration = TimeSpan.FromMinutes(10); // Для теста (реально 2.5 часа)
        public static readonly TimeSpan WarningBefore = TimeSpan.FromMinutes(5);    // Для теста (реально 15 минут)
        public static readonly TimeSpan BlockDuration = TimeSpan.FromMinutes(1);   // Для теста (реально 30 минут)

        private static DispatcherTimer _sessionTimer;
        private static DateTime _sessionEndTime;
        private static bool _isWarningShown;

        public static event Action<string> TimeUpdated;
        public static event Action SessionEnded;
        public static event Action WarningShown;

        public static TimeSpan RemainingTime => _sessionEndTime - DateTime.Now;

        public static void StartSession()
        {
            _sessionEndTime = DateTime.Now.Add(SessionDuration);
            _isWarningShown = false;

            _sessionTimer = new DispatcherTimer();
            _sessionTimer.Interval = TimeSpan.FromSeconds(1);
            _sessionTimer.Tick += SessionTimer_Tick;
            _sessionTimer.Start();
        }
        public static TimeSpan GetBlockDuration() => BlockDuration;
        private static void SessionTimer_Tick(object sender, EventArgs e)
        {
            var timeLeft = RemainingTime;
            TimeUpdated?.Invoke($"Осталось: {timeLeft:mm\\:ss}");

            if (timeLeft <= WarningBefore && !_isWarningShown)
            {
                _isWarningShown = true;
                WarningShown?.Invoke();
            }

            if (timeLeft <= TimeSpan.Zero)
            {
                EndSession();
            }
        }

        public static void EndSession()
        {
            _sessionTimer?.Stop();
            SessionEnded?.Invoke();
        }
    }
}
