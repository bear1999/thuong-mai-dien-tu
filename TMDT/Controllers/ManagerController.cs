using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TMDT.Models;

namespace TMDT.Controllers
{
    public class ManagerController : Controller
    {
        ChoDoCuEntities db = new ChoDoCuEntities();

        //Danh sach san pham doi duyet//
        [HttpGet]
        public ActionResult SanPhamDangDoiDuyet()
        {
            if (Session["idAccount"] == null) return RedirectToAction("DangNhap", "Login");
            if (!Equals(Session["idRole"], 4) && !Equals(Session["idRole"], 5)) return HttpNotFound();
            IList<_FullProduct> infoAcc = new List<_FullProduct>();
            var query = from cus in db.infoAccounts
                        join tt in db.Products on cus.idAccount equals tt.idAccount
                        join hihi in db.Category_Product on tt.idCategory_Product equals hihi.idCategory_Product
                        where tt.hideProduct == true && cus.idAccount == tt.idAccount && tt.confirmProduct == null
                        orderby tt.datePost descending
                        select new { cus, tt, hihi };

            var infoAccs = query.ToList();
            foreach (var info in infoAccs)
            {
                infoAcc.Add(new _FullProduct()
                {
                    idProduct = info.tt.idProduct,
                    nameProduct = info.tt.nameProduct,
                    priceProduct = info.tt.priceProduct,
                    amountProduct = info.tt.amountProduct,
                    descriptionProduct = info.tt.descriptionProduct,
                    datePost = info.tt.datePost,
                    nameCategory = info.hihi.nameCategory,
                    Fullname = info.cus.Fullname,
                    imageProduct_1 = info.tt.imageProduct_1,
                    imageProduct_2 = info.tt.imageProduct_2,
                    imageProduct_3 = info.tt.imageProduct_3,
                    imageProduct_4 = info.tt.imageProduct_4
                });
            }
            return View(infoAcc);
        }

