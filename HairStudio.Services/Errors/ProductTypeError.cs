using HairStudio.Services.Common;

namespace HairStudio.Services.Errors
{
    public static class ProductTypeErrors
    {
        public static readonly Error ProductTypeNotFound = new Error(
            "ProductType.ProductTypeNotFound", "Product type not found.");

        public static readonly Error ProductTypeHasProduct = new Error(
            "ProductType.ProductTypeHasProduct", "Product type cannot be deleted because it is referenced by a product.");

        public static readonly Error ProductTypeExists = new Error(
            "ProductType.ProductTypeExists", "Product type with that name already exists.");
    }
}
