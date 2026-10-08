using UnityEngine;
using FashionStylist.Backend.Core;

namespace FashionStylist.Backend.Utilities
{
    public class AuthTesterUI : MonoBehaviour
    {
        private string email = "designer@fashion.com";
        private string password = "Password123!";
        private string fullName = "Vidhi Fashion Lead";
        private string statusMessage = "Ready to test authentication.";

        private void OnGUI()
        {
            // Create a clean testing box on screen
            GUI.Box(new Rect(20, 20, 360, 420), "=== MEMBER 2: AUTH & DATABASE TESTER ===");

            GUI.Label(new Rect(30, 55, 100, 20), "Full Name:");
            fullName = GUI.TextField(new Rect(130, 55, 230, 25), fullName);

            GUI.Label(new Rect(30, 90, 100, 20), "Email:");
            email = GUI.TextField(new Rect(130, 90, 230, 25), email);

            GUI.Label(new Rect(30, 125, 100, 20), "Password:");
            password = GUI.PasswordField(new Rect(130, 125, 230, 25), password, '*');

            // Button 1: Sign Up
            if (GUI.Button(new Rect(30, 165, 330, 35), "1. Sign Up (Create Account & Firestore Profile)"))
            {
                statusMessage = "Registering...";
                AuthManager.Instance.SignUp(email, password, fullName, (success, msg) =>
                {
                    statusMessage = success ? $"[SUCCESS] {msg}" : $"[FAILED] {msg}";
                });
            }

            // Button 2: Sign In
            if (GUI.Button(new Rect(30, 210, 330, 35), "2. Sign In"))
            {
                statusMessage = "Signing in...";
                AuthManager.Instance.SignIn(email, password, (success, msg) =>
                {
                    statusMessage = success ? $"[SUCCESS] {msg}" : $"[FAILED] {msg}";
                });
            }

            // Button 3: Sign Out
            if (GUI.Button(new Rect(30, 255, 330, 35), "3. Sign Out"))
            {
                AuthManager.Instance.SignOut();
                statusMessage = "User signed out.";
            }

            // Status display
            GUI.Label(new Rect(30, 305, 330, 20), "<b>Status:</b>");
            GUI.TextArea(new Rect(30, 325, 330, 60), statusMessage);

            // Logged in indicator
            bool loggedIn = AuthManager.Instance != null && AuthManager.Instance.IsLoggedIn;
            string userStatus = loggedIn ? $"LOGGED IN as:\n{AuthManager.Instance.CurrentUser.Email}\n(UID: {AuthManager.Instance.CurrentUserId})" : "LOGGED OUT";
            GUI.Label(new Rect(30, 390, 330, 40), userStatus);
        }
    }
}