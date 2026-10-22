# T080: SceneOrganizer — группировка иерархии сцены
- Status: [x]
- Phase: 10.5
- Files:
  - Assets/Editor/SceneOrganizer.cs

## Задача
Создать Editor-скрипт `TankSurvival.Editor.SceneOrganizer` с пунктом меню
`Tools/Dev/Organize Scene Hierarchy`. Метод перебирает только корневые объекты сцены
(`transform.parent == null`) и раскладывает их по пустым GameObject-заголовкам,
создаваемым при первом запуске (приоритет при нескольких категориях — в порядке перечисления):
- `_Managers` — компоненты: GameBootstrap, PoolManager, SessionState, RoundController,
  WaveSpawner, EnemyKillSystem, ProgressionSystem, StatBlock.
- `_Player` — компонент PlayerController (вместе со всеми дочерними, включая Turret).
- `_Combat` — компоненты: StatusEffectSystem, ControlEffectSystem, SlowOnHitHandler,
  FreezeOnHitHandler, BurnOnHitHandler, KnockbackOnHitHandler, ExplosionOnHitHandler, GroundOnHitHandler.
- `_SpawnPoints` — компонент SpawnPoint.
- `_UI` — компоненты: HUDController, LevelUpUI, RoundEndUI, Canvas, EventSystem.
- `_Dev` — компоненты с именем типа, начинающимся на `Dev` (зарезервировано под Phase 10.5/дальше).

Все изменения — через `Undo.RecordObject`/`Undo.SetTransformParent` (отменяемо). Объекты,
не подходящие ни под одну категорию, не трогать.

## Ограничения
- Editor-only (папка `Assets/Editor`), в сборку не попадает.
- Не менять имена объектов и их вложенность внутри категорий (только перемещение корневых объектов).
- Не удалять и не создавать игровые объекты, кроме пустых заголовков.

## Unity (человеку)
- Открыть сцену `Main`. Меню `Tools → Dev → Organize Scene Hierarchy`. Проверить, что
  иерархия сгруппирована, дочерние связи игровых объектов не нарушены (Turret остался под Player).
- При необходимости вручную подправить раскладку (Undo доступен).

## Проверка человеком
- Иерархия сцены содержит заголовки `_Managers`, `_Player`, `_Combat`, `_SpawnPoints`, `_UI`, `_Dev`;
  объекты распределены по категориям, визуальный шум убран.
- Turret и тестовые турели (`Turret_Flamethrower`/`Turret_Laser`/`Turret_Beam`) остались дочерними Player.
- Игра после группировки работает как раньше (регресс не сломан).

## Отчёт Исполнителя
Готово: Assets/Editor/SceneOrganizer.cs: создан Editor-скрипт TankSurvival.Editor.SceneOrganizer с пунктом меню Tools/Dev/Organize Scene Hierarchy — создаёт отсутствующие пустые заголовки _Managers/_Player/_Combat/_SpawnPoints/_UI/_Dev (Undo.RegisterCreatedObjectUndo) и раскладывает по ним только корневые объекты активной сцены (SceneManager.GetActiveScene().GetRootGameObjects() + проверка transform.parent == null) через Undo.SetTransformParent, приоритет при нескольких категориях — в порядке перечисления (Managers → Player → Combat → SpawnPoints → UI → Dev), _Dev матчит компоненты с именем типа, начинающимся на Dev, объекты без совпадений не перемещаются, имена и вложенность внутри категорий не меняются, сцена помечается EditorSceneManager.MarkSceneDirty.
Блокер: —
