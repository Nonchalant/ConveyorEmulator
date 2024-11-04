using System.Collections.Generic;
using Application;
using Config;
using UniRx;
using UnityEngine;
using Zenject;

namespace Title {
    public interface IViewModel
    {
        void OnClickLevel1();
        void OnClickLevel2();
        void OnClickLevel3();
        void OnClickHighScore();
        void OnClickExit();
        void OnClickBack();

        void InputEscape();
        
        IReactiveProperty<bool> IsHighScoreOverlayActive { get; }
        IReactiveProperty<string> HighScoreText { get; }
    }
    
    public class ViewModel : IViewModel
    {
        [Inject] private IGameManager _gameManager;
        [Inject] private IRouter _router;
        
        private readonly ReactiveProperty<bool> _isHighScoreOverlayActive = new();
        public IReactiveProperty<bool> IsHighScoreOverlayActive => _isHighScoreOverlayActive;
        
        private readonly ReactiveProperty<string> _highScoreText = new();
        public IReactiveProperty<string> HighScoreText => _highScoreText;

        public ViewModel(IGameManager gameManager, IRouter router)
        {
            _gameManager = gameManager;
            _router = router;
        }

        public void OnClickLevel1()
        {
            _gameManager.SetLevel(LevelType.Level1);
            _router.Game();
        }
        
        public void OnClickLevel2()
        {
            _gameManager.SetLevel(LevelType.Level2);
            _router.Game();
        }
        
        public void OnClickLevel3()
        {
            _gameManager.SetLevel(LevelType.Level3);
            _router.Game();
        }
        
        public void OnClickHighScore()
        {
            var highScoreText = "HighScore";
            
            var levelTypes = new List<LevelType> { LevelType.Level1, LevelType.Level2, LevelType.Level3 };
            foreach (var levelType in levelTypes)
                highScoreText += $"\n\n{Helper.EnumHelper.GetDescription(levelType)}: {Helper.EnumHelper.GetDescription(_gameManager.GetEvaluationByLevelType(levelType))}";
            
            _highScoreText.Value = highScoreText;
            _isHighScoreOverlayActive.Value = true;
        }
        
        public void OnClickExit()
        {
            _router.Exit();
        }
        
        public void OnClickBack()
        {
            _isHighScoreOverlayActive.Value = false;
        }

        public void InputEscape()
        {
            _isHighScoreOverlayActive.Value = false;
        }
    }
}