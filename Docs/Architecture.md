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
        │            ▼                  ▼                ├──► [ Projectile ]  (пул)
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
        └──► [ TurretController ] ── кулдаун (aps из StatBlock) ──► [ DeliveryHandler ]
                                        ├── ProjectileDeliveryHandler ──► [ Projectile ] ──► IDamageable
                                        └── HitscanDeliveryHandler ────────────────► IDamageable
                                             │  при попадании
                                             ▼
                                        [ IHitHandler ]  (список on_hit, по priority)
                                        ├── ExplosionOnHitHandler ──► AoE ──► IDamageable + [ VFX ]
                                        └── BurnOnHitHandler ──► [ StatusEffectSystem ] ──► EnemyHealth (тики)

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
| TurretController | DeliveryHandler / IHitHandler | делегирование выстрела и on_hit-эффектов |
| Delivery/OnHit | IDamageable | единый контракт нанесения урона |
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
| StatusEffectSystem | активные DoT-статусы врагов | OnHit-модули (добавить), система (тик) | смерть врага / рестарт раунда |
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
- **Два интерфейса на разных слоях.** `IDamageable` — «это можно повредить» (враг, игрок),
  единая точка получения урона для всех путей доставки; `IHitHandler` — контракт
  on_hit-модулей (реакция на попадание). Оба используются в MVP.
- **Delivery/OnHit — отдельный слой-композиция** (turret → список модулей), а не
  наследование конкретных турелей: новые модули добавляются без переписывания существующих.
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

## 4. Заложено под v1.0

- Активные способности шасси: поле `ability` в `ChassisData` зарезервировано, в MVP не используется.
- Continuous hitscan (Laser): поле `duration` в Hitscan-конфиге заложено, MVP использует только `instant`.
- Специализации турелей и полный набор delivery/on_hit: модульный слой готов, добавляются новые `.asset`/хендлеры без перестройки.
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
- `DeliveryModuleData.cs` — SO delivery-модуля: kind (Projectile/Hitscan) + параметры (speed/range/pellet/max_targets).
- `OnHitModuleData.cs` — SO on_hit-модуля: kind (Explosion/Burn) + параметры (radius/damage/tick/duration/max_depth).

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

- `TurretController.cs` — поиск цели, кулдаун по aps из StatBlock, вызов delivery.
- `DeliveryHandler.cs` — базовый контракт delivery (MonoBehaviour, активный выбирается по kind).
- `ProjectileDeliveryHandler.cs` — спавн снарядов из пула, распределение по pellet/spread.
- `HitscanDeliveryHandler.cs` — мгновенный raycast-урон (режим `instant`).
- `IHitHandler.cs` — интерфейс on_hit-модуля: контракт «реакция на попадание» (вызов по priority).
- `ExplosionOnHitHandler.cs` — реализация IHitHandler: AoE-урон в радиусе + VFX из пула.
- `BurnOnHitHandler.cs` — реализация IHitHandler: наложение DoT-статуса на цель.
- `Projectile.cs` — летящий снаряд (пул), контакт → OnHit.
- `StatusEffectSystem.cs` — тики DoT (burn) по врагам.

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

Итого: **34 файла**. Input System `.inputactions`-обёртка генерируется Unity
(не рукописный `.cs`), поэтому в список не входит.

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
