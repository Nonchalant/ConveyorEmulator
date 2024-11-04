using Helper;

namespace Config
{
    public enum MineType
    {
        Diamond,
        Ruby,
        Emerald,
    }

    public static class MineTypeResources
    {
        public static ResourceKey GetKey(MineType mineType)
        {
            switch (mineType)
            {
                case MineType.Diamond:
                    return ResourceKey.Diamond;
                case MineType.Ruby:
                    return ResourceKey.Ruby; 
                case MineType.Emerald: 
                    return ResourceKey.Emerald;
            }

            return ResourceKey.Diamond;
        }
    }
}