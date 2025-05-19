using CsvHelper;
using CsvHelper.Configuration;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
    /// Логика взаимодействия для buh.xaml
    /// </summary>
    public partial class buh : Window
    {
        public buh()
        {
            InitializeComponent();
            Title="Бухгалтер"+" "+date.name;
        }
        public void export(ComboBox company,TextBox FIO, TextBox cast, TextBox servis, TextBox sum, DatePicker date1,DatePicker date2)
        {
            var records = new List<dynamic>
            {
                new
                {
                    Название=company.Text,
                    ФИО=FIO.Text,
                    Услуга=servis.Text,
                    СтоимостьУслуги=cast.Text,
                    ИтоговаяСумма=sum.Text,
                    От=Convert.ToDateTime(date1.Text),
                    До=Convert.ToDateTime(date2.Text),
                }
            };
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv",
                DefaultExt = "csv",
                Title = "Export TextBox values to CSV"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        Delimiter = ";",
                        Encoding = Encoding.UTF8,
                    };
                    using (var writer=new StreamWriter(saveFileDialog.FileName))
                    using (var csv=new CsvWriter(writer,config))
                    {
                        csv.WriteRecord(records);
                    }
                    MessageBox.Show("Data exported successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch(Exception ex)
                {
                    MessageBox.Show($"Error exporting data:{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            using(var writer=new StreamWriter(saveFileDialog.FileName))
                using (var csv=new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(records);
            }
        }
        private static readonly string FONT_PATH = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf");
        public void ExportPDF(ComboBox company, TextBox FIO, TextBox cast, TextBox servis, TextBox sum, DatePicker date1, DatePicker date2)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf",
                DefaultExt = "pdf",
                Title = "Export to PDF"
            };
            if(saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    BaseFont baseFont = BaseFont.CreateFont(FONT_PATH, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    Font font = new Font(baseFont, 12, Font.NORMAL);
                    Document document = new Document();
                    PdfWriter.GetInstance(document, new FileStream(saveFileDialog.FileName, FileMode.Create));
                    document.Open();
                    document.Add(new iTextSharp.text.Paragraph(" "));
                    document.Add(new iTextSharp.text.Paragraph($"Название страховой компании: {company.Text}",font));
                    document.Add(new iTextSharp.text.Paragraph($"ФИО пациента: {FIO.Text}",font));
                    document.Add(new iTextSharp.text.Paragraph($"Услуга: {servis.Text}",font));
                    document.Add(new iTextSharp.text.Paragraph($"Стоимость услуги: {cast.Text}",font));
                    document.Add(new iTextSharp.text.Paragraph($"Итоговая Сумма: {sum.Text}",font));
                    document.Add(new iTextSharp.text.Paragraph($"C: {date1.Text}",font));
                    document.Add(new iTextSharp.text.Paragraph($"По {date2.Text}",font));
                    document.Close();
                    MessageBox.Show("Успешно");
                }
                catch(Exception ex)
                {
                    MessageBox.Show($"ошибка: {ex.Message}");
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ExportPDF(company, FIO, cast, sum,servis, date1, date2);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            export(company, FIO, cast, sum,servis,date1, date2);
        }
    }


}
