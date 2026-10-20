using System.Collections.Generic;
using TankSurvival.Core;
using UnityEngine;

namespace TankSurvival.Enemy
{
    public class EnemyKillSystem : MonoBehaviour
    {
        public PoolManager pool;

        private readonly List<GameObject> _pending = new List<GameObject>();

        public void Enqueue(GameObject enemy)
        {
            if (enemy == null)
            {
                return;
            }

            _pending.Add(enemy);
        }

        private void LateUpdate()
        {
            if (pool == null || _pending.Count == 0)
            {
                return;
            }

            for (int i = 0; i < _pending.Count; i++)
            {
                pool.Release(_pending[i]);
            }

            _pending.Clear();
        }
    }
}
