using System;
using System.Collections.Generic;
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

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для laborantiss.xaml
    /// </summary>
    public partial class laborantiss : Window
    {
        /*  private readonly LabDbContext _context;
          private readonly HttpClient _httpClient;
          private readonly List<Analyzer> _analyzers;
          private System.Timers.Timer _statusCheckTimer;
          public laborantiss()
          {
              InitializeComponent();
          }
          public class Service
          {
              public int Id { get; set; }
              public string Code { get; set; }
              public string Name { get; set; }
              public decimal Price { get; set; }
              public string ResultType { get; set; }
              public string AvailableAnalyzers { get; set; } // Разделитель "|"
          }

          public class Analyzer
          {
              public int Id { get; set; }
              public string Name { get; set; }
              public bool IsBusy { get; set; }
          }

          public class OrderService
          {
              public int Id { get; set; }
              public int OrderId { get; set; }
              public int ServiceId { get; set; }
              public string Status { get; set; } // "Не выполнено", "Отправлено", "Выполнено", "Ошибка"
              public string Result { get; set; }
              public int? AnalyzerId { get; set; }
              public DateTime? SentDate { get; set; }
              public DateTime? CompletedDate { get; set; }
              public int Progress { get; set; }

              public virtual Service Service { get; set; }
              public virtual Analyzer Analyzer { get; set; }
          }
          private void LoadAnalyzers()
          {
              _analyzers = _context.Analyzers.ToList();
              cbAnalyzers.ItemsSource = _analyzers;
              if (_analyzers.Any())
                  cbAnalyzers.SelectedIndex = 0;
          }
          private void LoadPendingServices()
          {
              var pendingServices = _context.OrderServices
                  .Include(os => os.Service)
                  .Include(os => os.Analyzer)
                  .Where(os => os.Status == "Не выполнено")
                  .ToList();

              dgPendingServices.ItemsSource = pendingServices;
          }
          private void LoadInProgressServices()
          {
              var inProgressServices = _context.OrderServices
                  .Include(os => os.Service)
                  .Include(os => os.Analyzer)
                  .Where(os => os.Status == "Отправлено")
                  .ToList();

              dgInProgressServices.ItemsSource = inProgressServices;
          }
          private async void BtnSendToAnalyzer_Click(object sender, RoutedEventArgs e)
          {
              if (dgPendingServices.SelectedItem is not OrderService selectedService)
                  return;

              var selectedAnalyzer = cbAnalyzers.SelectedItem as Analyzer;
              if (selectedAnalyzer == null || selectedAnalyzer.IsBusy)
              {
                  MessageBox.Show("Выберите доступный анализатор");
                  return;
              }

              try
              {
                  // Подготовка данных для API
                  var requestData = new
                  {
                      patient = selectedService.Order.PatientId,
                      services = new[] { new { serviceCode = selectedService.Service.Code } }
                  };

                  var json = JsonConvert.SerializeObject(requestData);
                  var content = new StringContent(json, Encoding.UTF8, "application/json");

                  // Отправка на анализатор
                  var response = await _httpClient.PostAsync(
                      $"http://localhost:5000/api/analyzer/{selectedAnalyzer.Name}", content);

                  if (response.IsSuccessStatusCode)
                  {
                      // Обновление статуса в БД
                      selectedService.Status = "Отправлено";
                      selectedService.AnalyzerId = selectedAnalyzer.Id;
                      selectedService.SentDate = DateTime.Now;
                      selectedAnalyzer.IsBusy = true;

                      await _context.SaveChangesAsync();

                      LoadPendingServices();
                      LoadInProgressServices();

                      MessageBox.Show("Услуга отправлена на анализатор");
                  }
                  else
                  {
                      var error = await response.Content.ReadAsStringAsync();
                      MessageBox.Show($"Ошибка: {error}");
                  }
              }
              catch (Exception ex)
              {
                  MessageBox.Show($"Ошибка при отправке на анализатор: {ex.Message}");
              }
          }
          private async void CheckServicesStatus(object sender, System.Timers.ElapsedEventArgs e)
          {
              await Dispatcher.InvokeAsync(async () =>
              {
                  var inProgressServices = _context.OrderServices
                      .Include(os => os.Service)
                      .Include(os => os.Analyzer)
                      .Where(os => os.Status == "Отправлено")
                      .ToList();

                  foreach (var service in inProgressServices)
                  {
                      try
                      {
                          var response = await _httpClient.GetAsync(
                              $"http://localhost:5000/api/analyzer/{service.Analyzer.Name}");

                          if (response.IsSuccessStatusCode)
                          {
                              var content = await response.Content.ReadAsStringAsync();
                              var result = JsonConvert.DeserializeObject<dynamic>(content);

                              if (result.progress != null)
                              {
                                  // Обновление прогресса
                                  service.Progress = result.progress;
                              }
                              else if (result.services != null)
                              {
                                  // Результат готов
                                  var serviceResult = result.services[0];
                                  service.Result = serviceResult.result.ToString();

                                  // Проверка аномальных результатов
                                  if (IsAbnormalResult(service.Service.ResultType, service.Result))
                                  {
                                      MessageBox.Show($"Внимание! Возможен сбой исследования или некачественный биоматериал. Результат: {service.Result}");
                                  }
                              }

                              await _context.SaveChangesAsync();
                          }
                      }
                      catch (Exception ex)
                      {
                          Debug.WriteLine($"Ошибка проверки статуса: {ex.Message}");
                      }
                  }

                  LoadInProgressServices();
              });
          }
          private bool IsAbnormalResult(string resultType, string result)
          {
              if (resultType == "Integer" && int.TryParse(result, out int intValue))
              {
                  // Логика проверки для числовых результатов
                  // (пример: если значение в 5 раз больше среднего)
                  return false; // Заменить на реальную логику
              }
              return false;
          }
          private async void BtnApproveResult_Click(object sender, RoutedEventArgs e)
          {
              if (dgInProgressServices.SelectedItem is not OrderService selectedService)
                  return;

              selectedService.Status = "Выполнено";
              selectedService.CompletedDate = DateTime.Now;
              selectedService.Analyzer.IsBusy = false;

              await _context.SaveChangesAsync();

              LoadInProgressServices();
              MessageBox.Show("Результат одобрен");
          }
          private async void BtnRejectResult_Click(object sender, RoutedEventArgs e)
          {
              if (dgInProgressServices.SelectedItem is not OrderService selectedService)
                  return;

              selectedService.Status = "Ошибка";
              selectedService.Result = "Требуется повторный забор биоматериала";
              selectedService.Analyzer.IsBusy = false;

              await _context.SaveChangesAsync();

              LoadInProgressServices();
              MessageBox.Show("Результат отклонен");
          }
          protected override void OnClosed(EventArgs e)
          {
              _statusCheckTimer?.Stop();
              _statusCheckTimer?.Dispose();
              _context?.Dispose();
              base.OnClosed(e);
          }


          private void CbAnalyzers_SelectionChanged(object sender, SelectionChangedEventArgs e)
          {

          }
      }*/
    }
}
