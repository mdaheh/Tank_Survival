# T081: DevVisualizer + отладочные маркеры и плейсхолдеры шасси/турели
- Status: [x]
- Phase: 10.5
- Depends: T078
- Files:
  - Assets/Scripts/Dev/DevVisualizer.cs
  - Assets/Scripts/Dev/DevMenuController.cs
  - Assets/Scripts/Combat/HitscanDeliveryHandler.cs
  - Assets/Scripts/Combat/ConeDeliveryHandler.cs
  - Assets/Scripts/Combat/ExplosionOnHitHandler.cs
  - Assets/Scripts/Combat/GroundZone.cs

## Задача
Создать dev-визуализатор `DevVisualizer` (в `#if UNITY_EDITOR || DEBUG`),
самоинициализирующийся через `RuntimeInitializeOnLoadMethod` и переиспользующий маркеры из пула
(ноль `Instantiate`/аллокаций в игровом цикле):
- `ShowBeam(Vector3 from, Vector3 to, float seconds, Color color)` — линия через `LineRenderer`.
- `ShowSector(Vector3 origin, Vector3 flatDir, float angle, float range, float seconds, Color color)` —
  заполненный wedge-меш (фиксированное разрешение, 32 сегмента дуги). Меш строится один раз на каждый
  маркер по первому `angle`; в рантайме позиционируется/вращается и масштабируется под `range`
  (unit-радиус × `range`). Каждый маркер запоминает `lastAngle`: если `angle != lastAngle`, позиции
  вершин дуги пересчитываются в пре-аллоцированном `Vector3[]` (число вершин постоянно) и пишутся в тот же
  `Mesh` (`mesh.vertices = buffer`) — новая аллокация не создаётся, пересчёт только при изменении `angle`.
- `ShowCircle(Vector3 center, float radius, float seconds, Color color)` — окружность радиуса
  (кольцо `LineRenderer` с пре-аллоцированным буфером вершин; масштабируется под `radius`).

Пул маркеров — пре-варм до `initial`, рост по требованию до `max`, при превышении `max`
переиспользуется самый старый маркер того же типа:
- Beam: initial 8, max 32.
- Sector: initial 4, max 16.
- Circle: initial 16, max 64.

Материалы — кодом (`Shader.Find("Unlit/Color")`, полупрозрачные); публичное свойство `Enabled` (тумблер).

Добавить хуки по 1–2 строки внутрь `#if UNITY_EDITOR || DEBUG` на фактические значения:
- `HitscanDeliveryHandler.Fire` → `DevVisualizer.ShowBeam(origin, end, 0.15f, цвет)` (значения `origin`/`end` из метода).
- `ConeDeliveryHandler.Fire` → `DevVisualizer.ShowSector(origin, flatDir, module.angle, module.range, 0.25f, цвет)`.
- `ExplosionOnHitHandler.HandleHit` → `DevVisualizer.ShowCircle(position, module.radius, 0.3f, цвет)`.
- `GroundZone.Init` → `DevVisualizer.ShowCircle(position, module.radius, module.duration, цвет)`.

В `DevMenuController` (T078) добавить тумблер «Маркеры», привязанный к `DevVisualizer.Enabled`.

## Ограничения
- Весь код маркеров — в `#if UNITY_EDITOR || DEBUG`; в релизе отсутствует.
- Маркеры читают `module.angle`/`module.range`/`module.radius` из тех же полей SO, что и боевая логика
  (визуально совпадают с реальной зоной/дальностью), не захардкоженные значения.
- Геометрические поля остаются редактируемыми в Dev Menu (T077, без фильтра). Синхронизация маркера:
  `range`/`radius` — масштаб трансформа; `angle` — пересчёт дуги при изменении (`lastAngle`), чтобы маркер
  всегда совпадал с реальной зоной поражения.
- Логику стрельбы/попаданий не менять (хуки аддитивные).
- Пул маркеров — переиспользуемые инстансы; меш сектора строится один раз (пересчёт дуги — только при изменении `angle`).

## Unity (человеку)
- На `Player` добавить дочерний `ChassisVisual` (Cube), отмасштабированный под габариты шасси,
  с плейсхолдер-материалом. (production-визуал, не dev-only.)
- На каждую тестовую турель (`Turret`, `Turret_Flamethrower`, `Turret_Laser`, `Turret_Beam`)
  добавить дочерний `TurretVisual` (Cube/Cylinder) — корпус турели; назначить плейсхолдер-материал.
  (production-визуал, не dev-only.) Направление ствола визуально показывает dev-маркер луча/сектора,
  т.к. поворот турели к цели — логика Phase 12 и сейчас не реализуется.
- `TurretMountPoint` сейчас не вводить: `ChassisData`/`TurretData` не имеют полей `prefab`,
  сборка (PlayerManager) появится в Phase 12/13; до тех пор Turret остаётся дочерним Player.

## Проверка человеком
- Выстрел Hitscan оставляет линию от `muzzle` на `module.range` (~0.15 с) — видно попадание/промах.
- Cone рисует сектор с углом `module.angle` и радиусом `module.range`; враги в секторе получают урон, вне — нет.
- Изменение `angle` конуса в Dev Menu (60 → 30) перестраивает сектор маркера; маркер продолжает совпадать с реальной зоной поражения.
- Explosion рисует окружность радиуса `module.radius`; Ground — окружность радиуса зоны, живёт/пульсирует пока зона активна; при нескольких зонах одновременно окружности не пропадают (до max 64).
- Корпус шасси (`ChassisVisual`) виден на сцене; на активной турели виден `TurretVisual`.
- Маркеры отключаются тумблером в Dev Menu; в релизном WebGL-билде маркеров нет.

## Отчёт Исполнителя
Готово: Assets/Scripts/Dev/DevVisualizer.cs: создан dev-визуализатор (RuntimeInitializeOnLoadMethod AfterSceneLoad, статические ShowBeam/ShowSector/ShowCircle, пул с пре-вармом Beam 8/32, Sector 4/16, Circle 16/64 и переиспользованием самого старого при превышении max, Beam — LineRenderer с буфером из 2 точек, Sector — wedge-меш на 32 сегмента в пре-аллоцированном Vector3[] с пересчётом дуги только при изменении angle (lastAngle), Circle — кольцо LineRenderer из 64 пре-аллоцированных точек, масштаб трансформа под range/radius, цвет через vertex colors/MaterialPropertyBlock, свойство Enabled с мгновенным скрытием маркеров, всё в #if UNITY_EDITOR || DEBUG); Assets/Scripts/Dev/DevMenuController.cs: добавлен тумблер «Маркеры», привязанный к DevVisualizer.Enabled; Assets/Scripts/Combat/HitscanDeliveryHandler.cs: хук ShowBeam(origin, end, 0.15f, цвет) в Fire; Assets/Scripts/Combat/ConeDeliveryHandler.cs: хук ShowSector(origin, flatDir, module.angle, module.range, 0.25f, цвет) в Fire; Assets/Scripts/Combat/ExplosionOnHitHandler.cs: хук ShowCircle(position, module.radius, 0.3f, цвет) в HandleHit; Assets/Scripts/Combat/GroundZone.cs: хук ShowCircle(position, module.radius, module.duration, цвет) в Init (во всех четырёх файлах using TankSurvival.Dev и хуки обёрнуты в #if UNITY_EDITOR || DEBUG, логика не изменена).
Блокер: —
