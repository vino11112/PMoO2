using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Data;
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


namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для admin.xaml
    /// </summary>
    public partial class admin : Window
    {
        string connectionString = "Server=HOME-PC4\\SQLEXPRESS;Database=laba;Trusted_Connection=True";
        public DataTable _usersTable;

        public admin()
        {
            InitializeComponent();
            DataContext = this;
            GetAllUsers();
        }
        public DataTable GetAllUsers()
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand("SELECT * FROM Patients", connection))
            {
                var adapter = new SqlDataAdapter(command);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable;
            }
        }
        public int SaveChanges(DataTable dataTable)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var adapter = new SqlDataAdapter("SELECT * FROM Patients", connection))
            {
                var commandBuilder = new SqlCommandBuilder(adapter);
                adapter.InsertCommand = commandBuilder.GetInsertCommand();
                adapter.UpdateCommand = commandBuilder.GetUpdateCommand();
                adapter.DeleteCommand = commandBuilder.GetDeleteCommand();

                return adapter.Update(dataTable);
            }
        }
        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            AddPatientWindow addPatientWindow = new AddPatientWindow();
            addPatientWindow.ShowDialog();

        }

        public void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                usersGrid.CommitEdit(); // Сохраняем текущее редактирование
                var changesCount = SaveChanges(_usersTable);
              MessageBox.Show($"Изменения сохранены. Затронуто записей: {changesCount}")  ;
               GetAllUsers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка");
                
            }
        }
        private void RefreshData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _usersTable = GetAllUsers();
                usersGrid.ItemsSource = _usersTable.DefaultView;
                statusText.Text = $"Данные загружены. Записей: {_usersTable.Rows.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка");
                statusText.Text = "Ошибка загрузки данных";
            }
        }
    }
}
