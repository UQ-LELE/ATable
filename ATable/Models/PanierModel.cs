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

        public bool AddItem(ItemPanier itemToAdd)
        {
            int idProduit = itemToAdd.GetIdProduit();
            int idMenu = itemToAdd.GetIdMenu();
            bool isReturnOk = true;
            ItemPanier isSameItem = null;

            if (itemToAdd != null)
            {
                int sameValidation = 0;
                //Si l'item est un produit simple
                if ((itemToAdd is ProduitPanier || itemToAdd is ProduitComposePanier) && idProduit > 0)
                {
                    //on vérifie si le produit ajouté est déjà présent dans le panier
                    isSameItem = this.FirstOrDefault(p => p.GetIdProduit() == idProduit);
                }
                else if (itemToAdd is MenuPanier && idMenu > 0) //Si l'item est un menu
                {
                    //on vérifie si le produit ajouté est déjà présent dans le panier
                    isSameItem = this.FirstOrDefault(p => p.GetIdMenu() == idMenu);

                    if(isSameItem != null)
                    {
                        MenuPanier menuSame = (MenuPanier)isSameItem;
                        MenuPanier menuToAdd = (MenuPanier)itemToAdd;
                        foreach (ProduitPanier itemSame in menuSame.produits)
                        {
                           foreach(ProduitPanier itemMenu in menuToAdd.produits)
                            {
                                if (itemSame.IdProduit == itemMenu.IdProduit)
                                {
                                    sameValidation += 1;
                                }
                            }
                        }
                    }

                }
                else //si l'item n'est ni un produit ni un menu
                {
                    isReturnOk = false;
                }

                //si le menu ou le produit n'est pas null, cela signifie qu'il est déjà présent dans le paniermodel et donc Qte++
                if (isSameItem != null && isReturnOk == true && sameValidation == 3)
                {
                    isSameItem.Quantite++;
                }
                else
                {
                    this.Add(itemToAdd);
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