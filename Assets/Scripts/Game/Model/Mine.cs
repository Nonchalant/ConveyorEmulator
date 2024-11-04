using Config;
using Helper;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Model
{
    public class MineFactory : PlaceholderFactory<Mine> {}
    
    public class Mine : MonoBehaviour
    {
        public GameObject self;
        public MineType mineType;

        [Inject] private IResourceLoader _resourceLoader;

        public void Supply()
        {
            UpdateMineType(MineType.Diamond);
        }

        public void ConvertToRuby()
        {
            UpdateMineType(MineType.Ruby);
        }

        public void ConvertToEmerald()
        {
            if (mineType != MineType.Ruby) return;
            UpdateMineType(MineType.Emerald);
        }

        private void UpdateMineType(MineType newValue)
        {
            mineType = newValue;
            self.GetComponent<Image>().sprite = _resourceLoader.Load(MineTypeResources.GetKey(newValue));
        }
    }
}
