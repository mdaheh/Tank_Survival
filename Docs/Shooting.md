# Shooting — Lifecycle, Modules, Autonomy

## TL;DR

- Выстрел — **оболочка (Controller)** + **модули**.
- Стадии: **Spawn** (один раз), **Flight** (опционально), **Hit**, **PostHit**, **Destroy** (терминал).
- **Hit и PostHit могут выполняться многократно.**
- Модули **не знают друг о друге**, состояние живёт **в контексте**.
- Модуль реализует **1+ контрактов** из `IOnSpawn`, `IOnFlight`, `IOnHit`, `IOnPostHit`, `IOnDestroy`.
- Контракты **универсальны**. Контроллер — типоспецифичен.
- Дочерние сущности — **отдельные контексты**, `depth + 1`, с **явно переданным** `buildSnapshot`.
- **Hit бывает двух видов:** Терминальный (снаряд) и Периодический (луч, конус).

---

## 1. Назначение и границы

**Покрывает:** жизненный цикл выстрела, контракты модулей, правила переходов, дочерние сущности, владение визуалом (ShotView / VFX), инварианты, минимальные структуры данных.

**Не покрывает:** конкретные модули и эффекты, конкретный список типов доставки, реализацию пулинга, Spatial Hash, баланс, конкретные визуальные ассеты и арт (спрайты/партиклы/шейдеры).

---

## 2. Термины

| Термин | Значение |
|---|---|
| **Контекст** | Объект, живущий от Spawn до Dead. Хранит **всё** состояние. |
| **Контроллер** | Реализует FSM для **своего** типа доставки. |
| **Тип доставки** | Определяется контроллером. Спецификация **не фиксирует** список. |
| **Модуль** | Stateless-объект, реализующий контракты стадий. |
| **Terminal Hit** | Hit-событие, после которого выстрел умрёт, если модули не продлят жизнь (`keepAlive`). |
| **Periodic Hit** | Hit-событие, которое повторяется каждый `tickRate`, пока жив выстрел. `keepAlive` игнорируется. |
| **Дочерний контекст** | ShotContext, порождённый модулем (`depth = parent.depth + 1`). |
| **Вид (ShotView)** | Тонкий MonoBehaviour из пула. Каждый кадр читает `ctx.position/direction` и двигает Transform. Логику не хранит. |
| **Эффект (VFX)** | Косметика без логики (вспышка взрыва, луч-визуал). Не выстрел, контрактов нет. |
| **vfxQueue** | Канал косметики: запросы VFX, обрабатываемые отдельно от `spawnQueue`. |
| **IVfxSpawner** | Интерфейс спавна косметики. Принадлежит контроллеру, **не** является частью `ShotContext`. |

---

## 3. Базовая модель

Выстрел — **оболочка**, которая хранит контекст, вызывает модули и принимает решения о переходах на основе полей контекста.

Модули **не вызывают друг друга**, **не управляют переходами**, **не хранят состояние**. Модуль читает контекст, меняет поля и ставит запросы в `spawnQueue`.

---

## 4. Стадии и их наличие

### 4.1 Правила наличия стадий

| Стадия | Обязательна? | Комментарий |
|---|---|---|
| **Spawn** | ✅ Да | Один раз. |
| **Flight** | ❌ Опциональна | Есть, если выстрел существует во времени. |
| **Hit** | ✅ Да | Хотя бы один раз. |
| **PostHit** | ✅ Да | Всегда после Hit. |
| **Destroy** | ✅ Да | Всегда. Вызываются модули `IOnDestroy` (без права отмены). |

**Следствие:** `Spawn → [Flight] → Hit → PostHit → (Flight | Destroy) → Dead`.

### 4.2 Поля контекста

**Общие для всех типов:** `state`, `age`, `lifetime`, `keepAlive`, `destroyReason`, `depth`, `spawnQueue`, `buildSnapshot`, `hitTargets`, `hitPoint`.

**Специфичные для типа:** `position`, `direction`, `speed`, `prevPosition`, `remainingRange`, `tickRate`, `nextTickAt`, `currentTarget` и т.д.

---

