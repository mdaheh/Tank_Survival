# Architecture (MVP)

Целевая архитектура прототипа. MVP доказывает, что системы работают через
данные (ScriptableObject) и пулы, а не через хардкод, и что цепочка
«спавн → урон → смерть → XP → level-up → победа/поражение» замкнута.

## 1. Диаграмма подсистем

```
                        [ Data (SO) ]  — читается всеми, никем не изменяется в рантайме
  ChassisData / TurretData / EnemyData / WaveData / DifficultyData /
  UpgradeData / DeliveryModuleData / OnHitModuleData

                              │ читают (только чтение)
                              ▼
                      [ GameBootstrap ]  — композиция зависимостей, prewarm пулов
                              │
        ┌─────────────────────┼──────────────────────────┐
        ▼                     ▼                          ▼
 [ SessionState ]     [ RoundController ]          [ PoolManager ]
 выбранные SO          фазы раунда, победа/        выдаёт/возвращает
 (не сбрасывается)     поражение, сброс без         инстансы (единственный
                       LoadScene                   владелец объектов)
        │                     │                          │
        │            ┌────────┴─────────┐                ├──► [ Enemy ]       (пул)
        │            ▼                  ▼                ├──► [ ShotView ]   (пул)
        │     [ WaveSpawner ]    [ Progression ]        └──► [ VFX ]         (пул)
        │            │                  │
        │            ▼                  │ пишет апгрейды
        │     [ Enemy ] ── смерть → XP ─┘
        │            │                        ▼
        │            ▼                  [ StatBlock ]   — единый изменяемый
        │     (конец волны)              damage/fire_rate/move_speed/max_health
        │            │                        ▲ читают
        │            └──────────► [ RoundController ] (конец волны / победа)
        │
        ├──► [ PlayerController ] ── движение ──► StatBlock.move_speed
        ├──► [ PlayerHealth ] ── HP / урон / смерть ──► RoundController (поражение)
        │
        └──► [ TurretController ] ── кулдаун (aps из StatBlock) ──► [ ShotFactory ]
                                        ├──► SpawnRequest → [ ShotContext ] + контроллер
                                        │       (Projectile / Hitscan / Cone / AoE) ──► IDamageable
                                        └──► модули (IOnSpawn / IOnFlight / IOnHit / IOnPostHit / IOnDestroy)
                                                ├── Explosion / BeamSplit ──► spawnQueue (дочерний ShotContext)
                                                ├── Burn / Slow / Freeze / Knockback ──► Status / ControlEffectSystem
                                                └── ShotView / VFX (IVfxSpawner) — косметика, без логики

                     [ HUD ]  ← подписан на события RoundController / Progression /
                                PlayerHealth / WaveSpawner (только чтение, не пишет состояние)
```

Взаимодействия (сводно):

| Источник | Получатель | Характер |
|---|---|---|
| GameBootstrap | все подсистемы | создаёт и связывает (composition root) |
| Все подсистемы | Data (SO) | чтение статов/параметров |
| WaveSpawner | PoolManager | взять/вернуть Enemy |
| EnemyHealth (смерть) | WaveSpawner | событие смерти → декремент живых врагов волны |
| TurretController | ShotFactory / ShotContext | делегирование выстрела (SpawnRequest) |
| Контроллеры/модули выстрела | IDamageable | единый контракт нанесения урона |
| EnemyHealth (смерть) | Progression | событие «+XP» |
| Progression | StatBlock | применение выбранного апгрейда |
| PlayerHealth / WaveSpawner | RoundController | события смерти игрока / конца волны |
| HUD | события подсистем | только чтение для отображения |

## 2. Правила владения состоянием

