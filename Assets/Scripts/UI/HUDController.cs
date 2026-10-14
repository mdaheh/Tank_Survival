using TankSurvival.Player;
using TankSurvival.Progression;
using TankSurvival.Round;
using UnityEngine;
using UnityEngine.UI;

namespace TankSurvival.UI
{
    public class HUDController : MonoBehaviour
    {
        public PlayerHealth playerHealth;
        public ProgressionSystem progression;
        public RoundController roundController;
        public Image hpFill;
        public Text hpText;
        public Image xpFill;
        public Text xpText;
        public Text levelText;
        public Text waveText;
        public Text killsText;

        private void Awake()
        {
            playerHealth.OnHealthChanged += UpdateHealth;
            progression.OnProgressChanged += UpdateProgression;
            roundController.OnWaveChanged += UpdateWave;
            roundController.OnKillsChanged += UpdateKills;
        }

        private void UpdateHealth(float current, float max)
        {
            hpFill.fillAmount = max > 0f ? Mathf.Clamp01(current / max) : 0f;
            hpText.text = Mathf.CeilToInt(Mathf.Max(0f, current)) + " / " + Mathf.CeilToInt(max);
        }

        private void UpdateProgression()
        {
            float need = progression.XpNeeded;
            xpFill.fillAmount = need > 0f ? Mathf.Clamp01(progression.xp / need) : 0f;
            xpText.text = Mathf.CeilToInt(progression.xp) + " / " + Mathf.CeilToInt(need);
            levelText.text = progression.level.ToString();
        }

        private void UpdateWave()
        {
            waveText.text = roundController.currentWave + " / " + roundController.TotalWaves;
        }

        private void UpdateKills()
        {
            killsText.text = roundController.kills.ToString();
        }
    }
}
