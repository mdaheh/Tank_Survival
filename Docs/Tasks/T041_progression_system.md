# T041: ProgressionSystem — XP, кривая уровней, level-up
- Status: [x]
- Phase: 6
- Depends: T037, T038, T039
- Files:
  - Assets/Scripts/Progression/ProgressionSystem.cs

## Задача
Создать `ProgressionSystem : MonoBehaviour` (namespace `TankSurvival.Progression`, `using System;`, `using System.Collections.Generic;`, `using TankSurvival.Core;`, `using TankSurvival.Data;`, `using UnityEngine;`).

Поля:
- `public StatBlock statBlock;`
- `public UpgradeData[] upgradePool;`
- `public event Action OnLevelUp;`
- `public event Action OnLevelUpResolved;`
- `public float xp;` (публичное, для наблюдения в инспекторе)
- `public int level = 1;`
- `private const int LevelCap = 30;`
- `private bool _pending;`
- `private readonly Dictionary<UpgradeData, int> _stacks = new Dictionary<UpgradeData, int>();`

Методы:
- `public void AddXp(float amount)`: `xp += amount;` если `_pending || level >= LevelCap` — `return;`
  иначе `float need = 5f + level * 3f;` если `xp >= need` → `xp -= need; level++; _pending = true; OnLevelUp?.Invoke();`
- `public bool ChooseUpgrade(UpgradeData upgrade)`: если `upgrade == null` — `false`; взять текущий стек
  (`_stacks.TryGetValue`); если `стек >= upgrade.maxStacks` — `false`; иначе `statBlock.ApplyUpgrade(upgrade);`,
  `_stacks[upgrade] = стек + 1; _pending = false; OnLevelUpResolved?.Invoke(); return true;`
- `private List<UpgradeData> GetAvailableUpgrades()`: собрать из `upgradePool` те, у которых
  стек < `maxStacks` (null пропускать).
- `public List<UpgradeData> RollUpgrades(int count)`: взять `GetAvailableUpgrades()`, перемешать
  (Fisher–Yates через `Random.Range`), вернуть первые `min(count, Count)` через `GetRange(0, take)`.
- `public void Reset()`: `xp = 0f; level = 1; _pending = false; _stacks.Clear();`

## Ограничения
- Без LINQ; `Random.Range` только в `RollUpgrades` (не в `Update`; у класса нет `Update`).
- `AddXp` — один level-up за вызов (не цикл по нескольким уровням).
- Уровень стартует с 1; мягкий кап — 30 (на капе XP продолжает накапливаться, level-up не происходит).

## Unity (человеку)
1. Создать пустой GameObject `ProgressionSystem` в `Assets/Scenes/Main.unity`; добавить компонент `ProgressionSystem`.
2. Заполнить поля: `statBlock` → компонент `StatBlock` (на объекте `Player`); `upgradePool` → 4 ассета из T038 (`DamageUpgrade`, `FireRateUpgrade`, `MoveSpeedUpgrade`, `MaxHealthUpgrade`).

## Проверка человеком
- Инспектор `ProgressionSystem`: `statBlock` и `upgradePool` (4 элемента) заполнены; `level = 1`, `xp = 0`.
- Проект компилируется без ошибок.

## Отчёт Исполнителя
Готово: Assets/Scripts/Progression/ProgressionSystem.cs — создан ProgressionSystem: AddXp с кривой 5 + level*3 и капом 30, ChooseUpgrade со стеками, RollUpgrades (Fisher–Yates), Reset, события OnLevelUp/OnLevelUpResolved.
Блокер: —
