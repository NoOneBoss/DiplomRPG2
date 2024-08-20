using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Other
{
    public class PasswordAdvisor : MonoBehaviour
    {
        public enum PasswordScore
        {
            Blank = 0,
            VeryWeak = 1,
            Weak = 2,
            Medium = 3,
            Strong = 4,
            VeryStrong = 5
        }

        private static List<string> blacklistedPasswords;

        void Start()
        {
            LoadBlacklistFromFile();
        }

        private void LoadBlacklistFromFile()
        {
            blacklistedPasswords = new List<string>();

            TextAsset blacklistFile = Resources.Load<TextAsset>("500-worst-passwords");
            if (blacklistFile != null)
            {
                string[] lines = blacklistFile.text.Split('\n');
                foreach (string line in lines)
                {
                    string trimmedLine = line.Trim();
                    if (!string.IsNullOrEmpty(trimmedLine))
                    {
                        blacklistedPasswords.Add(trimmedLine);
                    }
                }

                Debug.Log($"Loaded {blacklistedPasswords.Count} blacklist passwords!");
            }
            else
            {
                Debug.LogError("Failed to load password blacklist file.");
            }
        }

        public static PasswordScore CheckStrength(string password)
        {
            int score = 1;

            if (password.Length < 1)
                return PasswordScore.Blank;
            if (password.Length < 4)
                return PasswordScore.VeryWeak;

            if (password.Length >= 8)
                score++;
            if (password.Length >= 12)
                score++;
            if (Regex.IsMatch(password, @"\d"))
                score++;
            if (Regex.IsMatch(password, "[a-z]") && Regex.IsMatch(password, "[A-Z]"))
                score++;
            if (Regex.IsMatch(password, "[а-я]", RegexOptions.IgnoreCase) || Regex.IsMatch(password, "[А-Я]", RegexOptions.IgnoreCase))
                score++;
            if (Regex.IsMatch(password, @"[!@#$%^&*?_~\-£()]", RegexOptions.IgnoreCase))
                score++;

            if (IsBlacklisted(password))
                return PasswordScore.VeryWeak;

            return (PasswordScore)score;
        }

        private static bool IsBlacklisted(string password)
        {
            if (blacklistedPasswords == null)
            {
                Debug.LogError("Blacklist not loaded. Make sure the blacklist file is in the correct location.");
                return false;
            }

            foreach (string blacklistedPassword in blacklistedPasswords)
            {
                if (password.Equals(blacklistedPassword, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }
    }
}