        //Danh sach san pham duoc duyet//
        [HttpGet]
        public ActionResult SanPhamDaDuyet()
        {
            if (Session["idAccount"] == null) return RedirectToAction("DangNhap", "Login");
            if (!Equals(Session["idRole"], 4) && !Equals(Session["idRole"], 5)) return HttpNotFound();
            IList<_FullProduct> infoAcc = new List<_FullProduct>();
            var query = from cus in db.infoAccounts
                        join tt in db.Products on cus.idAccount equals tt.idAccount
                        join hihi in db.Category_Product on tt.idCategory_Product equals hihi.idCategory_Product
                        where tt.hideProduct == false && cus.idAccount == tt.idAccount
                        orderby tt.datePost descending
                        select new { cus, tt, hihi };

            var infoAccs = query.ToList();
            foreach (var info in infoAccs)
            {
                infoAcc.Add(new _FullProduct()
                {
                    idProduct = info.tt.idProduct,
                    nameProduct = info.tt.nameProduct,
                    priceProduct = info.tt.priceProduct,
                    amountProduct = info.tt.amountProduct,
                    descriptionProduct = info.tt.descriptionProduct,
                    datePost = info.tt.datePost,
                    nameCategory = info.hihi.nameCategory,
                    Fullname = info.cus.Fullname,
                    imageProduct_1 = info.tt.imageProduct_1,
                    imageProduct_2 = info.tt.imageProduct_2,
                    imageProduct_3 = info.tt.imageProduct_3,
                    imageProduct_4 = info.tt.imageProduct_4
                });
            }
            return View(infoAcc);
        }
        //Duyet san pham//
        public ActionResult DuyetSanPham()
        {
            var getid = Url.RequestContext.RouteData.Values["id"];
            int g = Int32.Parse(getid.ToString());
            var a = from b in db.Products
                    join h in db.Logins on b.idAccount equals h.idAccount
                    where b.idProduct == g && b.hideProduct == true
                    select new { b, h };
            foreach (var d in a)
            {
                d.b.hideProduct = false;
                d.b.confirmProduct = true;
                string content = System.IO.File.ReadAllText(Server.MapPath("~/Assets/SendMail/DuyetSanPham.html"));
                content = content.Replace("{{idproduct}}", d.b.idProduct.ToString());
                new MailHelper().SendMail(d.h.Email, "[No Reply] Tình Trạng Sản Phẩm", content);
            }
            db.SaveChanges();
            return RedirectToAction("SanPhamDangDoiDuyet");
        }
        //Tu choi san pham//
        public ActionResult TuChoiSanPham()
        {
            var getid = Url.RequestContext.RouteData.Values["id"];
            int g = Int32.Parse(getid.ToString());
            var a = from b in db.Products
                    join h in db.Logins on b.idAccount equals h.idAccount
                    where b.idProduct == g && b.hideProduct == true
                    select new { b, h };
            foreach (var d in a)
            {
                d.b.hideProduct = true;
                d.b.confirmProduct = false;
                string content = System.IO.File.ReadAllText(Server.MapPath("~/Assets/SendMail/TuChoiSanPham.html"));
                content = content.Replace("{{idproduct}}", d.b.idProduct.ToString());
                new MailHelper().SendMail(d.h.Email, "[No Reply] Tình Trạng Sản Phẩm", content);
            }
            db.SaveChanges();
            return RedirectToAction("SanPhamDangDoiDuyet");
        }
        [NonAction]
        public void DropDownListChucVu()
        {
            var dataList = new SelectList(
                            (
                                from position in db.Roles
                                where position.groupRole == 2
                                select new SelectListItem { Text = position.nameRole, Value = position.idRole.ToString() }
                            ), "Value", "Text");
            ViewBag.Loadchucvu = dataList;
        }
        // GET: Manager
        //Quản lý khách hàng//
        [HttpGet]
        public ActionResult QuanLyTaiKhoanKH()
        {
            if (Session["idAccount"] == null) return RedirectToAction("DangNhap", "Login");
            if (!Equals(Session["idRole"], 4) && !Equals(Session["idRole"], 5)) return HttpNotFound();
            IList<_FullAccountInfo> infoAcc = new List<_FullAccountInfo>();
            var query = from acc in db.infoAccounts
                        join lg in db.Logins on acc.idAccount equals lg.idAccount
                        join role in db.Roles on acc.idRole equals role.idRole
                        where role.groupRole == 1 && lg.hideAcc == false
                        orderby lg.idAccount descending
                        select new { acc, lg, role };

            var infoAccs = query.ToList();
            foreach (var info in infoAccs)
            {
                infoAcc.Add(new _FullAccountInfo()
                {
                    idAccount = info.lg.idAccount,
                    Fullname = info.acc.Fullname,
                    Email = info.lg.Email,
                    Birthday = info.acc.Birthday,
                    Address = info.acc.Address,
                    PhoneNumber = info.acc.PhoneNumber,
                    ChucVu = info.role.nameRole
                });
            }
            return View(infoAcc);
        }

