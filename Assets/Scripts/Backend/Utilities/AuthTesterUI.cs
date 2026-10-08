using System.Collections.Generic;
using UnityEngine;
using FashionStylist.Backend.Core;
using FashionStylist.Backend.Commerce;
using FashionStylist.Backend.Models;

namespace FashionStylist.Backend.Utilities
{
    public class AuthTesterUI : MonoBehaviour
    {
        private string email = "vidhi@fashion.com";
        private string password = "Password123!";
        private string fullName = "Vidhi Fashion Lead";
        private string statusMessage = "Ready. Click 'Load All 10 Accessories' below!";

        private List<Product> loadedAccessories = new List<Product>();
        private Vector2 scrollPosition = Vector2.zero;

        private void Start()
        {
            // Auto-load accessories once Firebase is initialized
            FirebaseInit.OnFirebaseReady += LoadAccessories;
        }

        private void OnDestroy()
        {
            FirebaseInit.OnFirebaseReady -= LoadAccessories;
        }

        private void LoadAccessories()
        {
            if (DatabaseManager.Instance != null)
            {
                DatabaseManager.Instance.GetProductsByCategory("accessories", products =>
                {
                    loadedAccessories = products;
                    statusMessage = $"Loaded {products.Count} accessories from Cloud Firestore!";
                });
            }
        }

        private void OnGUI()
        {
            // Left Panel: Authentication & Cart Summary
            GUI.Box(new Rect(20, 15, 340, 480), "=== AUTH & CART CONTROLS ===");

            GUI.Label(new Rect(30, 45, 80, 20), "Name:");
            fullName = GUI.TextField(new Rect(110, 45, 230, 22), fullName);

            GUI.Label(new Rect(30, 70, 80, 20), "Email:");
            email = GUI.TextField(new Rect(110, 70, 230, 22), email);

            GUI.Label(new Rect(30, 95, 80, 20), "Password:");
            password = GUI.PasswordField(new Rect(110, 95, 230, 22), password, '*');

            if (GUI.Button(new Rect(30, 125, 150, 28), "Sign Up"))
            {
                AuthManager.Instance.SignUp(email, password, fullName, (s, m) => statusMessage = m);
            }

            if (GUI.Button(new Rect(190, 125, 150, 28), "Sign In"))
            {
                AuthManager.Instance.SignIn(email, password, (s, m) => statusMessage = m);
            }

            // Cart & Checkout
            GUI.Label(new Rect(30, 160, 320, 20), "<b>Shopping Cart Actions:</b>");

            if (GUI.Button(new Rect(30, 185, 310, 28), "📋 Refresh Cart & Calculate Total"))
            {
                CartManager.Instance.FetchCartItems(items =>
                {
                    double total = CartManager.Instance.CalculateTotal(items);
                    statusMessage = $"Cart has {items.Count} items! Total: ${total:F2}";
                });
            }

            GUI.backgroundColor = new Color(0.2f, 0.8f, 0.3f);
            if (GUI.Button(new Rect(30, 220, 310, 32), "💳 Place Order (Checkout)"))
            {
                CheckoutManager.Instance.PlaceOrder((s, msg) => statusMessage = msg);
            }
            GUI.backgroundColor = Color.white;

            if (GUI.Button(new Rect(30, 260, 310, 28), "🔄 Fetch 10 Accessories from Cloud"))
            {
                LoadAccessories();
            }

            // Status display
            GUI.Label(new Rect(30, 300, 310, 20), "<b>Status:</b>");
            GUI.TextArea(new Rect(30, 320, 310, 70), statusMessage);

            bool loggedIn = AuthManager.Instance != null && AuthManager.Instance.IsLoggedIn;
            string userStatus = loggedIn ? $"Logged in as: {AuthManager.Instance.CurrentUser.Email}" : "LOGGED OUT";
            GUI.Label(new Rect(30, 400, 310, 20), userStatus);


            // Right Panel: Scrollable Dynamic Accessories Catalog (All 10 Items!)
            GUI.Box(new Rect(380, 15, 450, 480), $"=== ACCESSORIES CATALOGUE ({loadedAccessories.Count} Products) ===");

            if (loadedAccessories.Count == 0)
            {
                GUI.Label(new Rect(400, 60, 400, 30), "Click 'Fetch 10 Accessories from Cloud' to load.");
            }
            else
            {
                scrollPosition = GUI.BeginScrollView(new Rect(390, 45, 430, 435), scrollPosition, new Rect(0, 0, 410, loadedAccessories.Count * 75));

                for (int i = 0; i < loadedAccessories.Count; i++)
                {
                    Product item = loadedAccessories[i];
                    int y = i * 75;

                    GUI.Box(new Rect(0, y, 410, 70), "");

                    // Product title and price
                    GUI.Label(new Rect(10, y + 5, 260, 20), $"<b>{item.name}</b>");
                    GUI.Label(new Rect(10, y + 25, 260, 20), $"<color=green>${item.price:F2}</color> | <i>{item.subcategory}</i> (Anchor: {item.arAnchorType})");
                    GUI.Label(new Rect(10, y + 45, 260, 20), $"Colors: {string.Join(", ", item.colors)}");

                    // Add to Cart Button for this specific item
                    if (GUI.Button(new Rect(275, y + 8, 125, 26), "🛒 Add to Cart"))
                    {
                        CartManager.Instance.AddToCart(item, item.colors.Count > 0 ? item.colors[0] : "Default", "One Size", (s, m) =>
                        {
                            statusMessage = m;
                        });
                    }

                    // Wishlist Button for this specific item
                    if (GUI.Button(new Rect(275, y + 38, 125, 24), "❤️ Wishlist"))
                    {
                        WishlistManager.Instance.ToggleWishlist(item, (s, inWishlist) =>
                        {
                            statusMessage = inWishlist ? $"Added {item.name} to Wishlist!" : $"Removed {item.name} from Wishlist.";
                        });
                    }
                }

                GUI.EndScrollView();
            }
        }
    }
}