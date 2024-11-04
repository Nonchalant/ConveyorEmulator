using UnityEngine;

namespace Helper
{
    public interface IPlayerPrefsManager
    {
        int GetInt(PlayerPrefsKey key, int defaultValue);
        void SetInt(PlayerPrefsKey key, int value);
    }

    public class PlayerPrefsManager : IPlayerPrefsManager 
    {
        public int GetInt(PlayerPrefsKey key, int defaultValue)
        {
            return PlayerPrefs.GetInt(EnumHelper.GetDescription(key), defaultValue);
        }

        public void SetInt(PlayerPrefsKey key, int value)
        { 
            PlayerPrefs.SetInt(EnumHelper.GetDescription(key), value);
        }
    }
}