using System;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Firestore;
using Firebase.Extensions;
using FashionStylist.Backend.Core;
using FashionStylist.Backend.Models;

namespace FashionStylist.Backend.Commerce
{
    public class WishlistManager : MonoBehaviour
    {
        private static WishlistManager _instance;
        public static WishlistManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<WishlistManager>();
                }
                return _instance;
            }
            private set => _instance = value;
        }

        private FirebaseFirestore db;
        public HashSet<string> WishlistProductIds { get; private set; } = new HashSet<string>();

        public event Action OnWishlistUpdated;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this);
                return;
            }

            _instance = this;
        }

        private void Start()
        {
            if (FirebaseInit.Instance != null && FirebaseInit.Instance.IsInitialized)
            {
                InitWishlist();
            }
            else
            {
                FirebaseInit.OnFirebaseReady += InitWishlist;
            }
        }

        private void OnDestroy()
        {
            FirebaseInit.OnFirebaseReady -= InitWishlist;
        }

        private void InitWishlist()
        {
            db = FirebaseFirestore.DefaultInstance;
        }

        /// <summary>
        /// Toggles a product in/out of the user's wishlist.
        /// </summary>
        public void ToggleWishlist(Product product, Action<bool, bool> callback)
        {
            if (!AuthManager.Instance.IsLoggedIn)
            {
                callback?.Invoke(false, false);
                return;
            }

            string uid = AuthManager.Instance.CurrentUserId;
            DocumentReference docRef = db.Collection("users").Document(uid).Collection("wishlist").Document(product.productId);

            if (WishlistProductIds.Contains(product.productId))
            {
                // Remove from wishlist
                docRef.DeleteAsync().ContinueWithOnMainThread(task =>
                {
                    WishlistProductIds.Remove(product.productId);
                    OnWishlistUpdated?.Invoke();
                    Debug.Log($"<color=yellow>[Wishlist] Removed: {product.name}</color>");
                    callback?.Invoke(true, false); // success: true, isNowInWishlist: false
                });
            }
            else
            {
                // Add to wishlist
                docRef.SetAsync(product).ContinueWithOnMainThread(task =>
                {
                    WishlistProductIds.Add(product.productId);
                    OnWishlistUpdated?.Invoke();
                    Debug.Log($"<color=green>[Wishlist] Added: {product.name}</color>");
                    callback?.Invoke(true, true); // success: true, isNowInWishlist: true
                });
            }
        }

        /// <summary>
        /// Fetches all wishlist products from Firestore.
        /// </summary>
        public void FetchWishlist(Action<List<Product>> callback)
        {
            if (!AuthManager.Instance.IsLoggedIn)
            {
                callback?.Invoke(new List<Product>());
                return;
            }

            string uid = AuthManager.Instance.CurrentUserId;

            db.Collection("users").Document(uid).Collection("wishlist").GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    callback?.Invoke(new List<Product>());
                    return;
                }

                List<Product> products = new List<Product>();
                WishlistProductIds.Clear();

                foreach (DocumentSnapshot doc in task.Result.Documents)
                {
                    if (doc.Exists)
                    {
                        Product p = doc.ConvertTo<Product>();
                        products.Add(p);
                        WishlistProductIds.Add(p.productId);
                    }
                }

                OnWishlistUpdated?.Invoke();
                Debug.Log($"<color=cyan>[Wishlist] Loaded {products.Count} wishlist items.</color>");
                callback?.Invoke(products);
            });
        }
    }
}