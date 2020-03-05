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
    public class UtilisateursController : Controller
    {
        private AfpEatEntities db = new AfpEatEntities();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Connexion([Bind(Include = "Matricule,Password")] Utilisateur utilisateur)
        {
            string previousUrl = Request.UrlReferrer.ToString();

            if (ModelState.IsValid)
            {
                var user = db.Utilisateurs.Single(u => u.Matricule == utilisateur.Matricule && u.Password == utilisateur.Password);

                if (user != null)
                {
                    user.IdSession = Session.SessionID;
                    Session["Utilisateur"] = user;

                    return Redirect(previousUrl);
                }
            }
            return View();
        }

        // GET: Utilisateurs/Details/5
        public ActionResult MonCompte(int? id)
        {
            Utilisateur utilisateur = new Utilisateur();

            if (Session["Utilisateur"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                Utilisateur user = (Utilisateur)Session["Utilisateur"];

                utilisateur = db.Utilisateurs.Find(id);

                if (utilisateur == null)
                {
                    return HttpNotFound();
                }

                if(user.Matricule == utilisateur.Matricule && user.Password == utilisateur.Password)
                {
                    return View(utilisateur);
                }
            }
            return RedirectToAction("Index", "Restaurants");
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
