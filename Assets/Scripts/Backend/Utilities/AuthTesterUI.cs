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
        private string statusMessage = "Ready to test authentication & cart.";

        private void OnGUI()
        {
            GUI.Box(new Rect(20, 20, 360, 430), "=== MEMBER 2: BACKEND & COMMERCE TESTER ===");

            GUI.Label(new Rect(30, 50, 100, 20), "Full Name:");
            fullName = GUI.TextField(new Rect(130, 50, 230, 25), fullName);

            GUI.Label(new Rect(30, 80, 100, 20), "Email:");
            email = GUI.TextField(new Rect(130, 80, 230, 25), email);

            GUI.Label(new Rect(30, 110, 100, 20), "Password:");
            password = GUI.PasswordField(new Rect(130, 110, 230, 25), password, '*');

            // Auth Buttons
            if (GUI.Button(new Rect(30, 145, 160, 30), "Sign Up"))
            {
                statusMessage = "Registering...";
                AuthManager.Instance.SignUp(email, password, fullName, (success, msg) =>
                {
                    statusMessage = success ? $"[SUCCESS] {msg}" : $"[FAILED] {msg}";
                });
            }

            if (GUI.Button(new Rect(200, 145, 160, 30), "Sign In"))
            {
                statusMessage = "Signing in...";
                AuthManager.Instance.SignIn(email, password, (success, msg) =>
                {
                    statusMessage = success ? $"[SUCCESS] {msg}" : $"[FAILED] {msg}";
                });
            }

            // Cart Actions
            GUI.Label(new Rect(30, 185, 330, 20), "<b>Shopping Cart Tests (Accessories):</b>");

            if (GUI.Button(new Rect(30, 205, 330, 30), "🛒 Add Pearl Earrings ($45) to Cart"))
            {
                Product pearlEarrings = new Product
                {
                    productId = "acc_earrings_01",
                    name = "Classic Pearl Drop Earrings",
                    category = "accessories",
                    price = 45.00,
                    imageUrl = "https://images.unsplash.com/photo-1535632066927-ab7c9ab60908?w=500"
                };

                CartManager.Instance.AddToCart(pearlEarrings, "White", "One Size", (success, msg) =>
                {
                    statusMessage = msg;
                });
            }

            if (GUI.Button(new Rect(30, 240, 330, 30), "🛒 Add Rose Gold Watch ($120) to Cart"))
            {
                Product goldWatch = new Product
                {
                    productId = "acc_watch_01",
                    name = "Rose Gold Mesh Watch",
                    category = "accessories",
                    price = 120.00,
                    imageUrl = "https://images.unsplash.com/photo-1524805444758-089113d48a6d?w=500"
                };

                CartManager.Instance.AddToCart(goldWatch, "Rose Gold", "36mm", (success, msg) =>
                {
                    statusMessage = msg;
                });
            }

            if (GUI.Button(new Rect(30, 275, 330, 30), "📋 View Cart Items & Total"))
            {
                CartManager.Instance.FetchCartItems(items =>
                {
                    double total = CartManager.Instance.CalculateTotal(items);
                    statusMessage = $"Cart has {items.Count} items! Total: ${total:F2}";
                });
            }

            // Status display
            GUI.Label(new Rect(30, 315, 330, 20), "<b>Status:</b>");
            GUI.TextArea(new Rect(30, 335, 330, 50), statusMessage);

            // Logged in indicator
            bool loggedIn = AuthManager.Instance != null && AuthManager.Instance.IsLoggedIn;
            string userStatus = loggedIn ? $"Logged in as: {AuthManager.Instance.CurrentUser.Email}" : "LOGGED OUT (Please Sign In first)";
            GUI.Label(new Rect(30, 395, 330, 30), userStatus);
        }
    }
}