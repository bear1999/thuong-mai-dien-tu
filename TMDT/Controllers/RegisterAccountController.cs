using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TMDT.Models;
using TMDT.Function;

namespace TMDT.Controllers
{
    public class RegisterAccountController : Controller
    {
        ChoDoCuEntities db = new ChoDoCuEntities();
        [HttpGet]
        public ActionResult ThemNhanVien()
        {
            if (Session["idAccount"] == null) return RedirectToAction("DangNhap", "Login");
            if (!Equals(Session["idRole"], 5)) return HttpNotFound();
            DropDownListChucVu();
            return View();
        }
        [HttpPost]
        public ActionResult ThemNhanVien(FormCollection collection)
        {
            if (Session["idAccount"] == null) return RedirectToAction("DangNhap", "Login");
            if (!Equals(Session["idRole"], 5)) return HttpNotFound();
            DropDownListChucVu();
            var hoten = collection["Fullname"];
            var ngaysinh = String.Format("{0:dd/MM/yyy}", collection["Birthday"]);
            var mail = collection["Email"];
            var sdt = collection["Phonenumber"];
            var mk = collection["Password"];
            var mk1 = collection["Password1"];
            var chucvu = collection["idRole"];
            var dc = collection["Address"];

            var csdt = from lg in db.Logins
                       join acc in db.infoAccounts on lg.idAccount equals acc.idAccount
                       where acc.PhoneNumber == sdt || lg.Email == mail
                       select new { lg, acc };
            foreach (var checksdt in csdt)
            {
                if (checksdt.acc.PhoneNumber == sdt)
                {
                    ViewData["Err1"] = "Số điện thoại này đã tồn tại trong hệ thống!";
                    return View();
                }
                else if (checksdt.lg.Email == mail)
                {
                    ViewData["Err1"] = "Email này đã tồn tại trong hệ thống!";
                    return View();
                }
                else return View();
            }
            if (String.IsNullOrEmpty(hoten))
                ViewData["Err1"] = "Vui lòng nhập Họ tên!";
            else if (String.IsNullOrEmpty(chucvu))
                ViewData["Err1"] = "VUi lòng chọn Chức vụ!!";
            else if (String.IsNullOrEmpty(ngaysinh))
                ViewData["Err1"] = "Vui lòng nhập Ngày sinh!";
            else if (String.IsNullOrEmpty(mail))
                ViewData["Err1"] = "Vui lòng nhập Địa chỉ Email!";
            else if (String.IsNullOrEmpty(sdt))
                ViewData["Err1"] = "Vui lòng nhập Số điện thoại!";
            else if (String.IsNullOrEmpty(dc))
                ViewData["Err1"] = "Vui lòng nhập Địa chỉ!";
            else if (String.IsNullOrEmpty(mk))
                ViewData["Err1"] = "Vui lòng nhập Mật khẩu!";
            else if (mk != mk1)
                ViewData["Err1"] = "Mật khẩu nhập lại không đúng!";
            else
            {
                infoAccount info = new infoAccount();
                Login dn = new Login();

                dn.Email = mail;
                info.PhoneNumber = sdt;
                dn.Password = _Function.md5(mk);
                dn.hideAcc = false;
                db.Logins.Add(dn);
                db.SaveChanges();
                var add = from lg in db.Logins
                          where lg.Email == mail
                          select lg.idAccount;
                foreach (var c in add)
                {
                    info.idAccount = c;
                    info.Fullname = hoten;
                    info.Birthday = DateTime.Parse(ngaysinh);
                    info.idRole = Int32.Parse(chucvu);
                    info.Address = dc;
                    db.infoAccounts.Add(info);
                }
                db.SaveChanges();
                ViewData["Reg1"] = "Đăng ký thành công!";
                return View();
            }
            return View();
        }
        // GET: RegisterAccount
        [HttpGet]
        public ActionResult DangKyKhachHang()
        {
            return View();
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DangKyKhachHang(FormCollection collect, Login login, infoAccount infoAcc)
        {
            var Email = collect["Email"];
            var Hoten = collect["Hoten"];
            var Birthday = String.Format("{0:MM/dd/yyy}", collect["Birthday"]);
            var PhoneNumber = collect["PhoneNumber"];
            var Address = collect["Address"];
            var idRole = collect["idRole"];
            var MatKhau = collect["Password"];

            if (String.IsNullOrEmpty(Email))
                ViewData["Err"] = "(*) Vui lòng nhập Địa chỉ email";
            else if (String.IsNullOrEmpty(Hoten))
                ViewData["Err"] = "(*) Vui lòng nhập Họ tên";
            else if (String.IsNullOrEmpty(Birthday))
                ViewData["Err"] = "(*) Vui lòng nhập Ngày sinh";
            else if (String.IsNullOrEmpty(PhoneNumber))
                ViewData["Err"] = "(*) Vui lòng nhập Số điện thoại";
            else if (String.IsNullOrEmpty(Address))
                ViewData["Err"] = "(*) Vui lòng nhập Địa chỉ";
            else if (String.IsNullOrEmpty(MatKhau))
                ViewData["Err"] = "(*) Vui lòng nhập Mật khẩu";
            else if (String.IsNullOrEmpty(idRole))
                ViewData["Err"] = "(*) Vui lòng chọn Người bán hoặc Người mua";
            else
            {
                login.Email = Email;
                login.Password = _Function.md5(MatKhau);
                db.Logins.Add(login);

                infoAcc.Fullname = Hoten;
                infoAcc.Birthday = DateTime.Parse(Birthday);
                infoAcc.PhoneNumber = PhoneNumber;
                infoAcc.Address = Address;
                infoAcc.idRole = int.Parse(idRole);
                infoAcc.idAccount = login.idAccount;
                db.infoAccounts.Add(infoAcc);

                db.SaveChanges();

                ViewData["Err"] = "(*) ĐĂNG KÝ THÀNH CÔNG";
                return View();
            }
            return View();
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
    }
}