        //Quản lý nhân viên//
        [HttpGet]
        public ActionResult QuanLyNhanVien()
        {
            if (Session["idAccount"] == null) return RedirectToAction("DangNhap", "Login");
            if (!Equals(Session["idRole"], 5)) return HttpNotFound();
            IList<_FullAccountInfo> infoAcc = new List<_FullAccountInfo>();
            var query = from acc in db.infoAccounts
                        join lg in db.Logins on acc.idAccount equals lg.idAccount
                        join role in db.Roles on acc.idRole equals role.idRole
                        where role.groupRole == 2 && lg.hideAcc == false
                        orderby lg.idAccount descending
                        select new { acc, lg, role };

            var infoAccs = query.ToList();
            foreach (var info in infoAccs)
            {
                infoAcc.Add(new _FullAccountInfo()
                {
                    idAccount = info.lg.idAccount,
                    Fullname = info.acc.Fullname,
                    Email = info.lg.Email,
                    Birthday = info.acc.Birthday,
                    Address = info.acc.Address,
                    PhoneNumber = info.acc.PhoneNumber,
                    ChucVu = info.role.nameRole,
                });
            }
            return View(infoAcc);
        }
        //Danh muc san pham//
        public ActionResult DanhMucSanPham()
        {
            if (Session["idAccount"] == null) return RedirectToAction("DangNhap", "Login");
            if (!Equals(Session["idRole"], 5)) return HttpNotFound();
            var query = from acc in db.Category_Product
                        select acc;
            return View(query.ToList());
        }
        [HttpGet]
        public ActionResult ThemDanhMuc()
        {
            if (Session["idAccount"] == null) return RedirectToAction("DangNhap", "Login");
            if (!Equals(Session["idRole"], 5)) return HttpNotFound();
            return View();
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult ThemDanhMuc(FormCollection collection, HttpPostedFileBase imageCategory)
        {
            if (Session["idAccount"] == null) return RedirectToAction("DangNhap", "Login");
            if (!Equals(Session["idRole"], 5)) return HttpNotFound();
            var ma = collection["id"];
            var ten = collection["name"];
            var icon = collection["icon"];


            var csdt = from lg in db.Category_Product
                       select lg;
            foreach (var checksdt in csdt)
            {
                if (checksdt.idCategory_Product == Int32.Parse(ma.ToString()))
                {
                    ViewData["Err55"] = "Mã này đã tồn tại trong hệ thống!";
                    return View();
                }
                else if (checksdt.nameCategory == ten)
                {
                    ViewData["Err55"] = "Tên này đã tồn tại trong hệ thống!";
                    return View();
                }
            }
            if (String.IsNullOrEmpty(ten))
                ViewData["Err5"] = "Vui lòng nhập Tên danh mục!";
            else if (String.IsNullOrEmpty(ma))
                ViewData["Err5"] = "VUi lòng chọn Mã danh mục!";
            else if (String.IsNullOrEmpty(icon))
                ViewData["Err5"] = "Vui lòng nhập Icon danh mục!";
            else
            {
                Category_Product info = new Category_Product();
                if (ModelState.IsValid)
                {
                    string extension1 = System.IO.Path.GetExtension(imageCategory.FileName);
                    if (Equals(extension1, ".png") || Equals(extension1, ".jpg"))
                    {
                        var filename1 = DateTime.Now.ToString("ddMMyyyyHHmmss-") + Guid.NewGuid().ToString() + ".jpg";
                        var path1 = Path.Combine(Server.MapPath("~/Assets/ImageCategory"), filename1);

                        if (System.IO.File.Exists(path1))
                            ViewData["Err55"] = "(*) Hình ảnh đã tồn tại !";
                        else
                        {
                            imageCategory.SaveAs(path1);
                            info.imageCategory = filename1;
                            info.idCategory_Product = Int32.Parse(ma.ToString());
                            info.iconCategory = icon;
                            info.nameCategory = ten;
                            info.iconCategory = icon;
                            db.Category_Product.Add(info);
                        }
                        db.SaveChanges();
                    }
                };
            }
            return RedirectToAction("DanhMucSanPham", "Manager");
        }
        [HttpGet]
        public ActionResult CapNhatDanhMuc()
        {
            var id = Url.RequestContext.RouteData.Values["id"];
            if (id == null) return RedirectToAction("DanhMucSanPham");
            return LoadDanhMuc(Int32.Parse(id.ToString()));
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult CapNhatDanhMuc(FormCollection collection, HttpPostedFileBase imageCategory)
        {
            if (Session["idAccount"] == null) return RedirectToAction("DangNhap", "Login");
            if (!Equals(Session["idRole"], 5)) return HttpNotFound();
            var getidUser = Url.RequestContext.RouteData.Values["id"];
            var id = Int32.Parse(getidUser.ToString());
            LoadDanhMuc(id);

            var ma = collection["id"];
            var ten = collection["name"];
            var icon = collection["icon"];

            var csdt = from acc in db.Category_Product
                       where acc.idCategory_Product != id
                       select acc;
            foreach (var checksdt in csdt)
            {
                if (Int32.Parse(ma.ToString()) == checksdt.idCategory_Product)
                {
                    ViewData["Err4"] = "Mã này đã tồn tại trong hệ thống!";
                    return View();
                }
                else if (checksdt.nameCategory.Equals(ten))
                {
                    ViewData["Err4"] = "Tên này đã tồn tại trong hệ thống!";
                    return View();
                }
            }
            if (String.IsNullOrEmpty(ten))
                ViewData["Err4"] = "Vui lòng nhập Tên danh mục!";
            else if (String.IsNullOrEmpty(ma))
                ViewData["Err4"] = "Vui lòng nhập Mã danh mục!";
            else if (String.IsNullOrEmpty(icon))
                ViewData["Err4"] = "Vui lòng nhập Icon!";
            else
            {
                var query1 = from acc in db.Category_Product
                             where acc.idCategory_Product == id
                             select acc;
                foreach (var info in query1)
                {
                    info.idCategory_Product = Int32.Parse(ma.ToString());
                    info.nameCategory = ten;
                    info.iconCategory = icon;
                    if (ModelState.IsValid)
                    {
                        if (imageCategory != null)
                        {
                            string extension1 = System.IO.Path.GetExtension(imageCategory.FileName);
                            if (Equals(extension1, ".png") || Equals(extension1, ".jpg"))
                            {
                                var filename1 = DateTime.Now.ToString("ddMMyyyyHHmmss-") + Guid.NewGuid().ToString() + ".jpg";
                                var path1 = Path.Combine(Server.MapPath("~/Assets/ImageCategory"), filename1);

                                if (System.IO.File.Exists(path1))
                                    ViewData["Err4"] = "(*) Hình ảnh đã tồn tại !";
                                else
                                {
                                    if (info.imageCategory != null)
                                        System.IO.File.Delete(Path.Combine(Server.MapPath("~/Assets/ImageCategory"), info.imageCategory));
                                    imageCategory.SaveAs(path1);
                                    info.imageCategory = filename1;
                                }
                            }
                        }
                    }
                }
                db.SaveChanges();
                ViewData["Err4"] = "(*) CẬP NHẬT THÀNH CÔNG";
            }
            return View();
        }
        public ActionResult LoadDanhMuc(int id)
        {
            var _id = Int32.Parse(id.ToString());
            var query = from acc in db.Category_Product
                        where acc.idCategory_Product == _id
                        select acc;
            return View(query.ToList());
        }
        //Thong tin nhan vien//
        public ActionResult LoadCapnhat(int id)
        {
            var _id = Int32.Parse(id.ToString());
            IList<_FullAccountInfo> infoacc = new List<_FullAccountInfo>();
            var query = from acc in db.infoAccounts
                        join lg in db.Logins on acc.idAccount equals lg.idAccount
                        join tt in db.Roles on acc.idRole equals tt.idRole
                        where lg.idAccount == _id
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

        //Cap nhat nhan vien//
        [HttpGet]
        public ActionResult CapNhatNhanVien()
        {
            var id = Url.RequestContext.RouteData.Values["id"];
            if (id == null) return RedirectToAction("QuanLyNhanVien");
            LoadCapnhat(Int32.Parse(id.ToString()));
            DropDownListChucVu();
            return View();
        }
        [HttpPost]
        public ActionResult CapNhatNhanVien(FormCollection collection)
        {
            if (Session["idAccount"] == null) return RedirectToAction("DangNhap", "Login");
            if (!Equals(Session["idRole"], 5)) return HttpNotFound();
            var getidUser = Url.RequestContext.RouteData.Values["id"];
            var id = Int32.Parse(getidUser.ToString());

            DropDownListChucVu();
            LoadCapnhat(id);

            var hoten = collection["Username"];
            var ngaysinh = String.Format("{0:MM/dd/yyy}", collection["Birthday"]);
            var dc = collection["Address"];
            var mail = collection["Email"];
            var sdt = collection["PhoneNumber"];
            var cv = collection["idStaff"];

            var csdt = from acc in db.infoAccounts
                       join lg in db.Logins on acc.idAccount equals lg.idAccount
                       where lg.idAccount != id
                       select new { lg.Email, acc.PhoneNumber };

            foreach (var checksdt in csdt)
            {
                if (sdt == checksdt.PhoneNumber)
                {
                    ViewData["Err2"] = "Số điện thoại này đã tồn tại trong hệ thống!";
                    return View();
                }
                else if (checksdt.Email == mail)
                {
                    ViewData["Err2"] = "Email này đã tồn tại trong hệ thống!";
                    return View();
                }
            }

            if (String.IsNullOrEmpty(hoten))
                ViewData["Err2"] = "Vui lòng nhập Họ tên!";
            else if (String.IsNullOrEmpty(ngaysinh))
                ViewData["Err2"] = "Vui lòng nhập Ngày sinh!";
            else if (String.IsNullOrEmpty(dc))
                ViewData["Err2"] = "Vui lòng nhập Địa chỉ!";
            else if (String.IsNullOrEmpty(mail))
                ViewData["Err2"] = "Vui lòng nhập Email!";
            else if (String.IsNullOrEmpty(sdt))
                ViewData["Err2"] = "Vui lòng nhập SĐT!";
            else
            {
                var query1 = from acc in db.infoAccounts
                             join lg in db.Logins on acc.idAccount equals lg.idAccount
                             where lg.idAccount == id
                             select new { acc, lg };
                foreach (var info in query1)
                {
                    info.acc.Fullname = hoten;
                    info.acc.Birthday = DateTime.Parse(ngaysinh);
                    info.acc.Address = dc;
                    info.lg.Email = mail;
                    info.acc.PhoneNumber = sdt;
                    if (!String.IsNullOrEmpty(cv))
                        info.acc.idRole = Int32.Parse(cv.ToString());
                }
                db.SaveChanges();
                return RedirectToAction("QuanLyNhanVien", "Manager");
            }
            return View();
        }

        //Thong tin khach hang//
        public ActionResult LoadThongTin(int id)
        {
            var _id = Int32.Parse(id.ToString());
            IList<_FullAccountInfo> infoacc = new List<_FullAccountInfo>();
            var query = from acc in db.infoAccounts
                        join lg in db.Logins on acc.idAccount equals lg.idAccount
                        join tt in db.Roles on acc.idRole equals tt.idRole
                        where lg.idAccount == _id
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

        //Cap nhat khach hang//
        [HttpGet]
        public ActionResult CapNhatKhachHang()
        {
            var id = Url.RequestContext.RouteData.Values["id"];
            if (id == null) return RedirectToAction("QuanLyTaiKhoanKH");
            LoadThongTin(Int32.Parse(id.ToString()));
            return View();
        }
        [HttpPost]
        public ActionResult CapNhatKhachHang(FormCollection collection)
        {
            if (Session["idAccount"] == null) return RedirectToAction("DangNhap", "Login");
            if (!Equals(Session["idRole"], 5)) return HttpNotFound();
            var getidUser = Url.RequestContext.RouteData.Values["id"];
            var id = Int32.Parse(getidUser.ToString());

            LoadThongTin(id);

            var hoten = collection["Username"];
            var ngaysinh = String.Format("{0:MM/dd/yyy}", collection["Birthday"]);
            var dc = collection["Address"];
            var mail = collection["Email"];
            var sdt = collection["PhoneNumber"];

            var csdt = from acc in db.infoAccounts
                       join lg in db.Logins on acc.idAccount equals lg.idAccount
                       where lg.idAccount != id
                       select new { lg.Email, acc.PhoneNumber };

            foreach (var checksdt in csdt)
            {
                if (sdt == checksdt.PhoneNumber)
                {
                    ViewData["Err2"] = "Số điện thoại này đã tồn tại trong hệ thống!";
                    return View();
                }
                else if (checksdt.Email == mail)
                {
                    ViewData["Err2"] = "Email này đã tồn tại trong hệ thống!";
                    return View();
                }
            }

            if (String.IsNullOrEmpty(hoten))
                ViewData["Err2"] = "Vui lòng nhập Họ tên!";
            else if (String.IsNullOrEmpty(ngaysinh))
                ViewData["Err2"] = "Vui lòng nhập Ngày sinh!";
            else if (String.IsNullOrEmpty(dc))
                ViewData["Err2"] = "Vui lòng nhập Địa chỉ!";
            else if (String.IsNullOrEmpty(mail))
                ViewData["Err2"] = "Vui lòng nhập Email!";
            else if (String.IsNullOrEmpty(sdt))
                ViewData["Err2"] = "Vui lòng nhập SĐT!";
            else
            {
                var query1 = from acc in db.infoAccounts
                             join lg in db.Logins on acc.idAccount equals lg.idAccount
                             where lg.idAccount == id
                             select new { acc, lg };
                foreach (var info in query1)
                {
                    info.acc.Fullname = hoten;
                    info.acc.Birthday = DateTime.Parse(ngaysinh);
                    info.acc.Address = dc;
                    info.lg.Email = mail;
                    info.acc.PhoneNumber = sdt;
                }
                db.SaveChanges();
                return RedirectToAction("QuanLyTaiKhoanKH", "Manager");
            }
            return View();
        }
        //Vo hieu hoa tai khoan//
        public ActionResult VoHieuHoa()
        {
            var getid = Url.RequestContext.RouteData.Values["id"];
            int g = Int32.Parse(getid.ToString());
            var a = from b in db.Logins
                    join h in db.infoAccounts on b.idAccount equals h.idAccount
                    where b.idAccount == g && b.hideAcc == false
                    select new { b, h };
            foreach (var d in a)
            {
                d.b.hideAcc = true;

                string content = System.IO.File.ReadAllText(Server.MapPath("~/Assets/SendMail/VoHieuHoa.html"));
                content = content.Replace("{{fullname}}", d.h.Fullname);
                new MailHelper().SendMail(d.b.Email, "[No Reply] Tình Trạng Tài Khoản", content);
            }
            db.SaveChanges();
            return Redirect(Request.UrlReferrer.ToString());
        }

        //Danh sach tai khoan bi vo hieu hoa//
        public ActionResult DanhSachTaiKhoanVoHieuHoa()
        {
            if (Session["idAccount"] == null) return RedirectToAction("DangNhap", "Login");
            if (!Equals(Session["idRole"], 5)) return HttpNotFound();

            IList<_FullAccountInfo> infoAcc = new List<_FullAccountInfo>();
            var query = from acc in db.infoAccounts
                        join lg in db.Logins on acc.idAccount equals lg.idAccount
                        join tt in db.Roles on acc.idRole equals tt.idRole
                        where lg.hideAcc == true
                        select new { lg, acc, tt };

            var infoAccs = query.ToList();
            foreach (var info in infoAccs)
            {
                infoAcc.Add(new _FullAccountInfo()
                {
                    idAccount = info.lg.idAccount,
                    Fullname = info.acc.Fullname,
                    Email = info.lg.Email,
                    Birthday = info.acc.Birthday,
                    Address = info.acc.Address,
                    PhoneNumber = info.acc.PhoneNumber,
                    ChucVu = info.tt.nameRole
                });
            }
            return View(infoAcc);
        }

        //Kich hoat tai khoan//
        public ActionResult KichHoat()
        {
            var getid = Url.RequestContext.RouteData.Values["id"];
            int g = Int32.Parse(getid.ToString());
            var a = from b in db.Logins
                    join h in db.infoAccounts on b.idAccount equals h.idAccount
                    where b.idAccount == g && b.hideAcc == true
                    select new { b, h };
            foreach (var d in a)
            {
                d.b.hideAcc = false;
                string content = System.IO.File.ReadAllText(Server.MapPath("~/Assets/SendMail/KichHoat.html"));
                content = content.Replace("{{fullname}}", d.h.Fullname);
                new MailHelper().SendMail(d.b.Email, "[No Reply] Tình Trạng Tài Khoản", content);
            }
            db.SaveChanges();
            return RedirectToAction("DanhSachTaiKhoanVoHieuHoa");
        }

        //An san pham tren trang chu//
        public ActionResult AnSanPham(int getid)
        {
            int g = Int32.Parse(getid.ToString());
            var a = from b in db.Products
                    join h in db.Logins on b.idAccount equals h.idAccount
                    where b.idProduct == g && b.confirmProduct == true
                    select new { b, h };
            foreach (var d in a)
            {
                d.b.confirmProduct = false;
                d.b.hideProduct = true;
                string content = System.IO.File.ReadAllText(Server.MapPath("~/Assets/SendMail/AnSanPham.html"));
                content = content.Replace("{{idproduct}}", d.b.idProduct.ToString());
                new MailHelper().SendMail(d.h.Email, "[No Reply] Tình Trạng Sản Phẩm", content);
            }
            db.SaveChanges();
            return Redirect(Request.UrlReferrer.ToString());
        }

        //Danh Sach san pham bi an di//
        public ActionResult DanhSachAn()
        {
            if (Session["idAccount"] == null) return RedirectToAction("DangNhap", "Login");
            if (!Equals(Session["idRole"], 5)) return HttpNotFound();

            IList<_FullProduct> infoAcc = new List<_FullProduct>();
            var query = from cus in db.infoAccounts
                        join tt in db.Products on cus.idAccount equals tt.idAccount
                        join hihi in db.Category_Product on tt.idCategory_Product equals hihi.idCategory_Product
                        where tt.hideProduct == true && cus.idAccount == tt.idAccount && tt.confirmProduct == false
                        orderby tt.datePost descending
                        select new { cus, tt, hihi };

            var infoAccs = query.ToList();
            foreach (var info in infoAccs)
            {
                infoAcc.Add(new _FullProduct()
                {
                    idProduct = info.tt.idProduct,
                    nameProduct = info.tt.nameProduct,
                    priceProduct = info.tt.priceProduct,
                    amountProduct = info.tt.amountProduct,
                    descriptionProduct = info.tt.descriptionProduct,
                    datePost = info.tt.datePost,
                    nameCategory = info.hihi.nameCategory,
                    Fullname = info.cus.Fullname,
                    imageProduct_1 = info.tt.imageProduct_1,
                    imageProduct_2 = info.tt.imageProduct_2,
                    imageProduct_3 = info.tt.imageProduct_3,
                    imageProduct_4 = info.tt.imageProduct_4
                });
            }
            return View(infoAcc);
        }

        //Hien thi lai san pham//
        public ActionResult HienThiSanPham()
        {
            var getid = Url.RequestContext.RouteData.Values["id"];
            int g = Int32.Parse(getid.ToString());
            var a = from b in db.Products
                    join h in db.Logins on b.idAccount equals h.idAccount
                    where b.idProduct == g && b.confirmProduct == false && b.hideProduct == true
                    select new { b, h };
            foreach (var d in a)
            {
                d.b.confirmProduct = true;
                d.b.hideProduct = false;
                string content = System.IO.File.ReadAllText(Server.MapPath("~/Assets/SendMail/HienThiSanPham.html"));
                content = content.Replace("{{idproduct}}", d.b.idProduct.ToString());
                new MailHelper().SendMail(d.h.Email, "[No Reply] Tình Trạng Sản Phẩm", content);
            }
            db.SaveChanges();
            return RedirectToAction("DanhSachAn");
        }
    }
}