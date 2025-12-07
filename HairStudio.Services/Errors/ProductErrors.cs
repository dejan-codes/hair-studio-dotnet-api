using HairStudio.Services.Common;

namespace HairStudio.Services.Errors
{
    public static class ProductErrors
    {
        public static readonly Error ProductNotFound = new Error(
            "Product.ProductNotFound", "Product not found.");
    }
}
