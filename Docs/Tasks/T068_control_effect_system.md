# T068: ControlEffectSystem — slow/freeze
- Status: [ ]
- Phase: 10
- Depends: T067
- Files:
  - Assets/Scripts/Combat/ControlEffectSystem.cs

## Задача
Создать `ControlEffectSystem : MonoBehaviour` (namespace `TankSurvival.Combat`). Приватный вложенный класс `ControlStatus` с полями `EnemyController enemy`, `float slowAmount`, `int freezeStacks`, `bool frozen`, `float duration`, `float elapsed`; приватный `List<ControlStatus> _statuses`.
- `ApplySlow(EnemyController enemy, float slowAmount, float duration)`: если `enemy == null || enemy.health == null || enemy.health.currentHealth <= 0f || !enemy.gameObject.activeSelf` — выйти. Если статус для врага есть — обновить `slowAmount`/`duration`, сбросить `elapsed`; иначе добавить. Сразу выставить `enemy.controlSpeedMultiplier = 1f - slowAmount`.
- `ApplyFreeze(EnemyController enemy, float slowAmount, float duration, int freezeThreshold)`: те же guard'ы. Если статус есть — обновить `slowAmount`/`duration`, сбросить `elapsed`, увеличить `freezeStacks`; иначе добавить с `freezeStacks = 1`. Если `freezeStacks >= freezeThreshold` — `frozen = true`. Выставить `enemy.controlSpeedMultiplier = frozen ? 0f : (1f - slowAmount)`.
- `RemoveFreeze(EnemyController enemy)`: найти статус врага, удалить из списка, вернуть `enemy.controlSpeedMultiplier = 1f`.
В `Update` (обратная итерация): `elapsed += dt`; удалить и вернуть `controlSpeedMultiplier = 1f`, если `enemy.health.currentHealth <= 0f || !enemy.gameObject.activeSelf || elapsed >= duration`; иначе `enemy.controlSpeedMultiplier = frozen ? 0f : (1f - slowAmount)`.

## Ограничения
- Урон не наносится; только `controlSpeedMultiplier`. Без `new`/LINQ/`GetComponent` в `Update` (удаление — `RemoveAt` при обратной итерации).
- `EnemyHealth` брать из `enemy.health` (без `GetComponent`). namespace `TankSurvival.Combat`; `using TankSurvival.Enemy` (EnemyController), `using System.Collections.Generic`, `using UnityEngine`.

## Unity (человеку)
1. В сцене создать пустой GameObject `ControlEffectSystem` → добавить компонент `ControlEffectSystem`.

## Проверка человеком
- Проект компилируется без ошибок.

## Отчёт Исполнителя
Готово:
Блокер:
