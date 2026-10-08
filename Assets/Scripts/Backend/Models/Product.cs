using System.Collections.Generic;
using Firebase.Firestore;

namespace FashionStylist.Backend.Models
{
    [FirestoreData]
    public class Product
    {
        [FirestoreProperty]
        public string productId { get; set; }

        [FirestoreProperty]
        public string category { get; set; } // "accessories", "clothes", "footwear", "makeup"

        [FirestoreProperty]
        public string subcategory { get; set; } // "earrings", "glasses", "watches", "necklaces", "bags"

        [FirestoreProperty]
        public string name { get; set; }

        [FirestoreProperty]
        public string description { get; set; }

        [FirestoreProperty]
        public double price { get; set; }

        [FirestoreProperty]
        public string currency { get; set; } = "USD";

        [FirestoreProperty]
        public List<string> colors { get; set; }

        [FirestoreProperty]
        public List<string> sizes { get; set; }

        [FirestoreProperty]
        public List<string> tags { get; set; }

        [FirestoreProperty]
        public string imageUrl { get; set; }

        [FirestoreProperty]
        public string model3DUrl { get; set; }

        [FirestoreProperty]
        public bool inStock { get; set; } = true;

        [FirestoreProperty]
        public bool arSupported { get; set; } = true;

        [FirestoreProperty]
        public string arAnchorType { get; set; } // "face", "wrist", "head", "hand"

        // Required empty constructor for Firestore
        public Product()
        {
            colors = new List<string>();
            sizes = new List<string>();
            tags = new List<string>();
        }
    }
}