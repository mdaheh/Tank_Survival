# T109: Homing — самонаведение снаряда
- Status: [ ]
- Phase: 11
- Depends: T108
- Files:
  - Assets/Scripts/Combat/Projectile.cs

## Задача
Снаряд с модулем Homing искривляет траекторию к ближайшему живому врагу в XZ-радиусе `homingRadius`; максимальная угловая скорость поворота = `homingStrength` (рад/с). Реализовать в `Projectile`:
1. Новые приватные поля: `private float _homingStrength;`, `private float _homingRadius;`.
2. В `Init(...)` (после резолва Pierce из T108): `OnHitModuleData homing = onHit != null ? onHit.GetModule(OnHitModuleData.OnHitKind.Homing) : null; _homingStrength = homing != null ? homing.homingStrength : 0f; _homingRadius = homing != null ? homing.homingRadius : 0f;`
3. В `Update` ПЕРЕД перемещением (`transform.Translate`), если `_homingStrength > 0f`: найти ближайшего врага в `_homingRadius` (XZ-дистанция, `enemy.health != null && enemy.health.currentHealth > 0`); если найден — `Vector3 toEnemy = enemyPos - transform.position; toEnemy.y = 0; float d = toEnemy.magnitude; if (d > 0.001f) direction = Vector3.RotateTowards(direction, toEnemy / d, _homingStrength * Time.deltaTime, 0f);`
4. Отладочный маркер радиуса: в `Init`, если `_homingRadius > 0f`, в блоке `#if UNITY_EDITOR || DEBUG` вызвать `DevVisualizer.ShowCircle(transform.position, _homingRadius, 1f, new Color(0.2f, 0.7f, 1f, 0.35f));` — добавить `using TankSurvival.Dev;` под тем же `#if` (как в `HitscanDeliveryHandler.cs`). Параметр маркера — `_homingRadius` из SO `Homing.asset`.

## Ограничения
- Без `new`/LINQ в `Update`. Сигнатуру `Init` не менять.
- namespace `TankSurvival.Combat`.

## Unity (человеку)
1. `Assets/Data/OnHit/` → создать `Homing.asset`: `kind = Homing`, `homingStrength = 3`, `homingRadius = 3`, `priority = 1`.
2. `Assets/Data/Turrets/Hummer.asset` (Projectile delivery) → `onHitModules` добавить `Homing.asset`.

## Проверка человеком
- Проект компилируется без ошибок.
- Play Mode → Dev Menu → турель Hummer → Start Round. Снаряд с Homing заметно заворачивает к ближайшему врагу; при спавне снаряда виден синий круг-маркер радиуса.

## Отчёт Исполнителя
Готово:
Блокер:
