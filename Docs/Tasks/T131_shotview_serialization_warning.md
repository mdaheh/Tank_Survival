# T131: ShotView — убрать предупреждение сериализации (UAC1001)
- Status: [x]
- Phase: 14
- Files:
  - Assets/Scripts/Combat/Shooting/ShotView.cs

## Задача
Убрать предупреждение компилятора Unity UAC1001: поле `public ShotContext ctx;` в `ShotView` сериализуется, но `ShotContext` не помечен `[Serializable]` (и помечать его не нужно — это рантайм-контекст из пула, а не данные инспектора).

Заменить `public ShotContext ctx;` на `private ShotContext ctx;` (приватные поля Unity не сериализует). Публичный `Bind()` остаётся точкой входа; меняется только модификатор поля. Больше ничего не менять.

## Ограничения
- Не добавлять `[Serializable]` к `ShotContext`.
- Не трогать `Bind`, `Update`, `OnGetFromPool`, `OnReleaseFromPool` (они работают с полем `ctx` внутри класса).
- `ShotView` остаётся `MonoBehaviour, IPoolable`; `using TankSurvival.Core` и `using UnityEngine` не менять.

## Unity (человеку)
Не требуется.

## Проверка человеком
- Console не содержит предупреждения UAC1001 про `ShotView.cs`.
- Проект компилируется без ошибок и предупреждений.

## Отчёт Исполнителя
Готово: ShotView.cs: поле ctx изменено с public на private (Unity его не сериализует, UAC1001 уходит); Bind/Update/OnGetFromPool/OnReleaseFromPool, usings и базовые типы не тронуты
Блокер: —