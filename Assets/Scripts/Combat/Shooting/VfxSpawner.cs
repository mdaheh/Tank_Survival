using TankSurvival.Combat;
using TankSurvival.Core;
using UnityEngine;

namespace TankSurvival.Combat.Shooting
{
    public class VfxSpawner : MonoBehaviour, IVfxSpawner
    {
        private PoolManager pool;

        private void Awake()
        {
            GameServices.Register<IVfxSpawner>(this);
        }

        public void Spawn(GameObject prefab, Vector3 position)
        {
            Spawn(prefab, position, Vector3.zero);
        }

        public void Spawn(GameObject prefab, Vector3 position, Vector3 direction)
        {
            if (prefab == null) return;

            if (pool == null)
            {
                pool = GameServices.Get<PoolManager>();
                if (pool == null) return;
            }

            GameObject vfx = pool.Get(prefab);
            if (vfx == null) return;

            vfx.transform.position = position;

            if (direction.sqrMagnitude > 1e-6f)
            {
                Vector3 fwd = direction;
                fwd.y = 0f;
                if (fwd.sqrMagnitude > 1e-6f)
                {
                    vfx.transform.rotation = Quaternion.LookRotation(fwd);
                }
            }

            PooledVFX pooled = vfx.GetComponent<PooledVFX>();
            if (pooled != null)
            {
                pooled.Play(pool);
            }
        }
    }
}
