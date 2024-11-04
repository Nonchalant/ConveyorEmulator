using System;
using System.Threading;
using System.Threading.Tasks;
using Application;
using Config;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Zenject;

namespace Game
{
    public interface IViewModel
    {
        void Start();
        
        void InputLeftClick(Vector3 position);
        void InputRightClick(Vector3 position);
        void InputEscape();

        void ReachGoal(MineType mineType);

        void OnClickRun();

        void OnClickContinue();
        void OnClickTitle();

        bool GetIsEmulating();
        
        IObservable<bool> IsEmulating { get; }
        IObservable<Vector3> RotateTile { get; }
        IObservable<Vector3> EraseTile { get; }
        IObservable<Unit> Supply { get; } 
        IObservable<Unit> Forward { get; } 
        IObservable<Unit> Reset { get; }
        IObservable<MineType> Target { get; }
        IObservable<string> RemainText { get; } 
        IObservable<string> TimerText { get; }
        IObservable<bool> IsResultOverlayActive { get; } 
        IObservable<string> ResultText { get; }
    }

    public class ViewModel : IViewModel
    {
        private readonly BehaviorSubject<bool> _isEmulating = new(false);
        public IObservable<bool> IsEmulating => _isEmulating;
        
        private readonly Subject<Vector3> _rotateTile = new();
        public IObservable<Vector3> RotateTile => _rotateTile.AsObservable();
        
        private readonly Subject<Vector3> _eraseTile = new();
        public IObservable<Vector3> EraseTile => _eraseTile.AsObservable();
        
        private readonly Subject<Unit> _supply = new();
        public IObservable<Unit> Supply => _supply.AsObservable();
        
        private readonly Subject<Unit> _forward = new();
        public IObservable<Unit> Forward => _forward.AsObservable();
        
        private readonly Subject<Unit> _reset = new();
        public IObservable<Unit> Reset => _reset.AsObservable();
        
        private readonly BehaviorSubject<MineType> _target = new(MineType.Diamond);
        public IObservable<MineType> Target => _target;

        private readonly BehaviorSubject<int> _remain = new(0);
        public IObservable<string> RemainText => _remain
            .Select(value => $"x {value}")
            .ToReadOnlyReactiveProperty();
        
        private readonly BehaviorSubject<int> _timer = new(0);
        public IObservable<string> TimerText => _timer
            .Select(value => $"{value} sec")
            .ToReadOnlyReactiveProperty();
        
        private readonly BehaviorSubject<bool> _isResultOverlayActive = new(false);
        public IObservable<bool> IsResultOverlayActive => _isResultOverlayActive;

        private readonly BehaviorSubject<string> _resultText = new("");
        public IObservable<string> ResultText => _resultText;
        
        [Inject] private IGameManager _gameManager;
        [Inject] private IRouter _router;

        private CancellationTokenSource _supplySource = new();
        private CancellationTokenSource _forwardSource = new();
        
        private const int SupplyDuration = 3;
        private const float ForwardDuration = 1;

        public ViewModel(IGameManager gameManager, IRouter router)
        {
            _gameManager = gameManager;
            _router = router;
        }

        public void Start()
        {
            ResetAll();
        }

        public void InputLeftClick(Vector3 position)
        {
            if (!_isEmulating.Value)
                _rotateTile.OnNext(position);
        }

        public void InputRightClick(Vector3 position)
        {
            if (!_isEmulating.Value)
                _eraseTile.OnNext(position);
        }
        
        public void InputEscape()
        {
            if (_isResultOverlayActive.Value) return;
            
            _resultText.OnNext("");
            _isResultOverlayActive.OnNext(true);
        }

        public void ReachGoal(MineType mineType)
        {
            if (mineType == _gameManager.GetLevel().target)
                _remain.OnNext(_remain.Value - 1);
        }

        public void OnClickRun()
        {
            if (_isEmulating.Value)
            {
                ResetAll();
            }
            else
            {
                _isEmulating.OnNext(true);
                _ = RunSupplyTask(true);
                _ = RunForwardTask();
            }
        }

        public void OnClickContinue()
        {
            ResetAll();
            _isResultOverlayActive.OnNext(false);
        }

        public void OnClickTitle()
        {
            _router.Title();
            
            ResetAll();
            _isResultOverlayActive.OnNext(false);
        }

        public bool GetIsEmulating()
        {
            return _isEmulating.Value;
        }

        private void ResetAll()
        {
            _isEmulating.OnNext(false);

            _target.OnNext(_gameManager.GetLevel().target);
            _remain.OnNext(_gameManager.GetLevel().remain);
            _timer.OnNext(_gameManager.GetLevel().timer);
            
            _supplySource.Cancel();
            _forwardSource.Cancel();
            
            _reset.OnNext(Unit.Default);
        }

        private async Task RunSupplyTask(bool isStart)
        {
            _supplySource = new CancellationTokenSource();
            CancellationToken token = _supplySource.Token;
            
            try {
                await UniTask.Delay(TimeSpan.FromSeconds(isStart ? 0 : SupplyDuration), cancellationToken: token);
                
                _supply.OnNext(Unit.Default);
                
                await RunSupplyTask(false);
            } catch (OperationCanceledException e) {
                Debug.Log("Error: " + e.Message);
            }
        }

        private async Task RunForwardTask()
        {
            _forwardSource = new CancellationTokenSource();
            CancellationToken token = _forwardSource.Token;
            
            try {
                await UniTask.Delay(TimeSpan.FromSeconds(ForwardDuration), cancellationToken: token);
                
                _timer.OnNext(_timer.Value - 1);
                _forward.OnNext(Unit.Default);
                
                if (_timer.Value <= 0 || _remain.Value <= 0)
                    Finish();
                else
                    await RunForwardTask();
            } catch (OperationCanceledException e) {
                Debug.Log("Error: " + e.Message);
            }
        }

        private void Finish()
        {
            _isEmulating.OnNext(false);
            
            _supplySource.Cancel();
            _forwardSource.Cancel();
            
            _isResultOverlayActive.OnNext(true);
            
            var level = _gameManager.GetLevel();
            var remain = _remain.Value;
            var timer = _timer.Value;
            
            var remainResultText = "Remain: " + remain + "\n" + (remain == Level.BestRemain ? "S" : "B");
            var timeResultText = "Timer: " + timer + " sec\n" + (_timer.Value >= level.bestTimer ? "S" : "B");

            _resultText.OnNext("Finish\n\n" + remainResultText + "\n\n" + timeResultText);
            
            _gameManager.SaveScore(remain, timer);
        }
    }
}