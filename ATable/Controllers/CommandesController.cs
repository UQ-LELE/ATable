using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ATable.Models;

namespace ATable.Controllers
{
    public class CommandesController : Controller
    {
        private AfpEatEntities db = new AfpEatEntities();


        public ActionResult Panier()
        {
            PanierModel panier = (PanierModel)HttpContext.Application[Session.SessionID];

            if(panier != null && panier.Count > 0)
            {                
                return View(panier);
            }
            return RedirectToAction("Index", "Restaurants");
        }

        // GET: Commandes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null){return new HttpStatusCodeResult(HttpStatusCode.BadRequest);}

            List<CommandeProduit> commandeProduits = db.CommandeProduits.Where(c => c.IdCommande == id).ToList();

            if (commandeProduits == null){return HttpNotFound();}

            return PartialView("_Details", commandeProduits);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing){db.Dispose();}
            base.Dispose(disposing);
        }
    }
}