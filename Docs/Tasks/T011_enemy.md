# T011: Враг — EnemyController + EnemyHealth
- Status: [x]
- Phase: 2
- Priority: High
- Depends: T009, T010
- Files:
  - Assets/Scripts/Enemy/EnemyController.cs
  - Assets/Scripts/Enemy/EnemyHealth.cs

## Задача
`EnemyHealth` (MonoBehaviour, `IDamageable`, `IPoolable`, namespace `TankSurvival.Enemy`):
поля `EnemyData data`, `float currentHealth`; метод `void Init(EnemyData data)` →
`currentHealth = data.health`; `TakeDamage(float amount)` вычитает и при ≤ 0 вызывает
`event Action OnDeath`; `OnGetFromPool()` → `currentHealth = data.health`;
`OnReleaseFromPool()` → сброс/отписка.
`EnemyController` (MonoBehaviour, namespace `TankSurvival.Enemy`): статический реестр
`public static readonly List<EnemyController> Active`; `OnEnable` добавляет себя, `OnDisable`
убирает. Поля `EnemyData data`, `Transform target`, `EnemyHealth health` (кэш в `Awake`,
публичное). Метод `void Init(EnemyData data, Transform target, PoolManager pool)`: сохраняет
data/target, кэширует `IDamageable targetDamageable = target.GetComponent<IDamageable>()`,
вызывает `health.Init(data)`, подписывает `health.OnDeath += () => pool.Release(gameObject)`.
В `Update`: движение к `target` со скоростью `data.speed`; при дистанции ≤ 1.0 к цели
`targetDamageable.TakeDamage(data.damage)` не чаще 1 раза/сек (кулдаун).

## Ограничения
- Без физики: контакт — по дистанции, не через коллайдеры/триггеры.
- `GetComponent` игрока — один раз в `Init`, не в `Update`.
- Реестр `Active` наполняется в `OnEnable`/`OnDisable`, не в `Update`.
- namespace `TankSurvival.Enemy`; `using TankSurvival.Data` (EnemyData), `using TankSurvival.Core` (IDamageable, PoolManager).

## Unity (человеку)
Создать префаб врага:
1. ПКМ в Hierarchy → 3D Object → Capsule → назвать `Runner`.
2. Добавить компоненты `EnemyController` и `EnemyHealth`.
3. Перетащить объект в `Assets/Prefabs/Enemies/` (создаст `Runner.prefab`); объект из Hierarchy удалить.

## Проверка человеком
- Проект компилируется без ошибок.

## Отчёт Исполнителя
Готово: EnemyHealth.cs: MonoBehaviour + IDamageable + IPoolable — поля data/currentHealth, Init, TakeDamage с event OnDeath при ≤0, OnGetFromPool (восстановление currentHealth), OnReleaseFromPool (сброс подписок), EnemyController.cs: статический реестр Active (наполнение в OnEnable/OnDisable), кэш health в Awake, Init (сохранение data/target, кэш IDamageable, подписка OnDeath→pool.Release), Update — MoveTowards к цели со скоростью data.speed и атака по дистанции ≤1.0 с кулдауном 1с
Блокер: —
