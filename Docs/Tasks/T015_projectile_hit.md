# T015: Урон снаряда — непрерывное попадание по размеру противника
- Status: [x]
- Phase: 2
- Priority: Critical
- Depends: T012
- Files:
  - Assets/Scripts/Combat/Projectile.cs

## Задача
Заменить в `Projectile` проверку попадания «точка-точка» (радиус 0.5) на свип по отрезку движения с учётом размера. В `Update` перед шагом запомнить `Vector3 previous = transform.position`; после `Translate` взять `Vector3 current = transform.position`; для каждого врага из `EnemyController.Active` вычислить квадрат расстояния от его центра до отрезка `[previous, current]` в горизонтальной плоскости XZ (Y игнорировать). Если `distanceSqr <= HitRadiusSqr` — `enemy.health.TakeDamage(damage)` и `pool.Release(gameObject)`. Поднять `HitRadiusSqr` с `0.25f` до `1.0f` (радиус 1.0 = радиус врага 0.5 + радиус снаряда 0.5). Добавить приватный статический метод `DistanceToSegmentXZSqr(Vector3 point, Vector3 a, Vector3 b)`.

Формула точка-отрезок в XZ: t = clamp(скалярное(p-a, b-a) / скалярное(b-a, b-a), 0, 1); q = a + t*(b-a); вернуть (p.x-q.x)^2 + (p.z-q.z)^2.

## Ограничения
- В `Update` без `new`, LINQ, `GetComponent` в цикле; вспомогательный метод — чистая статика без аллокаций.
- Деактивацию по `range` (`_distanceTravelled >= range`) не менять.
- namespace `TankSurvival.Combat` и остальную структуру класса не менять.

## Unity (человеку)
Не требуется.

## Проверка человеком
- В Play Mode снаряд при попадании снимает 30 HP; враг (20 HP) гибнет и возвращается в пул.

## Отчёт Исполнителя
Готово: Projectile.cs: HitRadiusSqr поднят с 0.25f до 1.0f; в Update запоминается previous до Translate и current после, попадание проверяется по свипу отрезок [previous, current] через новый приватный статический DistanceToSegmentXZSqr (формула t=clamp01(dot(p-a,b-a)/|b-a|²), только скаляры, без аллокаций, Y игнорируется); деактивация по range и структура класса не менялись
Блокер: —