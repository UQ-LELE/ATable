using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
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
                Utilisateur user = db.Utilisateurs.Single(u => u.Matricule == utilisateur.Matricule && u.Password == utilisateur.Password);


                if (user != null)
                {
                    user.IdSession = Session.SessionID;
                    db.SaveChanges();

                    Session["Utilisateur"] = user;

                    return RedirectToAction("MonCompte", new { id = user.IdUtilisateur });

                }
            }
            return Redirect(previousUrl);
        }

        public ActionResult Deconnexion(int id)
        {
            string previousUrl = Request.UrlReferrer.ToString();

            Utilisateur user = (Utilisateur)Session["Utilisateur"];

            if (user.IdUtilisateur == id)
            {
                Session.Remove("Utilisateur");
            }

            return Redirect(previousUrl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(FormCollection forms)
        {
            Utilisateur user = (Utilisateur)Session["Utilisateur"];
            var password = forms["oldPassword"];
            var newPassword = forms["newPassword"];
            var confirmPassword = forms["confirmPassword"];

            if (user.Password == password && newPassword == confirmPassword)
            {
                user = db.Utilisateurs.Single(u => u.Password == password);

                user.Password = confirmPassword;
                db.SaveChanges();
                Session["Utilisateur"] = user;
                TempData["NewPassword"] = "isChange";
                return RedirectToAction("MonCompte", new { id = user.IdUtilisateur });
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

                utilisateur = db.Utilisateurs.Find(id);
                Session["Utilisateur"] = utilisateur;

                if (utilisateur == null)
                {
                    return HttpNotFound();
                }

                var commandes = db.Commandes.Include(c => c.EtatCommande).Include(c => c.Restaurant).Where(c => c.IdUtilisateur == utilisateur.IdUtilisateur);
                return View(commandes.ToList());

            }
            return RedirectToAction("Index", "Restaurants");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
