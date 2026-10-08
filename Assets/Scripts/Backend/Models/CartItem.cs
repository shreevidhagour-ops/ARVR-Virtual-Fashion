using Firebase.Firestore;

namespace FashionStylist.Backend.Models
{
    [FirestoreData]
    public class CartItem
    {
        [FirestoreProperty]
        public string cartItemId { get; set; }

        [FirestoreProperty]
        public string productId { get; set; }

        [FirestoreProperty]
        public string productName { get; set; }

        [FirestoreProperty]
        public string category { get; set; }

        [FirestoreProperty]
        public double price { get; set; }

        [FirestoreProperty]
        public string selectedColor { get; set; }

        [FirestoreProperty]
        public string selectedSize { get; set; }

        [FirestoreProperty]
        public int quantity { get; set; } = 1;

        [FirestoreProperty]
        public string imageUrl { get; set; }

        // Required empty constructor
        public CartItem() { }

        public CartItem(Product product, string color, string size, int qty = 1)
        {
            productId = product.productId;
            cartItemId = product.productId; // Uses productId as key in cart
            productName = product.name;
            category = product.category;
            price = product.price;
            selectedColor = string.IsNullOrEmpty(color) ? (product.colors.Count > 0 ? product.colors[0] : "Default") : color;
            selectedSize = string.IsNullOrEmpty(size) ? (product.sizes.Count > 0 ? product.sizes[0] : "One Size") : size;
            quantity = qty;
            imageUrl = product.imageUrl;
        }
    }
}