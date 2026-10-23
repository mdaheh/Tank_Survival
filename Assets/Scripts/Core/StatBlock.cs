using System;
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
        public float turn;

        public event Action<float> MaxHealthIncreased;

        public void Init(ChassisData chassis, TurretData turret)
        {
            moveSpeed = chassis.speed;
            turn = chassis.turn;
            maxHealth = chassis.health;
            damage = turret.delivery != null ? turret.delivery.damage : 0f;
            fireRate = turret.delivery != null ? turret.delivery.aps : 0f;
        }

        public void ApplyUpgrade(UpgradeData upgrade)
        {
            switch (upgrade.stat)
            {
                case UpgradeData.StatType.Damage:
                    damage += upgrade.value;
                    break;
                case UpgradeData.StatType.FireRate:
                    fireRate += upgrade.value;
                    break;
                case UpgradeData.StatType.MoveSpeed:
                    moveSpeed += upgrade.value;
                    break;
                case UpgradeData.StatType.MaxHealth:
                    maxHealth += upgrade.value;
                    MaxHealthIncreased?.Invoke(upgrade.value);
                    break;
            }
        }
    }
}
