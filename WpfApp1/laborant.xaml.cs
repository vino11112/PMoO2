using BarcodeStandard;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using SkiaSharp;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Data.Entity.Infrastructure;
namespace WpfApp1
{
    public partial class laborant : Window
    {
        public laborant()
        {
            InitializeComponent();
            LoadUserData();
            SessionManager.TimeUpdated += UpdateTimeDisplay;
            SessionManager.WarningShown += ShowWarning;
            SessionManager.SessionEnded += OnSessionEnded;
            LoadImage(1, photo);
            // Запускаем сеанс
            SessionManager.StartSession();
        }
        public BitmapSource GenerateBarcode(string text)
        {
            var barcode = new Barcode
            {
                IncludeLabel = true,
                Width = 300,
                Height = 100
            };
            using (var image = barcode.Encode(BarcodeStandard.Type.Code128, text))
            using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
            using (var stream = new MemoryStream(data.ToArray()))
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = stream;
                bitmap.EndInit();
                return bitmap;
            }
        }
        public void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            string textToEncode = textBox1.Text;
            if (!string.IsNullOrEmpty(textToEncode))
            {
                BarcodeImage.Source = GenerateBarcode(textToEncode);
            }
        }
        private void UpdateTimeDisplay(string timeText)
        {
            SessionTimeText.Text = timeText;
            // Теперь правильно обращаемся к публичным константам
            var timeLeft = SessionManager.RemainingTime;
            if (timeLeft <= SessionManager.WarningBefore)
            {
                SessionTimeText.Foreground = System.Windows.Media.Brushes.Orange;
            }
            if (timeLeft <= TimeSpan.FromMinutes(1))
            {
                SessionTimeText.Foreground = System.Windows.Media.Brushes.Red;
            }
        }
        private void ShowWarning()
        {
            MessageBox.Show($"До окончания сеанса осталось {SessionManager.WarningBefore.Minutes} минут. " +
                          "Завершите работу и сохраните данные.",
                          "Предупреждение");
        }
        private void OnSessionEnded()
        {
            MessageBox.Show("Время сеанса истекло. Система будет заблокирована.");
            // Показываем окно блокировки
            var blockedWindow = new BlockedWindow(SessionManager.GetBlockDuration());
            blockedWindow.Show();
            // Закрываем текущее окно
            this.Close();
        }
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            SessionManager.EndSession();
            this.Close();
        }
        protected override void OnClosed(EventArgs e)
        {
            // Отписываемся от событий при закрытии окна
            SessionManager.TimeUpdated -= UpdateTimeDisplay;
            SessionManager.WarningShown -= ShowWarning;
            SessionManager.SessionEnded -= OnSessionEnded;
            base.OnClosed(e);
        }
        private void LoadUserData()
        {
            string a = date.id_role.ToString();
            Role.Content = a;
            name.Content = date.name;
            services.Content = date.services;
        }
        public byte[] GetImageFromDatabase(int imageId)
        {
            byte[] imageData = null;
            string connectionString = "Server=HOME-PC4\\SQLEXPRESS;Database=laba;Trusted_Connection=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = $"SELECT foto FROM Users  WHERE Id ={date.id}";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", imageId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            imageData = (byte[])reader["foto"];
                        }
                    }
                }
            }
            return imageData;
        }
        public BitmapImage ConvertByteArrayToImage(byte[] imageData)
        {
            using (MemoryStream ms = new MemoryStream(imageData))
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = ms;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
                image.Freeze(); // Замораживаем изображение для использования в другом потоке
                return image;
            }
        }
        public void LoadImage(int imageId, System.Windows.Controls.Image imageControl)
        {
            byte[] imageData = GetImageFromDatabase(imageId);
            if (imageData != null)
            {
                BitmapImage bitmapImage = ConvertByteArrayToImage(imageData);
                imageControl.Source = bitmapImage;
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AddPatientWindow patient = new AddPatientWindow();
            patient.ShowDialog();
       
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string servies = textBox1.Text;
            string textToEncode = textBox1.Text;
            int ff = Convert.ToInt32(date.id);
            DateTime dat = date1.SelectedDate.Value;
            int kod_pat = Convert.ToInt32(patient.Text);
            string connectionString = "Server=HOME-PC4\\SQLEXPRESS;Database=laba;Trusted_Connection=True";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (var context = new labaEntities3())
                    {
                        var newBio = new Biomaterials
                        {
                            Barcode = textToEncode,
                            ReceptionDate = dat,
                            id_user = ff,                           
                            Id_Patients = kod_pat                         
                        };
                        context.Biomaterials.Add(newBio);
                        context.SaveChanges();
                        int id_bio = newBio.BiomaterialID;
                        int recordsAffected = context.SaveChanges();
                        using (var context1 = new labaEntities3())
                        {
                            var newOrders = new Orders
                            {
                                BiomaterialID = id_bio,
                                OrderDate = dat,
                                PatientID = kod_pat,   
                                services=" ",
                               
                              TotalCost=Convert.ToDecimal(sum.Text),
                              Status="ожидание"
                              
                            };
                            context1.Orders.Add(newOrders);
                            context1.SaveChanges();
                            if (!context.Patients.Any(p => p.PatientID == newOrders.PatientID))
                            {
                                throw new ArgumentException("Указанный пациент не существует");
                            }
                            else MessageBox.Show("запись добавлена");
                        }
                    }
                    connection.Close();
                }
            }
            catch (DbUpdateException dbEx)
            {
                MessageBox.Show($"Ошибка сохранения данных: {dbEx.InnerException?.Message ?? dbEx.Message}",
                    "Ошибка базы данных", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}




