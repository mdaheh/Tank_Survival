using System;
using System.Collections.Generic;
using TankSurvival.Core;
using TankSurvival.Data;
using UnityEngine;

namespace TankSurvival.Progression
{
    public class ProgressionSystem : MonoBehaviour
    {
        public StatBlock statBlock;
        public UpgradeData[] upgradePool;

        public event Action OnLevelUp;
        public event Action OnLevelUpResolved;
        public event Action OnProgressChanged;

        public float xp;
        public int level = 1;

        public float XpNeeded => 5f + level * 3f;

        private const int LevelCap = 30;
        private bool _pending;
        private readonly Dictionary<UpgradeData, int> _stacks = new Dictionary<UpgradeData, int>();

        public void AddXp(float amount)
        {
            xp += amount;
            OnProgressChanged?.Invoke();
            if (_pending || level >= LevelCap)
            {
                return;
            }

            float need = 5f + level * 3f;
            if (xp >= need)
            {
                xp -= need;
                level++;
                OnProgressChanged?.Invoke();
                _pending = true;
                OnLevelUp?.Invoke();
            }
        }

        public bool ChooseUpgrade(UpgradeData upgrade)
        {
            if (upgrade == null)
            {
                return false;
            }

            _stacks.TryGetValue(upgrade, out int stack);
            if (stack >= upgrade.maxStacks)
            {
                return false;
            }

            statBlock.ApplyUpgrade(upgrade);
            _stacks[upgrade] = stack + 1;
            _pending = false;
            OnLevelUpResolved?.Invoke();
            return true;
        }

        private List<UpgradeData> GetAvailableUpgrades()
        {
            List<UpgradeData> available = new List<UpgradeData>();
            for (int i = 0; i < upgradePool.Length; i++)
            {
                UpgradeData upgrade = upgradePool[i];
                if (upgrade == null)
                {
                    continue;
                }

                _stacks.TryGetValue(upgrade, out int stack);
                if (stack < upgrade.maxStacks)
                {
                    available.Add(upgrade);
                }
            }

            return available;
        }

        public List<UpgradeData> RollUpgrades(int count)
        {
            List<UpgradeData> available = GetAvailableUpgrades();

            for (int i = available.Count - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                UpgradeData temp = available[i];
                available[i] = available[j];
                available[j] = temp;
            }

            int take = Mathf.Min(count, available.Count);
            return available.GetRange(0, take);
        }

        public void Reset()
        {
            xp = 0f;
            level = 1;
            _pending = false;
            _stacks.Clear();
            OnProgressChanged?.Invoke();
        }
    }
}
