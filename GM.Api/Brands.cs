using System;
using System.Collections.Generic;
using System.Text;

namespace GM.Api
{

    public enum Brand
    {
        Opel,
        Vauxhall,
        Chevrolet,
        OnStar,
        Cadillac,
        Buick,
        Gmc
    }


    public static class BrandHelpers
    {

        public static string GetDisplayName(this Brand brand)
        {
            return brand.ToString();
        }

        public static string GetName(this Brand brand)
        {
            switch (brand)
            {
                case Brand.Opel:
                    return "opel";
                case Brand.Vauxhall:
                    return "vauxhall";
                case Brand.Chevrolet:
                    return "chevrolet";
                case Brand.OnStar:
                    return "onstar";
                case Brand.Cadillac:
                    return "cadillac";
                case Brand.Buick:
                    return "buick";
                case Brand.Gmc:
                    return "gmc";
                default:
                    throw new InvalidOperationException("Unknown Brand");
            }
        }

        public static string GetUrl(this Brand brand)
        {
            switch (brand)
            {
                case Brand.Opel:
                    return "https://api.eur.onstar.com/api";
                case Brand.Vauxhall:
                    return "https://api.eur.onstar.com/api";
                case Brand.Chevrolet:
                    return "https://api.gm.com/api";
                case Brand.OnStar:
                    return "https://api.gm.com/api";
                case Brand.Cadillac:
                    return "https://api.gm.com/api";
                case Brand.Buick:
                    return "https://api.gm.com/api";
                case Brand.Gmc:
                    return "https://api.gm.com/api";
                default:
                    throw new InvalidOperationException("Unknown Brand");
            }
        }


        public static Brand GetBrand(string brandName)
        {
            var cleanName = brandName.ToLowerInvariant();

            switch (cleanName)
            {
                case "opel":
                    return Brand.Opel;
                case "vauxhall":
                    return Brand.Vauxhall;
                case "chevrolet":
                    return Brand.Chevrolet;
                case "onstar":
                    return Brand.OnStar;
                case "cadillac":
                    return Brand.Cadillac;
                case "buick":
                    return Brand.Buick;
                case "gmc":
                    return Brand.Gmc;
                default:
                    throw new InvalidOperationException("Unknown Brand");
            }
        }
    }
}
