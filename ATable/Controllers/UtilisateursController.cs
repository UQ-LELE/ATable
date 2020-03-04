﻿using System;
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

        public ActionResult Connexion(int? idFromRestaurant)
        {
            if(idFromRestaurant != null)
            {
                Session["ReturnUrl"] = idFromRestaurant;
            }

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Connexion([Bind(Include = "Matricule,Password")] Utilisateur utilisateur)
        {          
            if (ModelState.IsValid)
            {
                var user = db.Utilisateurs.Single(u => u.Matricule == utilisateur.Matricule && u.Password == utilisateur.Password);

                if (user != null)
                {
                    user.IdSession = Session.SessionID;
                    Session["Utilisateur"] = user;

                    if(Session["ReturnUrl"] != null)
                    {
                        int idFromRestaurant = Convert.ToInt32(Session["ReturnUrl"]);
                        Session.Remove("ReturnUrl");

                        return RedirectToAction("Details", "Restaurants", new { id = idFromRestaurant });
                    }

                    return View("Index", "Restaurants");
                }
            }
            return View();
        }

        public ActionResult ConnexionModal(string matricule, string password, string previousPage)
        {
            if (ModelState.IsValid)
            {
                var user = db.Utilisateurs.Single(u => u.Matricule == matricule && u.Password == password);

                if (user != null)
                {
                    user.IdSession = Session.SessionID;
                    Session["Utilisateur"] = user;

                    return RedirectToAction("Index", "Restaurants");
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
