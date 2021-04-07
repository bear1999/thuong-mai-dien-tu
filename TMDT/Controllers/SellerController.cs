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
        public ActionResult DangBanSanPham() //Đăng tin
        {
            if (Session["idAccount"] == null) return RedirectToAction("DangNhap", "Login");
            if (!Equals(Session["idRole"], 1)) return HttpNotFound();
            DropDownListCategorySanPham();
            return View();
        }
        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult DangBanSanPham(FormCollection collect, Product sp, HttpPostedFileBase imageProduct_1, HttpPostedFileBase imageProduct_2, HttpPostedFileBase imageProduct_3, HttpPostedFileBase imageProduct_4) //Đăng tin
        {
            DropDownListCategorySanPham();

            var nameProduct = collect["nameProduct"];
            var priceProduct = collect["priceProduct"];
            var amountProduct = collect["amountProduct"];
            var descriptionProduct = collect["descriptionProduct"];
            var idCategory_Product = collect["idCategory_Product"];

            if (String.IsNullOrEmpty(nameProduct))
                ViewData["Err"] = "(*) Vui lòng nhập Tên sản phẩm";
            else if (String.IsNullOrEmpty(priceProduct))
                ViewData["Err"] = "(*) Vui lòng nhập Giá sản phẩm";
            else if (String.IsNullOrEmpty(amountProduct))
                ViewData["Err"] = "(*) Vui lòng nhập Số lượng sản phẩm";
            else if (String.IsNullOrEmpty(descriptionProduct))
                ViewData["Err"] = "(*) Vui lòng nhập Mô tả sản phẩm";
            else if (imageProduct_1 == null)
                ViewData["Err"] = "(*) Không để trống Ảnh 1";
            else
            {
                var idAcc = Int32.Parse(Session["idAccount"].ToString());
                sp.nameProduct = nameProduct;
                sp.priceProduct = decimal.Parse(priceProduct.ToString());
                sp.amountProduct = Int32.Parse(amountProduct.ToString());
                sp.descriptionProduct = descriptionProduct;
                sp.datePost = DateTime.Parse(DateTime.Now.ToString());
                sp.idCategory_Product = Int32.Parse(idCategory_Product.ToString());
                string KhongDau = _Function.convertKhongDau(nameProduct);
                sp.Alias = Regex.Replace(KhongDau, @"\s+", "-");
                sp.idAccount = idAcc;
                sp.hideProduct = true;
                sp.confirmProduct = null;
                //Ảnh
                #region UpAnh
                if (ModelState.IsValid)
                {
                    //Ảnh 1
                    string extension1 = System.IO.Path.GetExtension(imageProduct_1.FileName);
                    if (Equals(extension1, ".png") || Equals(extension1, ".jpg"))
                    {
                        var filename1 = DateTime.Now.ToString("ddMMyyyyHHmmss-") + Guid.NewGuid().ToString() + ".jpg";
                        var path1 = Path.Combine(Server.MapPath("~/Assets/ImageProduct"), filename1);

                        if (System.IO.File.Exists(path1))
                            ViewData["Err"] = "(*) Hình ảnh đã tồn tại !";
                        else
                        {
                            imageProduct_1.SaveAs(path1);
                            sp.imageProduct_1 = filename1;
                        }
                    }
                    //Ảnh 2
                    if (imageProduct_2 != null)
                    {
                        string extension2 = System.IO.Path.GetExtension(imageProduct_2.FileName);
                        if (Equals(extension2, ".png") || Equals(extension2, ".jpg"))
                        {
                            var filename2 = DateTime.Now.ToString("ddMMyyyyHHmmss-") + Guid.NewGuid().ToString() + ".jpg";
                            var path2 = Path.Combine(Server.MapPath("~/Assets/ImageProduct"), filename2);

                            if (System.IO.File.Exists(path2))
                                ViewData["Err"] = "(*) Hình ảnh đã tồn tại !";
                            else
                            {
                                imageProduct_2.SaveAs(path2);
                                sp.imageProduct_2 = filename2;
                            }
                        }
                    }
                    //Ảnh 3
                    if (imageProduct_3 != null)
                    {
                        string extension3 = System.IO.Path.GetExtension(imageProduct_3.FileName);
                        if (Equals(extension3, ".png") || Equals(extension3, ".jpg"))
                        {
                            var filename3 = DateTime.Now.ToString("ddMMyyyyHHmmss-") + Guid.NewGuid().ToString() + ".jpg";
                            var path3 = Path.Combine(Server.MapPath("~/Assets/ImageProduct"), filename3);

                            if (System.IO.File.Exists(path3))
                                ViewData["Err"] = "(*) Hình ảnh đã tồn tại !";
                            else
                            {
                                imageProduct_3.SaveAs(path3);
                                sp.imageProduct_3 = filename3;
                            }
                        }
                    }
                    //Ảnh 4
                    if (imageProduct_4 != null)
                    {
                        string extension4 = System.IO.Path.GetExtension(imageProduct_4.FileName);
                        if (Equals(extension4, ".png") || Equals(extension4, ".jpg"))
                        {
                            var filename4 = DateTime.Now.ToString("ddMMyyyyHHmmss-") + Guid.NewGuid().ToString() + ".jpg";
                            var path4 = Path.Combine(Server.MapPath("~/Assets/ImageProduct"), filename4);

                            if (System.IO.File.Exists(path4))
                                ViewData["Err"] = "(*) Hình ảnh đã tồn tại !";
                            else
                            {
                                imageProduct_4.SaveAs(path4);
                                sp.imageProduct_4 = filename4;
                            }
                        }
                    }
                }
                #endregion
                //end ảnh
                db.Products.Add(sp);
                db.SaveChanges();
                ViewData["Err"] = "(*) Đăng sản phẩm thành công vui lòng đợi duyệt";
            }
            return View();
        }
        //Danh sách đặt mua //
        [HttpGet]
        public ActionResult DanhSachDatMua()
        {
            if (Session["idAccount"] == null) return RedirectToAction("DangNhap", "Login");
            if (!Equals(Session["idRole"], 1)) return HttpNotFound();
            int ss = Int32.Parse(Session["idAccount"].ToString());
            IList<_GioHang> infoAcc = new List<_GioHang>();
            var query = from acc in db.infoAccounts
                        join order in db.Orders on acc.idAccount equals order.idAccount
                        join list in db.listOrders on order.idOrder equals list.idOrder
                        join pro in db.Products on list.idProduct equals pro.idProduct
                        join tt in db.Category_Status on order.Category_Status equals tt.idCate_status
                        where order.Category_Status == 1 && pro.idAccount == ss && order.hideOrder == false && order.idAccount != ss
                        orderby order.dateOrder descending
                        select new { order, pro, tt, acc, list };

            var infoAccs = query.ToList();
            foreach (var info in infoAccs)
            {
                infoAcc.Add(new _GioHang()
                {
                    Id = info.order.idOrder,
                    sanpham = info.pro.nameProduct,
                    hinh = info.pro.imageProduct_1,
                    dongia = double.Parse(info.pro.priceProduct.ToString()),
                    soluong = info.list.amountProduct,
                    Ngaydat = info.order.dateOrder,
                    status = info.tt.nameStatus,
                    alias = info.pro.Alias
                });
            }
            return View(infoAcc);
        }
        public ActionResult XacNhanDatMua()
        {
            var getid = Url.RequestContext.RouteData.Values["id"];
            int g = Int32.Parse(getid.ToString());
            int ss = Int32.Parse(Session["idAccount"].ToString());
            var a = from b in db.Orders
                    join h in db.Logins on b.idAccount equals h.idAccount
                    where b.idOrder == g && b.Category_Status == 1 && b.hideOrder == false
                    select new { b, h };
            foreach (var d in a)
            {
                listShip ship = new listShip();
                ship.confirmShip = true;
                ship.idCustomer = d.b.idAccount;
                ship.idOrder = d.b.idOrder;
                ship.idAccount = ss;
                ship.Category_Status = 2;
                ship.hideShip = false;
                db.listShips.Add(ship);
                d.b.Category_Status = 2;
                string content = System.IO.File.ReadAllText(Server.MapPath("~/Assets/SendMail/XacNhanDon.html"));
                content = content.Replace("{{idorder}}", d.b.idOrder.ToString());
                new MailHelper().SendMail(d.h.Email, "[No Reply] Tình Trạng Đơn Hàng", content);
            }
            db.SaveChanges();
            return RedirectToAction("DanhSachDatMua");
        }
        public ActionResult TuChoiDatMua()
        {
            var getid = Url.RequestContext.RouteData.Values["id"];
            int g = Int32.Parse(getid.ToString());
            var a = from b in db.Orders
                    join h in db.Logins on b.idAccount equals h.idAccount
                    where b.idOrder == g && b.Category_Status == 1 && b.hideOrder == false
                    select new { b, h };
            foreach (var d in a)
            {
                d.b.hideOrder = true;
                string content = System.IO.File.ReadAllText(Server.MapPath("~/Assets/SendMail/TuChoiDon.html"));
                content = content.Replace("{{idorder}}", d.b.idOrder.ToString());
                new MailHelper().SendMail(d.h.Email, "[No Reply] Tình Trạng Đơn Hàng", content);
            }
            db.SaveChanges();
            return RedirectToAction("DanhSachDatMua");
        }
        public ActionResult CapNhatSanPham()
        {
            if (Session["idAccount"] == null) return RedirectToAction("DangNhap", "Login");
            if (!Equals(Session["idRole"], 1)) return HttpNotFound();
            var getid = Url.RequestContext.RouteData.Values["id"];
            var id = Int32.Parse(getid.ToString());
            DropDownListCategorySanPhamCapNhat();
            return LoadThongTinSP(id);
        }
        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult CapNhatSanPham(FormCollection collect, HttpPostedFileBase imageProduct_1, HttpPostedFileBase imageProduct_2, HttpPostedFileBase imageProduct_3, HttpPostedFileBase imageProduct_4)
        {
            var getid = Url.RequestContext.RouteData.Values["id"];
            var id = Int32.Parse(getid.ToString());
            DropDownListCategorySanPhamCapNhat();

            var nameProduct = collect["nameProduct"];
            var priceProduct = collect["priceProduct"];
            var amountProduct = collect["amountProduct"];
            var descriptionProduct = collect["descriptionProduct"];
            var idCategory_Product = collect["idCategory_Product"];

            if (Int32.Parse(idCategory_Product.ToString()) == 0)
                ViewData["Err"] = "(*) Vui lòng chọn danh mục sản phẩm";
            else if (String.IsNullOrEmpty(nameProduct))
                ViewData["Err"] = "(*) Vui lòng nhập Tên sản phẩm";
            else if (String.IsNullOrEmpty(priceProduct))
                ViewData["Err"] = "(*) Vui lòng nhập Giá sản phẩm";
            else if (String.IsNullOrEmpty(amountProduct))
                ViewData["Err"] = "(*) Vui lòng nhập Số lượng sản phẩm";
            else if (String.IsNullOrEmpty(descriptionProduct))
                ViewData["Err"] = "(*) Vui lòng nhập Mô tả sản phẩm";
            else
            {
                var update = from a in db.Products
                             where a.idProduct == id
                             select a;
                foreach (var c in update)
                {
                    c.nameProduct = nameProduct;
                    c.priceProduct = decimal.Parse(priceProduct.ToString());
                    c.amountProduct = Int32.Parse(amountProduct.ToString());
                    c.descriptionProduct = descriptionProduct;
                    c.datePost = DateTime.Parse(DateTime.Now.ToString());
                    c.idCategory_Product = Int32.Parse(idCategory_Product.ToString());
                    c.hideProduct = true;
                    c.confirmProduct = null;
                    string KhongDau = _Function.convertKhongDau(nameProduct);
                    c.Alias = Regex.Replace(KhongDau, @"\s+", "-");
                    //Ảnh
                    #region UpAnh
                    if (ModelState.IsValid)
                    {
                        //Ảnh 1
                        if (imageProduct_2 != null)
                        {
                            string extension1 = System.IO.Path.GetExtension(imageProduct_1.FileName);
                            if (Equals(extension1, ".png") || Equals(extension1, ".jpg"))
                            {
                                var filename1 = DateTime.Now.ToString("ddMMyyyyHHmmss-") + Guid.NewGuid().ToString() + ".jpg";
                                var path1 = Path.Combine(Server.MapPath("~/Assets/ImageProduct"), filename1);

                                if (System.IO.File.Exists(path1))
                                    ViewData["Err"] = "(*) Hình ảnh đã tồn tại !";
                                else
                                {
                                    if (c.imageProduct_1 != null)
                                        System.IO.File.Delete(Path.Combine(Server.MapPath("~/Assets/ImageProduct"), c.imageProduct_1));
                                    imageProduct_1.SaveAs(path1);
                                    c.imageProduct_1 = filename1;
                                }
                            }
                        }
                        //Ảnh 2
                        if (imageProduct_2 != null)
                        {
                            string extension2 = System.IO.Path.GetExtension(imageProduct_2.FileName);
                            if (Equals(extension2, ".png") || Equals(extension2, ".jpg"))
                            {
                                var filename2 = DateTime.Now.ToString("ddMMyyyyHHmmss-") + Guid.NewGuid().ToString() + ".jpg";
                                var path2 = Path.Combine(Server.MapPath("~/Assets/ImageProduct"), filename2);

                                if (System.IO.File.Exists(path2))
                                    ViewData["Err"] = "(*) Hình ảnh đã tồn tại !";
                                else
                                {
                                    if (c.imageProduct_2 != null)
                                        System.IO.File.Delete(Path.Combine(Server.MapPath("~/Assets/ImageProduct"), c.imageProduct_2));
                                    imageProduct_2.SaveAs(path2);
                                    c.imageProduct_2 = filename2;
                                }
                            }
                        }
                        //Ảnh 3
                        if (imageProduct_3 != null)
                        {
                            string extension3 = System.IO.Path.GetExtension(imageProduct_3.FileName);
                            if (Equals(extension3, ".png") || Equals(extension3, ".jpg"))
                            {
                                var filename3 = DateTime.Now.ToString("ddMMyyyyHHmmss-") + Guid.NewGuid().ToString() + ".jpg";
                                var path3 = Path.Combine(Server.MapPath("~/Assets/ImageProduct"), filename3);

                                if (System.IO.File.Exists(path3))
                                    ViewData["Err"] = "(*) Hình ảnh đã tồn tại !";
                                else
                                {
                                    if (c.imageProduct_3 != null)
                                        System.IO.File.Delete(Path.Combine(Server.MapPath("~/Assets/ImageProduct"), c.imageProduct_3));
                                    imageProduct_3.SaveAs(path3);
                                    c.imageProduct_3 = filename3;
                                }
                            }
                        }
                        //Ảnh 4
                        if (imageProduct_4 != null)
                        {
                            string extension4 = System.IO.Path.GetExtension(imageProduct_4.FileName);
                            if (Equals(extension4, ".png") || Equals(extension4, ".jpg"))
                            {
                                var filename4 = DateTime.Now.ToString("ddMMyyyyHHmmss-") + Guid.NewGuid().ToString() + ".jpg";
                                var path4 = Path.Combine(Server.MapPath("~/Assets/ImageProduct"), filename4);

                                if (System.IO.File.Exists(path4))
                                    ViewData["Err"] = "(*) Hình ảnh đã tồn tại !";
                                else
                                {
                                    if (c.imageProduct_4 != null)
                                        System.IO.File.Delete(Path.Combine(Server.MapPath("~/Assets/ImageProduct"), c.imageProduct_4));
                                    imageProduct_4.SaveAs(path4);
                                    c.imageProduct_4 = filename4;
                                }
                            }
                        }
                    }
                    #endregion
                    //end ảnh
                }
                db.SaveChanges();
                ViewData["Err"] = "(*) Sản phẩm đã cập nhật thành công VUI LÒNG ĐỢI DUYỆT";
            }
            return LoadThongTinSP(id);
        }
        [NonAction]
        private ActionResult LoadThongTinSP(int id)
        {
            var _id = Int32.Parse(id.ToString());
            IList<_FullProduct> infoProduct = new List<_FullProduct>();
            var query = from product in db.Products
                        join catePro in db.Category_Product on product.idCategory_Product equals catePro.idCategory_Product
                        where product.idProduct == _id
                        orderby product.datePost descending
                        select new { product, catePro };
            var infoProducts = query.ToList();

            foreach (var info in infoProducts)
            {
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
                });
            }
            return View(infoProduct);
        }
        [NonAction]
        public void DropDownListCategorySanPham()
        {
            var dataList = new SelectList(
                            (
                                from catePro in db.Category_Product
                                where catePro.idCategory_Product != 0
                                select new SelectListItem { Text = catePro.nameCategory, Value = catePro.idCategory_Product.ToString() }
                            ), "Value", "Text");
            ViewBag.CategorySanPham = dataList;
        }
        [NonAction]
        public void DropDownListCategorySanPhamCapNhat()
        {
            var dataList = new SelectList(
                            (
                                from catePro in db.Category_Product
                                select new SelectListItem { Text = catePro.nameCategory, Value = catePro.idCategory_Product.ToString() }
                            ), "Value", "Text");
            ViewBag.CategorySanPham = dataList;
        }
    }
}