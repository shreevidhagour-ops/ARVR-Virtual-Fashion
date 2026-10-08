using System;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Firestore;
using Firebase.Extensions;
using FashionStylist.Backend.Core;
using FashionStylist.Backend.Models;

namespace FashionStylist.Backend.Commerce
{
    [FirestoreData]
    public class OrderData
    {
        [FirestoreProperty] public string orderId { get; set; }
        [FirestoreProperty] public string userId { get; set; }
        [FirestoreProperty] public double totalAmount { get; set; }
        [FirestoreProperty] public int totalItems { get; set; }
        [FirestoreProperty] public string status { get; set; } = "Completed";
        [FirestoreProperty] public string createdAt { get; set; }

        public OrderData() { }

        public OrderData(string id, string uid, double total, int items)
        {
            orderId = id;
            userId = uid;
            totalAmount = total;
            totalItems = items;
            status = "Completed";
            createdAt = DateTime.UtcNow.ToString("o");
        }
    }

    public class CheckoutManager : MonoBehaviour
    {
        private static CheckoutManager _instance;
        public static CheckoutManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<CheckoutManager>();
                }
                return _instance;
            }
            private set => _instance = value;
        }

        private FirebaseFirestore db;

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
                db = FirebaseFirestore.DefaultInstance;
            }
            else
            {
                FirebaseInit.OnFirebaseReady += () => db = FirebaseFirestore.DefaultInstance;
            }
        }

        /// <summary>
        /// Places an order using the current cart items and empties the cart.
        /// </summary>
        public void PlaceOrder(Action<bool, string> callback)
        {
            if (!AuthManager.Instance.IsLoggedIn)
            {
                callback?.Invoke(false, "User not signed in.");
                return;
            }

            string uid = AuthManager.Instance.CurrentUserId;
            List<CartItem> cartItems = CartManager.Instance.CachedCartItems;

            if (cartItems.Count == 0)
            {
                callback?.Invoke(false, "Cannot checkout: Cart is empty!");
                return;
            }

            double total = CartManager.Instance.CalculateTotal(cartItems);
            string orderId = "ORD_" + DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");

            OrderData order = new OrderData(orderId, uid, total, cartItems.Count);

            // 1. Save order to root "orders" collection
            db.Collection("orders").Document(orderId).SetAsync(order).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    callback?.Invoke(false, $"Order failed: {task.Exception?.Message}");
                    return;
                }

                Debug.Log($"<color=green><b>[CheckoutManager] Order {orderId} placed successfully! Total: ${total:F2}</b></color>");

                // 2. Clear all items from the user's cart in Firestore
                foreach (var item in cartItems)
                {
                    db.Collection("users").Document(uid).Collection("cart").Document(item.cartItemId).DeleteAsync();
                }

                // Refresh local cart
                CartManager.Instance.FetchCartItems(null);
                callback?.Invoke(true, $"Order #{orderId} placed successfully! Total paid: ${total:F2}");
            });
        }
    }
}