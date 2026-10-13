using TankSurvival.Data;
using UnityEngine;

namespace TankSurvival.Core
{
    public class SessionState : MonoBehaviour
    {
        [SerializeField] private ChassisData selectedChassis;

        public ChassisData SelectedChassis => selectedChassis;
    }
}
