using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TMDT.Models;

namespace TMDT.Controllers
{
    public class ShipperController : Controller
    {
        ChoDoCuEntities db = new ChoDoCuEntities();
        // GET: Shipper
        [HttpGet]
        public ActionResult DonVanChuyen()
        {
            if (Session["idAccount"] == null) return RedirectToAction("DangNhap", "Login");
            if (!Equals(Session["idRole"], 3)) return HttpNotFound();
            int ss = Int32.Parse(Session["idAccount"].ToString());
            IList<_GioHang> infoAcc = new List<_GioHang>();
            var query = from acc in db.infoAccounts
                        join order in db.Orders on acc.idAccount equals order.idAccount
                        join list in db.listOrders on order.idOrder equals list.idOrder
                        join pro in db.Products on list.idProduct equals pro.idProduct
                        join ship in db.listShips on order.idOrder equals ship.idOrder
                        join tt in db.Category_Status on order.Category_Status equals tt.idCate_status
                        where order.Category_Status == 2  && order.hideOrder == false && ship.confirmShip ==true
                        orderby order.dateOrder descending
                        select new { order, pro, tt, acc };

            var infoAccs = query.ToList();
            foreach (var info in infoAccs)
            {
                infoAcc.Add(new _GioHang()
                {
                    Id = info.order.idOrder,
                    sanpham = info.pro.nameProduct,
                    hinh = info.pro.imageProduct_1,
                    dongia = double.Parse(info.pro.priceProduct.ToString()),
                    soluong = 1,
                    Ngaydat = info.order.dateOrder,                   
                });
            }
            return View(infoAcc);
        }
        public ActionResult NhanShip()
        {
            var getid = Url.RequestContext.RouteData.Values["id"];
            int ss = Int32.Parse(getid.ToString());
            int g = Int32.Parse(getid.ToString());
            var a = from b in db.Orders
                    join h in db.Logins on b.idAccount equals h.idAccount
                    join tt in db.Category_Status on b.Category_Status equals tt.idCate_status
                    join vc in db.listShips on b.idOrder equals vc.idOrder
                    where b.idOrder == g && b.Category_Status == 2 && b.hideOrder == false
                    select vc;
            foreach (var d in a)
            {               
                d.confirmShip = false;
            }
            db.SaveChanges();
            return RedirectToAction("DonVanChuyen");
        }
        [HttpGet]
        public ActionResult DonVanChuyenDaNhan()
        {
            if (Session["idAccount"] == null) return RedirectToAction("DangNhap", "Login");
            if (!Equals(Session["idRole"], 3)) return HttpNotFound();
            int ss = Int32.Parse(Session["idAccount"].ToString());
            IList<_GioHang> infoAcc = new List<_GioHang>();
            var query = from acc in db.infoAccounts
                        join order in db.Orders on acc.idAccount equals order.idAccount
                        join list in db.listOrders on order.idOrder equals list.idOrder
                        join pro in db.Products on list.idProduct equals pro.idProduct
                        join tt in db.Category_Status on order.Category_Status equals tt.idCate_status
                        join ship in db.listShips on order.idOrder equals ship.idOrder
                        where order.Category_Status == 2 && acc.idAccount == ss && order.hideOrder == false && ship.confirmShip==false
                        orderby order.dateOrder descending
                        select new { order, pro, tt, acc };

            var infoAccs = query.ToList();
            foreach (var info in infoAccs)
            {
                infoAcc.Add(new _GioHang()
                {
                    Id = info.order.idOrder,
                    sanpham = info.pro.nameProduct,
                    hinh = info.pro.imageProduct_1,
                    dongia = double.Parse(info.pro.priceProduct.ToString()),
                    soluong = 1,
                    Ngaydat = info.order.dateOrder,              
                });
            }
            return View(infoAcc);
        }
        public ActionResult ThatBai()
        {
            var getid = Url.RequestContext.RouteData.Values["id"];
            int g = Int32.Parse(getid.ToString());
            var a = from b in db.Orders
                    join h in db.Logins on b.idAccount equals h.idAccount
                    join tt in db.Category_Status on b.Category_Status equals tt.idCate_status
                    join vc in db.listShips on b.idOrder equals vc.idOrder
                    where b.idOrder == g && b.Category_Status == 2 && b.hideOrder == false
                    select new { b,h,tt,vc};
            foreach (var d in a)
            {
                d.vc.Category_Status = 4;
                d.b.Category_Status = 4;        
                d.vc.hideShip = true;
                string content = System.IO.File.ReadAllText(Server.MapPath("~/Assets/SendMail/ShipThatBai.html"));
                content = content.Replace("{{idorder}}", d.b.idOrder.ToString());
                new MailHelper().SendMail(d.h.Email, "[No Reply] Tình Trạng Đơn Hàng", content);
            }
            db.SaveChanges();
            return RedirectToAction("DonVanChuyenDaNhan");
        }
        public ActionResult ThanhCong()
        {
            var getid = Url.RequestContext.RouteData.Values["id"];           
            int g = Int32.Parse(getid.ToString());
            var a = from b in db.Orders
                    join h in db.Logins on b.idAccount equals h.idAccount
                    join tt in db.Category_Status on b.Category_Status equals tt.idCate_status
                    join vc in db.listShips on b.idOrder equals vc.idOrder
                    join c in db.listOrders on b.idOrder equals c.idOrder
                    join z in db.Products on c.idProduct equals z.idProduct
                    where b.idOrder == g && b.Category_Status == 2 && b.hideOrder == false
                    select new { b, h, tt, vc ,z,c};
            foreach (var d in a)
            {
                listSell list = new listSell();
                list.idProduct = d.z.idProduct;
                list.dateSell = DateTime.Now;
                list.idAccount_Buy = d.b.idAccount;
                list.amountProduct = d.c.amountProduct;
                db.listSells.Add(list);
                d.vc.Category_Status = 3;
                d.b.Category_Status = 3;
                d.vc.confirmShip =true;
                string content = System.IO.File.ReadAllText(Server.MapPath("~/Assets/SendMail/ShipThanhCong.html"));
                content = content.Replace("{{idorder}}", d.b.idOrder.ToString());
                new MailHelper().SendMail(d.h.Email, "[No Reply] Tình Trạng Đơn Hàng", content);
            }
            db.SaveChanges();
            return RedirectToAction("DonVanChuyenDaNhan");
        }
    }
}