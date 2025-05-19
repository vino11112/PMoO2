using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public  class patientadmin
    {
        public int PatientID { get; set; }
        public  string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public  DateTime BirthDate { get; set; }
        public int PassportSeries { get; set; }
        public int PassportNumber { get; set; }
        public int PhoneNumber { get; set; }
        public string Email { get; set; }
        public string InsurancePolicyNumber { get; set; }
        public string InsurancePolicyType { get; set; }
        public int InsuranceCompanyId { get; set; }
    }
}
