using Microsoft.AspNetCore.Mvc;
using Northwind.Store.Data;
using Northwind.Store.Model;

namespace WA70.Controllers
{
    public class CartController : Controller
    {
        private readonly NWContext _db;
        private readonly SessionSettings _ss;
        private readonly RequestSettings _rs;

        public CartController(NWContext db, SessionSettings ss)
        {
            _db = db;
            _ss = ss;
            _rs = new RequestSettings(this);
        }

        public IActionResult Index()
        {
            var productId = TempData[nameof(Product.ProductId)];
            //var productName = TempData[nameof(Product.ProductName)];
            //TempData.Keep(nameof(Product.ProductName));
            //var productName = TempData.Peek(nameof(Product.ProductName));

            //var productAdded = _rs.ProductAdded;

            ViewBag.productAdded = _rs.ProductAdded;
            //TempData.Keep();

            return View(_ss.Cart);
        }

        public ActionResult Add(int? id)
        {

            if (id.HasValue)
            {               
                #region Session
                var cart = _ss.Cart;

                if (!cart.Items.Any(i => i.ProductId == id))
                {
                    var p = _db.Products.Find(id);
                    cart.Items.Add(p);
                    _ss.Cart = cart;

                    #region TempData
                    //TempData["ProductId"] = p.ProductId;
                    TempData[nameof(Product.ProductId)] = p.ProductId;
                    TempData[nameof(Product.ProductName)] = p.ProductName;

                    _rs.ProductAdded = p;
                    #endregion
                }
                #endregion
            }

            return RedirectToAction("Index");
        }
    }
}
