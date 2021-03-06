﻿using System.Web;
using System.Web.Optimization;

namespace ATable
{
    public class BundleConfig
    {
        // Pour plus d'informations sur le regroupement, visitez https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryUI").Include(
                       "~/Scripts/jquery-ui-{version}.js"));


            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Utilisez la version de développement de Modernizr pour le développement et l'apprentissage. Puis, une fois
            // prêt pour la production, utilisez l'outil de génération à l'adresse https://modernizr.com pour sélectionner uniquement les tests dont vous avez besoin.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/materialize").Include(
                      "~/Scripts/materialize/materialize.min.js")); 
            
            bundles.Add(new ScriptBundle("~/bundles/fontawesome").Include(
                      "~/Scripts/fontawesome/all.min.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/materialize/css/materialize.css",
                      "~/Content/fontawesome-all.css",
                      "~/Content/themes/base/jquery-ui.css",
                      "~/Content/site.css").Include("~/Content/fontawesome.css", new CssRewriteUrlTransform()));
        }
    }
}
