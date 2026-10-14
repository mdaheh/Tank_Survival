using TankSurvival.Core;
using UnityEngine;

namespace TankSurvival.Combat
{
    public class PooledVFX : MonoBehaviour, IPoolable
    {
        public float duration = 1f;

        private PoolManager _pool;
        private float _timer;

        public void Play(PoolManager pool)
        {
            _pool = pool;
            _timer = duration;
        }

        private void Update()
        {
            if (_pool == null)
                return;

            _timer -= Time.deltaTime;
            if (_timer <= 0f)
            {
                _pool.Release(gameObject);
                _pool = null;
            }
        }

        public void OnGetFromPool()
        {
            _timer = 0f;
            _pool = null;
        }

        public void OnReleaseFromPool()
        {
            _timer = 0f;
            _pool = null;
        }
    }
}