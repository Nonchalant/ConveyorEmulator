using Config;
using Helper;
using UnityEngine;
using Zenject;

namespace Application
{
    public interface IGameManager
    {
        public Level GetLevel();
        public void SetLevel(LevelType levelType);
    
        public void SaveScore(int remain, int timer);
    
        public Evaluation GetEvaluationByLevelType(LevelType levelType);
    }

    public class GameManager : IGameManager
    {
        private readonly Config.Game _gameConfig = new();
 
        [Inject] private IPlayerPrefsManager _playerPrefsManager;
        
        public Level GetLevel()
        {
            return Level.Load(_gameConfig.LevelType);
        }

        public void SetLevel(LevelType levelType)
        {
            _gameConfig.LevelType = levelType;
        }

        public void SaveScore(int remain, int timer)
        {
            var level = Level.Load(_gameConfig.LevelType);

            switch (_gameConfig.LevelType)
            {
                case LevelType.Level1:
                    SaveScore(level, remain, timer, PlayerPrefsKey.Level1Remain, PlayerPrefsKey.Level1Timer);
                    break;
                case LevelType.Level2:
                    SaveScore(level, remain, timer, PlayerPrefsKey.Level2Remain, PlayerPrefsKey.Level2Timer);
                    break;
                case LevelType.Level3:
                    SaveScore(level, remain, timer, PlayerPrefsKey.Level3Remain, PlayerPrefsKey.Level3Timer);
                    break;
            }
        }

        public Evaluation GetEvaluationByLevelType(LevelType levelType)
        {
            var level = Level.Load(levelType);
        
            switch (levelType)
            {
                case LevelType.Level1:
                    return GetEvaluation(level, PlayerPrefsKey.Level1Remain, PlayerPrefsKey.Level1Timer);
                case LevelType.Level2:
                    return GetEvaluation(level, PlayerPrefsKey.Level2Remain, PlayerPrefsKey.Level2Timer);
                case LevelType.Level3:
                    return GetEvaluation(level, PlayerPrefsKey.Level3Remain, PlayerPrefsKey.Level3Timer);
            }
        
            return Evaluation.B;
        }
    
        private void SaveScore(Level level, int remain, int timer, PlayerPrefsKey remainKey, PlayerPrefsKey timerKey)
        {
            var bestRemain = _playerPrefsManager.GetInt(remainKey, level.remain);
            var bestTimer = _playerPrefsManager.GetInt(timerKey, level.timer);
    
            if (remain <= bestRemain)
                _playerPrefsManager.SetInt(remainKey, remain);
    
            if (timer >= bestTimer)
                _playerPrefsManager.SetInt(timerKey, timer);
        }
    
        private Evaluation GetEvaluation(Level level, PlayerPrefsKey remainKey, PlayerPrefsKey timerKey)
        {
            var remainEvaluation = _playerPrefsManager.GetInt(remainKey, level.remain) <= Level.BestRemain;
            var timerEvaluation = _playerPrefsManager.GetInt(timerKey, level.timer) >= level.bestTimer;
        
            switch (remainEvaluation, timerEvaluation)
            {
                case (true, true):
                    return Evaluation.S;
                case (true, false):
                case (false, true):
                    return Evaluation.A;
                case (false, false):
                    return Evaluation.B;
            }
        }
    }
}