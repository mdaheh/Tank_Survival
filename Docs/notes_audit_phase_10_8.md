# Аудит кода после Phase 10.8

**Дата:** 2026-10-25
**Источник:** Архитектор, по запросу на аудит всего проекта.
**Статус:** информация к рассмотрению. Не является планом работ. 
Приоритезация — за Архитектором после завершения блока Dev Tools.

---

## 1. Общее состояние

Архитектурный фундамент здоровый, WebGL-принципы соблюдены:
- Нет `async/await`, `Task`, DOTS/ECS/Burst/Jobs.
- `Instantiate` только в `PoolManager` (Prewarm, ленивый Get).
- LINQ в runtime отсутствует, списки кэшируются.
- Одна сцена, рестарт без `LoadScene`.
- Контент — ScriptableObject.
- Dev-слой изолирован `#if UNITY_EDITOR || DEBUG`.

Прямых нарушений WebGL-ограничений не найдено.

---

## 2. Находки (по категориям)

### 2.1. `OnHitDispatcher` — switch вместо реестра

**Факт:** `Architecture.md` и `Technical Requirements §9` декларируют 
«новые модули добавляются без переписывания существующих», on-hit 
описан как «список `IHitHandler` по priority».

**Реальность:** `OnHitDispatcher` — жёсткий switch по `module.kind` 
с шестью захардкоженными полями (`explosion`, `burn`, `ground`, 
`slow`, `freeze`, `knockback`). Интерфейс `IHitHandler` есть, 
диспетчер им не пользуется.

**Следствие:** Phase 11 (Pierce, Homing, Beam Split, Laser Focus) — 
каждый модуль потребует новое поле + новый case + новый GameObject 
в сцене + проводку.

### 2.2. `PoolManager.Release` — нет guard от двойного возврата

**Факт:** `Release` не проверяет, что объект активен / не в пуле.

**Сценарий:**
1. Враг умирает → `EnemyKillSystem.Enqueue(gameObject)`.
2. В том же кадре игрок умирает → `RoundController.HandlePlayerDeath` 
   → `WaveSpawner.ClearActiveEnemies()` → `poolManager.Release(враг)`.
3. `EnemyKillSystem.LateUpdate` выпускает того же врага ещё раз.

**Результат:** один `GameObject` попадает в очередь пула дважды, 
следующий `Get` вернёт объект из двух слотов — рассинхронизация пула.

### 2.3. Статическое состояние vs Play Mode «Do not reload Domain»

**Факт:** три статических хранилища:
- `EnemyController.Active` (`static readonly List<EnemyController>`);
- `GameServices._services` (`static Dictionary<Type, object>`);
- dev: `DevParamRegistry.All`, `DevParamRegistry._playModeHookInstalled`.

**Проблема:** при настройках Enter Play Mode с выключенными Domain/
Scene Reload (используется намеренно для скорости) статика переживает 
выход из Play Mode. `GameServices` не имеет `Unregister`/`Clear` — 
после уничтожения объекта в словаре остаётся fake-null ссылка. Нет 
явной точки сброса статики между сессиями.

### 2.4. `PlayerHealth.TakeDamage` — нет guard после смерти

**Факт:** после `currentHealth <= 0` урон продолжает проходить, 
`OnDeath` срабатывает повторно. `RoundController.HandlePlayerDeath` 
вызывается несколько раз подряд.

**Примечание:** сейчас идемпотентно, но скрытая мина при усложнении 
логики Lose.

---

## 3. Среднеприоритетные несостыковки

### 3.1. `GetComponent` в игровом цикле

**Факт:** 7 точек:
- `WaveSpawner.SpawnNext` (из `Update`): `go.GetComponent<EnemyController>()`;
- `EnemyController.Update` (ranged): `p.GetComponent<EnemyProjectile>()`;
- `EnemyProjectile.Init`: `target.GetComponent<IDamageable>()`;
- `ProjectileDeliveryHandler.Fire`: `projectile.GetComponent<Projectile>()` 
  в цикле по `pelletCount`;
- `ConeDeliveryHandler.Fire`: `vfx.GetComponent<PooledVFX>()`;
- `ExplosionOnHitHandler.HandleHit`: `vfx.GetComponent<PooledVFX>()`;
- `GroundOnHitHandler.HandleHit`: `zone.GetComponent<GroundZone>()`.

**Примечание:** `PoolManager` кэширует `IPoolable[]`, но не кэширует 
конкретные компоненты.

### 3.2. Смешение двух подходов к DI

