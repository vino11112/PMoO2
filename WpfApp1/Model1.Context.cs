﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WpfApp1
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class labaEntities3 : DbContext
    {
        public labaEntities3()
            : base("name=labaEntities3")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Biomaterials> Biomaterials { get; set; }
        public virtual DbSet<InsuranceCompanies> InsuranceCompanies { get; set; }
        public virtual DbSet<Orders> Orders { get; set; }
        public virtual DbSet<OrderServices> OrderServices { get; set; }
        public virtual DbSet<PatientInsurance> PatientInsurance { get; set; }
        public virtual DbSet<Patients> Patients { get; set; }
        public virtual DbSet<Reports> Reports { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<service> service { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<users> users { get; set; }
    }
}
