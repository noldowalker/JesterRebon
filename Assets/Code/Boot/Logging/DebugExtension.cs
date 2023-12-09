using UnityEngine;

namespace Code.Boot.Logging
{
    public static class DebugExtension
    {
        public static void InitNotice(string text)
        {
            PrintColoredMessage("#9ACD32", "[INIT] " + text);
        }
        
        public static void SystemNotice(string text)
        {
            PrintColoredMessage("#7FFF00", "[SYSTEM] " + text);
        }
        
        public static void DebugNotice(string text)
        {
            PrintColoredMessage("#FFD700", "[DEBUG] " + text);
        }
        
        public static void Warning(string text)
        {
            PrintColoredWarning("##FF4500", "[WARNING] " + text);
        }

        private static void PrintColoredMessage(string color, string text)
        {
            Debug.Log($"<b><color={color}>{text}</color></b>");
        }

        private static void PrintColoredWarning(string color, string text)
        {
            Debug.LogError($"<b><color={color}>{text}</color></b>");
        }
    }
}