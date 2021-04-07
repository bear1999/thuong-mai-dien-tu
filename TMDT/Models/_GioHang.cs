using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TMDT.Models
{
    public class _GioHang
    {
        ChoDoCuEntities db = new ChoDoCuEntities();
        public int Id { set; get; }
        public string sanpham { set; get; }
        public string hinh { set; get; }
        public Double dongia { set; get; }
        public DateTime Ngaydat { set; get; }
        public int soluong { set; get; }
        public string alias { get; set; }
        public string status { get; set; }
        public Double thanhtien
        {
            get { return soluong * dongia; }
        }
        public _GioHang(int id)
        {
            Id = id;
            Product product = db.Products.Single(n => n.idProduct == Id);
            //Category_Status sta = db.Category_Status.Single(n => n.idCate_status == Id);
            sanpham = product.nameProduct;
            hinh = product.imageProduct_1;
            dongia = double.Parse(product.priceProduct.ToString());
            soluong = 1;
            alias = product.Alias;
            //status = sta.nameStatus;
        }

        public _GioHang()
        {
        }
    }
}
