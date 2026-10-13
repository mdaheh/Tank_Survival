# T010: Generic-пул — IPoolable + PoolManager
- Status: [x]
- Phase: 2
- Priority: Critical
- Files:
  - Assets/Scripts/Core/IPoolable.cs
  - Assets/Scripts/Core/PoolManager.cs

## Задача
`IPoolable` (namespace `TankSurvival.Core`): интерфейс с `void OnGetFromPool();` и
`void OnReleaseFromPool();`.
`PoolManager` (MonoBehaviour, namespace `TankSurvival.Core`): методы
`void Prewarm(GameObject prefab, int count)`, `GameObject Get(GameObject prefab)`,
`void Release(GameObject instance)`.
`Prewarm` создаёт `count` неактивных инстансов `prefab` и кладёт в очередь. `Get` берёт
из очереди (или создаёт, если пусто), `SetActive(true)`, вызывает `OnGetFromPool` у всех
`IPoolable` компонентов инстанса, возвращает его. `Release` вызывает `OnReleaseFromPool`
у всех `IPoolable`, `SetActive(false)`, кладёт обратно в очередь. Префаб — ключ очереди.

## Ограничения
- Никакого `Instantiate`/`Destroy` в `Update`/`FixedUpdate` (только в `Prewarm`/`Get`, вызываемых из событий/`Start`).
- В `Update` нет `new`, LINQ, `GetComponent` в цикле, `Find*`.
- `GetComponent<IPoolable>` кэшировать на инстансе при `Prewarm`/первом `Get`, не в `Update`.

## Unity (человеку)
Не требуется (компонент `PoolManager` появится в сцене в T014).

## Проверка человеком
- Проект компилируется без ошибок.

## Отчёт Исполнителя
Готово: IPoolable.cs: интерфейс с OnGetFromPool/OnReleaseFromPool в TankSurvival.Core, PoolManager.cs: MonoBehaviour с Prewarm/Get/Release, словарями prefab→очередь, инстанс→префаб и кэшем IPoolable[] на инстансе (GetComponents вызывается только в CreateInstance при Prewarm/Get, Update отсутствует)
Блокер: —
