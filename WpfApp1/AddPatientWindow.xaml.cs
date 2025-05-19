using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
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
using System.Data.SqlClient;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для AddPatientWindow.xaml
    /// </summary>
    public partial class AddPatientWindow : Window
    {

        public AddPatientWindow()
        {
            InitializeComponent();
            dpBirthDate.SelectedDate = DateTime.Now.AddYears(-30);
        

        }
        public bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                MessageBox.Show("Введите фамилию пациента", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtLastName.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtFirstName.Text))
            {
                MessageBox.Show("Введите имя пациента", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtFirstName.Focus();
                return false;
            }

            if (dpBirthDate.SelectedDate == null)
            {
                MessageBox.Show("Укажите дату рождения пациента", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                dpBirthDate.Focus();
                return false;
            }

            if (dpBirthDate.SelectedDate > DateTime.Now)
            {
                MessageBox.Show("Дата рождения не может быть в будущем", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                dpBirthDate.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                MessageBox.Show("Введите номер телефона пациента", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPhone.Focus();
                return false;
            }

            // Проверка формата телефона (базовая)
            if (txtPhone.Text.Length < 10)
            {
                MessageBox.Show("Номер телефона слишком короткий", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPhone.Focus();
                return false;
            }

            // Проверка email, если введен
            if (!string.IsNullOrWhiteSpace(txtEmail.Text) &&
                !txtEmail.Text.Contains("@") && !txtEmail.Text.Contains("."))
            {
                MessageBox.Show("Введите корректный email адрес", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtEmail.Focus();
                return false;
            }

            return true;
        }

        public void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput())
                return;

           
                var LastName = txtLastName.Text.Trim();
                var FirstName = txtFirstName.Text.Trim();
                var MiddleName = txtMiddleName.Text.Trim();
                DateTime BirthDate = dpBirthDate.SelectedDate.Value;
                var Gender = (cbGender.SelectedItem as ComboBoxItem)?.Content.ToString();
            int InsuranceCompanyId=0;
                var PhoneNumber = txtPhone.Text.Trim();
                var Email = txtEmail.Text.Trim();
                var Address = txtAddress.Text.Trim();
                var PassportSeries = txtPassportSeries.Text.Trim();
                var PassportNumber = txtPassportNumber.Text.Trim();
                var InsurancePolicyType = (cbInsuranceType.SelectedItem as ComboBoxItem)?.Content.ToString();
                var InsurancePolicyNumber = txtInsuranceNumber.Text.Trim();
                if(cbInsuranceCompany.Text=="Фонтан")
                InsuranceCompanyId=1;
                else if(cbInsuranceCompany.Text=="Рошан")
               InsuranceCompanyId = 2;
                var CreatedDate = DateTime.Now;
                var LastModifiedDate = DateTime.Now;
                string connectionString = "Server=HOME-PC4\\SQLEXPRESS;Database=laba;Trusted_Connection=True";
                string insertStatement = "INSERT INTO Patients (FirstName, LastName,MiddleName, BirthDate,PassportSeries, PassportNumber, PhoneNumber, Email,InsurancePolicyNumber, InsurancePolicyType,InsuranceCompanyId) VALUES (@FirstName, @LastName, @MiddleName, @BirthDate,@PassportSeries,@PassportNumber,@PhoneNumber,@Email,@InsurancePoliceType,@InsurancePoliceNumber,@InsuranceCompanyId)";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(insertStatement, connection))
                    {
                        command.Parameters.AddWithValue("@FirstName", FirstName);
                        command.Parameters.AddWithValue("@LastName", LastName);
                        command.Parameters.AddWithValue("@BirthDate", BirthDate);
                        command.Parameters.AddWithValue("@MiddleName", MiddleName);
                        command.Parameters.AddWithValue("@PassportSeries", PassportSeries);
                        command.Parameters.AddWithValue("@PassportNumber", PassportNumber);
                        command.Parameters.AddWithValue("@PhoneNumber", PhoneNumber);
                        command.Parameters.AddWithValue("@Email", Email);
                        command.Parameters.AddWithValue("@InsurancePoliceType", InsurancePolicyType);
                        command.Parameters.AddWithValue("@InsurancePoliceNumber", InsurancePolicyNumber);
            
                        command.Parameters.AddWithValue("@InsuranceCompanyId", InsuranceCompanyId);
                        int rowsAffected = command.ExecuteNonQuery();
                        MessageBox.Show($"Добавлено строк: {rowsAffected}", "Информация");
                    }
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
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();

        }
     
    }
}
