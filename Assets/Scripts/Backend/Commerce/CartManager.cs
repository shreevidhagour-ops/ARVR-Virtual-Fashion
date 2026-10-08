using System;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Firestore;
using Firebase.Extensions;
using FashionStylist.Backend.Core;
using FashionStylist.Backend.Models;

namespace FashionStylist.Backend.Commerce
{
    public class CartManager : MonoBehaviour
    {
        private static CartManager _instance;
        public static CartManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<CartManager>();
                }
                return _instance;
            }
            private set => _instance = value;
        }

        private FirebaseFirestore db;
        public List<CartItem> CachedCartItems { get; private set; } = new List<CartItem>();

        public event Action<List<CartItem>> OnCartUpdated;

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
                InitCart();
            }
            else
            {
                FirebaseInit.OnFirebaseReady += InitCart;
            }
        }

        private void OnDestroy()
        {
            FirebaseInit.OnFirebaseReady -= InitCart;
        }

        private void InitCart()
        {
            db = FirebaseFirestore.DefaultInstance;
            Debug.Log("<color=cyan>[CartManager] Cart service ready.</color>");
        }

        /// <summary>
        /// Adds a product to the current user's cart in Cloud Firestore: users/{userId}/cart/{productId}
        /// </summary>
        public void AddToCart(Product product, string color, string size, Action<bool, string> callback)
        {
            if (!AuthManager.Instance.IsLoggedIn)
            {
                callback?.Invoke(false, "User must be logged in to add to cart.");
                return;
            }

            string uid = AuthManager.Instance.CurrentUserId;
            CartItem item = new CartItem(product, color, size, 1);

            DocumentReference docRef = db.Collection("users").Document(uid).Collection("cart").Document(item.cartItemId);

            docRef.SetAsync(item).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError($"[CartManager] Failed to add item to cart: {task.Exception?.Message}");
                    callback?.Invoke(false, task.Exception?.Message);
                }
                else
                {
                    Debug.Log($"<color=green>[CartManager] Added to cart: {item.productName} for user: {uid}</color>");
                    FetchCartItems(null); // Refresh cache
                    callback?.Invoke(true, $"Added {item.productName} to cart!");
                }
            });
        }

        /// <summary>
        /// Fetches all items in the user's cart from Firestore.
        /// </summary>
        public void FetchCartItems(Action<List<CartItem>> callback)
        {
            if (!AuthManager.Instance.IsLoggedIn)
            {
                callback?.Invoke(new List<CartItem>());
                return;
            }

            string uid = AuthManager.Instance.CurrentUserId;

            db.Collection("users").Document(uid).Collection("cart").GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError($"[CartManager] Failed to fetch cart: {task.Exception?.Message}");
                    callback?.Invoke(new List<CartItem>());
                    return;
                }

                List<CartItem> items = new List<CartItem>();
                foreach (DocumentSnapshot doc in task.Result.Documents)
                {
                    if (doc.Exists)
                    {
                        items.Add(doc.ConvertTo<CartItem>());
                    }
                }

                CachedCartItems = items;
                OnCartUpdated?.Invoke(items);
                Debug.Log($"<color=cyan>[CartManager] Cart refreshed: {items.Count} items. Total: ${CalculateTotal(items):F2}</color>");
                callback?.Invoke(items);
            });
        }

        /// <summary>
        /// Removes an item from the cart.
        /// </summary>
        public void RemoveFromCart(string cartItemId, Action<bool, string> callback)
        {
            if (!AuthManager.Instance.IsLoggedIn) return;

            string uid = AuthManager.Instance.CurrentUserId;
            DocumentReference docRef = db.Collection("users").Document(uid).Collection("cart").Document(cartItemId);

            docRef.DeleteAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    callback?.Invoke(false, task.Exception?.Message);
                }
                else
                {
                    Debug.Log($"[CartManager] Removed item {cartItemId} from cart.");
                    FetchCartItems(null);
                    callback?.Invoke(true, "Item removed.");
                }
            });
        }

        public double CalculateTotal(List<CartItem> items)
        {
            double total = 0;
            foreach (var item in items)
            {
                total += item.price * item.quantity;
            }
            return total;
        }
    }
}