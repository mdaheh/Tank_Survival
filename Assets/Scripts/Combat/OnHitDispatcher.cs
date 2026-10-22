using System.Collections.Generic;
using TankSurvival.Core;
using TankSurvival.Data;
using TankSurvival.Enemy;
using UnityEngine;

namespace TankSurvival.Combat
{
    public class OnHitDispatcher : MonoBehaviour
    {
        public ExplosionOnHitHandler explosion;
        public BurnOnHitHandler burn;
        public GroundOnHitHandler ground;
        public SlowOnHitHandler slow;
        public FreezeOnHitHandler freeze;
        public KnockbackOnHitHandler knockback;

        private readonly List<OnHitModuleData> _modules = new List<OnHitModuleData>();

        private void Awake()
        {
            GameServices.Register(this);
        }

        public void SetModules(OnHitModuleData[] modules)
        {
            _modules.Clear();

            if (modules == null)
                return;

            for (int i = 0; i < modules.Length; i++)
            {
                if (modules[i] != null)
                    _modules.Add(modules[i]);
            }

            for (int i = 1; i < _modules.Count; i++)
            {
                OnHitModuleData key = _modules[i];
                int j = i - 1;
                while (j >= 0 && _modules[j].priority < key.priority)
                {
                    _modules[j + 1] = _modules[j];
                    j--;
                }
                _modules[j + 1] = key;
            }
        }

        public void ProcessHit(Vector3 position, EnemyController target)
        {
            ProcessHit(position, target, 0);
        }

        public void ProcessHit(Vector3 position, EnemyController target, int depth)
        {
            if (target == null)
                return;

            for (int i = 0; i < _modules.Count; i++)
            {
                OnHitModuleData module = _modules[i];

                switch (module.kind)
                {
                    case OnHitModuleData.OnHitKind.Explosion:
                        if (explosion != null)
                            explosion.HandleHit(module, position, target);
                        break;
                    case OnHitModuleData.OnHitKind.Burn:
                        if (burn != null)
                            burn.HandleHit(module, position, target);
                        break;
                    case OnHitModuleData.OnHitKind.Ground:
                        if (ground != null && depth < module.maxDepth)
                            ground.HandleHit(module, position, target, depth);
                        break;
                    case OnHitModuleData.OnHitKind.Slow:
                        if (slow != null)
                            slow.HandleHit(module, position, target);
                        break;
                    case OnHitModuleData.OnHitKind.Freeze:
                        if (freeze != null)
                            freeze.HandleHit(module, position, target);
                        break;
                    case OnHitModuleData.OnHitKind.Knockback:
                        if (knockback != null)
                            knockback.HandleHit(module, position, target);
                        break;
                }
            }
        }
    }
}