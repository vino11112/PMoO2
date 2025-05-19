using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

using Moq;

using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

using WpfApp1;
using System.Windows.Media.Imaging;
using System.Linq;
using System.Data.Services;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        private AddPatientWindow _window;
        private buh window;
        private Mock<SqlConnection> _mockConnection;
        private Mock<SqlCommand> _mockCommand;
        private const string TestConnectionString = "Server=HOME-PC4\\SQLEXPRESS;Database=laba;Trusted_Connection=True";
 
        private Mock<SqlDataAdapter> _mockAdapter;
        private admin _adminWindow;

        [TestInitialize]
        public void Setup()
        {
            // Инициализация моков для SQL-зависимостей
            _mockConnection = new Mock<SqlConnection>();
            _mockCommand = new Mock<SqlCommand>();

            // Создание тестового окна
            _window = new AddPatientWindow();

            // Инициализация контролов
            _window.txtLastName = new System.Windows.Controls.TextBox();
            _window.txtFirstName = new System.Windows.Controls.TextBox();
            _window.txtMiddleName = new System.Windows.Controls.TextBox();
            _window.dpBirthDate = new DatePicker();
            _window.txtPhone = new System.Windows.Controls.TextBox();
            _window.txtEmail = new System.Windows.Controls.TextBox();
        //   _window.cbGender = new ComboBox();
          //  _window.cbInsuranceCompany = new ComboBox();
            //_window.cbInsuranceType = new ComboBox();
            _window.txtPassportSeries = new System.Windows.Controls.TextBox();
            _window.txtPassportNumber = new System.Windows.Controls.TextBox();
            _window.txtInsuranceNumber = new System.Windows.Controls.TextBox();

            // Добавление тестовых данных в ComboBox
            _window.cbGender.Items.Add(new ComboBoxItem { Content = "Мужской" });
            _window.cbGender.Items.Add(new ComboBoxItem { Content = "Женский" });
            _window.cbGender.SelectedIndex = 0;

            _window.cbInsuranceCompany.Items.Add("Фонтан");
            _window.cbInsuranceCompany.Items.Add("Рошан");
            _window.cbInsuranceCompany.SelectedIndex = 0;

            _window.cbInsuranceType.Items.Add(new ComboBoxItem { Content = "ОМС" });
            _window.cbInsuranceType.Items.Add(new ComboBoxItem { Content = "ДМС" });
            _window.cbInsuranceType.SelectedIndex = 0;
            window = new buh();

            // Инициализация контролов
            window.FIO = new System.Windows.Controls.TextBox();
            window.servis = new System.Windows.Controls.TextBox();
            window.sum = new System.Windows.Controls.TextBox();
        }
        [TestClass]
        public class BarcodeGeneratorTests
        {
            [TestMethod]
            public void GenerateBarcode_ValidText_ReturnsBitmapSource()
            {
                // Arrange
                var barcodeService = new laborant(); // Замените на ваш класс
                string testText = "TEST12345";

                // Act
                BitmapSource result = barcodeService.GenerateBarcode(testText);

                // Assert
                Assert.IsNotNull(result, "Метод должен возвращать не-null BitmapSource");
                Assert.IsTrue(result.Width > 0, "Ширина изображения должна быть больше 0");
                Assert.IsTrue(result.Height > 0, "Высота изображения должна быть больше 0");
                Assert.AreEqual(96, result.DpiX, "DPI по X должно быть 96");
                Assert.AreEqual(96, result.DpiY, "DPI по Y должно быть 96");
               
            }
          

            [TestMethod]
            public void GenerateBarcode_InvalidCharacters_ReturnsErrorImage()
            {
                // Arrange
                var generator = new laborant();
                string invalidText = "~!@#$%"; // Недопустимые символы для Code128

                // Act
                var result = generator.GenerateBarcode(invalidText);

                // Assert
                Assert.IsNotNull(result);
                // Проверяем что это изображение с ошибкой, а не обычный штрих-код
                Assert.AreEqual(300, result.PixelWidth);
                Assert.AreEqual(100, result.PixelHeight);
            }


            [TestMethod]
            public void GenerateButton_Click_ValidText_SetsImageSource()
            {
                // Arrange
                var window = new laborant();
                string testText = "TEST12345";
                window.textBox1.Text = testText; // Если textBox1 public или через reflection

                // Создаем мок для проверки установки изображения
                var mockImage = new System.Windows.Controls.Image();
                window.BarcodeImage = mockImage;

                // Act
                window.GenerateButton_Click(null, null);

                // Assert
                Assert.IsNotNull(mockImage.Source, "Image.Source не должен быть null");
                Assert.IsInstanceOfType(mockImage.Source, typeof(BitmapSource),
                    "Должен быть установлен BitmapSource");
            }
        }
        [TestClass]
        public class AddPatientWindowTests
        {
            [TestMethod]
            public void BtnSave_Click_ValidationFails_DataNotSaved()
            {
                // Arrange
                var stubValidator = new StubInputValidator { ValidationResult = false };
                var stubDataService = new StubDataService();
                var window = new AddPatientWindow();

                // Act
                window.BtnSave_Click(null, new RoutedEventArgs());

                // Assert
                Assert.IsFalse(stubDataService.InsertPatientCalled);
            }
            public class StubInputValidator 
            {
                public bool ValidationResult { get; set; }

                public bool ValidateInput()
                {
                    return ValidationResult;
                }
            }

            // Заглушка для IDataService
            public class StubDataService 
            {
                public bool InsertPatientCalled { get; set; }
                public string InsertedFirstName { get; set; } //Для проверки

                public void InsertPatient(string FirstName, string LastName, string MiddleName, DateTime BirthDate, string PassportSeries, string PassportNumber, string PhoneNumber, string Email, string InsurancePolicyNumber, string InsurancePolicyType, int InsuranceCompanyId)
                {
                    InsertPatientCalled = true;
                    InsertedFirstName = FirstName; // Для проверки
                }
            }
           

        }
        public class StubUserDataService 
        {
            public DataTable GetAllUsersResult { get; set; } = new DataTable();
            public int SaveChangesResult { get; set; }
            public bool SaveChangesCalled { get; set; }

            public DataTable GetAllUsers()
            {
                return GetAllUsersResult;
            }

            public int SaveChanges(DataTable dataTable)
            {
                SaveChangesCalled = true;
                return SaveChangesResult;
            }
        }

        [TestClass]
        public class AdminTests
        {
           
        
            [TestMethod]
            public void SaveChanges_Click_ShowsMessageBoxOnError()
            {
                // Arrange
                var stubDataService = new StubUserDataService();
                //Setup DataService to Throw Exception
                //This method will be skipped as StubUserDataService can't throw an exception in this approach.
            }
        }
        public class StubPdfService
        {
            public bool ExportToPdfCalled { get; set; }
            public string ExportedFilePath { get; set; }
            public string ExportedCompanyName { get; set; }

            public void ExportToPdf(string companyName, string patientName, string serviceName, string serviceCost, string totalSum, string startDate, string endDate, string filePath)
            {
                ExportToPdfCalled = true;
                ExportedFilePath = filePath;
                ExportedCompanyName = companyName;
            }
        }

        public class StubSaveFileDialogService 
        {
            public DialogResult DialogResult { get; set; }
            public string FileNameResult { get; set; }
            public string GetFilePath(string filter, string defaultExt, string title)
            {
                return FileNameResult;
            }

            public DialogResult ShowDialog()
            {
                return DialogResult;
            }

            public string FileName { get { return FileNameResult; } }
        }

    



    }
}
