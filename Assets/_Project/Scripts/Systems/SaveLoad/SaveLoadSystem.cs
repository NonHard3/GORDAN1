using UnityEngine;

namespace Project.Systems.SaveLoad
{
    public class SaveLoadSystem
    {
        public void Save<T>(string key, T data)
        {
            string json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString(key, json);
            PlayerPrefs.Save();
        }

        public T Load<T>(string key)
        {
            if (!PlayerPrefs.HasKey(key))
                return default;

            string json = PlayerPrefs.GetString(key);
            return JsonUtility.FromJson<T>(json);
        }

        public void Delete(string key)
        {
            PlayerPrefs.DeleteKey(key);
        }
    }
}
