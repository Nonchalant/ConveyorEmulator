using UnityEngine;

namespace Game.Model
{
    public class MineState
    {
        public readonly Mine Item;
        
        // Tilemapで起点となるマス左下の座標
        public Vector3Int Origin;
        
        // 進入方向の座標
        public Vector3 Current;

        public MineState(Mine item, Vector3Int origin, Vector3 current)
        {
            Item = item;
            Origin = origin;
            Current = current;
        }
    }
}
