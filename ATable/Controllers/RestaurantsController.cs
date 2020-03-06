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

        // GET: Restaurants/Details/5
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

            ViewBag.Menu  = db.Menus.Where(m => m.IdRestaurant == id).ToList();
            ViewBag.Entree = db.Produits.Where(p => p.IdRestaurant == id && p.IdCategorie == 1).ToList();
            ViewBag.Plat = db.Produits.Where(p => p.IdRestaurant == id && p.IdCategorie == 2).ToList();
            ViewBag.Dessert = db.Produits.Where(p => p.IdRestaurant == id && p.IdCategorie == 3).ToList();
            ViewBag.Restaurant = db.Restaurants.Where(p => p.IdRestaurant == id).SingleOrDefault();

            ViewBag.User = Session["Utilisateur"] != null ? Session["Utilisateur"] : null;

            return View();
        }


        // GET: Restaurants/Edit/5
        public ActionResult Carte(int? id)
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

            ViewBag.Menu = db.Menus.Where(m => m.IdRestaurant == id).ToList();
            ViewBag.Entree = db.Produits.Where(p => p.IdRestaurant == id && p.IdCategorie == 1).ToList();
            ViewBag.Plat = db.Produits.Where(p => p.IdRestaurant == id && p.IdCategorie == 2).ToList();
            ViewBag.Dessert = db.Produits.Where(p => p.IdRestaurant == id && p.IdCategorie == 3).ToList();
            ViewBag.Restaurant = db.Restaurants.Where(p => p.IdRestaurant == id).SingleOrDefault();

            ViewBag.User = Session["Utilisateur"] != null ? Session["Utilisateur"] : null;

            return View();
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
