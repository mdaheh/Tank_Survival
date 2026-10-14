using System.Collections.Generic;
using TankSurvival.Data;
using TankSurvival.Progression;
using UnityEngine;
using UnityEngine.UI;

namespace TankSurvival.UI
{
    public class LevelUpUI : MonoBehaviour
    {
        public ProgressionSystem progression;
        public GameObject panel;
        public Button[] cardButtons;
        public Text[] cardLabels;

        private void Awake()
        {
            progression.OnLevelUp += Show;
            panel.SetActive(false);
        }

        private void Show()
        {
            List<UpgradeData> upgrades = progression.RollUpgrades(3);
            for (int i = 0; i < cardButtons.Length; i++)
            {
                if (i < upgrades.Count)
                {
                    cardButtons[i].gameObject.SetActive(true);
                    cardLabels[i].text = upgrades[i].stat.ToString() + " +" + upgrades[i].value;
                    cardButtons[i].onClick.RemoveAllListeners();
                    UpgradeData chosen = upgrades[i];
                    cardButtons[i].onClick.AddListener(() => Choose(chosen));
                }
                else
                {
                    cardButtons[i].gameObject.SetActive(false);
                }
            }

            Time.timeScale = 0f;
            panel.SetActive(true);
        }

        private void Choose(UpgradeData upgrade)
        {
            if (progression.ChooseUpgrade(upgrade))
            {
                panel.SetActive(false);
                Time.timeScale = 1f;
            }
        }
    }
}
