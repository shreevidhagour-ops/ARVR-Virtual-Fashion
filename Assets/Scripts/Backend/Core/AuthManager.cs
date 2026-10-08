using FashionStylist.Backend.Models;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Readme;

namespace FashionStylist.Backend.Core
{
    public class AuthManager : MonoBehaviour
    {
        public static AuthManager Instance { get; private set; }

        private FirebaseAuth auth;
        private FirebaseFirestore db;

        public FirebaseUser CurrentUser => auth?.CurrentUser;
        public bool IsLoggedIn => CurrentUser != null;
        public string CurrentUserId => CurrentUser?.UserId;

        public event Action<FirebaseUser> OnUserSignedIn;
        public event Action OnUserSignedOut;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            // Wait for FirebaseInit or initialize directly if ready
            if (FirebaseInit.Instance != null && FirebaseInit.Instance.IsInitialized)
            {
                InitAuthServices();
            }
            else
            {
                FirebaseInit.OnFirebaseReady += InitAuthServices;
            }
        }

        private void OnDestroy()
        {
            FirebaseInit.OnFirebaseReady -= InitAuthServices;
        }

        private void InitAuthServices()
        {
            auth = FirebaseAuth.DefaultInstance;
            db = FirebaseFirestore.DefaultInstance;

            auth.StateChanged += AuthStateChanged;
            Debug.Log("<color=cyan>[AuthManager] Auth & Firestore services initialized.</color>");

            if (CurrentUser != null)
            {
                Debug.Log($"<color=cyan>[AuthManager] Existing session detected for: {CurrentUser.Email}</color>");
            }
        }

        private void AuthStateChanged(object sender, EventArgs e)
        {
            if (auth.CurrentUser != null)
            {
                OnUserSignedIn?.Invoke(auth.CurrentUser);
            }
            else
            {
                OnUserSignedOut?.Invoke();
            }
        }

        /// <summary>
        /// Registers a new user with Email, Password, and Full Name.
        /// Automatically creates their profile in Cloud Firestore.
        /// </summary>
        public void SignUp(string email, string password, string fullName, Action<bool, string> onComplete)
        {
            if (auth == null)
            {
                onComplete?.Invoke(false, "Firebase Auth is not ready yet.");
                return;
            }

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                onComplete?.Invoke(false, "Email and Password cannot be empty.");
                return;
            }

            Debug.Log($"[AuthManager] Registering user: {email}...");

            auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    onComplete?.Invoke(false, "Registration was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    string error = GetFirebaseErrorMessage(task.Exception);
                    Debug.LogError($"[AuthManager] Sign up failed: {error}");
                    onComplete?.Invoke(false, error);
                    return;
                }

                AuthResult result = task.Result;
                FirebaseUser newUser = result.User;
                Debug.Log($"[AuthManager] Auth created: {newUser.UserId}. Creating Firestore profile...");

                // Create initial user document in Firestore: users/{userId}
                CreateUserProfileInFirestore(newUser.UserId, email, fullName, (profileSuccess, profileMsg) =>
                {
                    if (profileSuccess)
                    {
                        onComplete?.Invoke(true, $"Account successfully created for {email}!");
                    }
                    else
                    {
                        onComplete?.Invoke(true, $"Account created, but profile sync had an issue: {profileMsg}");
                    }
                });
            });
        }

        /// <summary>
        /// Signs in an existing user with Email and Password.
        /// </summary>
        public void SignIn(string email, string password, Action<bool, string> onComplete)
        {
            if (auth == null)
            {
                onComplete?.Invoke(false, "Firebase Auth is not ready yet.");
                return;
            }

            Debug.Log($"[AuthManager] Signing in: {email}...");

            auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    string error = GetFirebaseErrorMessage(task.Exception);
                    Debug.LogError($"[AuthManager] Sign in failed: {error}");
                    onComplete?.Invoke(false, error);
                    return;
                }

                FirebaseUser user = task.Result.User;
                Debug.Log($"<color=green>[AuthManager] Signed in successfully as: {user.Email} (UID: {user.UserId})</color>");
                onComplete?.Invoke(true, $"Welcome back, {user.Email}!");
            });
        }

        /// <summary>
        /// Signs out the current user session.
        /// </summary>
        public void SignOut()
        {
            if (auth != null && auth.CurrentUser != null)
            {
                Debug.Log($"[AuthManager] Signing out user: {auth.CurrentUser.Email}");
                auth.SignOut();
            }
        }

        private void CreateUserProfileInFirestore(string userId, string email, string fullName, Action<bool, string> callback)
        {
            UserData profile = new UserData(userId, email, fullName);
            DocumentReference docRef = db.Collection("users").Document(userId);

            docRef.SetAsync(profile).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError($"[AuthManager] Failed to create Firestore user profile: {task.Exception?.Message}");
                    callback?.Invoke(false, task.Exception?.Message);
                    return;
                }

                Debug.Log($"<color=green>[AuthManager] User profile created in Firestore: users/{userId}</color>");
                callback?.Invoke(true, "Profile created successfully.");
            });
        }

        private string GetFirebaseErrorMessage(AggregateException exception)
        {
            if (exception == null) return "Unknown error occurred.";
            var baseException = exception.GetBaseException();
            return baseException.Message;
        }
    }
}