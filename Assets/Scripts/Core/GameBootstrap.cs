using TankSurvival.Player;
using UnityEngine;

namespace TankSurvival.Core
{
    public class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private SessionState sessionState;
        [SerializeField] private StatBlock statBlock;
        [SerializeField] private PlayerHealth playerHealth;

        private void Start()
        {
            statBlock.Init(sessionState.SelectedChassis);
            playerHealth.Init();
        }
    }
}
