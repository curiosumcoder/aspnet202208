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

        public ActionResult Add(int? id)
        {

            if (id.HasValue)
            {
                var p = _db.Products.Find(id);

                #region Session
                var cart = _ss.Cart;

                cart.Items.Add(p);

                _ss.Cart = cart;
                #endregion
            }

            return RedirectToAction("Index");
        }
    }
}
