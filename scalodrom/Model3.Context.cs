﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace scalodrom
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class project_dbEntities1 : DbContext
    {
        public project_dbEntities1()
            : base("name=scalodromEntities3")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<angle_series> angle_series { get; set; }
        public virtual DbSet<grapple_series> grapple_series { get; set; }
        public virtual DbSet<login> login { get; set; }
        public virtual DbSet<tr_path> tr_path { get; set; }
        public virtual DbSet<trainee> trainee { get; set; }
        public virtual DbSet<training> training { get; set; }
        public virtual DbSet<testtbl> testtbl { get; set; }
    }
}
