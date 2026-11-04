using System.Collections.Generic;
using UnityEngine;

namespace TankSurvival.Combat.Shooting
{
    public class ShotContext
    {
        public ShotState state;
        public float age;
        public float lifetime;
        public bool keepAlive;
        public DestroyReason destroyReason;
        public int depth;
        public BuildSnapshot buildSnapshot;
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
        public Target currentTarget;

        public void Reset()
        {
            state = ShotState.Dead;
            age = 0f;
            lifetime = 0f;
            keepAlive = false;
            destroyReason = default;
            depth = 0;
            buildSnapshot = null;
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
            currentTarget = null;
        }
    }
}
