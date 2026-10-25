# T098: Dev-аксессоры статусов Slow / Ground / Knockback
- Status: [ ]
- Phase: 10.9
- Depends: —
- Files:
  - Assets/Scripts/Combat/ControlEffectSystem.cs
  - Assets/Scripts/Enemy/EnemyController.cs
  - Assets/Scripts/Combat/GroundZone.cs

## Задача
Добавить dev-only аксессоры (только чтение) для статусов Slow, Ground, Knockback — они нужны Debug Overlay (фаза 10.9), в релиз не попадают.

1. `Assets/Scripts/Combat/ControlEffectSystem.cs` (namespace `TankSurvival.Combat`) — внутри существующего блока `#if UNITY_EDITOR || DEBUG` (после `IsFrozen`) добавить:
```csharp
public float GetSlowAmount(EnemyController enemy)
{
    ControlStatus status = enemy != null ? Find(enemy) : null;

    return status != null ? status.slowAmount : 0f;
}

public float GetSlowRemaining(EnemyController enemy)
{
    ControlStatus status = enemy != null ? Find(enemy) : null;

    return status != null ? Mathf.Max(0f, status.duration - status.elapsed) : 0f;
}
```

2. `Assets/Scripts/Enemy/EnemyController.cs` (namespace `TankSurvival.Enemy`) — в конец класса (перед закрывающей `}` класса) добавить:
```csharp
#if UNITY_EDITOR || DEBUG
public float KnockbackRemaining => _knockbackTimer;
#endif
```

3. `Assets/Scripts/Combat/GroundZone.cs` (namespace `TankSurvival.Combat`):
   a. Изменить блок using вверху файла:
      было:
```csharp
#if UNITY_EDITOR || DEBUG
using TankSurvival.Dev;
#endif
```
      стало:
```csharp
#if UNITY_EDITOR || DEBUG
using System.Collections.Generic;
using TankSurvival.Dev;
#endif
```
   b. В начало класса (сразу после `public class GroundZone : MonoBehaviour, IPoolable {`) добавить:
```csharp
#if UNITY_EDITOR || DEBUG
public static readonly List<GroundZone> Active = new List<GroundZone>();

private void OnEnable()
{
    Active.Add(this);
}

private void OnDisable()
{
    Active.Remove(this);
}

public bool Contains(EnemyController enemy)
{
    if (enemy == null)
        return false;

    float dx = enemy.transform.position.x - transform.position.x;
    float dz = enemy.transform.position.z - transform.position.z;

    return dx * dx + dz * dz <= _radius * _radius;
}
#endif
```

## Ограничения
- Аксессоры только читают состояние, не меняют логику систем; всё в `#if UNITY_EDITOR || DEBUG`.
- `GroundZone.Active` регистрируется в `OnEnable`/`OnDisable` (пул вызывает `SetActive` в `PoolManager.Get`/`Release`), поэтому в списке только активные зоны. `OnEnable` вызывается до `Init`, но `Contains` читает `_radius` уже после `Init` (вызов идёт синхронно в одном кадре).
- `EnemyController.KnockbackRemaining` читает приватное `_knockbackTimer` (поле из T067).
- `Active`/`Contains`/аксессоры не использовать в продакшн-коде.

## Unity (человеку)
Не требуется.

## Проверка человеком
- Проект компилируется без ошибок и предупреждений.
- (Косвенно, через T099) после наложения Slow/Ground/Knockback значения читаются оверлеем.

## Отчёт Исполнителя
Готово: Assets/Scripts/Combat/ControlEffectSystem.cs: в #if добавлены GetSlowAmount/GetSlowRemaining через существующий Find; Assets/Scripts/Enemy/EnemyController.cs: в #if добавлено свойство KnockbackRemaining (читает _knockbackTimer); Assets/Scripts/Combat/GroundZone.cs: в #if добавлены using System.Collections.Generic, static readonly List<GroundZone> Active, OnEnable/OnDisable (регистрация в Active) и Contains(EnemyController) по XZ-радиусу.
Блокер: —
