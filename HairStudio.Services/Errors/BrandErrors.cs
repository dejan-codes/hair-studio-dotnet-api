using HairStudio.Services.Common;

namespace HairStudio.Services.Errors
{
    public static class BrandErrors
    {
        public static readonly Error BrandNotFound = new Error(
            "Brand.BrandNotFound", "Brand not found.");

        public static readonly Error BrandHasProduct = new Error(
            "Brand.BrandHasProduct", "Brand cannot be deleted because it is referenced by a product.");

        public static readonly Error BrandExists = new Error(
            "Brand.BrandExists", "Brand with that name already exists.");
    }
}
