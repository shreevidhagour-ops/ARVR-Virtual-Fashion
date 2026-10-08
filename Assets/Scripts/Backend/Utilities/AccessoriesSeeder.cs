using System.Collections.Generic;
using UnityEngine;
using FashionStylist.Backend.Core;
using FashionStylist.Backend.Models;

namespace FashionStylist.Backend.Utilities
{
    public class AccessoriesSeeder : MonoBehaviour
    {
        private string seedStatus = "Ready to seed 10 accessories.";

        private void OnGUI()
        {
            // Draws an easy seed button on screen below the Auth tester
            if (GUI.Button(new Rect(20, 460, 360, 40), "🌱 UPLOAD 10 ACCESSORIES TO CLOUD FIRESTORE"))
            {
                SeedAllAccessories();
            }

            GUI.Label(new Rect(20, 505, 360, 30), $"<b>Seeder Status:</b> {seedStatus}");
        }

        [ContextMenu("Seed Accessories Now")]
        public void SeedAllAccessories()
        {
            seedStatus = "Uploading accessories to Firestore...";
            Debug.Log("<color=yellow>[Seeder] Starting accessories upload to Cloud Firestore...</color>");

            List<Product> accessories = GetSeedProducts();

            int completedCount = 0;
            foreach (var item in accessories)
            {
                DatabaseManager.Instance.SaveProduct(item, (success, msg) =>
                {
                    completedCount++;
                    if (success)
                    {
                        Debug.Log($"<color=green>[Seeder] ({completedCount}/{accessories.Count}) Uploaded: {item.name}</color>");
                    }
                    else
                    {
                        Debug.LogError($"[Seeder] Error uploading {item.name}: {msg}");
                    }

                    if (completedCount == accessories.Count)
                    {
                        seedStatus = $"SUCCESS: All {accessories.Count} accessories live in Firestore!";
                        Debug.Log("<color=cyan><b>[Seeder] ALL ACCESSORIES SUCCESSFULLY SEEDED TO CLOUD FIRESTORE!</b></color>");
                    }
                });
            }
        }

