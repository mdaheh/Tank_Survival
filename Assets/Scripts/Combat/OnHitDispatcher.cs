using System.Collections.Generic;
using TankSurvival.Data;
using TankSurvival.Enemy;
using UnityEngine;

namespace TankSurvival.Combat
{
    public class OnHitDispatcher : MonoBehaviour
    {
        public ExplosionOnHitHandler explosion;
        public BurnOnHitHandler burn;

        private readonly List<OnHitModuleData> _modules = new List<OnHitModuleData>();

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
                }
            }
        }
    }
}