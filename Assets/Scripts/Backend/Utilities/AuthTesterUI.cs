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
        private string statusMessage = "Ready to test checkout & wishlist.";

        private void OnGUI()
        {
            GUI.Box(new Rect(20, 15, 360, 435), "=== MEMBER 2: BACKEND & COMMERCE TESTER ===");

            GUI.Label(new Rect(30, 45, 100, 20), "Full Name:");
            fullName = GUI.TextField(new Rect(130, 45, 230, 22), fullName);

            GUI.Label(new Rect(30, 70, 100, 20), "Email:");
            email = GUI.TextField(new Rect(130, 70, 230, 22), email);

            GUI.Label(new Rect(30, 95, 100, 20), "Password:");
            password = GUI.PasswordField(new Rect(130, 95, 230, 22), password, '*');

            // Auth Buttons
            if (GUI.Button(new Rect(30, 125, 160, 28), "Sign Up"))
            {
                statusMessage = "Registering...";
                AuthManager.Instance.SignUp(email, password, fullName, (success, msg) => statusMessage = msg);
            }

            if (GUI.Button(new Rect(200, 125, 160, 28), "Sign In"))
            {
                statusMessage = "Signing in...";
                AuthManager.Instance.SignIn(email, password, (success, msg) => statusMessage = msg);
            }

            // Cart Actions
            GUI.Label(new Rect(30, 155, 330, 20), "<b>Shopping Cart & Wishlist (Accessories):</b>");

            if (GUI.Button(new Rect(30, 175, 160, 28), "🛒 + Pearl Earrings"))
            {
                Product p = new Product { productId = "acc_earrings_01", name = "Classic Pearl Drop Earrings", category = "accessories", price = 45.00 };
                CartManager.Instance.AddToCart(p, "White", "One Size", (s, m) => statusMessage = m);
            }

            if (GUI.Button(new Rect(200, 175, 160, 28), "🛒 + Rose Gold Watch"))
            {
                Product p = new Product { productId = "acc_watch_01", name = "Rose Gold Mesh Watch", category = "accessories", price = 120.00 };
                CartManager.Instance.AddToCart(p, "Rose Gold", "36mm", (s, m) => statusMessage = m);
            }

            if (GUI.Button(new Rect(30, 207, 330, 28), "❤️ Toggle Pearl Earrings in Wishlist"))
            {
                Product p = new Product { productId = "acc_earrings_01", name = "Classic Pearl Drop Earrings", category = "accessories", price = 45.00 };
                WishlistManager.Instance.ToggleWishlist(p, (s, isInWishlist) =>
                {
                    statusMessage = isInWishlist ? "Added Pearl Earrings to Wishlist ❤️" : "Removed Pearl Earrings from Wishlist 💔";
                });
            }

            if (GUI.Button(new Rect(30, 240, 160, 30), "📋 View Cart Total"))
            {
                CartManager.Instance.FetchCartItems(items =>
                {
                    double total = CartManager.Instance.CalculateTotal(items);
                    statusMessage = $"Cart has {items.Count} items! Total: ${total:F2}";
                });
            }

            // Checkout Button
            GUI.backgroundColor = new Color(0.2f, 0.8f, 0.3f);
            if (GUI.Button(new Rect(200, 240, 160, 30), "💳 Place Order (Checkout)"))
            {
                statusMessage = "Processing order...";
                CheckoutManager.Instance.PlaceOrder((success, msg) =>
                {
                    statusMessage = msg;
                });
            }
            GUI.backgroundColor = Color.white;

            // Status display
            GUI.Label(new Rect(30, 280, 330, 20), "<b>Status:</b>");
            GUI.TextArea(new Rect(30, 300, 330, 50), statusMessage);

            // Logged in indicator
            bool loggedIn = AuthManager.Instance != null && AuthManager.Instance.IsLoggedIn;
            string userStatus = loggedIn ? $"Logged in as: {AuthManager.Instance.CurrentUser.Email}" : "LOGGED OUT";
            GUI.Label(new Rect(30, 360, 330, 20), userStatus);
        }
    }
}