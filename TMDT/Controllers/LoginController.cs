using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TMDT.Models;
using TMDT.Function;
using Common;

namespace TMDT.Controllers
{
    public class LoginController : Controller
    {
        ChoDoCuEntities db = new ChoDoCuEntities();
        // GET: Login
        public ActionResult DangXuat()
        {
            Session.RemoveAll();
            return RedirectToAction("DangNhap");
        }
        [HttpGet]
        public ActionResult DangNhap()
        {
            if (Session["idAccount"] != null) return RedirectToAction("ThongTinCaNhan", "Home");
            return View();
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DangNhap(FormCollection collect)
        {
            var Email = collect["Email"];
            var MatKhau = collect["Password"];

            if (String.IsNullOrEmpty(Email))
                ViewData["Err"] = "(*) Vui lòng nhập Địa chỉ email";
            else if (String.IsNullOrEmpty(MatKhau))
                ViewData["Err"] = "(*) Vui lòng nhập Mật khẩu";
            else
            {
                MatKhau = _Function.md5(MatKhau);
                var verify = from lg in db.Logins
                             join acc in db.infoAccounts on lg.idAccount equals acc.idAccount                          
                             where lg.Email.Equals(Email) && lg.Password.Equals(MatKhau) && lg.hideAcc == false
                             select new { lg,acc};
                foreach (var ok in verify)
                {
                    Session["idAccount"] = ok.lg.idAccount;
                    Session["idRole"] = ok.acc.idRole;
                    return RedirectToAction("ThongTinCaNhan", "Home");
                }

                if (Session["idAccount"] == null)
                    ViewData["Err"] = "(*) Sai thông tin đăng nhập";
            }
            return View();
        }
        [HttpGet]
        public ActionResult DoiMatKhau()
        {
            if (Session["GetOTP"] == null) return RedirectToAction("DangNhap", "Login");
            ViewData["Err"] = "(*) Mã OTP đã được gửi đến Email của bạn !";
            ViewData["Err1"] = "Lưu ý: Mã OTP chỉ có thời hạn 60 giây.";
            return View();
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DoiMatKhau(FormCollection collect)
        {
            if (Session["GetOTP"] == null) return RedirectToAction("QuenMatKhau", "Login");

            var otp = collect["GetOTP"];
            var mk1 = collect["MatKhau1"];
            var mk2 = collect["MatKhau2"];

            if (DateTime.Now > DateTime.Parse(Session["timeOut"].ToString()))
            {
                ViewData["Err"] = "(*) Đã hết thời gian timeout 60 giây xin vui lòng gửi lại OTP !";
                Session.RemoveAll();
                return View();
            }
            else if (String.IsNullOrEmpty(otp))
                ViewData["Err"] = "(*) Vui lòng nhập Mã OTP !";
            else if (String.IsNullOrEmpty(mk1))
                ViewData["Err"] = "(*) Vui lòng nhập Mật khẩu !";
            else if (String.IsNullOrEmpty(mk2))
                ViewData["Err"] = "(*) Vui lòng xác nhận lại Mật khẩu !";
            else if (mk1 != mk2)
                ViewData["Err"] = "(*) Mật khẩu nhập lại không khớp !";
            else if (!Equals(Session["GetOTP"], otp))
                ViewData["Err"] = "(*) Mã OTP không đúng !";
            else
            {
                var a = from b in db.Logins
                        where Equals(b.Email, Session["GetEmail"])
                        select b;
                foreach (var c in a)
                {
                    c.Password = _Function.md5(mk1);
                }
                db.SaveChanges();
                ViewData["Err"] = "(*) Đổi mật khẩu thành công !";
                Session.RemoveAll();
            }
            return View();
        }
        [HttpGet]
        public ActionResult QuenMatKhau()
        {
            return View();
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult QuenMatKhau(FormCollection collect)
        {
            var email = collect["Email"];
            if (String.IsNullOrEmpty(email))
            {
                ViewData["Err"] = "(*) Vui lòng nhập Địa chỉ Email";
                return View();
            }
            else
            {
                var a = from b in db.Logins
                        where Equals(b.Email, email)
                        select b;
                foreach (var c in a)
                {
                    Session["timeOut"] = DateTime.Now.AddSeconds(60);
                    Session["GetOTP"] = _Function.RandomOTP();
                    Session["GetEmail"] = email;

                    string content = System.IO.File.ReadAllText(Server.MapPath("~/Assets/SendMail/QuenMatKhau.html"));
                    content = content.Replace("{{OTP_CODE}}", Session["GetOTP"].ToString());
                    new MailHelper().SendMail(email, "[No Reply] Xác thực OTP", content);

                    return RedirectToAction("DoiMatKhau", "Login");
                }
                ViewData["Err"] = "(*) Địa chỉ Email không tồn tại trong hệ thống !";
                return View();
            }
        }
    }
}