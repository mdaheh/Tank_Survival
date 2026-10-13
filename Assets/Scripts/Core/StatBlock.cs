using TankSurvival.Data;
using UnityEngine;

namespace TankSurvival.Core
{
    public class StatBlock : MonoBehaviour
    {
        public float damage;
        public float fireRate;
        public float moveSpeed;
        public float maxHealth;

        public void Init(ChassisData chassis)
        {
            moveSpeed = chassis.speed;
            maxHealth = chassis.health;
        }
    }
}
