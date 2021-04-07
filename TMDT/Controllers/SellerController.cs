using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TMDT.Models;
using TMDT.Function;
using System.Text.RegularExpressions;
using Common;

namespace TMDT.Controllers
{
    public class SellerController : Controller
    {
        ChoDoCuEntities db = new ChoDoCuEntities();
        public ActionResult TrangCaNhan(int id)
        {
            if (id.ToString() == null) return HttpNotFound();

            _TrangCaNhan model = new _TrangCaNhan();
            model._FullAccountInfo = LoadTTCaNhan(id);
            model._FullProduct = LoadBaiDangCaNhan(id);
            return View(model);
        }
        public List<_FullAccountInfo> LoadTTCaNhan(int id)
        {
            List<_FullAccountInfo> LoadTTCaNhan = new List<_FullAccountInfo>();
            var query = from a in db.Logins
                        join b in db.infoAccounts on a.idAccount equals b.idAccount
                        where a.idAccount == id
                        select new { a, b };
            foreach(var i in query.ToList())
            {
                LoadTTCaNhan.Add(new _FullAccountInfo {
                    Email = i.a.Email,
                    Fullname = i.b.Fullname,
                    Address = i.b.Address,
                    PhoneNumber = i.b.PhoneNumber
                });
            }
            return LoadTTCaNhan;
        }
        public List<_FullProduct> LoadBaiDangCaNhan(int id)
        {
            List<_FullProduct> LoadBaiDangCaNhan = new List<_FullProduct>();
            var query = from a in db.Products
                        join b in db.infoAccounts on a.idAccount equals b.idAccount
                        where a.idAccount == id
                        select new { a, b };
            foreach (var i in query.ToList())
            {
                LoadBaiDangCaNhan.Add(new _FullProduct
                {
                    imageProduct_1 = i.a.imageProduct_1,
                    nameProduct = i.a.nameProduct,
                    datePost = i.a.datePost,
                    Fullname = i.b.Fullname,
                    priceProduct = i.a.priceProduct,
                    Alias = i.a.Alias,
                    idProduct = i.a.idProduct,
                });
            }
            return LoadBaiDangCaNhan;
        }
        // GET: Seller
        public ActionResult DanhSachSanPham() //Đăng tin
        {
            if (Session["idAccount"] == null) return RedirectToAction("DangNhap", "Login");
            if (!Equals(Session["idRole"], 1)) return HttpNotFound();
            var id = Int32.Parse(Session["idAccount"].ToString());

            IList<_FullProduct> infoProduct = new List<_FullProduct>();
            var query = from product in db.Products
                        join catePro in db.Category_Product on product.idCategory_Product equals catePro.idCategory_Product
                        where product.idAccount == id
                        orderby product.datePost descending
                        select new { product, catePro };
            var infoProducts = query.ToList();
            int _flagNull = 0;
            foreach (var info in infoProducts)
            {
                if (!info.product.confirmProduct.HasValue)
                    _flagNull = 1;
                infoProduct.Add(new _FullProduct()
                {
                    idProduct = info.product.idProduct,
                    priceProduct = info.product.priceProduct,
                    amountProduct = info.product.amountProduct,
                    descriptionProduct = info.product.descriptionProduct,
                    datePost = info.product.datePost,
                    idCategory_Product = info.product.idCategory_Product,
                    hideProduct = info.product.hideProduct,
                    imageProduct_1 = info.product.imageProduct_1,
                    imageProduct_2 = info.product.imageProduct_2,
                    imageProduct_3 = info.product.imageProduct_3,
                    imageProduct_4 = info.product.imageProduct_4,
                    nameCategory = info.catePro.nameCategory,
                    nameProduct = info.product.nameProduct,
                    flagNull = _flagNull,
                });
            }
            return View(infoProduct);
        }       
    }
}