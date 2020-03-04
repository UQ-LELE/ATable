using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATable.Models
{
    public class MenuPanier : ItemPanier
    {
        public int IdMenu { get; set; }

        //le constructeur me permet d'instancier la list<produitPanier> à chaque fois que l'on instancie MenuPanier
        public MenuPanier()
        {
            produits = new List<ProduitPanier>();
        }

        public List<ProduitPanier> produits { get; set; }

        public override int GetIdMenu()
        {
            return IdMenu;
        }
    }
}