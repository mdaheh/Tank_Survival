using TankSurvival.Round;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace TankSurvival.UI
{
    public class PauseMenuController : MonoBehaviour
    {
        public RoundController roundController;
        public GameObject panel;
        public Button resumeButton;
        public Button exitButton;

        public event System.Action OnExitToMenu;

        private void Awake()
        {
            resumeButton.onClick.AddListener(Resume);
            exitButton.onClick.AddListener(ExitToMenu);
            panel.SetActive(false);
        }

        private void Update()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null) return;
            if (!keyboard.escapeKey.wasPressedThisFrame) return;

            if (panel.activeSelf) { Resume(); return; }
            if (CanPause()) Show();
        }

        private bool CanPause()
        {
            return roundController.isRoundActive
                && roundController.phase == RoundController.RoundPhase.Playing;
        }

        private void Show()
        {
            Time.timeScale = 0f;
            panel.SetActive(true);
        }

        private void Resume()
        {
            Time.timeScale = 1f;
            panel.SetActive(false);
        }

        private void ExitToMenu()
        {
            Time.timeScale = 1f;
            panel.SetActive(false);
            roundController.AbortRound();
            OnExitToMenu?.Invoke();
        }
    }
}
