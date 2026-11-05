# T140: ShotSystem — подключить AoEController и ZoneController
- Status: [ ]
- Phase: 15
- Depends: T135, T136
- Files:
  - Assets/Scripts/Combat/Shooting/ShotSystem.cs

## Задача
Подключить площадные контроллеры к оркестратору.

1. Добавить поля: `private AoEController _aoe;` и `private ZoneController _zone;`.
2. В `Awake()`: `_aoe = new AoEController(); _zone = new ZoneController();`.
3. В `TickShot`: добавить в `switch (shot.type)`:
   - `case DeliveryType.AoE: _aoe.Tick(shot.ctx, dt); break;`
   - `case DeliveryType.Zone: _zone.Tick(shot.ctx, dt); break;`
4. `Fire` (`switch (request.type)` для `ShotView`) не менять: `AoE`/`Zone` не имеют постоянного `ShotView`.

## Ограничения
- Не менять существующие контроллеры и case-ы.
- Без аллокаций в `Update` (контроллеры создаются один раз в `Awake`).

## Unity (человеку)
Не требуется.

## Проверка человеком
- (совместно со smoke) взрыв тикается мгновенно, зона живёт `duration` и тикает `tickRate`.

## Отчёт Исполнителя
Готово:
Блокер:
