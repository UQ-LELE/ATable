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
                //faire une redirection vers actual page avec message d'erreur panier vide
                
                return View(panier);
            }
            return RedirectToAction("Index", "Restaurants");
        }

        // GET: Commandes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Commande commande = db.Commandes.Find(id);
            if (commande == null)
            {
                return HttpNotFound();
            }
            return View(commande);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