## 5. Контекст: жизненный цикл и сброс

- Контекст берётся из пула при входе в `Spawn`.
- **Сброс:** метод `Reset()`. Базовые поля сбрасываются контроллером. Кастомные — через `IResettable`.
- При переходе в `Dead` возвращается в пул.

---

## 6. Контракты модулей

### 6.1 `IOnSpawn`
`void OnSpawn(ShotContext ctx);`
- Один раз. Может менять поля, ставить `spawnQueue`. Не управляет переходами.

### 6.2 `IOnFlight`
`void OnFlight(ShotContext ctx, float dt);`
- Каждый тик Flight. Может менять `direction` и `speed`.
- ⚠️ **Тайминг:** `velocity` пересчитывается контроллером **после** вызова всех `OnFlight` в текущем кадре. Модуль не трогает `velocity` напрямую.

### 6.3 `IOnHit`
`void OnHit(ShotContext ctx, Target target);`
- Для каждой цели в Hit-событии. `ctx.hitPoint` содержит точку попадания. Не управляет переходами.

### 6.4 `IOnPostHit`
`void OnPostHit(ShotContext ctx);`
- **Один раз на Hit-событие** (батч), после всех `OnHit`.
- Может установить `ctx.keepAlive = true` (работает только для Terminal Hit). Не может установить `false`.

### 6.5 `IOnDestroy`
`void OnDestroy(ShotContext ctx);`
- Вызывается **один раз** при переходе в стадию `Destroy` (по любой причине: Hit, Expired, External).
- Может ставить запросы в `spawnQueue` (например, взрыв при истечении времени или лужа от луча).
- **Не может** отменить уничтожение или изменить `state`. Стадия `Destroy` остаётся терминальной.

### 6.6 Stateless и порядок
⚠️ Модуль **не хранит runtime-состояние**. Всё меняющееся — в контексте.
Порядок вызовов задаётся **порядком модулей в массивах** внутри `BuildSnapshot`. Спецификация не требует отдельных полей приоритета — как отсортирован массив, так и вызываются модули. Если модули пишут в одно поле, итог определяет последний в списке.

---

## 7. Ответственность контроллера

Каждый контроллер **обязан** определить:

1. **Наличие Flight:** есть ли состояние полёта.
2. **Тип Hit:** Terminal (по столкновению) или Periodic (каждый `tickRate`).
3. **`FindTargets(ctx)`:** как ищет цели (raycast, overlap, swept) и в каком порядке.
4. **Дедупликация:**
   - Для Terminal: использовать `hitTargets` (одна цель — одно попадание за жизнь).
   - Для Periodic: использовать `nextTickAt` (цели могут быть поражены повторно каждый тик).
5. **`keepAlive`:** для Terminal — сбрасывается в `false` перед PostHit. Для Periodic — игнорируется (выстрел живёт по `lifetime`).
6. **Вызов `IOnDestroy`:** перед переходом в `Dead` контроллер обязан вызвать `OnDestroy` у всех модулей из `buildSnapshot.destroyModules`.
7. **Порядок на Destroy:** вызвать `IOnDestroy` у модулей → обработать `spawnQueue` (логические дети) → обработать косметику (VFX смерти через `IVfxSpawner`) → вернуть `ShotView` и контекст в пул.

---

## 8. Порядок вызовов

1. Вызов модулей стадии.
2. Обработка `spawnQueue`.
3. Переход FSM.

*(Между стадиями: `Spawn → [Flight] → Hit → PostHit → (Flight | Destroy) → Dead`)*

---

## 9. Дочерние сущности

Любой `spawnQueue` → **новый ShotContext**.

1. Свой `Spawn`.
2. `depth = parent.depth + 1`. Если `> max_depth` — отмена.
3. ⚠️ **`buildSnapshot` не наследуется автоматически.**
   Модуль, создающий `SpawnRequest`, **обязан явно передать** снимок, который получит ребёнок. Это позволяет управлять комбо-механиками:
   - **Изоляция (базовый взрыв):** передать `BuildSnapshot.Empty`. Взрыв наносит только урон, никакой рекурсии.
   - **Избирательное наследование (Взрыв + Яд):** передать `ctx.buildSnapshot.Without<ExplosionModule>()`. Взрыв наследует все модули родителя, кроме самого взрыва. Получаем «взрыв, который травит».
   - **Полное наследование (Beam Split):** передать `ctx.buildSnapshot`. Луч разделяется, дочерние лучи работают с полным билдом.
