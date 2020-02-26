using ATable.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ATable.Controllers
{
    public class WebServiceController : Controller
    {
        // GET: WebService
        private AfpEatEntities db = new AfpEatEntities();

        //GET: SW
        public JsonResult AddProduit(int idProduit, string idSession)
        {
            string html = "";
            decimal total = 0;

            SessionUtilisateur sessionUtilisateur = db.SessionUtilisateurs.Find(Session.SessionID);

            List<ProduitPanier> panier = null;

            if (sessionUtilisateur != null)
            {
                Produit produit = db.Produits.Find(idProduit);

                if (HttpContext.Application[idSession] != null)
                {
                    panier = (List<ProduitPanier>)HttpContext.Application[idSession];
                }
                else
                {
                    panier = new List<ProduitPanier>();
                }

                ProduitPanier produitPanier = null;

                if (panier.Where(p =>p.IdProduit == idProduit).Count() > 0)
                {
                    produitPanier = panier.Where(p => p.IdProduit == idProduit).First();
                    produitPanier.Quantite += 1;
                }
                else
                {
                    produitPanier = new ProduitPanier();
                    produitPanier.IdProduit = idProduit;
                    produitPanier.Nom = produit.Nom;
                    produitPanier.Description = produit.Description;
                    produitPanier.Quantite = 1;
                    produitPanier.Prix = produit.Prix;
                    //produitPanier.Photo = produit.Photos.First().Nom;
                    produitPanier.IdRestaurant = produit.IdRestaurant;
                    panier.Add(produitPanier);

                }

                HttpContext.Application[idSession] = panier;

                foreach (ProduitPanier item in panier)
                {
                    html += ShowPanier(item);
                    total += item.Prix * item.Quantite;
                }
            }

            return Json(new {panier = html, total = string.Format("{0:0.00}", total) }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteProduit(int idProduit, string idSession)
        {
            string html = "";
            decimal total = 0;

            SessionUtilisateur sessionUtilisateur = db.SessionUtilisateurs.Find(Session.SessionID);

            List<ProduitPanier> panier = null;

            if (sessionUtilisateur != null)
            {
                Produit produit = db.Produits.Find(idProduit);

                if (HttpContext.Application[idSession] != null)
                {
                    panier = (List<ProduitPanier>)HttpContext.Application[idSession];
                }


                ProduitPanier produitPanier = panier.Where(p => p.IdProduit == idProduit).First();

                if (produitPanier.Quantite == 1)
                {
                    panier.Remove(produitPanier);
                }
                else
                {
                    produitPanier.Quantite -= 1;
                }

                HttpContext.Application[idSession] = panier;

                foreach (ProduitPanier item in panier)
                {
                    html += ShowPanier(item);
                    total += item.Prix * item.Quantite;
                }

            }

            return Json(new { panier = html, total = string.Format("{0:0.00}", total) }, JsonRequestBehavior.AllowGet);
        }

        public string ShowPanier(ProduitPanier item)
        {          
            string html = "";
            html += "<div class='row article valign-wrapper'>";
            html += "<div class='col m2' style='margin-left:0px;'>";
            html += "<button class='btn-floating btn-mini waves-effect waves-light red delete' onclick='deleteProduit(\""+ item.IdProduit +"\")'>-</button>";
            html += "</div>";
            html += "<div class='col m6' style='margin-left:0px;'><strong><span>" + item.Quantite + "&nbsp;&nbsp;x</span>&nbsp;&nbsp;" + item.Nom + "</strong></div>";
            html += "<div class='col m4' style='margin-left:0px;'>" + item.Prix * item.Quantite + " €</div>";
            html += "</div>";
            html += "<div class='divider'></div>";
            return html;
        }

        public JsonResult GetPanier(string idSession)
        {
            List<ProduitPanier> panier = null;


            SessionUtilisateur sessionUtilisateur = db.SessionUtilisateurs.Find(Session.SessionID);

            if (sessionUtilisateur != null)
            {
                if (HttpContext.Application[idSession] != null)
                {
                    panier = (List<ProduitPanier>)HttpContext.Application[idSession];


                }
            }
            return Json(panier, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveCommande(string idSession)
        {
            List<ProduitPanier> panier = null;
            //gérer les messages associés au déroulé de l'action (succes, solde insufisant, panier vide...)
            string message = "";

            SessionUtilisateur sessionUtilisateur = db.SessionUtilisateurs.Find(Session.SessionID);

            if (sessionUtilisateur != null && HttpContext.Application[idSession] != null)
            {
                panier = (List<ProduitPanier>)HttpContext.Application[idSession];
            }

            Utilisateur utilisateur = db.Utilisateurs.First(u => u.IdSession == idSession);

            if (utilisateur != null && utilisateur.Solde > 0 && panier != null && panier.Count > 0)
            {
                decimal prixTotal = 0;
                int idRestaurant = 0;

                foreach (ProduitPanier item in panier)
                {
                    prixTotal += item.Prix * item.Quantite;
                    idRestaurant = item.IdRestaurant;
                }

                if (prixTotal <= utilisateur.Solde)
                {

                    Commande commande = new Commande();
                    commande.IdUtilisateur = utilisateur.IdUtilisateur;
                    commande.IdRestaurant = idRestaurant;
                    commande.Date = DateTime.Now;
                    commande.Prix = prixTotal;
                    commande.IdEtatCommande = 1;

                    utilisateur.Solde -= prixTotal;

                    foreach (ProduitPanier item in panier)
                    {
                        CommandeProduit commandeProduit = new CommandeProduit();
                        commandeProduit.IdProduit = item.IdProduit;
                        commandeProduit.Prix = item.Prix;
                        commandeProduit.Quantite = item.Quantite;

                        commande.CommandeProduits.Add(commandeProduit);
                    }

                    db.Commandes.Add(commande);
                    db.SaveChanges();
                }
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Login(string idSession, string matricule, string password)
        {
            Utilisateur utilisateur = db.Utilisateurs.FirstOrDefault(u => u.Matricule == matricule && u.Password == password);

            if (utilisateur != null)
            {
                utilisateur.IdSession = idSession;
                db.SaveChanges();

                return Json(new { error = 0, message = idSession }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { error = 1, message = "Connexion échouée" }, JsonRequestBehavior.AllowGet);
        }
    }
}
