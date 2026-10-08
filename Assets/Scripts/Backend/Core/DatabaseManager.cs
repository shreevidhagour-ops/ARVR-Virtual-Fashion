using System;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Firestore;
using Firebase.Extensions;
using FashionStylist.Backend.Models;

namespace FashionStylist.Backend.Core
{
    public class DatabaseManager : MonoBehaviour
    {
        public static DatabaseManager Instance { get; private set; }

        private FirebaseFirestore db;

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
            if (FirebaseInit.Instance != null && FirebaseInit.Instance.IsInitialized)
            {
                InitDatabase();
            }
            else
            {
                FirebaseInit.OnFirebaseReady += InitDatabase;
            }
        }

        private void OnDestroy()
        {
            FirebaseInit.OnFirebaseReady -= InitDatabase;
        }

        private void InitDatabase()
        {
            db = FirebaseFirestore.DefaultInstance;
            Debug.Log("<color=cyan>[DatabaseManager] Cloud Firestore connected.</color>");
        }

        /// <summary>
        /// Fetches all products belonging to a category (e.g. "accessories", "clothes").
        /// </summary>
        public void GetProductsByCategory(string category, Action<List<Product>> callback)
        {
            if (db == null)
            {
                Debug.LogError("[DatabaseManager] Database not initialized.");
                callback?.Invoke(new List<Product>());
                return;
            }

            db.Collection("products")
              .WhereEqualTo("category", category)
              .GetSnapshotAsync()
              .ContinueWithOnMainThread(task =>
              {
                  if (task.IsFaulted)
                  {
                      Debug.LogError($"[DatabaseManager] Failed to fetch {category}: {task.Exception?.Message}");
                      callback?.Invoke(new List<Product>());
                      return;
                  }

                  List<Product> products = new List<Product>();
                  foreach (DocumentSnapshot doc in task.Result.Documents)
                  {
                      if (doc.Exists)
                      {
                          Product p = doc.ConvertTo<Product>();
                          products.Add(p);
                      }
                  }

                  Debug.Log($"<color=green>[DatabaseManager] Loaded {products.Count} {category} products from Firestore.</color>");
                  callback?.Invoke(products);
              });
        }

        /// <summary>
        /// Uploads or updates a product in Firestore.
        /// </summary>
        public void SaveProduct(Product product, Action<bool, string> callback)
        {
            if (db == null)
            {
                callback?.Invoke(false, "Database not initialized.");
                return;
            }

            DocumentReference docRef = db.Collection("products").Document(product.productId);
            docRef.SetAsync(product).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    callback?.Invoke(false, task.Exception?.Message);
                }
                else
                {
                    callback?.Invoke(true, $"Product {product.productId} saved.");
                }
            });
        }
    }
}