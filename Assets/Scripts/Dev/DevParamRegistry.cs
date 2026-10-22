#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System.Collections.Generic;
using System.Reflection;
using TankSurvival.Core;
using TankSurvival.Data;
using TankSurvival.Waves;
using UnityEngine;

namespace TankSurvival.Dev
{
    public static class DevParamRegistry
    {
        public static readonly List<DevParam> All = new List<DevParam>();

        public static void Init()
        {
            All.Clear();

            SessionState session = Object.FindFirstObjectByType<SessionState>();
            if (session != null)
            {
                Add(session.SelectedChassis, "ChassisData");

                TurretData turret = session.SelectedTurret;
                if (turret != null)
                {
                    Add(turret, "TurretData");

                    if (turret.delivery != null)
                    {
                        All.AddRange(Reflect(turret.delivery, "TurretData." + turret.name + ".delivery"));
                    }

                    if (turret.onHitModules != null)
                    {
                        for (int i = 0; i < turret.onHitModules.Length; i++)
                        {
                            Add(turret.onHitModules[i], "OnHitModuleData");
                        }
                    }
                }

                Add(session.SelectedDifficulty, "DifficultyData");
            }

            WaveSpawner spawner = Object.FindFirstObjectByType<WaveSpawner>();
            if (spawner != null && spawner.waveData != null && spawner.waveData.compositions != null)
            {
                for (int i = 0; i < spawner.waveData.compositions.Length; i++)
                {
                    Add(spawner.waveData.compositions[i].enemy, "EnemyData");
                }
            }
        }

        public static List<DevParam> Reflect(object target, string group)
        {
            List<DevParam> parameters = new List<DevParam>();

            if (target == null)
                return parameters;

            FieldInfo[] fields = target.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];

                bool isInteger;
                if (field.FieldType == typeof(float))
                    isInteger = false;
                else if (field.FieldType == typeof(int))
                    isInteger = true;
                else
                    continue;

                parameters.Add(new DevParam(target, field, group, field.Name, isInteger));
            }

            return parameters;
        }

        private static void Add(Object asset, string groupPrefix)
        {
            if (asset == null)
                return;

            All.AddRange(Reflect(asset, groupPrefix + "." + asset.name));
        }
    }
}
#endif
