# T078: DevBootstrap + DevMenuController (Cheat Panel)
- Status: [ ]
- Phase: 10.5
- Depends: T077
- Files:
  - Assets/Scripts/Dev/DevBootstrap.cs
  - Assets/Scripts/Dev/DevMenuController.cs

## Задача
Создать dev-only Dev Menu (оба файла в `#if UNITY_EDITOR || DEVELOPMENT_BUILD`).
`DevBootstrap` — статический `[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]`,
создающий GameObject `DevMenu` с `Canvas` (Screen Space — Overlay, `sortingOrder` выше игрового),
`CanvasScaler`, `GraphicRaycaster` и компонентом `DevMenuController`.

`DevMenuController` программно строит панель: фон (`Image`), заголовок, кнопку закрытия,
`ScrollRect` + `Content` c `VerticalLayoutGroup`, и для каждого `DevParam` из `DevParamRegistry.All`
генерирует строку `Text(label) + Slider(min..max) + Text(значение)`. При изменении слайдера
вызывается `DevParam.SetValue` (запись в инстанс SO в памяти). Тогл панели — клавиши F1 и `~`
(`Keyboard.current.f1Key.wasPressedThisFrame` / `Keyboard.current.backquoteKey.wasPressedThisFrame`,
с защитой от `Keyboard.current == null`). Панель скрыта по умолчанию.

## Ограничения
- Никаких префабов и `Resources/`: весь UI строится кодом (в релиз не попадают ни скрипты, ни ассеты).
- UI строится один раз; обновление значений — по событию слайдера, не в `Update`.
- Значения пишутся только через `DevParam.SetValue` (инстанс SO в памяти), не через `SerializedObject`/`AssetDatabase`.

## Unity (человеку)
Не требуется (панель создаётся автоматически при входе в Play Mode).

## Проверка человеком
- F1 и `~` открывают/закрывают панель; по умолчанию она скрыта и не мешает игре.
- В панели видны параметры `EnemyData` (health/speed/damage/xpReward), `ChassisData` (health/speed),
  `TurretData → delivery` (damage/aps), `OnHitModuleData` (duration/tickDamage/tickRate/slowAmount…).
- Изменение `EnemyData.health` меняет HP новых спавнов; значение живёт между рестартами раунда
  и сменой турели; после выхода из Play Mode ассет в Project не изменён.
- Новое поле `EnemyData` (временное `testParam`) появляется в панели без правки кода панели.

## Отчёт Исполнителя
Готово:
Блокер:
