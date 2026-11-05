# T141: TurretController — on-hit через ShotFactory, убрать OnHitDispatcher
- Status: [ ]
- Phase: 15
- Depends: T139
- Files:
  - Assets/Scripts/Combat/TurretController.cs

## Задача
Перевести on-hit модули турели на новый пайплайн и убрать legacy-точку входа.

1. Убрать поле `public OnHitDispatcher onHit;`.
2. Удалить метод `Start()` (он больше ничего не делает после удаления `onHit.SetModules(...)`).
3. В `Update()` оба вызова заменить на новую перегрузку с передачей `turret.onHitModules` вторым аргументом:
   - continuous-ветка: `shotSystem.Fire(_factory.Build(turret.delivery, turret.onHitModules, position, dir, statBlock.damage));`
   - instant/projectile-ветка: `shotSystem.Fire(_factory.Build(turret.delivery, turret.onHitModules, position, shotDirection, statBlock.damage));`
4. Убедиться, что `using TankSurvival.Combat.Shooting;` присутствует; ссылку на `OnHitDispatcher` больше не использовать.

## Ограничения
- Не менять логику наведения (ближайший враг в радиусе) и кулдаун для instant/projectile.
- `turret.onHitModules` может быть `null` или пустым — передавать как есть (ShotFactory обрабатывает в T139).

## Unity (человеку)
1. Объект `Turret` → компонент `TurretController` → поле `On Hit` очистить (больше не используется).

## Проверка человеком
- Inspector: у `TurretController` больше нет поля `On Hit`.
- Play Mode: on-hit модули, назначенные турели, работают через новую систему (smoke фазы); урон не дублируется.

## Отчёт Исполнителя
Готово:
Блокер:
