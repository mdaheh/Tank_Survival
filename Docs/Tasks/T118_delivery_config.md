# T118: DeliveryConfig — параметры доставки
- Status: [x]
- Phase: 13
- Depends: T113, T116
- Files:
  - Assets/Scripts/Combat/Shooting/DeliveryConfig.cs
  - Assets/Scripts/Combat/Shooting/SpawnRequest.cs
  - Assets/Scripts/Combat/Shooting/ShotContext.cs

## Задача
1. Создать `DeliveryConfig` (struct, namespace `TankSurvival.Combat.Shooting`) — параметры
   доставки, которых нет в `BuildSnapshot`/`ShotContext` (Shooting.md §4.2, §7):
   - `public float range;`
   - `public float angle;` — полный угол сектора конуса в градусах (как `DeliveryModuleData.angle`)
   - `public int maxTargets;` — максимум целей за Hit (hitscan)
   - `public bool continuous;` — `true` = Periodic Hit, `false` = Terminal Hit
   - `public float tickRate;` — интервал тика при Periodic
2. Добавить поле `public DeliveryConfig config;` в `SpawnRequest` (файл из T113).
3. Добавить поле `public DeliveryConfig config;` в `ShotContext` (файл из T116) и в `Reset()`
   добавить `config = default;`.

## Ограничения
- `DeliveryConfig` — plain struct без логики и ссылок на GameObject.
- Не добавлять `speed`/`damage` в config — они уже есть в `BuildSnapshot` (T115).

## Unity (человеку)
Не требуется.

## Проверка человеком
- Проект компилируется без ошибок.

## Отчёт Исполнителя
Готово: DeliveryConfig.cs: создан struct DeliveryConfig (range, angle, maxTargets, continuous, tickRate); SpawnRequest.cs: поле config; ShotContext.cs: поле config + сброс config = default в Reset()
Блокер: —
