# T043: XP за убийство — WaveSpawner
- Status: [x]
- Phase: 6
- Depends: T037, T041
- Files:
  - Assets/Scripts/Waves/WaveSpawner.cs

## Задача
В `WaveSpawner` (namespace `TankSurvival.Waves`, добавить `using TankSurvival.Progression;`):
- добавить публичное поле `public ProgressionSystem progression;`.
- в методе спавна врага расширить замыкание подписки на смерть, чтобы при смерти также вызывался
  `progression.AddXp(...)`: строка `controller.health.OnDeath += () => HandleEnemyDeath(controller);`
  заменяется на `controller.health.OnDeath += () => { HandleEnemyDeath(controller); progression.AddXp(controller.data.xpReward * difficulty.xpMultiplier); };`

## Ограничения
- Подписка остаётся ДО `controller.Init` (порядок не менять).
- `Instantiate`/`Destroy` не добавлять; `AddXp` вызывается только из обработчика смерти (событие), не в `Update`.

## Unity (человеку)
- Объект `WaveSpawner` → поле `progression` → компонент `ProgressionSystem`.

## Проверка человеком
- Инспектор `WaveSpawner.progression` заполнен.
- Play Mode: убийство врага увеличивает `ProgressionSystem.xp` (видно в инспекторе).

## Отчёт Исполнителя
Готово: Assets/Scripts/Waves/WaveSpawner.cs — using TankSurvival.Progression, поле progression, в подписке OnDeath при смерти вызывается progression.AddXp(xpReward * difficulty.xpMultiplier); подписка осталась до controller.Init.
Блокер: —
