using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Title
{
    public class Controller : MonoBehaviour
    {
        [SerializeField] private Button level1Button;
        [SerializeField] private Button level2Button;
        [SerializeField] private Button level3Button;
        [SerializeField] private Button highScoreButton;
        [SerializeField] private Button exitButton;

        [SerializeField] private GameObject highScoreOverlay;
        [SerializeField] private TextMeshProUGUI highScoreText;
        [SerializeField] private Button backButton;
        
        [Inject] private IViewModel _viewModel; 

        private void Start()
        {
            level1Button.onClick.AddListener(_viewModel.OnClickLevel1);
            level2Button.onClick.AddListener(_viewModel.OnClickLevel2);
            level3Button.onClick.AddListener(_viewModel.OnClickLevel3);
            highScoreButton.onClick.AddListener(_viewModel.OnClickHighScore);
            exitButton.onClick.AddListener(_viewModel.OnClickExit);
            backButton.onClick.AddListener(_viewModel.OnClickBack);

            this.FixedUpdateAsObservable()
                .WithLatestFrom(_viewModel.IsHighScoreOverlayActive, (_, isActive) => isActive)
                .Subscribe(isActive =>
                {
                    highScoreOverlay.SetActive(isActive);
                });
            
            this.FixedUpdateAsObservable()
                .WithLatestFrom(_viewModel.HighScoreText, (_, text) => text)
                .Subscribe(text =>
                {
                    highScoreText.text = text;
                });
            
            this.FixedUpdateAsObservable()
                .Where(_ => Input.GetKey(KeyCode.Escape))
                .Subscribe(_ =>
                {
                    _viewModel.InputEscape();
                });
        }
    }
}
