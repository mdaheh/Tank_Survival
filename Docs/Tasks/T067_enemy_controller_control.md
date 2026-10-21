# T067: EnemyController — controlSpeedMultiplier + knockback
- Status: [x]
- Phase: 10
- Files:
  - Assets/Scripts/Enemy/EnemyController.cs

## Задача
В `EnemyController` добавить `public float controlSpeedMultiplier = 1f` (множитель контроля, отдельно от `speedMultiplier` сложности) и приватные `Vector3 _knockbackVelocity`, `float _knockbackDuration`, `float _knockbackTimer`. Добавить `public void ApplyKnockback(Vector3 velocity, float duration)` — записать `_knockbackVelocity = velocity`, `_knockbackDuration = Mathf.Max(0.0001f, duration)`, `_knockbackTimer = duration`. В `Update`: в обоих местах движения (`data.speed * speedMultiplier * Time.deltaTime`) умножить скорость на `controlSpeedMultiplier`; в начале `Update` (после проверки `data == null || target == null`), если `_knockbackTimer > 0f`, применить `transform.position += _knockbackVelocity * (_knockbackTimer / _knockbackDuration) * Time.deltaTime` и `_knockbackTimer -= Time.deltaTime`. В `Init` сбросить `controlSpeedMultiplier = 1f`, `_knockbackVelocity = Vector3.zero`, `_knockbackTimer = 0f`, `_knockbackDuration = 0f`.

## Ограничения
- Не трогать `speedMultiplier` (множитель сложности из WaveSpawner), `Active`, `health`, AI-логику ranged/melee.
- Knockback — смещение позиции без Rigidbody; скорость затухает линейно к нулю за `duration`.
- Без `new`/LINQ/`GetComponent` в `Update`.

## Unity (человеку)
Не требуется.

## Проверка человеком
- Проект компилируется без ошибок.

## Отчёт Исполнителя
Готово: Assets/Scripts/Enemy/EnemyController.cs: добавлены controlSpeedMultiplier = 1f, приватные _knockbackVelocity/_knockbackDuration/_knockbackTimer и ApplyKnockback(velocity, duration); в Update после null-проверки применяется затухающий knockback, обе ветки движения учитывают controlSpeedMultiplier; в Init сброшены controlSpeedMultiplier и параметры knockback.
Блокер: —