| Подсистема | Чем владеет | Кто имеет право писать | Где сбрасывается |
|---|---|---|---|
| Data (SO) | статический контент (статы, модули, волны, сложности) | никто в рантайме (только чтение) | не сбрасывается |
| SessionState | ссылки на выбранные Chassis/Turret/Difficulty SO | выбор в инспекторе (MVP) | переживает рестарт раунда |
| StatBlock | производные статы: damage, fire_rate, move_speed, max_health | только Progression (применение апгрейда) | начало раунда — пересчёт от базовых SO |
| Progression | XP, уровень, взятые апгрейды (стек), флаг level-up | Progression (XP от убийств); LevelUpUI (выбор карточки) | начало раунда |
| RoundController | фаза (Playing/LevelUp/Win/Lose), номер волны, kills за забег | RoundController + входящие события | начало раунда |
| WaveSpawner | очередь спавна, таймер интервала, счётчик живых врагов волны | WaveSpawner (спавн и декремент по событию смерти от EnemyHealth) | начало раунда / начало волны |
| PlayerHealth | текущее HP игрока | IDamageable-потребители (наносят урон) | начало раунда: HP = StatBlock.max_health |
| EnemyHealth | текущее HP врага, DoT-статус | IDamageable-потребители, StatusEffectSystem | при возврате в пул (OnReleaseFromPool) |
| TurretController | кулдаун выстрела, текущая цель | TurretController | начало раунда |
| ShotContext | состояние выстрела (стадия, age, position/direction, keepAlive, hitTargets, spawnQueue) | контроллер + модули (только через ctx) | Reset при Spawn; возврат в пул при Dead |
| StatusEffectSystem | активные DoT-статусы врагов | модули OnHit (контракты), система (тик) | смерть врага / рестарт раунда |
| PoolManager | списки свободных/занятых инстансов | только PoolManager (get/release) | при загрузке сцены (prewarm) |
| HUD | отображаемые значения | никто из игровых подсистем; HUD читает события | обновляется по событиям |

**Правило сброса:** единственный механизм очистки объекта, возвращаемого в пул, —
`IPoolable.OnReleaseFromPool`. Второго механизма сброса состояния не существует.
Сброс раунда = ресет состояний подсистем (StatBlock/Progression/RoundController/
WaveSpawner/PlayerHealth/TurretController/StatusEffectSystem) без перезагрузки сцены.

**Правило `EnemyController.Active`:** живой реестр активных врагов. Наполняется и очищается
только в `OnEnable`/`OnDisable` (спавн/возврат в пул). Смерть врага не мутирует `Active`
сразу: `TakeDamage` → `OnDeath` лишь помечает врага мёртвым (`currentHealth <= 0`) и
рассылает бухгалтерию (XP, счётчик волны), а фактическое освобождение
(`pool.Release` → `SetActive(false)` → `OnDisable` → `Active.Remove`) выполняет
`EnemyKillSystem` одной отложенной фазой в `LateUpdate`. Поэтому `Active` стабилен на всём
протяжении игровой логики кадра: циклы (AoE, конус, hitscan, поиск цели) итерируют его
напрямую, без снапшотов. Мёртвых (ещё не возвращённых в пул) врагов потребители пропускают
по `currentHealth <= 0`.

## 3. Ключевые решения (WebGL-first)

- **Одна сцена на всю сессию.** Сброс раунда — ресет состояния + возврат объектов
  в пулы, без `SceneManager.LoadScene`: перезагрузка сцены в WebGL дорогая и рушит пулы.
- **Весь контент — ScriptableObject.** Код читает данные, не знает конкретных чисел;
  новый контент добавляется `.asset` без правки кода.
- **Контракт урона и контракты стадий выстрела.** `IDamageable` — «это можно
  повредить» (враг, игрок), единая точка получения урона для всех путей доставки;
  контракты `IOnSpawn`/`IOnFlight`/`IOnHit`/`IOnPostHit`/`IOnDestroy` — реакция
  модулей на стадии жизненного цикла выстрела (см. `Docs/Shooting.md`).
- **Модули выстрела — отдельный слой-композиция** (turret → `BuildSnapshot` модулей),
  а не наследование конкретных турелей: новые модули добавляются без переписывания существующих.
- **`StatBlock` — единственная изменяемая точка статов игрока.** Апгрейды пишут в него,
  акторы (движение/стрельба/HP) только читают: эффект сохраняется между спавнами.
