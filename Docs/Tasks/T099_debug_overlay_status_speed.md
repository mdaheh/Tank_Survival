# T099: Debug Overlay — фактическая скорость + статусы в блоке врага
- Status: [ ]
- Phase: 10.9
- Depends: T097, T098
- Files:
  - Assets/Scripts/Dev/DebugOverlayController.cs

## Задача
Расширить блок «Враг» (основной и preview) фактической скоростью и полным списком статусов (Burn/Freeze/Frozen/Slow/Ground/Knockback); блок «Статусы» оставить только агрегатами.

`Assets/Scripts/Dev/DebugOverlayController.cs`:

1. Расширить `BuildEnemyBlock(...)` — после строки «Дистанция» (после `_builder.Append('\n');`, завершающего «Дистанция») добавить:
```csharp
_builder.Append("Скорость: ");
_builder.Append(ActualSpeed(enemy).ToString("0.##"));
_builder.Append('\n');

_builder.Append("Burn остаток: ");
_builder.Append(_statusEffects != null ? _statusEffects.GetBurnRemaining(enemy).ToString("0.##") : "0");
_builder.Append('\n');
_builder.Append("Freeze стаки: ");
_builder.Append(_controlEffects != null ? _controlEffects.GetFreezeStacks(enemy) : 0);
_builder.Append('\n');
_builder.Append("Frozen: ");
_builder.Append(_controlEffects != null && _controlEffects.IsFrozen(enemy) ? "да" : "нет");
_builder.Append('\n');
_builder.Append("Slow: ");
_builder.Append(_controlEffects != null && _controlEffects.GetSlowRemaining(enemy) > 0f ? "да" : "нет");
_builder.Append(" (");
_builder.Append(_controlEffects != null ? _controlEffects.GetSlowAmount(enemy).ToString("0.##") : "0");
_builder.Append(")\n");
_builder.Append("Ground: ");
_builder.Append(IsEnemyInGround(enemy) ? "да" : "нет");
_builder.Append('\n');
_builder.Append("Knockback: ");
_builder.Append(enemy != null && enemy.KnockbackRemaining > 0f ? "да" : "нет");
_builder.Append(" (");
_builder.Append(enemy != null ? enemy.KnockbackRemaining.ToString("0.##") : "0");
_builder.Append(")\n");
```

2. Добавить вспомогательные методы:
```csharp
private static float ActualSpeed(EnemyController enemy)
{
    if (enemy == null || enemy.data == null)
        return 0f;

    return enemy.data.speed * enemy.speedMultiplier * enemy.controlSpeedMultiplier;
}

private static bool IsEnemyInGround(EnemyController enemy)
{
    if (enemy == null)
        return false;

    for (int i = 0; i < GroundZone.Active.Count; i++)
    {
        GroundZone zone = GroundZone.Active[i];
        if (zone != null && zone.Contains(enemy))
            return true;
    }

    return false;
}
```

3. Упростить `BuildStatusText()` до агрегатов (убрать per-enemy строки Burn остаток / Freeze стаки / Frozen — они теперь в `BuildEnemyBlock`):
```csharp
private string BuildStatusText()
{
    _builder.Clear();
    _builder.Append("<b>Статусы</b>\n");
    _builder.Append("Врагов активно: ");
    _builder.Append(EnemyController.Active.Count);
    _builder.Append('\n');
    _builder.Append("Burn активно: ");
    _builder.Append(_statusEffects != null ? _statusEffects.ActiveBurnCount : 0);
    _builder.Append('\n');

    return _builder.ToString();
}
```

## Ограничения
- `ActualSpeed` = `data.speed * speedMultiplier * controlSpeedMultiplier` (формула движения из `EnemyController.Update`).
- int — без дробей (`GetFreezeStacks`, `Active.Count`), float — `"0.##"`.
- `GroundZone.Active`/`Contains`/`GetSlow*`/`KnockbackRemaining` — из T098; `GetBurnRemaining`/`ActiveBurnCount` — T079; `GetFreezeStacks`/`IsFrozen` — T079.
- `enemy.data` может быть null до `Init` — защищено в `ActualSpeed`.

## Unity (человеку)
Не требуется.

## Проверка человеком
- Проект компилируется без ошибок.
- Play Mode → Start Round → hover/pin на враге: блок «Враг» показывает «Скорость» = `speed × speedMultiplier × controlSpeedMultiplier` (на обычном враге равна `data.speed × speedMultiplier`).
- Наложить Slow (турель с on-hit-модулем Slow) → «Slow: да (amount)»; Frozen → «Frozen: да», «Скорость: 0.00».
- Наложить Ground → «Ground: да»; Knockback → «Knockback: да (остаток)».
- Блок «Статусы» показывает только «Врагов активно» и «Burn активно».

## Отчёт Исполнителя
Готово: Assets/Scripts/Dev/DebugOverlayController.cs: `BuildEnemyBlock` дополнен строками «Скорость», «Burn остаток», «Freeze стаки», «Frozen», «Slow (amount)», «Ground», «Knockback (остаток)»; добавлены `ActualSpeed` и `IsEnemyInGround`; `BuildStatusText` упрощён до агрегатов (Врагов активно, Burn активно).
Блокер: —
