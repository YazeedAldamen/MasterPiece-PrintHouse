//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PrintHouse.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class subCategory
    {
        public int subCategoryId { get; set; }
        public int subCategoryName { get; set; }
        public string subCategoryImage { get; set; }
        public string subCategoryDescription { get; set; }
        public Nullable<int> categoryId { get; set; }
    
        public virtual Category Category { get; set; }
    }
}
