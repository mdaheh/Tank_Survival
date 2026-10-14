using TankSurvival.Data;
using UnityEngine;

namespace TankSurvival.Core
{
    public class SessionState : MonoBehaviour
    {
        [SerializeField] private ChassisData selectedChassis;
        [SerializeField] private TurretData selectedTurret;
        [SerializeField] private DifficultyData selectedDifficulty;

        public ChassisData SelectedChassis => selectedChassis;
        public TurretData SelectedTurret => selectedTurret;
        public DifficultyData SelectedDifficulty => selectedDifficulty;
    }
}
