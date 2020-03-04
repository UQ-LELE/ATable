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

        public ActionResult ValidationCommande(int? id)
        {
            Utilisateur utilisateur = db.Utilisateurs.Find(id);
            Session["Utilisateur"] = utilisateur;

            return RedirectToAction("RecapCommande", "Commandes");
        }

        public ActionResult ResumeCommande()
        {
            return RedirectToAction("Resume", "Commandes");
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
