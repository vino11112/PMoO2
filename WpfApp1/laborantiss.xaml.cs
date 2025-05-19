using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

using System.IO;


namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для laborantiss.xaml
    /// </summary>
    /// 
    public partial class laborantiss : Window
    {
        string connectionString = "Server=HOME-PC4\\SQLEXPRESS;Database=laba;Trusted_Connection=True";
        public DataTable _usersTable;
        public laborantiss()
        {
            InitializeComponent();
            DataContext = this;
            Title = "Лаборант-исследователь" + " " + date.name;
           
           
        }
        public DataTable GetAllUsers()
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand("SELECT * FROM Orders", connection))
            {
                var adapter = new SqlDataAdapter(command);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable;
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

            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int orderId = Convert.ToInt32(aa.Text);
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Параметризованный запрос для защиты от SQL-инъекций
                using (var command = new SqlCommand(
                    "UPDATE Orders SET Status = @status WHERE OrderID = @orderId",
                    connection))
                {
                    command.Parameters.AddWithValue("@status", "в_обработке");
                    command.Parameters.AddWithValue("@orderId", orderId);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show($"Статус заказа {orderId} изменен на 'в_обработке'",
                                      "Успех",
                                      MessageBoxButton.OK,
                                      MessageBoxImage.Information);
                       
                    }
                    else
                    {
                        MessageBox.Show($"Заказ с ID {orderId} не найден",
                                      "Предупреждение",
                                      MessageBoxButton.OK,
                                      MessageBoxImage.Warning);
                    }
                }
            }
        }
        private string ExportToJson(DataRow row)
        {
            var result = new JObject();
            foreach (DataColumn column in row.Table.Columns)
            {
                result[column.ColumnName] = JToken.FromObject(row[column]);
            }

            var logEntry = new
            {
                Timestamp = DateTime.Now,
                Operation = "StatusUpdate",
                OrderData = result
            };

            return JsonConvert.SerializeObject(logEntry, Formatting.Indented);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            int orderId = Convert.ToInt32(aa.Text);
            DataTable updatedOrder = new DataTable();
            using (var connection = new SqlConnection(connectionString))
           
            using (var updatedCommand = new SqlCommand(
                "SELECT * FROM Orders WHERE OrderID = @orderId",
                connection))
            {
                updatedCommand.Parameters.AddWithValue("@orderId", orderId);
                using (SqlDataAdapter adapter = new SqlDataAdapter(updatedCommand))
                {
                    adapter.Fill(updatedOrder);
                }
            }
            string json = ExportToJson(updatedOrder.Rows[0]);
            string filePath =$"Order_{orderId}_{DateTime.Now:yyyyMMddHHmmss}.json";
            File.WriteAllText(filePath, json);

            MessageBox.Show($"Статус изменен. Данные экспортированы в {filePath}",
                          "Успех",
                          MessageBoxButton.OK,
                          MessageBoxImage.Information);

        }
    }
}
