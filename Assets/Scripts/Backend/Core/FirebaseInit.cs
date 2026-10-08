using Firebase;
using Firebase.Extensions;
using System;
using UnityEngine;
using static Readme;

namespace FashionStylist.Backend.Core
{
    public class FirebaseInit : MonoBehaviour
    {
        public static FirebaseInit Instance { get; private set; }

        public bool IsInitialized { get; private set; } = false;
        public static event Action OnFirebaseReady;

        private void Awake()
        {
            // Singleton pattern: ensures only one backend manager exists across scenes
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
            InitializeFirebase();
        }

        private void InitializeFirebase()
        {
            Debug.Log("[FirebaseInit] Checking Firebase dependencies...");

            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                var dependencyStatus = task.Result;

                if (dependencyStatus == DependencyStatus.Available)
                {
                    IsInitialized = true;
                    Debug.Log("<color=green><b>[FirebaseInit] SUCCESS: Firebase initialized successfully! Live cloud connection ready.</b></color>");

                    OnFirebaseReady?.Invoke();
                }
                else
                {
                    Debug.LogError($"[FirebaseInit] FAILED: Could not resolve Firebase dependencies: {dependencyStatus}");
                }
            });
        }
    }
}