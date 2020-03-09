using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATable.Models
{
    public class PanierModel : List<ItemPanier>
    {
        public int IdRestaurant { get; set; }
        public int Quantite { get; set; }

        public decimal Total { get; set; }

        public void GetQuantite()
        {
            Quantite = 0;

            Parcourir();
        }

        public void GetTotal()
        {
            Total = 0;
            Parcourir();

        }

        public bool AddItem(ItemPanier item)
        {
            int idProduit = item.GetIdProduit();
            int idMenu = item.GetIdMenu();
            bool isReturnOk = true;
            ItemPanier itemPanier = null;

            if (item != null)
            {
                //Si l'item est un produit simple
                if ((item is ProduitPanier || item is ProduitComposePanier) && idProduit > 0)
                {
                    //on vérifie si le produit ajouté est déjà présent dans le panier
                    itemPanier = this.FirstOrDefault(p => p.GetIdProduit() == idProduit);
                }
                else if (item is MenuPanier && idMenu > 0) //Si l'item est un menu
                {
                    //on vérifie si le produit ajouté est déjà présent dans le panier
                    itemPanier = this.FirstOrDefault(p => p.GetIdMenu() == idMenu);
                }
                else //si l'item n'est ni un produit ni un menu
                {
                    isReturnOk = false;
                }

                //si le menu ou le produit n'est pas null, cela signifie qu'il est déjà présent dans le paniermodel et donc Qte++
                if (itemPanier != null && isReturnOk == true)
                {
                    itemPanier.Quantite++;
                }
                else
                {
                    this.Add(item);
                }
            }
            else //gérer l'erreur si l'item est null
            {
                isReturnOk = false;
            }
            return isReturnOk;
        }

        public bool RemoveItem(int? idProduit, int? idMenu)
        {
            bool isReturnOk = false;
            ItemPanier itemPanier = null;

            if (idProduit != null && idProduit > 0)
            {
                itemPanier = this.FirstOrDefault(p => p.GetIdProduit() == idProduit);
            }
            else if (idMenu != null && idMenu > 0)
            {
                itemPanier = this.FirstOrDefault(p => p.GetIdMenu() == idMenu);

            }

            if (itemPanier != null)
            {
                this.Remove(itemPanier);
                isReturnOk = true;
            }
            return isReturnOk;
        }

        private void Parcourir()
        {
            foreach (ItemPanier itemPanier in this)
            {
                Total += itemPanier.Prix * itemPanier.Quantite;
                Quantite += itemPanier.Quantite;
                IdRestaurant = itemPanier.IdRestaurant;

            }
        }


    }
}