//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace scalodrom
{
    using System;
    using System.Collections.Generic;
    
    public partial class tr_path
    {
        public long id { get; set; }
        public long order { get; set; }
        public long id_training { get; set; }
        public long duration { get; set; }
        public long speed { get; set; }
        public long num_path { get; set; }
    
        public virtual training training { get; set; }
    }
}