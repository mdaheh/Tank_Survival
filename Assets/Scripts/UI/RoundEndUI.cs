using TankSurvival.Round;
using UnityEngine;
using UnityEngine.UI;

namespace TankSurvival.UI
{
    public class RoundEndUI : MonoBehaviour
    {
        public RoundController roundController;
        public GameObject panel;
        public Text resultText;
        public Button restartButton;

        public event System.Action OnContinue;

        private void Awake()
        {
            roundController.OnPhaseChanged += HandlePhaseChanged;
            restartButton.onClick.AddListener(GoToMenu);
            panel.SetActive(false);
        }

        private void HandlePhaseChanged(RoundController.RoundPhase phase)
        {
            if (phase == RoundController.RoundPhase.Win)
            {
                resultText.text = "Победа";
                Show();
            }
            else if (phase == RoundController.RoundPhase.Lose)
            {
                resultText.text = "Поражение";
                Show();
            }
        }

        private void Show()
        {
            Time.timeScale = 0f;
            panel.SetActive(true);
        }

        private void GoToMenu()
        {
            Time.timeScale = 1f;
            panel.SetActive(false);
            OnContinue?.Invoke();
        }
    }
}
