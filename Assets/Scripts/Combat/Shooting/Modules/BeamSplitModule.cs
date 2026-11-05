using TankSurvival.Data;
using UnityEngine;

namespace TankSurvival.Combat.Shooting
{
    public class BeamSplitModule : IOnHit
    {
        private const float MinDirectionSqr = 1e-6f;

        private readonly OnHitModuleData _data;

        public BeamSplitModule(OnHitModuleData data)
        {
            _data = data;
        }

        public void OnHit(ShotContext ctx, Target target)
        {
            if (_data == null)
            {
                return;
            }

            if (ctx.depth >= _data.maxDepth)
            {
                return;
            }

            int count = _data.splitCount;
            if (count <= 0)
            {
                return;
            }

            Vector3 baseDir = ctx.direction;
            baseDir.y = 0f;
            if (baseDir.sqrMagnitude < MinDirectionSqr)
            {
                baseDir = Vector3.forward;
            }

            float splitAngle = _data.splitAngle;
            float step = count > 1 ? splitAngle / (count - 1) : 0f;
            float start = -splitAngle * 0.5f;

            for (int i = 0; i < count; i++)
            {
                Vector3 dir = Quaternion.AngleAxis(start + step * i, Vector3.up) * baseDir;
                dir.Normalize();

                SpawnRequest req = new SpawnRequest();
                req.type = DeliveryType.Hitscan;
                req.position = ctx.hitPoint;
                req.direction = dir;
                req.buildSnapshot = ctx.buildSnapshot;
                req.config = ctx.config;
                req.depth = ctx.depth + 1;

                ctx.spawnQueue.Add(req);
            }
        }
    }
}
