using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TMDT.helper;
using TMDT.Models;

namespace TMDT.Controllers
{
    public class PaypalController : Controller
    {
        ChoDoCuEntities db = new ChoDoCuEntities();
        private Payment payment;
        // GET: Paypal
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult PaymentWithPaypal(string url)
        {
            if (Session["idAccount"] == null) return RedirectToAction("DangNhap", "Login");
            //getting the apiContext as earlier
            APIContext apiContext = helper.Configuration.GetAPIContext();
            try
            {
                string payerId = Request.Params["PayerID"];
                if (string.IsNullOrEmpty(payerId))
                {
                    //this section will be executed first because PayerID doesn't exist
                    //it is returned by the create function call of the payment class
                    // Creating a payment
                    // baseURL is the url on which paypal sendsback the data.
                    // So we have provided URL of this controller only
                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/Paypal/PaymentWithPayPal?";
                    //guid we are generating for storing the paymentID received in session
                    //after calling the create function and it is used in the payment execution
                    var guid = Convert.ToString((new Random()).Next(100000));
                    //CreatePayment function gives us the payment approval url
                    //on which payer is redirected for paypal account payment
                    var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid);
                    //get links returned from paypal in response to Create function  call
                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectUrl = null;
                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;
                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            //saving the payapalredirect URL to which user will be  redirected for payment
                            paypalRedirectUrl = lnk.href;
                        }
                    }
                    // saving the paymentID in the key guid
                    Session.Add(guid, createdPayment.id);
                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    // This section is executed when we have received all the payments parameters
                    // from the previous call to the function Create
                    // Executing a payment
                    var guid = Request.Params["guid"];
                    var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);
                    if (executedPayment.state.ToLower() != "approved")
                    {
                        return View("FailureView");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error" + ex.Message);
                return View("FailureView");
            }
            return ThanhToan();
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
	private Payment CreatePayment(APIContext apiContext, string redirectUrl)
        {
            var itemList = new ItemList() { items = new List<Item>() };
            //Các giá trị bao gồm danh sách sản phẩm, thông tin đơn hàng
            //Sẽ được thay đổi bằng hành vi thao tác mua hàng trên website
            List<_GioHang> lstGiohang = Laygiohang();
            if (lstGiohang == null) return null;
            double _subtotal = 0;
            foreach (var i in lstGiohang)
            {
                var _dongia = Math.Round((i.dongia / 23000), 2);
                itemList.items.Add(new Item()
                {
                    //Thông tin đơn hàng
                    name = @i.sanpham,
                    currency = "USD",
                    price = _dongia.ToString(),
                    quantity = @i.soluong.ToString(),
                    sku = null
                });
                _subtotal += _dongia * i.soluong;
            }
            //Hình thức thanh toán qua paypal
            var payer = new Payer() { payment_method = "paypal" };
            // Configure Redirect Urls here with RedirectUrls object
            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl,
                return_url = redirectUrl
            };
            //các thông tin trong đơn hàng
            var details = new Details()
            {
                tax = "0",
                shipping = "0",
                subtotal = _subtotal.ToString()
            };
            //Đơn vị tiền tệ và tổng đơn hàng cần thanh toán
            var amount = new Amount()
            {
                currency = "USD",
                total = _subtotal.ToString(), // Total must be equal to sum of shipping, tax andsubtotal.
                details = details
            };
            var transactionList = new List<Transaction>();
            //Tất cả thông tin thanh toán cần đưa vào transaction
            transactionList.Add(new Transaction()
            {
                description = "Transaction description.",
                invoice_number = Guid.NewGuid().ToString(),
                amount = amount,
                item_list = itemList
            });
            this.payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };
            // Create a payment using a APIContext
            return this.payment.Create(apiContext);
        }
        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution() { payer_id = payerId };
            this.payment = new Payment() { id = paymentId };
            return this.payment.Execute(apiContext, paymentExecution);
        }
        public ActionResult FailureView()
        {
            return View();
        }
        public ActionResult XoaTatcaGioHang()
        {
            List<_GioHang> lstGiohang = Laygiohang();
            lstGiohang.Clear();
            return RedirectToAction("TrangChu", "Home");
        }
        public ActionResult ThanhToan()
        {
            if (Session["idAccount"] == null) return RedirectToAction("DangNhap", "Login");

            List<_GioHang> lstGiohang = Laygiohang();
            if (lstGiohang == null) return RedirectToAction("TrangChu", "Home");

            foreach (var i in lstGiohang)
            {
                var order = new Models.Order();
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
    }
}