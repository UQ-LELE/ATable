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
        private AfpEatEntities db = new AfpEatEntities();

        //ajout d'un produit au panier
        public JsonResult AddProduit(int idProduit, string idSession)
        {
            //gérer le renvoit isReturnOk sur le JS avec message erreur
            bool isReturnOk = false;
            PanierHtml panierHtml = new PanierHtml();

            //récupération de l'utilisateur sur le SW
            SessionUtilisateur sessionUtilisateur = db.SessionUtilisateurs.Find(idSession);

            //récupération du panier || création d'un panier
            PanierModel panierModel = (PanierModel)HttpContext.Application[idSession] ?? new PanierModel();
            panierModel.IdRestaurant = 0;

            if (sessionUtilisateur != null && panierModel != null && idProduit > 0)
            {
                //méthode FindProduit cf.
                ProduitPanier produitPanier = FindProduit(idProduit);

                if (produitPanier != null)
                {
                    //si le panier est vide
                    if(panierModel.IdRestaurant == 0)
                    {
                        panierModel.IdRestaurant = produitPanier.IdRestaurant;
                        panierModel.AddItem(produitPanier);
                        isReturnOk = true;
                    }
                    //si le panier n'est pas vide
                    else if(panierModel.IdRestaurant == produitPanier.IdRestaurant)
                    {
                        panierModel.AddItem(produitPanier);
                        isReturnOk = true;
                    }
                }

                //enregistrement du panier
                HttpContext.Application[idSession] = panierModel;

                //creation html de panier lateral
                panierHtml = ShowPanier(panierModel);
            }

            return Json(new { panier = panierHtml.hmtl, isreturn = isReturnOk,  total = string.Format("{0:0.00}", panierHtml.total) }, JsonRequestBehavior.AllowGet);
        }

        //ajout d'un menu au panier
        public JsonResult AddMenu(int idMenu, List<int> idProduits, string idSession)
        {
            PanierHtml panierHtml = new PanierHtml();

            SessionUtilisateur sessionUtilisateur = db.SessionUtilisateurs.Find(idSession);

            PanierModel panierModel = (PanierModel)HttpContext.Application[idSession] ?? new PanierModel();
            panierModel.IdRestaurant = 0;

            if (sessionUtilisateur != null && idMenu > 0 && idProduits.Count > 0)
            {
                Menu menu = db.Menus.Find(idMenu);
                panierModel.IdRestaurant = menu.IdRestaurant;

                if (menu != null)
                {
                    //creation du menu
                    MenuPanier menuPanier = new MenuPanier();
                    menuPanier.IdMenu = idMenu;
                    menuPanier.Nom = menu.Nom;
                    menuPanier.Prix = menu.Prix;
                    menuPanier.Quantite = 1;
                    
                    //ajout des produits selectionnés dans le menu
                    foreach (int idProduit in idProduits)
                    {
                        ProduitPanier produitPanier = FindProduit(idProduit);

                        if (produitPanier != null) menuPanier.produits.Add(produitPanier);                    
                    }

                    //ajout du menu avec ces produits dans le panier
                    panierModel.AddItem(menuPanier);
                }

                HttpContext.Application[idSession] = panierModel;

                panierHtml = ShowPanier(panierModel);
            }
            return Json(new { panier = panierHtml.hmtl, total = string.Format("{0:0.00}", panierHtml.total) }, JsonRequestBehavior.AllowGet);
        }

        //méthode qui récupère toutes les données liées à un produit et les ajoute à un objet de type ProduitPanier
        private ProduitPanier FindProduit(int idProduit)
        {
            Produit produit = db.Produits.Find(idProduit);
            ProduitPanier produitPanier = new ProduitPanier();

            if (produit != null)
            {
                produitPanier.IdProduit = idProduit;
                produitPanier.Nom = produit.Nom;
                produitPanier.Description = produit.Description;
                produitPanier.Quantite = 1;
                produitPanier.Prix = produit.Prix;
                produitPanier.Photo = produit.Photos.First().Nom ?? "~/Images/plats/default_image(p).png";
                produitPanier.IdRestaurant = produit.IdRestaurant;
            }
            return produitPanier;
        }

        //suppression d'un produit/menu du panier
        public JsonResult RemoveProduit(int idProduit, string idSession)
        {
            PanierHtml panierHtml = null;
            PanierModel panierModel = null;

            SessionUtilisateur sessionUtilisateur = db.SessionUtilisateurs.Find(idSession);

            if (sessionUtilisateur != null && idProduit > 0)
            {
                Produit produit = db.Produits.Find(idProduit);

                if (HttpContext.Application[idSession] != null)
                {
                    panierModel = (PanierModel)HttpContext.Application[idSession];
                }

                ItemPanier itemPanier = panierModel.FirstOrDefault(p => p.GetIdProduit() == idProduit);
                
                //si l'itemPanier est déjà présent dans le panier alors quantité -1, sinon ajout de l'itemPanier
                if (itemPanier.Quantite == 1)
                {
                    panierModel.Remove(itemPanier);
                }
                else
                {
                    itemPanier.Quantite -= 1;
                }

                panierHtml = ShowPanier(panierModel);

                HttpContext.Application[idSession] = panierModel;

            }

            return Json(new { panier = panierHtml.hmtl, total = string.Format("{0:0.00}", panierHtml.total) }, JsonRequestBehavior.AllowGet);
        }

        //création du panier latéral en html
        public PanierHtml ShowPanier(PanierModel panier)
        {
            PanierHtml panierHtml = new PanierHtml();

            panierHtml.hmtl = "";
            panierHtml.total = 0;
            foreach (ItemPanier itemPanier in panier)
            {
                panierHtml.hmtl += "<div class='row valign-wrapper mt-2 mb-0'>";
                panierHtml.hmtl += "<div class='col m2' style='margin-left:0px;'>";

                if (itemPanier is MenuPanier)
                {
                    panierHtml.hmtl += "<button class='btn-floating btn-small waves-effect waves-light red remove' onclick='removeMenu(\"" + itemPanier.GetIdMenu() + "\")'><i class='material-icons'>remove</i></button>";
                }
                else
                {
                    panierHtml.hmtl += "<button class='btn-floating btn-small waves-effect waves-light red remove' onclick='removeProduit(\"" + itemPanier.GetIdProduit() + "\")'><i class='material-icons'>remove</i></button>";
                }
                panierHtml.hmtl += "</div>";
                panierHtml.hmtl += "<div class='col m6' style='margin-left:0px;'><strong><span>" + itemPanier.Quantite + "&nbsp;&nbsp;x</span>&nbsp;&nbsp;" + itemPanier.Nom + "</strong></div>";
                panierHtml.hmtl += "<div class='col m4' style='margin-left:0px;'>" + itemPanier.Prix * itemPanier.Quantite + " €</div>";
                panierHtml.hmtl += "</div>";

                if (itemPanier is MenuPanier)
                {
                    panierHtml.hmtl += "<div class='row'>";
                    panierHtml.hmtl += "<div class='col s12 center-align'>";
                    panierHtml.hmtl += "<ul>";
                    MenuPanier menuPanier = (MenuPanier)itemPanier;

                    foreach (ItemPanier produitMenu in menuPanier.produits)
                    {
                        panierHtml.hmtl += "<li>" + produitMenu.Nom + "</li>";
                    }
                    panierHtml.hmtl += "</ul>";
                    panierHtml.hmtl += "</div>";
                    panierHtml.hmtl += "</div>";
                }

                panierHtml.hmtl += "<div class='divider'></div>";

                panierHtml.total += itemPanier.Prix * itemPanier.Quantite;
            }
            return panierHtml;
        }

        //si le panier n'est pas vide, creation du panier latéral ET du total à chaque chargement de vue;
        public JsonResult GetPanierHtml(string idSession)
        {
            PanierModel panierModel = null;

            PanierHtml panierHtml = new PanierHtml();
            //panier et total n'est valent rien si l'utilisateur n'a pas de panier
            panierHtml.hmtl = "";
            panierHtml.total = 0;

            SessionUtilisateur sessionUtilisateur = db.SessionUtilisateurs.Find(idSession);

            if (sessionUtilisateur != null)
            {
                if (HttpContext.Application[idSession] != null)
                {
                    panierModel = (PanierModel)HttpContext.Application[idSession];

                    panierHtml = ShowPanier(panierModel);
                }
            }
            return Json(new { panier = panierHtml.hmtl, total = string.Format("{0:0.00}", panierHtml.total) }, JsonRequestBehavior.AllowGet);
        }

        //méthode de sauvegarde du panier en bdd
        public JsonResult SaveCommande(string idSession)
        {
            PanierModel panierModel = null;

            //à faire : gérer les messages associés au déroulé de l'action (succes, solde insufisant, panier vide...)
            string message = "Une erreur est survenue dans votre commande";

            SessionUtilisateur sessionUtilisateur = db.SessionUtilisateurs.Find(idSession);

            //on vérifie l'existence d'un utilisateur en session et on récupère son panier
            if (sessionUtilisateur != null && HttpContext.Application[idSession] != null)
            {
                panierModel = (PanierModel)HttpContext.Application[idSession];
            }

            //on récupère l'utilisateur en base de données
            Utilisateur utilisateur = db.Utilisateurs.First(u => u.IdSession == idSession);

            if (utilisateur != null && utilisateur.Solde > 0 && panierModel != null && panierModel.Count > 0)
            {
                decimal prixTotal = 0;

                //calcul du montant total de la commande
                panierModel.GetTotal();
                prixTotal = panierModel.Total;


                //on vérifie si le montant est inférieur au solde avant d'enregistrer la commande
                if (prixTotal <= utilisateur.Solde)
                {
                    //on créé la commande
                    Commande commande = new Commande();
                    commande.IdUtilisateur = utilisateur.IdUtilisateur;
                    commande.IdRestaurant = panierModel.IdRestaurant;
                    commande.Date = DateTime.Now;
                    commande.Prix = prixTotal;
                    commande.IdEtatCommande = 1;

                    utilisateur.Solde -= prixTotal;

                    //on parcours tous les produits dans le panierModel pour les enregistrer en bdd
                    foreach (ItemPanier itemPanier in panierModel)
                    {

                        if (itemPanier is ProduitPanier)
                        {
                            CommandeProduit commandeProduit = new CommandeProduit();
                            commandeProduit.IdProduit = itemPanier.GetIdProduit();
                            commandeProduit.Prix = itemPanier.Prix;
                            commandeProduit.Quantite = itemPanier.Quantite;
                            commande.CommandeProduits.Add(commandeProduit);
                        }
                        else if (itemPanier is MenuPanier menuPanier)
                        {
                            List<ProduitPanier> produitPaniers = menuPanier.produits;
                            Menu menu = db.Menus.Find(itemPanier.GetIdMenu());

                            foreach (ProduitPanier itemProduit in produitPaniers)
                            {
                                CommandeProduit commandeProduit = new CommandeProduit();
                                commandeProduit.IdProduit = itemPanier.GetIdProduit();
                                commandeProduit.Prix = itemPanier.Prix;
                                commandeProduit.Quantite = itemPanier.Quantite;
                                commandeProduit.Menus.Add(menu);
                                commande.CommandeProduits.Add(commandeProduit);
                            }
                        }
                        else if (itemPanier is ProduitComposePanier produitComposePanier)
                        {
                            List<ProduitPanier> produitPaniers = produitComposePanier.produits;
                            foreach (ProduitPanier itemProduit in produitPaniers)
                            {
                                CommandeProduit commandeProduit = new CommandeProduit();
                                commandeProduit.IdProduit = itemPanier.GetIdProduit();
                                commandeProduit.Prix = itemPanier.Prix;
                                commandeProduit.Quantite = itemPanier.Quantite;
                                commande.CommandeProduits.Add(commandeProduit);
                            }
                        }
                    }

                    db.Commandes.Add(commande);
                    db.SaveChanges();
                    //le panier est vidé
                    panierModel.Clear();

                    message = "Votre commande a bien été enregistré !";
                }
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }
      
        public JsonResult ClearPanier(string idSession)
        {
            //gérer message
            string message = "Une erreur est surenue";

            SessionUtilisateur sessionUtilisateur = db.SessionUtilisateurs.Find(idSession);

            PanierModel panierModel = (PanierModel)HttpContext.Application[idSession] ?? null;

            if (sessionUtilisateur != null && panierModel != null)
            {
                panierModel.Clear();
                HttpContext.Application[idSession] = panierModel;
                message = "Votre panier a été vidé";
            }

            return Json(message, JsonRequestBehavior.AllowGet);

        }
    }
}
