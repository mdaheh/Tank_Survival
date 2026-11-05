# T132: Непрерывный выстрел следует за турелью (origin в реальном времени)
- Status: [x]
- Phase: 14
- Depends: T130
- Files:
  - Assets/Scripts/Combat/Shooting/ShotSystem.cs
  - Assets/Scripts/Combat/TurretController.cs

## Задача
Непрерывные выстрелы (ContinuousHitscan — лазер, Cone — огнемёт) пишут точку старта `ctx.position` только один раз в `ShotSystem.Fire` (из `SpawnRequest.position`, который `TurretController` снимает на момент выстрела) и дальше её не трогают. Контроллеры (`ContinuousHitscanController`, `ConeController`) только доворачивают `ctx.direction` к цели, но `origin` остаётся в старой точке, поэтому при движении танка луч/конус «жжёт» из покинутой точки. Привязать точку старта живого непрерывного выстрела к текущей позиции турели.

1. `Assets/Scripts/Combat/Shooting/ShotSystem.cs` — добавить public-метод:
   ```
   public bool SetOrigin(int id, Vector3 position)
   ```
   Пройти `_active` индексным циклом; если найден `ActiveShot` с `id == id` — присвоить `shot.ctx.position = position;` и вернуть `true`; иначе вернуть `false`. Без LINQ и аллокаций.

2. `Assets/Scripts/Combat/TurretController.cs` — в `Update()`, внутри ветки `if (continuous)` (после уже существующей проверки `continuous && tickRate <= 0f` и уже выполненного поиска `closest`) заменить текущий блок:
   ```
   if ((_continuousShotId == -1 || !shotSystem.IsAlive(_continuousShotId)) && closest != null)
   {
       Vector3 dir = (closest.transform.position - position).normalized;
       _continuousShotId = shotSystem.Fire(_factory.Build(turret.delivery, position, dir, statBlock.damage));
   }

   return;
   ```
   на:
   ```
   if (_continuousShotId != -1 && shotSystem.IsAlive(_continuousShotId))
   {
       shotSystem.SetOrigin(_continuousShotId, position);
       return;
   }

   if (closest != null)
   {
       Vector3 dir = (closest.transform.position - position).normalized;
       _continuousShotId = shotSystem.Fire(_factory.Build(turret.delivery, position, dir, statBlock.damage));
   }

   return;
   ```
   (`position` — уже вычисленная выше `transform.position`.) Повторный `Fire` выполняется только когда выстрел мёртв; пока жив — только обновляем origin.

## Ограничения
- Не менять `ContinuousHitscanController`, `ConeController`, `InstantHitscanController`, `ShotFactory`, `ShotContext`, `SpawnRequest`.
- Наведение (ближайший враг в радиусе) и «один автономный выстрел на турель» остаются как есть: `SetOrigin` не выбирает цель и не меняет `ctx.direction` (его доворачивает контроллер).
- В `Update`/`SetOrigin` — без `new`, LINQ, `GetComponent`, `Find*`; итерация по `_active` индексная.
- Поведение instant/projectile не менять.

## Unity (человеку)
Не требуется (новых сериализованных полей нет).

## Проверка человеком
- Play Mode (Editor), Laser (ContinuousHitscan): жёлтый debug-луч (`DevVisualizer.ShowBeam`) начинается из ствола турели; при движении танка точка старта луча едет вместе с турелью, враг продолжает получать урон от актуального положения, а не из «покинутой» точки.
- Play Mode (Editor), Flamethrower (Cone): урон в конусе и debug-сектор (`DevVisualizer.ShowSector`) следуют за турелью при её движении; без врага в радиусе конус гаснет (External), при появлении врага снова активируется.
- Мгновенные выстрелы (Hammer, мгновенный hitscan) работают как до правки.

## Отчёт Исполнителя
Готово: Assets/Scripts/Combat/Shooting/ShotSystem.cs: добавлен public `bool SetOrigin(int id, Vector3 position)` — индексный проход по `_active`, при совпадении `id` присваивает `shot.ctx.position = position` и возвращает `true`, иначе `false`; Assets/Scripts/Combat/TurretController.cs: в `Update()` в ветке `if (continuous)` живой непрерывный выстрел обновляет origin через `SetOrigin(_continuousShotId, position)` и выходит, повторный `Fire` выполняется только при мёртвом выстреле и наличии `closest`.
Блокер: —
