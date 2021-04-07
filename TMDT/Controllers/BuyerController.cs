using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TMDT.Models;

namespace TMDT.Controllers
{
    public class BuyerController : Controller
    {
        ChoDoCuEntities db = new ChoDoCuEntities();

        [HttpGet]
        public ActionResult DanhSachMuaHang()
        {
            if (Session["idAccount"] == null) return RedirectToAction("DangNhap", "Login");
            if (!Equals(Session["idRole"], 2)) return HttpNotFound();

            int ss = Int32.Parse(Session["idAccount"].ToString());
            IList<_GioHang> infoAcc = new List<_GioHang>();
            var query = from order in db.Orders
                        join list in db.listOrders on order.idOrder equals list.idOrder
                        join pro in db.Products on list.idProduct equals pro.idProduct
                        join tt in db.Category_Status on order.Category_Status equals tt.idCate_status
                        where order.idAccount == ss && order.hideOrder == false
                        orderby order.dateOrder descending
                        select new { order, pro, tt, list };

            var infoAccs = query.ToList();
            foreach (var info in infoAccs)
            {
                infoAcc.Add(new _GioHang()
                {
                    sanpham = info.pro.nameProduct,
                    hinh = info.pro.imageProduct_1,
                    dongia = double.Parse(info.pro.priceProduct.ToString()),
                    soluong = info.list.amountProduct,
                    Ngaydat = info.order.dateOrder,
                    status = info.tt.nameStatus
                });
            }
            return View(infoAcc);
        }
        public ActionResult ThanhToan(string strUrl)
        {
            if (Session["idAccount"] == null) return RedirectToAction("DangNhap", "Login");
            if (!Equals(Session["idRole"], 2)) return HttpNotFound();

            List<_GioHang> lstGiohang = Laygiohang();
            if (lstGiohang == null) return RedirectToAction("TrangChu", "Home");

            foreach (var i in lstGiohang)
            {
                var order = new Order();
                order.idAccount = Int32.Parse(Session["idAccount"].ToString());
                order.dateOrder = DateTime.Parse(DateTime.Now.ToString());
                order.Category_Status = 1;

                var listOrder = new listOrder();
                listOrder.idOrder = order.idOrder;
                listOrder.idProduct = i.Id;
                listOrder.amountProduct = i.soluong;

                db.Orders.Add(order);
                db.listOrders.Add(listOrder);

                var a = from b in db.Products
                        where b.idProduct == i.Id
                        select b;
                foreach (var c in a)
                {
                    string thongbao = "<script language='javascript' type='text/javascript'>alert('LỖI: Sản phẩm: [ " + i.sanpham + " ] trong kho chỉ còn " + c.amountProduct + " sản phẩm'); window.location.href='/gio-hang';</script>";
                    if (c.amountProduct < i.soluong)
                        return Content(thongbao);
                    c.amountProduct -= i.soluong;
                }
                db.SaveChanges();
            }
            XoaTatcaGioHang();
            return Content("<script language='javascript' type='text/javascript'>alert('Thanh toán thành công'); window.location.href='/trang-chu';</script></script>");
        }
        public List<_GioHang> Laygiohang()
        {
            List<_GioHang> lstGiohang = Session["Giohang"] as List<_GioHang>;
            if (lstGiohang == null)
            {
                lstGiohang = new List<_GioHang>();
                Session["Giohang"] = lstGiohang;
            }
            return lstGiohang;
        }
        public ActionResult ThemGiohang(int Id, string strURL, FormCollection collect)
        {
            var sl = collect["SoLuong"];
            if (String.IsNullOrEmpty(sl))
                return Content("<script language='javascript' type='text/javascript'>alert('Số lượng sản phẩm không để trống!'); window.location.href='" + strURL + "';</script>");
            List<_GioHang> lstGioHang = Laygiohang();
            _GioHang sanpham = lstGioHang.Find(n => n.Id == Id);
            if (sanpham == null)
            {
                sanpham = new _GioHang(Id);
                lstGioHang.Add(sanpham);
                if (!String.IsNullOrEmpty(sl.ToString()))
                    sanpham.soluong += Int32.Parse(sl) - 1;
            }
            else
            {
                sanpham.soluong++;
            }
            return Content("<script language='javascript' type='text/javascript'>alert('Thêm sản phẩm thành công!'); window.location.href='" + strURL + "';</script>");
        }


        public int TongSoLuong()
        {
            int TongSoLuong = 0;
            List<_GioHang> lstGiohang = Session["GioHang"] as List<_GioHang>;
            if (lstGiohang != null)
            {
                TongSoLuong = lstGiohang.Sum(n => n.soluong);
            }
            return TongSoLuong;
        }

        public double TongTien()
        {
            double Tongtien = 0;
            List<_GioHang> lstGiohang = Session["GioHang"] as List<_GioHang>;
            if (lstGiohang != null)
            {
                Tongtien = lstGiohang.Sum(n => n.thanhtien);

            }
            return Tongtien;
        }
        public ActionResult GioHang()
        {
            List<_GioHang> lstGiohang = Laygiohang();
            if (lstGiohang.Count == 0)
            {
                return RedirectToAction("TrangChu", "Home");
            }
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            return View(lstGiohang);
        }
        public ActionResult XoaGiohang(int iMaSP)
        {
            List<_GioHang> lstGiohang = Laygiohang();
            _GioHang sanpham = lstGiohang.SingleOrDefault(n => n.Id == iMaSP);
            if (sanpham != null)
            {
                lstGiohang.RemoveAll(n => n.Id == iMaSP);
                return RedirectToAction("GioHang");
            }
            if (lstGiohang.Count == 0)
            {
                return RedirectToAction("TrangChu", "Home");
            }
            return RedirectToAction("GioHang");

        }
        public ActionResult XoaTatcaGioHang()
        {
            List<_GioHang> lstGiohang = Laygiohang();
            lstGiohang.Clear();
            return RedirectToAction("TrangChu", "Home");
        }
        public ActionResult Capnhatgiohang(FormCollection collect)
        {
            List<_GioHang> lstGiohang = Laygiohang();
            var iMaSP = Int32.Parse(collect["MaSP"].ToString());
            _GioHang sanpham = lstGiohang.SingleOrDefault(n => n.Id == iMaSP);
            if (sanpham != null)
            {
                sanpham.soluong = Int32.Parse(collect["txtsoluong"].ToString());
            }
            return RedirectToAction("GioHang");

        }
        public ActionResult GioHangPartial()
        {
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            return PartialView();
        }
    }
}