# T017: Урон врага игроку (melee) — дистанция в XZ и гард на IDamageable
- Status: [x]
- Phase: 2
- Priority: High
- Depends: T011
- Files:
  - Assets/Scripts/Enemy/EnemyController.cs

## Задача
В `EnemyController`:
1. Дистанцию до цели считать в плоскости XZ (без Y): `dx = target.position.x - transform.position.x`, `dz = target.position.z - transform.position.z`, `distanceSqr = dx*dx + dz*dz`; сравнивать с `AttackRange * AttackRange`.
2. Поднять `AttackRange` с `1.0f` до `1.5f`.
3. В `Init` после `_targetDamageable = target.GetComponent<IDamageable>();` добавить: если `_targetDamageable == null` — `Debug.LogError("EnemyController: target has no IDamageable.", this);`.

## Ограничения
- В `Update` без `new`, LINQ, `GetComponent` в цикле.
- Движение врага (`MoveTowards`) не менять.
- namespace `TankSurvival.Enemy` и остальную структуру не менять.

## Unity (человеку)
Не требуется. Для проверки melee дай врагу дойти до игрока: временно отключи `TurretController` или поставь спавн-точку рядом с игроком.

## Проверка человеком
- Враг доходит до игрока, и `PlayerHealth.currentHealth` падает на 5 за удар при контакте.
- Если в консоли появится `EnemyController: target has no IDamageable` — сообщить (это отдельная причина).

## Отчёт Исполнителя
Готово: EnemyController.cs: дистанция до цели считается в плоскости XZ через dx/dz и distanceSqr, сравнение с AttackRange * AttackRange; AttackRange поднят с 1.0f до 1.5f; в Init после кэша IDamageable добавлен Debug.LogError-гард «target has no IDamageable» при null; MoveTowards, namespace и структура класса не менялись
Блокер: —