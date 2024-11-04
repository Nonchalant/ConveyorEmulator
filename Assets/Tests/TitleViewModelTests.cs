using Application;
using Config;
using Moq;
using NUnit.Framework;

namespace Tests
{
    public class TitleViewModel
    {
        private Mock<IGameManager> _gameManager;
        private Mock<IRouter> _router;
        private Title.ViewModel _viewModel;
    
        [SetUp]
        public void Setup()
        {
            _gameManager = new Mock<IGameManager>();
            _router = new Mock<IRouter>();
            _viewModel = new Title.ViewModel(_gameManager.Object, _router.Object);
        }
    
        [Test]
        public void Test_TitleViewModel_OnClickLevel1()
        {
            _viewModel.OnClickLevel1();
            
            _gameManager.Verify(x => x.SetLevel(LevelType.Level1), Times.Once);
            _router.Verify(x => x.Game(), Times.Once);
        }
        
        [Test]
        public void Test_TitleViewModel_OnClickHighScore()
        {
            Assert.That(_viewModel.HighScoreText.Value, Is.Null);
            Assert.That(_viewModel.IsHighScoreOverlayActive.Value, Is.False);

            _gameManager.Setup(x => x.GetEvaluationByLevelType(LevelType.Level1)).Returns(Evaluation.S);
            _gameManager.Setup(x => x.GetEvaluationByLevelType(LevelType.Level2)).Returns(Evaluation.A);
            _gameManager.Setup(x => x.GetEvaluationByLevelType(LevelType.Level3)).Returns(Evaluation.B);
            
            _viewModel.OnClickHighScore();
            
            Assert.That(_viewModel.HighScoreText.Value, Is.EqualTo("HighScore\n\nLevel1: S\n\nLevel2: A\n\nLevel3: B"));
            Assert.That(_viewModel.IsHighScoreOverlayActive.Value, Is.True);
        }
    }
}
