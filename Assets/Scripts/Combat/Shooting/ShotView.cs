using TankSurvival.Core;
using UnityEngine;

namespace TankSurvival.Combat.Shooting
{
    public class ShotView : MonoBehaviour, IPoolable
    {
        private ShotContext ctx;

        public void Bind(ShotContext context)
        {
            ctx = context;
            if (ctx == null) return;
            transform.position = ctx.position;
        }

        private void Update()
        {
            if (ctx == null) return;

            transform.position = ctx.position;

            Vector3 fwd = ctx.direction;
            fwd.y = 0f;
            if (fwd.sqrMagnitude > 1e-6f)
            {
                transform.rotation = Quaternion.LookRotation(fwd);
            }
        }

        public void OnGetFromPool()
        {
            ctx = null;
        }

        public void OnReleaseFromPool()
        {
            ctx = null;
        }
    }
}
