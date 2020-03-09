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
    public class RestaurantsController : Controller
    {
        private AfpEatEntities db = new AfpEatEntities();
        public ActionResult Index()
        {
            ViewBag.Restaurants = db.Restaurants.Include(r => r.TypeCuisine).ToList();
            ViewBag.TypeCuisines = db.TypeCuisines.ToList();

            return View();
        }

        public ActionResult ParSpecialite(int id)
        {

            ViewBag.RestoByCuisine = db.Restaurants.Where(r => r.IdTypeCuisine == id).ToList();
            ViewBag.TypeCuisine = db.TypeCuisines.Where(t => t.IdTypeCuisine == id).FirstOrDefault();
            

            return View();
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.Error = null;

            PanierModel panier = (PanierModel)HttpContext.Application[Session.SessionID];

            if (panier != null && panier.IdRestaurant != id) 
            {
                ViewBag.Error = "error";
                ViewBag.IdRestaurantPanier = panier.IdRestaurant;
            }

            var produits = db.Produits.Where(p => p.IdRestaurant == id);


            if (produits == null)
            {
                return HttpNotFound();
            }
            //variante de viewbag, filtrer les catégories et type d'itempanier dans la vue
            Restaurant restaurant = db.Restaurants.Include(r => r.Produits).Include(r => r.Menus).Where(r => r.IdRestaurant == id).First();


            ViewBag.User = Session["Utilisateur"] != null ? Session["Utilisateur"] : null;

            return View(restaurant);
        }


        public ActionResult Carte(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.Error = null;

            PanierModel panier = (PanierModel)HttpContext.Application[Session.SessionID];

            if (panier != null && panier.Count > 0 && panier.IdRestaurant != id)
            {
                ViewBag.Error = "error";
                ViewBag.IdRestaurantPanier = panier.IdRestaurant;
            }

            Restaurant restaurant = db.Restaurants.Where(r => r.IdRestaurant == id).First();

            if (restaurant == null)
            {
                return HttpNotFound();
            }
            //variante de viewbag, filtrer les catégories et type d'itempanier dans la vue
            
            ViewBag.User = Session["Utilisateur"] != null ? Session["Utilisateur"] : null;

            return View(restaurant);
        }

        public ActionResult GetMenu(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Menu menu = db.Menus.Find(id);
            ViewBag.Menu = menu;

            List<Produit> produitsRestaurant = db.Produits.Where(p => p.IdRestaurant == menu.IdRestaurant).ToList();

            if (menu == null)
            {
                return HttpNotFound();
            }

            List<Produit> produitsMenu = new List<Produit>();

            foreach (var categorie in menu.Categories)
            {
                foreach(var item in produitsRestaurant.Where(pr=>pr.IdCategorie == categorie.IdCategorie))
                {
                    Produit produit = new Produit();
                    produit = item;
                    produitsMenu.Add(produit);
                }
            }

            return PartialView("_Menu", produitsMenu);
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