4. Не наследует `hitTargets`.

⚠️ **`spawnQueue` — только для логических дочерних сущностей** (выстрелы, зоны). Для косметики (VFX) контроллер использует отдельный интерфейс `IVfxSpawner`, который **не** является частью `ShotContext`.

---

## 10. Владение визуалом (ShotView / VFX)

Контекст и контроллер — **чистая логика без GameObject**. Экранный вид выстрела —
отдельный слой. Правила:

1. **Контекст не ссылается на GameObject.** `ShotContext` хранит только `Vector3`
   (`position`, `direction`) и скаляры. Никаких ссылок на `Transform`/prefab/рендер —
   это держит пул контекстов и пул объектов независимыми.

2. **Два канала.** `spawnQueue` — только логические дети (выстрелы, зоны).
   Косметика идёт через `vfxQueue` / `IVfxSpawner` и **не** создаёт `ShotContext`.

3. **Кто двигает Transform (pull).** `ShotView` (MonoBehaviour из пула) каждый кадр
   читает `ctx.position/direction` и применяет к Transform. Контроллер и модули про
   визуал не знают.

4. **Prefab закреплён за контроллером.** `SpawnRequest` несёт `type` и
   `buildSnapshot`, но не prefab. Контроллер (или фабрика `type → prefab`) знает,
   какой `ShotView` взять из `PoolManager`.

5. **Жизнь вида.** Вид берётся из пула при `Spawn`, возвращается в пул в конце
   `Destroy` (порядок — §7, п. 7). Вид не имеет контрактов и не управляет состоянием.

6. **Debug-визуализация отдельно.** `DevVisualizer` (`#if UNITY_EDITOR || DEBUG`) —
   debug-слой, геймплейным визуалом не является и этим правилам не подчиняется.

Три роли «того, что существует в мире»:

- **Выстрел** — `ShotContext` + `ShotView` (снаряд, зона, периодический луч/конус).
- **Эффект** — VFX через `IVfxSpawner` (вспышка взрыва, вспышка конуса).
- **Логика без постоянного вида** — мгновенный hitscan (контроллер + одноразовый VFX).

---

## 11. Контракты данных (Минимум)

⚠️ Агент должен реализовать эти сущности. Вот минимальные требования к их полям.

```csharp
class BuildSnapshot {
    IOnSpawn[] spawnModules;
    IOnFlight[] flightModules;
    IOnHit[] hitModules;
    IOnPostHit[] postHitModules;
    IOnDestroy[] destroyModules;
    // + базовые статы (урон, скорость)
}

class SpawnRequest {
    DeliveryType type;           // Какой контроллер использовать
    Vector3 position;
    Vector3 direction;
    BuildSnapshot buildSnapshot; // Явно передается родителем!
    int depth;                   // parent.depth + 1
}

class Target {
    Vector3 position;
    // + ссылка на здоровье/фракцию
}
```

---

## 12. Референс: ProjectileController

⚠️ Иллюстрация. `Initialize` — один раз, `Tick` — каждый кадр.

