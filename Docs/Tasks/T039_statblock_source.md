# T039: StatBlock — источник статов + ApplyUpgrade; SessionState.SelectedTurret
- Status: [x]
- Phase: 6
- Depends: T038
- Files:
  - Assets/Scripts/Core/StatBlock.cs
  - Assets/Scripts/Core/SessionState.cs
  - Assets/Scripts/Player/PlayerHealth.cs
  - Assets/Scripts/Core/GameBootstrap.cs

## Задача
1. `StatBlock` (namespace `TankSurvival.Core`, добавить `using System;`):
   - заменить `Init(ChassisData chassis)` на `Init(ChassisData chassis, TurretData turret)`:
     `moveSpeed = chassis.speed; maxHealth = chassis.health; damage = turret.delivery != null ? turret.delivery.damage : 0f; fireRate = turret.delivery != null ? turret.delivery.aps : 0f;`
   - добавить `public event Action<float> MaxHealthIncreased;`
   - добавить `public void ApplyUpgrade(UpgradeData upgrade)` — `switch (upgrade.stat)`:
     `Damage → damage += upgrade.value;`, `FireRate → fireRate += upgrade.value;`,
     `MoveSpeed → moveSpeed += upgrade.value;`,
     `MaxHealth → { maxHealth += upgrade.value; MaxHealthIncreased?.Invoke(upgrade.value); }`.
2. `SessionState`: добавить `[SerializeField] private TurretData selectedTurret;` и `public TurretData SelectedTurret => selectedTurret;` (рядом с `SelectedChassis`).
3. `PlayerHealth`: добавить `private void Awake() { statBlock.MaxHealthIncreased += delta => currentHealth += delta; }` (существующий `Init` не трогать).
4. `GameBootstrap`: в `Start` заменить `statBlock.Init(sessionState.SelectedChassis)` на `statBlock.Init(sessionState.SelectedChassis, sessionState.SelectedTurret)`.

## Ограничения
- `StatBlock.Init` принимает ровно два аргумента (chassis, turret); guard только для `turret.delivery == null`.
- `UpgradeData`/`TurretData`/`ChassisData` — `TankSurvival.Data`; `using` уже есть в StatBlock.
- Подписка в `PlayerHealth.Awake` — один раз; не отписываться (сцена не перезагружается).

## Unity (человеку)
- На GameObject `GameBootstrap` в `Assets/Scenes/Main.unity`: компонент `SessionState` → поле `selectedTurret` → `Assets/Data/Turrets/Hammer.asset` (тот же ассет, что стоит в `TurretController.turret` на объекте `Turret`).

## Проверка человеком
- Инспектор `SessionState` (на GameBootstrap): `selectedChassis` и `selectedTurret` заполнены.
- Проект компилируется без ошибок.

## Отчёт Исполнителя
Готово: Assets/Scripts/Core/StatBlock.cs — Init(chassis, turret) + событие MaxHealthIncreased + ApplyUpgrade; Assets/Scripts/Core/SessionState.cs — selectedTurret/SelectedTurret; Assets/Scripts/Player/PlayerHealth.cs — подписка в Awake; Assets/Scripts/Core/GameBootstrap.cs — Init с двумя аргументами.
Блокер: —