**Факт:** `GameServices` (static service locator) введён в T075. При 
этом composition root (`GameBootstrap`) по-прежнему связывает всё 
через `[SerializeField]`. `PoolManager` — и сериализованная ссылка, 
и зарегистрированный сервис. Два источника правды.

### 3.3. `TurretController` — 3 жёстких поля delivery

**Факт:** `delivery`, `hitscanDelivery`, `coneDelivery` — три ссылки, 
выбор через `if/else` по `kind`. Добавление нового delivery-типа 
требует правки `TurretController`.

### 3.4. `EnemyData.behavior` — строка вместо enum

**Факт:** `"melee"` / `"ranged"` — магическая строка, сравнивается в 
`EnemyController.Update` каждый кадр. `ChassisData.abilityId` — 
та же история.

---

## 4. Мелкие находки

- **`PlayerController`:** `input.x * Mathf.Sign(input.y)` — поворот 
  зеркалится при движении назад.
- **`namespace TankSurvival.Camera`** конфликтует с `UnityEngine.Camera` 
  (при совместном using — CS0104).
- **`Time.timeScale`** управляется в 3 местах (`LevelUpUI`, 
  `PauseMenuController`, `RoundEndUI`) без единого владельца.
- **Два реестра живых врагов:** `EnemyController.Active` и 
  `WaveSpawner._active`.
- **Локализация:** `Technical Requirements §8` требует «не хардкодить», 
  фактически все строки — хардкод. В документации не отражено.
- **Нет автотестов** при установленном `com.unity.test-framework`.
- **Микрооптимизации:** selection sort O(n²) в `HitscanDeliveryHandler` 
  при `maxTargets ≈ n`; `Vector3.Angle` (acos) в `ConeDeliveryHandler` 
  на каждый тик continuous.
- **`Progression.AddXp`:** XP накапливается после level cap (30-й 
  уровень).

---

## 5. WebGL-специфичное

1. **Статика в WebGL живёт весь жизненный цикл приложения**, не только 
   Play Mode. Сброс статических коллекций сейчас косвенный (через 
   `OnEnable`/`OnDisable`).
2. **Ленивый `Instantiate` при исчерпании пула** — единственная точка 
   аллокации в игровом цикле. Сценарии: `pelletCount` (Dev Menu может 
   выставить 25), `EnemyProjectile` (Shooter при большом числе). Лимиты 
   не enforced.

---

## 6. Устаревание `Architecture.md`

### §1 Диаграмма
Не содержит: `ControlEffectSystem`, `EnemyKillSystem`, `GroundZone`, 
`EnemyProjectile`, `OnHitDispatcher` (описан как «список IHitHandler»), 
`GameServices`, `CameraFollow`, `PauseMenuController`, весь Dev-слой.

### §2 Владение состоянием
Не описывает: статусы slow/freeze (`ControlEffectSystem`), очередь 
отложенного release (`EnemyKillSystem`), реестр сервисов 
(`GameServices`), dev-параметры (`DevParamRegistry`).

### §3 Ключевые решения
Не зафиксированы: deferred release (Phase 9.5), service locator 
`GameServices`, «Dev Menu как экран перед раундом» (10.8), маршрутизация 
delivery по `kind` через 3 поля.

### §4 «Заложено под v1.0»
Частично реализовано, но не перенесено: враги v1.0 (Shooter/Brute/Tank/
Boss), continuous hitscan, Cone, Freeze/Slow/Knockback/Ground, 
камера+аркадное движение, каркас паузы, `LevelCap = 30` — всё в коде 
(Phase 8–10.8), но в документе значится как «заложено/не реализуется».

### §5 «Файлы .cs для MVP»
Устарел: заявлено 34 файла, фактически ~55. Нет `Assets/Scripts/Dev/` 
(6 файлов), `Assets/Scripts/Camera/`, `EnemyProjectile`, 
`EnemyKillSystem`, `ControlEffectSystem`, `OnHitDispatcher`, 
`GroundZone`, `PooledVFX`, `PauseMenuController`. Namespace не 
включает `TankSurvival.Dev`, `TankSurvival.Camera`.

### §6 Структура папок
Не упомянуты: `Assets/Scripts/Dev/`, `Assets/Scripts/Camera/`, 
`Assets/UI/`.

---

## 7. `Phase_map.md`

**Факт:** Phase 9.5 отсутствует в карте. Файл `Phase_9.5_v1.md` 
существует и закрыт, но в карте идёт сразу Phase 9 → Phase 10.