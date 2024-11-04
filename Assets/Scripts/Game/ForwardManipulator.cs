using System;
using System.Collections.Generic;
using System.Linq;
using Game.Model;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game
{
    public interface IForwardManipulator
    {
        void Setup(
            Tilemap tilemap,
            TileBase straitConveyor,
            TileBase curveConveyor,
            TileBase convertor,
            GameObject original
        );

        List<int> RunByStep(List<MineState> mineStates);
    }
    
    public class ForwardManipulator : IForwardManipulator
    {
        private const int Distance = 1;

        private Tilemap _tilemap;
        private TileBase _straitConveyor;
        private TileBase _curveConveyor;
        private TileBase _convertor;

        private GameObject _original;
        
        public void Setup(
            Tilemap tilemap,
            TileBase straitConveyor,
            TileBase curveConveyor,
            TileBase convertor,
            GameObject original
        ) {
            _tilemap = tilemap;
            _straitConveyor = straitConveyor;
            _curveConveyor = curveConveyor;
            _convertor = convertor;
            _original = original;
        }

        public List<int> RunByStep(List<MineState> mineStates) {
            var removeIndices = new List<int>();
        
            foreach (var mineStateWithIndex in mineStates.Select((value, index) => new { value, index }))
            {
                var mineState = mineStateWithIndex.value;
                var index = mineStateWithIndex.index;

                Forward(mineState, () => { removeIndices.Add(index); });
            }
            
            return removeIndices;
        }
        
        private void Forward(
            MineState mineState, 
            Action remove
        ) {
            if (mineState.Item.transform.position == _original.transform.position)
            {
                var origin = new Vector3Int(
                    Mathf.RoundToInt(_original.transform.position.x - Distance / 2.0f), 
                    Mathf.RoundToInt(_original.transform.position.y - Distance / 2.0f), 
                    0);
            
                float scale = Mathf.Pow(10, 1);
                var current = new Vector3(origin.x, Mathf.Round(_original.transform.position.y * scale) / scale, 0);
            
                var forward = DirectionController.GetOperation(Direction.Right) * Distance;
                mineState.Item.transform.Translate(forward);
                mineState.Origin = origin + forward;
                mineState.Current = current + forward;
                return;
            }
        
            var tile = _tilemap.GetTile(mineState.Origin);
            var rotation = _tilemap.GetTransformMatrix(mineState.Origin);

            var point = mineState.Current - mineState.Origin;
            Direction? direction = null;

            Matrix4x4 identity1 = Matrix4x4.identity;
            Matrix4x4 identity2 = Matrix4x4.identity * Matrix4x4.Rotate(Quaternion.Euler(0, 0, 90));
            Matrix4x4 identity3 = Matrix4x4.identity * Matrix4x4.Rotate(Quaternion.Euler(0, 0, 180));
            Matrix4x4 identity4 = Matrix4x4.identity * Matrix4x4.Rotate(Quaternion.Euler(0, 0, 270));
            
            if (tile == _straitConveyor || tile == _convertor)
            {
                if (rotation == identity1 || rotation == identity3)
                {
                    if (tile == _convertor) mineState.Item.ConvertToRuby();
                    direction = GetDirection(point, Direction.Left, Direction.Right);
                }

                if (rotation == identity2 || rotation == identity4)
                {
                    if (tile == _convertor) mineState.Item.ConvertToEmerald();
                    direction = GetDirection(point, Direction.Up, Direction.Down);
                }
            }
            else if (tile == _curveConveyor)
            {
                if (rotation == identity1)
                    direction = GetDirection(point, Direction.Up, Direction.Left);

                if (rotation == identity2)
                    direction = GetDirection(point, Direction.Left, Direction.Down);
            
                if (rotation == identity3)
                    direction = GetDirection(point, Direction.Down, Direction.Right);
            
                if (rotation == identity4)
                    direction = GetDirection(point, Direction.Right, Direction.Up);
            }

            if (direction != null)
            {
                mineState.Current = mineState.Origin + DirectionController.GetPoint(direction.Value) * Distance;

                var operation = DirectionController.GetOperation(direction.Value) * Distance;
                mineState.Item.transform.Translate(operation);
                mineState.Origin += operation;
            }
            else remove();
        }

        private Direction? GetDirection(Vector3 point, Direction direction1, Direction direction2)
        {
            if (point == DirectionController.GetPoint(direction1)) 
                return direction2;
            
            if (point == DirectionController.GetPoint(direction2)) 
                return direction1;
        
            return null;
        }
    }
}