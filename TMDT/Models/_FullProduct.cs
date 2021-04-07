using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TMDT.Models
{
    public class _FullProduct
    {
        public int idProduct { get; set; }
        public string nameProduct { get; set; }
        public decimal priceProduct { get; set; }
        public int amountProduct { get; set; }
        public string descriptionProduct { get; set; }
        public DateTime datePost { get; set; }
        public int idCategory_Product { get; set; }
        public int idAccount { get; set; }
        public bool hideProduct { get; set; }
        public string Alias { get; set; }
        public string imageProduct_1 { get; set; }
        public string imageProduct_2 { get; set; }
        public string imageProduct_3 { get; set; }
        public string imageProduct_4 { get; set; }
        public string nameCategory { get; set; }
        public string Fullname { get; set; }
        public bool confirmProduct { get; set; }
        public int flagNull { get; set; }

    }
}