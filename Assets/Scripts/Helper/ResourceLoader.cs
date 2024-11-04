using UnityEngine;

namespace Helper
{
    public interface IResourceLoader
    {
        Sprite Load(ResourceKey key);
    }

    public class ResourceLoader : IResourceLoader 
    {
        public Sprite Load(ResourceKey key)
        {
            return Resources.Load<Sprite>(EnumHelper.GetDescription(key));
        }
    }
}