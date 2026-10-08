using System;
using System.Collections.Generic;
using Firebase.Firestore;

namespace FashionStylist.Backend.Models
{
    [FirestoreData]
    public class UserData
    {
        [FirestoreProperty]
        public string userId { get; set; }

        [FirestoreProperty]
        public string email { get; set; }

        [FirestoreProperty]
        public string fullName { get; set; }

        [FirestoreProperty]
        public string createdAt { get; set; }

        [FirestoreProperty]
        public string preferredStyle { get; set; }

        [FirestoreProperty]
        public List<string> favoriteColors { get; set; }

        [FirestoreProperty]
        public string genderPreference { get; set; }

        // Required empty constructor for Firestore deserialization
        public UserData()
        {
            favoriteColors = new List<string>();
        }

        public UserData(string id, string userEmail, string name)
        {
            userId = id;
            email = userEmail;
            fullName = name;
            createdAt = DateTime.UtcNow.ToString("o");
            preferredStyle = "Casual Chic"; // Default style
            favoriteColors = new List<string> { "Pastel Pink", "Beige", "Black" };
            genderPreference = "Unspecified";
        }
    }
}