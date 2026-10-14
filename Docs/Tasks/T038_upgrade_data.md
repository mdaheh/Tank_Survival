# T038: UpgradeData — SO апгрейда
- Status: [x]
- Phase: 6
- Files:
  - Assets/Scripts/Data/UpgradeData.cs

## Задача
Создать `UpgradeData : ScriptableObject` (namespace `TankSurvival.Data`, `[CreateAssetMenu(menuName = "Tank Survival/Upgrade Data", fileName = "UpgradeData")]`) с вложенным `public enum StatType { Damage, FireRate, MoveSpeed, MaxHealth }` и полями: `public StatType stat;`, `public float value;`, `public int maxStacks;`.

## Ограничения
- `enum StatType` менять нельзя (используется в T039/T041).
- `using UnityEngine`; стиль — как у соседних SO (см. `OnHitModuleData`).

## Unity (человеку)
Создать папку `Assets/Data/Upgrades/` и 4 ассета (ПКМ → Create → Tank Survival → Upgrade Data):
1. `DamageUpgrade.asset`: stat=Damage, value=5, maxStacks=5.
2. `FireRateUpgrade.asset`: stat=FireRate, value=0.5, maxStacks=5.
3. `MoveSpeedUpgrade.asset`: stat=MoveSpeed, value=0.5, maxStacks=5.
4. `MaxHealthUpgrade.asset`: stat=MaxHealth, value=20, maxStacks=5.

## Проверка человеком
- Каждый ассет в инспекторе показывает stat/value/maxStacks с указанными значениями.
- Проект компилируется без ошибок.

## Отчёт Исполнителя
Готово: Assets/Scripts/Data/UpgradeData.cs — создан SO UpgradeData с вложенным enum StatType (Damage, FireRate, MoveSpeed, MaxHealth) и полями stat/value/maxStacks.
Блокер: —
