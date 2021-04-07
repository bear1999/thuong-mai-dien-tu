using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace TMDT
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            /* Phuc */
            /* ShipperController */
            routes.MapRoute("DonVanChuyen", "shipper/don-van-chuyen", new { controller = "Shipper", action = "DonVanChuyen" });
            routes.MapRoute("DonVanChuyenDaNhan", "shipper/don-van-chuyen-da-nhan", new { controller = "Shipper", action = "DonVanChuyenDaNhan" });
            routes.MapRoute("NhanShip", "shipper/nhan-ship/{id}", new { controller = "Shipper", action = "NhanShip", id = UrlParameter.Optional });
            routes.MapRoute("ThanhCong", "shipper/thanh-cong/{id}", new { controller = "Shipper", action = "ThanhCong", id = UrlParameter.Optional });
            routes.MapRoute("ThatBai", "shipper/that-bai/{id}", new { controller = "Shipper", action = "ThatBai", id = UrlParameter.Optional });
            /* end ShipperController*/
            
            /* SellerController */
            routes.MapRoute("DanhSachDatMua", "seller/danh-sach-dat-mua", new { controller = "Seller", action = "DanhSachDatMua" });
            routes.MapRoute("XacNhanDatMua", "seller/xac-nhan-dat-mua/{id}", new { controller = "Seller", action = "XacNhanDatMua", id = UrlParameter.Optional });
            routes.MapRoute("TuChoiDatMua", "seller/tu-choi-dat-mua/{id}", new { controller = "Seller", action = "TuChoiDatMua", id = UrlParameter.Optional });
            /* end SellerController */
            
            /* ManagerController */
            routes.MapRoute("CapNhatNhanVien", "admin/cap-nhat-nhan-vien/{id}", new { controller = "Manager", action = "CapNhatNhanVien", id = UrlParameter.Optional });
            routes.MapRoute("CapNhatKhachHang", "admin/cap-nhat-khach-hang/{id}", new { controller = "Manager", action = "CapNhatKhachHang", id = UrlParameter.Optional });
            routes.MapRoute("SanPhamDaDuyet", "admin/san-pham-da-duyet", new { controller = "Manager", action = "SanPhamDaDuyet" });
            routes.MapRoute("DanhMucSanPham", "admin/danh-muc-san-pham", new { controller = "Manager", action = "DanhMucSanPham" });
            routes.MapRoute("CapNhatDanhMuc", "admin/cap-nhat-danh-muc/{id}", new { controller = "Manager", action = "CapNhatDanhMuc", id = UrlParameter.Optional });
            routes.MapRoute("ThemDanhMuc", "admin/them-danh-muc", new { controller = "Manager", action = "ThemDanhMuc" });
            routes.MapRoute("DanhSachTaiKhoanVoHieuHoa", "admin/danh-sach-tai-khoan-vo-hieu-hoa", new { controller = "Manager", action = "DanhSachTaiKhoanVoHieuHoa" });
            routes.MapRoute("DanhSachAn", "admin/danh-sach-an", new { controller = "Manager", action = "DanhSachAn" });
            routes.MapRoute("VoHieuHoa", "admin/vo-hieu-hoa/{id}", new { controller = "Manager", action = "VoHieuHoa", id = UrlParameter.Optional });
            routes.MapRoute("TuChoiSanPham", "admin/tu-choi-san-pham/{id}", new { controller = "Manager", action = "TuChoiSanPham", id = UrlParameter.Optional });
            routes.MapRoute("KichHoat", "admin/kich-hoat/{id}", new { controller = "Manager", action = "KichHoat", id = UrlParameter.Optional });
            /* end ManagerController*/
            /*--------------*/

            /* RegisterAccount */
            routes.MapRoute("ThemNhanVien", "admin/them-nhan-vien", new { controller = "RegisterAccount", action = "ThemNhanVien" });
            routes.MapRoute("DangKyKhachHang", "dang-ky", new { controller = "RegisterAccount", action = "DangKyKhachHang" });
            /* end RegisterAccount*/

            /* ManagerController */
            routes.MapRoute("QuanLyTaiKhoanKH", "nhan-vien/quan-ly-tai-khoan-khach-hang", new { controller = "Manager", action = "QuanLyTaiKhoanKH" });
            routes.MapRoute("QuanLyNhanVien", "admin/quan-ly-nhan-vien", new { controller = "Manager", action = "QuanLyNhanVien" });
            routes.MapRoute("SanPhamDangDoiDuyet", "nhan-vien/sp-dang-doi-duyet", new { controller = "Manager", action = "SanPhamDangDoiDuyet" });
            routes.MapRoute("DuyetSanPham", "nhan-vien/duyet-san-pham/{id}", new { controller = "Manager", action = "SanPhamDangDoiDuyet", id = UrlParameter.Optional });
            /* end ManagerController*/

            /* SellerController */
            routes.MapRoute("DangBanSanPham", "seller/dang-ban-san-pham", new { controller = "Seller", action = "DangBanSanPham" });
            routes.MapRoute("DanhSachSanPham", "seller/danh-sach-san-pham", new { controller = "Seller", action = "DanhSachSanPham" });
            routes.MapRoute("TrangCaNhan", "trang-ca-nhan", new { controller = "Seller", action = "TrangCaNhan" });
            routes.MapRoute("CapNhatSanPham", "seller/cap-nhat-san-pham/{id}", new { controller = "Seller", action = "CapNhatSanPham", id = UrlParameter.Optional });
            /* end SellerController */

            /* LoginController */
            routes.MapRoute("DoiMatKhau", "doi-mat-khau", new { controller = "Login", action = "DoiMatKhau" });
            routes.MapRoute("QuenMatKhau", "quen-mat-khau", new { controller = "Login", action = "QuenMatKhau" });
            routes.MapRoute("DangNhap", "dang-nhap", new { controller = "Login", action = "DangNhap" });
            /* end LoginController*/

            /* HomeController */
            routes.MapRoute("ThongTinCaNhan", "thong-tin-ca-nhan", new { controller = "Home", action = "ThongTinCaNhan" });
            routes.MapRoute("TimKiem", "tim-kiem", new { controller = "Home", action = "TimKiem" });
            routes.MapRoute("TrangChu", "trang-chu", new { controller = "Home", action = "TrangChu" });
            routes.MapRoute("ChiTietSP", "san-pham/{alias}/{id}", new { controller = "Home", action = "ChiTietSP", id = UrlParameter.Optional });
            /* end HomeController */

            /* BuyerController */
            routes.MapRoute("Capnhatgiohang", "gio-hang/cap-nhat", new { controller = "Buyer", action = "Capnhatgiohang" });
            routes.MapRoute("XoaTatcaGioHang", "gio-hang/xoa-tat-ca", new { controller = "Buyer", action = "XoaTatcaGioHang" });
            routes.MapRoute("XoaGiohang", "gio-hang/xoa-gio-hang", new { controller = "Buyer", action = "XoaGiohang" });
            routes.MapRoute("GioHang", "gio-hang", new { controller = "Buyer", action = "GioHang" });
            routes.MapRoute("ThanhToan", "gio-hang/thanh-toan", new { controller = "Buyer", action = "ThanhToan" });
            routes.MapRoute("DanhSachMuaHang", "buyer/danh-sach-mua-hang", new { controller = "Buyer", action = "DanhSachMuaHang" });
            routes.MapRoute("ThemGioHang", "gio-hang/them-gio-hang", new { controller = "Buyer", action = "ThemGioHang" });
            /* end BuyerController */

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "TrangChu", id = UrlParameter.Optional }
            );
        }
    }
}