```csharp
void Tick(ProjectileContext ctx, float dt) {
    ctx.age += dt;
    if (ctx.state != State.Flight) return;

    // 1. Модули полета (меняют direction, speed)
    foreach (var m in ctx.buildSnapshot.flightModules) 
        m.OnFlight(ctx, dt);
    FlushSpawnQueue(ctx);

    // 2. Обновление позиции (velocity считается ЗДЕСЬ, после модулей)
    Vector3 velocity = ctx.direction * ctx.speed;
    ctx.prevPosition = ctx.position;
    ctx.position += velocity * dt;
    ctx.remainingRange -= velocity.magnitude * dt;

    // 3. Проверка Expired
    if (ctx.remainingRange <= 0 || ctx.age >= ctx.lifetime) {
        ctx.destroyReason = DestroyReason.Expired;
        RunDestroy(ctx); return;
    }

    // 4. Коллизии
    var targets = FindCollisionsSwept(ctx.prevPosition, ctx.position, ctx.hitTargets);
    if (targets.Count == 0) return;

    foreach (var t in targets) ctx.hitTargets.Add(t);

    // 5. Hit
    foreach (var target in targets) {
        ctx.hitPoint = CalculateHitPoint(ctx.prevPosition, ctx.position, target);
        foreach (var m in ctx.buildSnapshot.hitModules) m.OnHit(ctx, target);
    }
    FlushSpawnQueue(ctx);

    // 6. PostHit (Terminal)
    ctx.keepAlive = false;
    foreach (var m in ctx.buildSnapshot.postHitModules) m.OnPostHit(ctx);
    FlushSpawnQueue(ctx);

    if (!ctx.keepAlive) {
        ctx.destroyReason = DestroyReason.Hit;
        RunDestroy(ctx); return;
    }
}

void RunDestroy(ShotContext ctx) {
    // 1. Модули прощания (взрыв по истечении, лужа и т.д.)
    foreach (var m in ctx.buildSnapshot.destroyModules) 
        m.OnDestroy(ctx);
    FlushSpawnQueue(ctx);
    
    // 2. Косметика смерти + возврат вида в пул (порядок — §7 п. 7)
    ctx.state = State.Dead;
}
```

---

## 13. Схемы других контроллеров (примеры)

| Тип (пример) | Flight | Тип Hit | Дедупликация | keepAlive |
|---|---|---|---|---|
| Движущийся снаряд | ✅ | Terminal | `hitTargets` | Работает |
| Мгновенный луч | ❌ | Terminal (1 раз) | `hitTargets` | Работает (но обычно false) |
| Непрерывный луч | ✅ (duration) | Periodic | `nextTickAt` | Игнорируется |
| Конус | ✅ (duration) | Periodic | `nextTickAt` | Игнорируется |
| AoE (взрыв) | ❌ | Terminal (1 раз) | `hitTargets` | Игнорируется (всегда Destroy) |

---

## 14. Инварианты

1. Один выстрел — один контекст.
2. Состояние — только в контексте.
3. Модуль не управляет переходами.
4. Модули не знают друг о друге.
5. Порядок: модули → spawnQueue → переход.
6. Spawn — один раз.
7. Destroy — терминал (модули не могут отменить).
8. Дочерние контексты начинают с Spawn.
9. `buildSnapshot` передаётся в `SpawnRequest` **явно**.
10. Сброс полей при Spawn обязателен.
11. Ноль аллокаций в стадиях (кроме пулинга).
12. Контекст не ссылается на GameObject (Transform/prefab/рендер) — только данные.
13. Визуал (`ShotView`) не управляет переходами и не хранит логику.

---

## 15. Анти-паттерны

| Анти-паттерн | Почему плохо |
|---|---|
| Модуль возвращает `bool` из `OnHit`. | Ломает «Hit всегда ведёт в PostHit». |
| Модуль меняет `ctx.state`. | Модуль не управляет FSM. |
| Модуль хранит runtime-состояние. | Сломает пулинг. |
| Модуль ставит `keepAlive = false`. | Сброс делает контроллер. |
| Модуль трогает `velocity`. | Пересчитывается контроллером после `OnFlight`. |
| Автоматическое наследование `buildSnapshot`. | Приведет к бесконечным рекурсиям (взрыв в взрыве). |
| Использовать `hitTargets` для Periodic Hit. | Луч ударит цель только один раз за всё время. |
| Один контроллер для всех типов. | Типы слишком разные. Нужны подклассы. |
| Модуль пытается отменить `Destroy` в `OnDestroy`. | `Destroy` — терминальная стадия. |

---

## 16. Расширение

- **Новый модуль:** реализовать контракты + данные (SO).
- **Новый тип:** новый контроллер, реализующий §7. Выбор через фабрику. Существующие модули не меняются.