        private List<Product> GetSeedProducts()
        {
            return new List<Product>
            {
                // 1. Pearl Drop Earrings (Matches Pastel Outfits)
                new Product
                {
                    productId = "acc_earrings_01",
                    category = "accessories",
                    subcategory = "earrings",
                    name = "Classic Pearl Drop Earrings",
                    description = "Handcrafted freshwater pearl drop earrings with silver hooks. Ideal for weddings and pastel styles.",
                    price = 45.00,
                    currency = "USD",
                    colors = new List<string> { "White", "Silver" },
                    sizes = new List<string> { "One Size" },
                    tags = new List<string> { "pearl", "earrings", "formal", "pastel", "wedding", "elegant" },
                    imageUrl = "https://images.unsplash.com/photo-1535632066927-ab7c9ab60908?w=500",
                    model3DUrl = "Accessories/Earrings_Pearl",
                    inStock = true,
                    arSupported = true,
                    arAnchorType = "face"
                },

                // 2. Gold Hoops (Minimalist Daily)
                new Product
                {
                    productId = "acc_earrings_02",
                    category = "accessories",
                    subcategory = "earrings",
                    name = "Minimalist Gold Hoops",
                    description = "Chic 18k gold plated everyday hoop earrings with secure click closure.",
                    price = 35.00,
                    currency = "USD",
                    colors = new List<string> { "Gold" },
                    sizes = new List<string> { "Medium" },
                    tags = new List<string> { "gold", "hoops", "minimalist", "casual", "chic" },
                    imageUrl = "https://images.unsplash.com/photo-1630019852942-f89202989a59?w=500",
                    model3DUrl = "Accessories/Earrings_GoldHoop",
                    inStock = true,
                    arSupported = true,
                    arAnchorType = "face"
                },

                // 3. Freshwater Pearl Choker (Pastel Companion)
                new Product
                {
                    productId = "acc_necklace_01",
                    category = "accessories",
                    subcategory = "necklaces",
                    name = "Freshwater Pearl Choker",
                    description = "Delicate pearl choker with adjustable gold clasp. Pairs harmoniously with floral and pastel dresses.",
                    price = 65.00,
                    currency = "USD",
                    colors = new List<string> { "White", "Gold" },
                    sizes = new List<string> { "Adjustable" },
                    tags = new List<string> { "pearl", "necklace", "pastel", "vintage", "party" },
                    imageUrl = "https://images.unsplash.com/photo-1599643478518-a784e5dc4c8f?w=500",
                    model3DUrl = "Accessories/Necklace_Pearl",
                    inStock = true,
                    arSupported = true,
                    arAnchorType = "face"
                },

                // 4. Geometric Silver Pendant (Black Dress Companion)
                new Product
                {
                    productId = "acc_necklace_02",
                    category = "accessories",
                    subcategory = "necklaces",
                    name = "Geometric Silver Pendant",
                    description = "Modern sterling silver geometric bar necklace with sleek chain.",
                    price = 50.00,
                    currency = "USD",
                    colors = new List<string> { "Silver" },
                    sizes = new List<string> { "18 inch" },
                    tags = new List<string> { "silver", "necklace", "modern", "formal", "cocktail" },
                    imageUrl = "https://images.unsplash.com/photo-1515562141207-7a88fb7ce338?w=500",
                    model3DUrl = "Accessories/Necklace_Silver",
                    inStock = true,
                    arSupported = true,
                    arAnchorType = "face"
                },

                // 5. Rose Gold Mesh Watch (Wrist Accessory)
                new Product
                {
                    productId = "acc_watch_01",
                    category = "accessories",
                    subcategory = "watches",
                    name = "Rose Gold Mesh Watch",
                    description = "Slim quartz watch with ultra-fine rose gold stainless steel mesh strap.",
                    price = 120.00,
                    currency = "USD",
                    colors = new List<string> { "Rose Gold" },
                    sizes = new List<string> { "36mm" },
                    tags = new List<string> { "watch", "rosegold", "elegant", "office", "formal" },
                    imageUrl = "https://images.unsplash.com/photo-1524805444758-089113d48a6d?w=500",
                    model3DUrl = "Accessories/Watch_RoseGold",
                    inStock = true,
                    arSupported = true,
                    arAnchorType = "wrist"
                },

                // 6. Classic Vintage Leather Watch
                new Product
                {
                    productId = "acc_watch_02",
                    category = "accessories",
                    subcategory = "watches",
                    name = "Vintage Tan Leather Watch",
                    description = "Classic analog watch featuring stitched genuine Italian leather band.",
                    price = 95.00,
                    currency = "USD",
                    colors = new List<string> { "Brown", "Gold" },
                    sizes = new List<string> { "40mm" },
                    tags = new List<string> { "watch", "leather", "vintage", "casual" },
                    imageUrl = "https://images.unsplash.com/photo-1522335789203-aabd1fc54bc9?w=500",
                    model3DUrl = "Accessories/Watch_Leather",
                    inStock = true,
                    arSupported = true,
                    arAnchorType = "wrist"
                },

                // 7. Retro Aviator Sunglasses
                new Product
                {
                    productId = "acc_glasses_01",
                    category = "accessories",
                    subcategory = "glasses",
                    name = "Retro Gold Aviators",
                    description = "Timeless aviator sunglasses with gold frames and gradient UV400 lenses.",
                    price = 85.00,
                    currency = "USD",
                    colors = new List<string> { "Gold", "Green Lens" },
                    sizes = new List<string> { "Standard" },
                    tags = new List<string> { "sunglasses", "aviator", "summer", "chic", "casual" },
                    imageUrl = "https://images.unsplash.com/photo-1511499767150-a48a237f0083?w=500",
                    model3DUrl = "Accessories/Glasses_Aviator",
                    inStock = true,
                    arSupported = true,
                    arAnchorType = "face"
                },

                // 8. Cat-Eye Sunglasses
                new Product
                {
                    productId = "acc_glasses_02",
                    category = "accessories",
                    subcategory = "glasses",
                    name = "Bold Cat-Eye Frames",
                    description = "Statement cat-eye sunglasses with glossy black acetate frame.",
                    price = 75.00,
                    currency = "USD",
                    colors = new List<string> { "Glossy Black" },
                    sizes = new List<string> { "Standard" },
                    tags = new List<string> { "glasses", "cateye", "bold", "party", "modern" },
                    imageUrl = "https://images.unsplash.com/photo-1572635196237-14b3f281503f?w=500",
                    model3DUrl = "Accessories/Glasses_CatEye",
                    inStock = true,
                    arSupported = true,
                    arAnchorType = "face"
                },

                // 9. Quilted Pastel Crossbody Bag
                new Product
                {
                    productId = "acc_bag_01",
                    category = "accessories",
                    subcategory = "bags",
                    name = "Quilted Pastel Pink Crossbody",
                    description = "Luxury vegan leather quilted bag with gold chain shoulder strap.",
                    price = 110.00,
                    currency = "USD",
                    colors = new List<string> { "Pastel Pink" },
                    sizes = new List<string> { "Small" },
                    tags = new List<string> { "bag", "crossbody", "pastel", "pink", "party", "date" },
                    imageUrl = "https://images.unsplash.com/photo-1584917865442-de89df76afd3?w=500",
                    model3DUrl = "Accessories/Bag_PinkCrossbody",
                    inStock = true,
                    arSupported = true,
                    arAnchorType = "hand"
                },

                // 10. Sterling Silver Tennis Bracelet
                new Product
                {
                    productId = "acc_bracelet_01",
                    category = "accessories",
                    subcategory = "bracelets",
                    name = "Sterling Silver Tennis Bracelet",
                    description = "Sparkling cubic zirconia set in 925 sterling silver with double safety lock.",
                    price = 70.00,
                    currency = "USD",
                    colors = new List<string> { "Silver" },
                    sizes = new List<string> { "7 inch" },
                    tags = new List<string> { "bracelet", "silver", "sparkle", "evening", "cocktail" },
                    imageUrl = "https://images.unsplash.com/photo-1611591475152-4735133d5945?w=500",
                    model3DUrl = "Accessories/Bracelet_Silver",
                    inStock = true,
                    arSupported = true,
                    arAnchorType = "wrist"
                }
            };
        }
    }
}