- **Generic Object Pool + контракт `IPoolable`.** Prewarm при загрузке, ноль
  `Instantiate/Destroy` в игровом цикле (критерий MVP #10, ограничение WebGL).
- **Ноль аллокаций в `Update/FixedUpdate` горячих циклов.** Кэшированные списки,
  без LINQ/замыканий/`GetComponent` в цикле — целевые 60 FPS в браузере.
- **Запрет DOTS/ECS/Burst/Jobs и `async/await`/`Task`.** Не работают стабильно в WebGL
  (прямое требование Technical Requirements).
- **Спавн-точки и префабы — ссылки в сцене/данных, не в коде.** Карта и контент меняются
  без рекомпиляции.
- **Событийная модель переходов** (конец волны, смерть, level-up): RoundController
  подписан на события, а не опрашивает состояние в `Update` — предсказуемые переходы фаз.
- **Каждая механика с пространственными/временными параметрами обязана иметь
  отладочную визуализацию.** Механики с радиусом/углом/дальностью/длительностью
  (hitscan, конус, AoE-взрыв, ground-зона; в будущем — Pierce, Homing, способности
  шасси) получают dev-маркер (линия/сектор/окружность), визуально совпадающий с
  реальными параметрами из SO. Маркер живёт в dev-слое
  (`#if UNITY_EDITOR || DEBUG`), в релиз не попадает и не влияет на геймплей.

## 4. Заложено под v1.0

- Активные способности шасси: поле `ability` в `ChassisData` зарезервировано, в MVP не используется.
- Continuous hitscan (Laser): поле `duration` в Hitscan-конфиге заложено, MVP использует только `instant`.
- Специализации турелей и полный набор delivery/on-hit модулей: слой контрактов стадий готов, добавляются новые `.asset`/модули без перестройки.
- Модульные апгрейды (wave reward на волнах 5/10/15/20): событийная точка начала волны есть, выбор модуля не реализуется.
- 8 сложностей и полная кривая уровней (30): `DifficultyData` и `ProgressionSystem` параметризованы, MVP — 2 сложности и линейная кривая.
- Мета-прогрессия и разблокировки: счётчик убийств за забег и точка сохранения (LocalStorage) зарезервированы, реализуются в v1.0.
- Финальные экраны (Main Menu/Hangar/Difficulty/Round End) и полноценная пауза: карта экранов заложена в UI-слое, MVP — временный выбор в инспекторе.
- Локализация RU/EN: точка входа для строк зарезервирована, MVP — русские плейсхолдеры.
- Дальнобойные враги (Shooter) и новые типы врагов: поле `behavior` в `EnemyData` заложено, MVP — только melee.
- Аудио/музыка, монетизация/SDK: не закладываются в MVP.

## 5. Файлы `.cs` для MVP

> **Namespace:** корень `TankSurvival`, подпространства по папкам — `TankSurvival.Data`,
> `TankSurvival.Core`, `TankSurvival.Round`, `TankSurvival.Player`, `TankSurvival.Combat`,
> `TankSurvival.Enemy`, `TankSurvival.Waves`, `TankSurvival.Progression`, `TankSurvival.UI`.

**Data (ScriptableObject) — `Assets/Scripts/Data/`**

- `ChassisData.cs` — SO шасси: health/speed/turn, зарезервированное поле способности.
- `TurretData.cs` — SO турели: ссылка на delivery-модуль, список on_hit-модулей, aps/range/damage.
- `EnemyData.cs` — SO врага: health/speed/damage/xp_reward, behavior (melee).
- `WaveData.cs` — SO волн: список составов волн, спавн-интервал.
- `DifficultyData.cs` — SO сложности: множители HP/speed/count/XP, число волн.
- `UpgradeData.cs` — SO апгрейда: целевой стат, величина, max_stacks.
- `DeliveryModuleData.cs` — SO delivery-модуля: kind (Projectile/Hitscan/Cone) + параметры (speed/range/pellet/max_targets/tick_rate/duration/angle).
- `OnHitModuleData.cs` — SO on_hit-модуля: kind (Explosion/Burn/Slow/Freeze/Knockback/Ground/Pierce/Homing/BeamSplit/LaserFocus) + параметры модуля (priority/radius/tick/duration/falloff/...).

**Core — `Assets/Scripts/Core/`**

- `GameBootstrap.cs` — composition root: создание подсистем, prewarm пулов, запуск/рестарт раунда.
- `SessionState.cs` — выбранные шасси/турель/сложность (переживает рестарт раунда).
- `StatBlock.cs` — единый изменяемый контейнер статов игрока.
- `IDamageable.cs` — интерфейс урона: контракт «это можно повредить» (враг, игрок).
- `IPoolable.cs` — контракт пула: `OnGetFromPool` / `OnReleaseFromPool`.
- `PoolManager.cs` — generic-пул: prewarm, get/release, владелец всех инстансов.

**Round — `Assets/Scripts/Round/`**

- `RoundController.cs` — жизненный цикл раунда: фазы, победа/поражение, сброс без LoadScene.

**Player — `Assets/Scripts/Player/`**

- `PlayerController.cs` — движение (Input System), чтение move_speed из StatBlock.
- `PlayerHealth.cs` — текущее HP игрока, получение урона, событие смерти.

**Combat — `Assets/Scripts/Combat/`**

- `TurretController.cs` — поиск цели, кулдаун по aps из StatBlock, спавн выстрела через `ShotFactory`.
- `StatusEffectSystem.cs` — тики DoT (burn) по врагам.
- `ControlEffectSystem.cs` — slow/freeze/knockback статусы врагов.

**Combat/Shooting — `Assets/Scripts/Combat/Shooting/`** (lifecycle-модель, `Docs/Shooting.md`)

- `ShotTypes.cs` — enum `ShotState`, `DestroyReason`, `DeliveryType`.
- `ShotContracts.cs` — контракты стадий `IOnSpawn`/`IOnFlight`/`IOnHit`/`IOnPostHit`/`IOnDestroy` + `IResettable`.
- `ShotContext.cs` — состояние выстрела (пул, `Reset()`).
- `BuildSnapshot.cs` — модули по стадиям + базовые статы; `Without<T>()`, `Empty`.
- `SpawnRequest.cs` — запрос спавна (type/position/direction/buildSnapshot/depth).
- `Target.cs` — цель (position + `IDamageable` + фракция).
- `IVfxSpawner.cs` — интерфейс спавна косметики (принадлежит контроллеру).
- `ShotFactory.cs` — фабрика `DeliveryModuleData → SpawnRequest`, выбор контроллера по `DeliveryType`.
- `ProjectileController.cs` — снаряд (Flight + Terminal Hit + keepAlive).
- `HitscanController.cs` — луч instant/continuous (Terminal/Periodic).
- `ConeController.cs` — конус (Periodic).
- `AoEController.cs` — взрыв/зона (Terminal/Periodic).
- `ShotView.cs` — тонкий MonoBehaviour из пула (pull `ctx.position/direction`).
- `Modules/*.cs` — on-hit модули (контракты): Explosion, Burn, Slow, Freeze, Knockback, Ground, Pierce, Homing, BeamSplit, LaserFocus.

**Enemy — `Assets/Scripts/Enemy/`**

- `EnemyController.cs` — движение к игроку, melee-контакт.
- `EnemyHealth.cs` — текущее HP врага, получение урона, событие смерти (для XP и волн).

**Waves — `Assets/Scripts/Waves/`**

- `WaveSpawner.cs` — батч-спавн по WaveData/DifficultyData, интервалы, конец волны, декремент живых врагов.
- `SpawnPoint.cs` — маркер точки спавна в сцене.

**Progression — `Assets/Scripts/Progression/`**

- `ProgressionSystem.cs` — XP, линейная кривая уровней, level-up пауза, 3 карточки апгрейдов.

**UI — `Assets/Scripts/UI/`**

- `HUDController.cs` — HP/XP-бар, номер волны, счётчик убийств (подписка на события).
- `LevelUpUI.cs` — пауза + 3 карточки апгрейда, выбор одной.
- `RoundEndUI.cs` — победа/поражение + кнопка рестарта раунда.

Стрельба реализуется lifecycle-моделью (`Docs/Shooting.md`); состав файлов в
`Assets/Scripts/Combat/Shooting/` — см. §7 и список выше. Input System
`.inputactions`-обёртка генерируется Unity (не рукописный `.cs`), поэтому в
список не входит.

## 6. Структура папок

- **Контент (ScriptableObject-ассеты) — `Assets/Data/`:**
  - `Assets/Data/Chassis/`, `Assets/Data/Turrets/`, `Assets/Data/Enemies/`
  - `Assets/Data/Waves/`, `Assets/Data/Difficulties/`, `Assets/Data/Upgrades/`
  - `Assets/Data/Delivery/`, `Assets/Data/OnHit/`
- **Префабы — `Assets/Prefabs/` (игровые) и `Assets/UI/` (интерфейс):**
  - `Assets/Prefabs/Player/`, `Assets/Prefabs/Enemies/`
  - `Assets/Prefabs/Projectiles/`, `Assets/Prefabs/VFX/`
  - `Assets/UI/` — `HUD.prefab`, `LevelUpUI.prefab`, `RoundEndUI.prefab`
- **Сцены — `Assets/Scenes/`:** `Assets/Scenes/Main.unity`
- **Скрипты — `Assets/Scripts/`** (подпапки и файлы — в §5):
  - `Assets/Scripts/Data/`, `Assets/Scripts/Core/`, `Assets/Scripts/Round/`,
    `Assets/Scripts/Player/`, `Assets/Scripts/Combat/`, `Assets/Scripts/Enemy/`,
    `Assets/Scripts/Waves/`, `Assets/Scripts/Progression/`, `Assets/Scripts/UI/`
- **Арт (исходные ресурсы) — `Assets/Art/`:**
  - `Assets/Art/Models/`, `Assets/Art/Materials/`, `Assets/Art/Textures/`, 
    `Assets/Audio/SFX/`, `Assets/Audio/Music/`
- **Системные:**
  - `Assets/Settings/` — только URP-ассеты (RP-ассеты, рендереры, volume profile,
    global settings)
  - `Assets/Input/InputSystem_Actions.inputactions` — Input System

---

## 7. Стрельба: lifecycle-модель и маппинг модулей

Архитектура выстрела — lifecycle-модель из `Docs/Shooting.md`
(`ShotContext` + типоспецифичные контроллеры + модули-контракты). Разделы §1/§2/§5
описывают эту целевую модель. Текущий код находится в процессе миграции с
legacy-модели (`DeliveryHandler`/`IHitHandler`/`OnHitDispatcher`); до завершения
рефакторинга legacy-классы остаются в коде, но новые сущности стрельбы создаются
только по lifecycle-модели.

### Маппинг: тип доставки → контроллер

| Тип доставки | Контроллер | Flight | Hit | Дедупликация | keepAlive |
|---|---|---|---|---|---|
| Projectile | `ProjectileController` | ✅ | Terminal | `hitTargets` | ✅ (Pierce) |
| Hitscan (instant) | `HitscanController` | ❌ | Terminal (1 раз) | `hitTargets` | ✅ (обычно false) |
| Hitscan (continuous) | `HitscanController` | ✅ duration | Periodic | `nextTickAt` | ❌ |
| Cone | `ConeController` | ✅ duration | Periodic | `nextTickAt` | ❌ |

`instant`/`continuous` — флаг `duration` одного `HitscanController`, а не два класса.

### Маппинг: on-hit модуль → контракты

| Модуль | Контракты | Примечание |
|---|---|---|
| Explosion | `IOnHit` + `spawnQueue` (AoE-ребёнок) | ребёнок избирательный: наследует билд, кроме Explosion |
| Burn | `IOnHit` | статус на цель |
| Freeze | `IOnHit` | статус на цель |
| Slow | `IOnHit` | статус на цель |
| Knockback | `IOnHit` | читает `ctx.damage`; сила = `force × damage_scaling` |
| Ground | `IOnHit` + `spawnQueue` (зона-ребёнок Periodic) | зона = логический выстрел (AoE Periodic) |
| Pierce | `IOnPostHit` | `ctx.keepAlive = true`; счётчик/falloff в `ctx` |
| Homing | `IOnFlight` | меняет `ctx.direction` каждый тик |
| Beam Split | `IOnHit` + `spawnQueue` (дети с полным билдом) | на каждую поражённую цель |
| Laser Focus | `IOnHit` | стаки в `ctx`, сброс при смене `ctx.currentTarget` |
