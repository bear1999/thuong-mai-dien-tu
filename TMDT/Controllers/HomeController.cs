using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TMDT.Models;
using TMDT.Function;
using System.Text.RegularExpressions;
using PagedList;

namespace TMDT.Controllers
{
    public class HomeController : Controller
    {
        ChoDoCuEntities db = new ChoDoCuEntities();
        public ActionResult ThongTinCaNhan()
        {
            if (Session["idAccount"] == null) return RedirectToAction("DangNhap", "Login");
            return LoadThongTin();
        }
        [HttpGet]
        public ActionResult CapNhatThongTin()
        {
            LoadThongTin();
            return View();
        }
        [HttpPost]
        public ActionResult CapNhatThongTin(FormCollection collection)
        {
            if (Session["idAccount"] == null) return RedirectToAction("DangNhap", "Login");
            LoadThongTin();
            var hoten = collection["Username"];
            var ngaysinh = String.Format("{0:MM/dd/yyy}", collection["Birthday"]);
            var dc = collection["Address"];
            var mail = collection["Email"];
            var sdt = collection["PhoneNumber"];
            var mk = collection["Password"];
            var mk2 = collection["Password2"];
            int ss = Int32.Parse(Session["idAccount"].ToString());

            var csdt = from acc in db.infoAccounts
                       join lg in db.Logins on acc.idAccount equals lg.idAccount
                       where lg.idAccount != ss
                       select new { lg.Email, acc.PhoneNumber };

            foreach (var checksdt in csdt)
            {
                if (sdt == checksdt.PhoneNumber)
                {
                    ViewData["Err0"] = "Số điện thoại này đã tồn tại trong hệ thống!";
                    return View();
                }
                else if (checksdt.Email == mail)
                {
                    ViewData["Err0"] = "Email này đã tồn tại trong hệ thống!";
                    return View();
                }
            }


            if (mk != mk2)
                ViewData["Err0"] = "Mật khẩu xác nhận không trùng!";
            else
            {
                var query1 = from acc in db.infoAccounts
                             join lg in db.Logins on acc.idAccount equals lg.idAccount
                             where lg.idAccount == ss
                             select new { acc, lg };
                foreach (var info in query1)
                {
                    info.acc.Fullname = hoten;
                    info.acc.Birthday = DateTime.Parse(ngaysinh);
                    info.acc.Address = dc;
                    info.lg.Email = mail;
                    info.acc.PhoneNumber = sdt;
                    if (!String.IsNullOrEmpty(mk) && mk == mk2)
                        info.lg.Password = _Function.md5(mk);
                }
                db.SaveChanges();
                return RedirectToAction("ThongTinCaNhan", "Home");
            }
            return View();
        }
        public ActionResult LoadThongTin()
        {
            int s = Int32.Parse(Session["idAccount"].ToString());
            IList<_FullAccountInfo> infoacc = new List<_FullAccountInfo>();
            var query = from acc in db.infoAccounts
                        join lg in db.Logins on acc.idAccount equals lg.idAccount
                        join tt in db.Roles on acc.idRole equals tt.idRole
                        where s == acc.idAccount
                        select new { acc, lg, tt };

            var infoaccs = query.ToList();
            foreach (var info in infoaccs)
            {
                infoacc.Add(new _FullAccountInfo()
                {
                    idAccount = info.acc.idAccount,
                    Fullname = info.acc.Fullname,
                    Birthday = DateTime.Parse(info.acc.Birthday.ToString()),
                    PhoneNumber = info.acc.PhoneNumber,
                    Email = info.lg.Email,
                    Address = info.acc.Address,
                    ChucVu = info.tt.nameRole,
                });
            }
            return View(infoacc);
        }
        public ActionResult removeIdCate()
        {
            Session.Remove("idCate");
            Session.Remove("nameCate");
            return RedirectToAction("TimKiem");
        }
        public ActionResult getCate()
        {
            var getCate = db.Category_Product.ToList();
            return PartialView(getCate);
        }
        public ActionResult TimKiem(int idCate, int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 10;
            List<_FullProduct> info = new List<_FullProduct>();
            var getNameCate = from e in db.Category_Product
                              where e.idCategory_Product == idCate
                              select e.nameCategory;
            foreach (var g in getNameCate)
                Session["nameCate"] = g;
            Session["idCate"] = idCate;
            if (idCate == 0)
            {
                var a = from b in db.Products
                        join c in db.infoAccounts on b.idAccount equals c.idAccount
                        join e in db.Category_Product on b.idCategory_Product equals e.idCategory_Product
                        where b.hideProduct == false && b.confirmProduct == true
                        orderby b.datePost descending
                        select new { b, c, e };
                foreach (var d in a)
                {
                    info.Add(new _FullProduct()
                    {
                        idProduct = d.b.idProduct,
                        priceProduct = d.b.priceProduct,
                        amountProduct = d.b.amountProduct,
                        datePost = d.b.datePost,
                        idCategory_Product = d.b.idCategory_Product,
                        imageProduct_1 = d.b.imageProduct_1,
                        nameCategory = d.e.nameCategory,
                        nameProduct = d.b.nameProduct,
                        descriptionProduct = d.b.descriptionProduct,
                        Alias = d.b.Alias,
                        Fullname = d.c.Fullname,
                    });
                }
            }
            else
            {
                var a = from b in db.Products
                        join c in db.infoAccounts on b.idAccount equals c.idAccount
                        join e in db.Category_Product on b.idCategory_Product equals e.idCategory_Product
                        where b.hideProduct == false && b.confirmProduct == true && b.idCategory_Product == idCate
                        orderby b.datePost descending
                        select new { b, c, e };
                foreach (var d in a)
                {
                    info.Add(new _FullProduct()
                    {
                        idProduct = d.b.idProduct,
                        priceProduct = d.b.priceProduct,
                        amountProduct = d.b.amountProduct,
                        datePost = d.b.datePost,
                        idCategory_Product = d.b.idCategory_Product,
                        imageProduct_1 = d.b.imageProduct_1,
                        nameCategory = d.e.nameCategory,
                        nameProduct = d.b.nameProduct,
                        descriptionProduct = d.b.descriptionProduct,
                        Alias = d.b.Alias,
                        Fullname = d.c.Fullname,
                    });
                }
            }
            return View(info.ToPagedList(pageNumber, pageSize));
        }
    }
}