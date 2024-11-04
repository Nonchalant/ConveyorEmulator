using System.Linq;
using System.Collections.Generic;
using Config;
using Game.Model;
using Helper;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Zenject;

namespace Game
{
    public class Controller : MonoBehaviour
    {
        [SerializeField] private Camera main;
            
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private TileBase straitConveyor;
        [SerializeField] private TileBase curveConveyor;
        [SerializeField] private TileBase convertor;
        [SerializeField] private TileBase goal;
        
        [SerializeField] private TileBase[] fixedTileBases;
        [SerializeField] private TileBase floorTileBase;
        
        [SerializeField] private Canvas canvas;
        [SerializeField] private GameObject original;

        [SerializeField] private Button runButton;
        
        [SerializeField] private GameObject target;
        [SerializeField] private TextMeshProUGUI remainText;
        [SerializeField] private TextMeshProUGUI timerText;
        
        [SerializeField] private GameObject resultOverlay;
        [SerializeField] private TextMeshProUGUI resultText;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button titleButton;
        
        [Inject] private IResourceLoader _resourceLoader;
        [Inject] private MineFactory _mineFactory;
        [Inject] private IForwardManipulator _forwardManipulator;
        [Inject] private IViewModel _viewModel;
        
        private List<MineState> _mineStates = new();

        private void Start()
        {
            // Game
            {
                _viewModel.IsEmulating
                    .Subscribe(isEmulating =>
                    {
                        runButton.GetComponent<Image>().sprite = isEmulating
                            ? _resourceLoader.Load(ResourceKey.Stop)
                            : _resourceLoader.Load(ResourceKey.Run);
                    })
                    .AddTo(this);
                
                _viewModel.RotateTile
                    .Subscribe(RotateTile)
                    .AddTo(this);
                
                _viewModel.EraseTile
                    .Subscribe(EraseTile)
                    .AddTo(this);

                _viewModel.Supply
                    .Subscribe(_ => Supply())
                    .AddTo(this);
                
                _viewModel.Forward
                    .Subscribe(_ => Forward())
                    .AddTo(this);
                
                _viewModel.Reset
                    .Subscribe(_ => Reset())
                    .AddTo(this);
                
                this.UpdateAsObservable()
                    .Where(_ => Input.GetMouseButtonDown(0))
                    .Subscribe(_ => { _viewModel.InputLeftClick(Input.mousePosition); });
                
                this.UpdateAsObservable()
                    .Where(_ => Input.GetMouseButtonDown(1))
                    .Subscribe(_ => { _viewModel.InputRightClick(Input.mousePosition); });
                
                runButton.onClick.AddListener(_viewModel.OnClickRun);
                
                _forwardManipulator.Setup(
                    tilemap, 
                    straitConveyor, 
                    curveConveyor, 
                    convertor, 
                    original
                );
            }
            
            // Score
            {
                _viewModel.Target
                    .Subscribe(mineType =>
                    {
                        target.GetComponent<Image>().sprite = _resourceLoader.Load(MineTypeResources.GetKey(mineType));
                    })
                    .AddTo(this);
                
                _viewModel.RemainText
                    .Subscribe(text => { remainText.text = text; })
                    .AddTo(this);
                
                _viewModel.TimerText
                    .Subscribe(text => { timerText.text = text; })
                    .AddTo(this);
            }
            
            // Result
            {
                _viewModel.IsResultOverlayActive
                    .Subscribe(isActive => { resultOverlay.SetActive(isActive); })
                    .AddTo(this);
                
                _viewModel.ResultText
                    .Subscribe(text => { resultText.text = text; })
                    .AddTo(this);

                this.FixedUpdateAsObservable()
                    .Where(_ => Input.GetKey(KeyCode.Escape))
                    .Subscribe(_ => { _viewModel.InputEscape(); });
                
                continueButton.onClick.AddListener(_viewModel.OnClickContinue);
                titleButton.onClick.AddListener(_viewModel.OnClickTitle);
            }
            
            _viewModel.Start();
        }

        private void RotateTile(Vector3 position)
        {
            Vector3 screenPoint = main.ScreenToWorldPoint(position);
            var endPosition = new Vector3Int(Mathf.RoundToInt(screenPoint.x), Mathf.RoundToInt(screenPoint.y), 0);

            var tile = tilemap.GetTile(endPosition);

            if (tile && !fixedTileBases.Contains(tile))
            {
                Matrix4x4 rotation = Matrix4x4.Rotate(Quaternion.Euler(0, 0, 90));
                tilemap.SetTransformMatrix(endPosition, tilemap.GetTransformMatrix(endPosition) * rotation);
            }
        }

        private void EraseTile(Vector3 position)
        {
            Vector3 screenPoint = main.ScreenToWorldPoint(position);
            var endPosition = new Vector3Int(Mathf.RoundToInt(screenPoint.x), Mathf.RoundToInt(screenPoint.y), 0);

            var tile = tilemap.GetTile(endPosition);
            
            if (tile && !fixedTileBases.Contains(tile))
            {
                tilemap.SetTransformMatrix(endPosition, Matrix4x4.Rotate(Quaternion.Euler(0, 0, 0)));
                tilemap.SetTile(endPosition, floorTileBase);
            }
        }
    
        private void Supply()
        {
            var next = _mineFactory.Create();
            next.transform.SetParent(canvas.transform, false);
            next.transform.position = original.transform.position;
            next.Supply();
            _mineStates.Add(new MineState(next, Vector3Int.zero, Vector3Int.zero));
        }

        private void Forward()
        {
            var removeIndices = _forwardManipulator.RunByStep(_mineStates);
            var goalPosition = FindGoalPosition();

            foreach (var index in removeIndices)
            {
                var mineState = _mineStates[index];

                if (mineState.Origin == goalPosition)
                    _viewModel.ReachGoal(mineState.Item.mineType);
                
                Destroy(mineState.Item.self);
                _mineStates.RemoveAt(index);
            }
        }

        private void Reset()
        {
            foreach (var mineState in _mineStates) 
                Destroy(mineState.Item.self);
            
            _mineStates = new List<MineState>();
        }

        private Vector3 FindGoalPosition()
        {
            BoundsInt bounds = tilemap.cellBounds;
            
            foreach (Vector3Int position in bounds.allPositionsWithin)
            {
                if (tilemap.GetTile(position) == goal)
                    return position;
            }

            return Vector3.zero;
        }
    }
}