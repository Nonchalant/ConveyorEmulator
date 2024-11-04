using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using Zenject;

namespace Game.Item
{
    public class Eraser : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField] private Camera main;
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private TileBase[] tileBases;
        [SerializeField] private TileBase floorTileBase;

        private Vector2 _originalPosition;
        private RectTransform _rectTransform;
        private RectTransform _parentRectTransform;

        [Inject] private IViewModel _viewModel;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _parentRectTransform = _rectTransform.parent as RectTransform;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_viewModel.GetIsEmulating()) return;
        
            _originalPosition = _rectTransform.anchoredPosition;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_viewModel.GetIsEmulating()) return;
        
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_parentRectTransform, eventData.position, Camera.main, out var result);
            _rectTransform.anchoredPosition = result;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_viewModel.GetIsEmulating()) return;
        
            Vector3 screenPoint = main.ScreenToWorldPoint(eventData.position);
            var endPosition = new Vector3Int(Mathf.RoundToInt(screenPoint.x), Mathf.RoundToInt(screenPoint.y), 0);

            var tile = tilemap.GetTile(endPosition);
            if (tile != null && !tileBases.Contains(tile))
            {
                tilemap.SetTransformMatrix(endPosition, Matrix4x4.Rotate(Quaternion.Euler(0, 0, 0)));
                tilemap.SetTile(endPosition, floorTileBase);
            }

            _rectTransform.anchoredPosition = _originalPosition;
        }
    }
}
