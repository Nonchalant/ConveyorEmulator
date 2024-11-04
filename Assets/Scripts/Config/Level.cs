using System.ComponentModel;
using UnityEngine;
using UnityEngine.Serialization;

namespace Config
{
    public enum LevelType
    {
        [Description("Level1")]
        Level1,
        
        [Description("Level2")]
        Level2,
        
        [Description("Level3")]
        Level3
    }
    
    [CreateAssetMenu(fileName = "Data", menuName = "LevelParamAsset/Level")]
    public class Level : ScriptableObject
    {
        [SerializeField] public MineType target;
        
        [SerializeField] public int remain;
        [SerializeField] public int timer;

        public const int BestRemain = 0;
        [SerializeField] public int bestTimer;
        
        [SerializeField] private LevelType levelType;

        public static Level Load(LevelType levelType)
        {
            return Resources.Load($"Level/{Helper.EnumHelper.GetDescription(levelType)}") as Level;
        }
    }
}