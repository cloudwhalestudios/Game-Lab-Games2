#define PLAYER_PREFERENCE_MANAGER
using UnityEngine;


namespace PlayerPreferences
{
    public class PlayerPreferenceManager : MonoBehaviour
    {
        public static T Load<T>(params string[] identifiers)
        {
            var typeName = typeof(T).Name;
            var storeName = typeName + ((identifiers != null) ? string.Concat(identifiers) : "");

            var templateJson = PlayerPrefs.GetString(typeName, "{}");

            Debug.Log($"Loading data for {storeName}: {templateJson}");

            return JsonUtility.FromJson<T>(templateJson);
        }

        public static void Save<T>(T template, params string[] identifiers)
        {
            var typeName = typeof(T).Name;
            var storeName = typeName + ((identifiers != null) ? string.Concat(identifiers) : "");

            var templateJson = JsonUtility.ToJson(template);

            PlayerPrefs.SetString(storeName, templateJson);
            PlayerPrefs.Save();

            Debug.Log($"Saving data under {storeName}: {templateJson}");
        }
    }
}