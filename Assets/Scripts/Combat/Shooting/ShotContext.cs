using System.Collections.Generic;
using TankSurvival.Core;
using UnityEngine;

namespace TankSurvival.Combat.Shooting
{
    public class ShotContext
    {
        public ShotState state;
        public float age;
        // -1 = без таймера; > 0 = секунды жизни
        public float lifetime;
        public bool keepAlive;
        public DestroyReason destroyReason;
        public int depth;
        public BuildSnapshot buildSnapshot;
        public DeliveryConfig config;
        public Faction faction;
        public List<SpawnRequest> spawnQueue = new List<SpawnRequest>();
        public List<Target> hitTargets = new List<Target>();
        public Vector3 hitPoint;

        public Vector3 position;
        public Vector3 direction;
        public float speed;
        public Vector3 prevPosition;
        public float remainingRange;
        public float tickRate;
        public float nextTickAt;
        // Для точечной доставки — жертва; для площадной — якорь направления (урон всем из FindTargets)
        public Target currentTarget;

        // Per-shot damage: база из buildSnapshot.damage, уменьшается Pierce (falloff).
        public float damage;
        // Pierce: оставшиеся пробития; -1 = модуль отсутствует/не срабатывает.
        public int pierceRemaining;
        // Laser Focus: накопленные стаки и цель (по IDamageable), на которой они накоплены.
        public int laserStacks;
        public IDamageable laserFocusedTarget;

        public void Reset()
        {
            state = ShotState.Dead;
            age = 0f;
            lifetime = -1f;
            keepAlive = false;
            destroyReason = default;
            depth = 0;
            buildSnapshot = null;
            config = default;
            faction = Faction.Player;
            spawnQueue.Clear();
            hitTargets.Clear();
            hitPoint = Vector3.zero;

            position = Vector3.zero;
            direction = Vector3.zero;
            speed = 0f;
            prevPosition = Vector3.zero;
            remainingRange = 0f;
            tickRate = 0f;
            nextTickAt = 0f;
            currentTarget = default;
            damage = 0f;
            pierceRemaining = -1;
            laserStacks = 0;
            laserFocusedTarget = null;
        }
    }
}
