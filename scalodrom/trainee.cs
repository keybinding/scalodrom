//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class trainee
    {
        public long id { get; set; }
        public string firstname { get; set; }
        public string middlename { get; set; }
        public string lastname { get; set; }
        public string birthdate { get; set; }
        public string create_date { get; set; }
        public long id_user_create { get; set; }
        public Nullable<long> id_user_modified { get; set; }
        public string modified_date { get; set; }
        public string fullname { get; set; }
    
        public virtual login login { get; set; }
        public virtual login login1 { get; set; }
    }
}
