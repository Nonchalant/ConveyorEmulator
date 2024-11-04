using UnityEngine;

namespace Game.Model
{
    public enum Direction
    {
        Up,
        Right,
        Left,
        Down,
    }

    public abstract class DirectionController
    {
        public static Vector3Int GetOperation(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return Vector3Int.up;
                case Direction.Right:
                    return Vector3Int.right;
                case Direction.Left:
                    return Vector3Int.left;
                case Direction.Down:
                    return Vector3Int.down;
            }

            return Vector3Int.zero;
        }
        
        public static Vector3 GetPoint(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return new Vector3(0.5f, 1, 0);
                case Direction.Right:
                    return new Vector3(1, 0.5f, 0);
                case Direction.Left:
                    return new Vector3(0, 0.5f, 0);
                case Direction.Down:
                    return new Vector3(0.5f, 0, 0);
            }

            return Vector3.zero;
        }
    }
}
