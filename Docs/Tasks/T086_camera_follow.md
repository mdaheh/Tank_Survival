# T086: CameraFollow — изометрическая камера
- Status: [x]
- Phase: 10.7
- Files:
  - Assets/Scripts/Camera/CameraFollow.cs

## Задача
Создать `Assets/Scripts/Camera/CameraFollow.cs` — класс `CameraFollow : MonoBehaviour` в namespace `TankSurvival.Camera` (нужен `using UnityEngine;`). Поля, редактируемые в Inspector:
- `[SerializeField] private Transform target;` — объект, за которым следует камера.
- `[SerializeField] private Vector3 offset = new Vector3(0f, 10f, -10f);` — смещение камеры относительно цели (мировые оси, изометрическое).
- `[SerializeField] private float smoothing = 8f;` — скорость сглаживания.

Метод `private void LateUpdate()`: если `target == null` — выйти. Иначе вычислить `Vector3 desired = target.position + offset;` и задать `transform.position = Vector3.Lerp(transform.position, desired, 1f - Mathf.Exp(-smoothing * Time.deltaTime));`.

Поворот камеры скрипт НЕ меняет: изометрический угол задаётся один раз в инспекторе (`Transform.rotation`); сглаживается только позиция.

## Ограничения
- Создавать только `Assets/Scripts/Camera/CameraFollow.cs`.
- Сглаживание frame-rate-независимое (экспоненциальный `Lerp` через `1f - Mathf.Exp(-smoothing * Time.deltaTime)`).
- Не менять `transform.rotation` камеры в коде.

## Unity (человеку)
- Hierarchy → объект `Main Camera` → Add Component → `CameraFollow`.
- `Main Camera` → компонент `CameraFollow` → поле `Target` → объект `Player`.
- `Main Camera` → `Transform` → `Rotation` → изометрический угол (например, X = 45, Y = 45, Z = 0). `Offset`/`Smoothing` подобрать под габариты сцены (стартовые: `Offset` = (0, 10, −10), `Smoothing` = 8).

## Проверка человеком
- Проект компилируется без ошибок.
- Play Mode: камера плавно следует за игроком (без рывков), игрок остаётся в кадре при движении.
- Угол камеры не меняется при движении (поворот фиксированный, изометрический).

## Отчёт Исполнителя
Готово: Assets/Scripts/Camera/CameraFollow.cs: класс CameraFollow : MonoBehaviour в namespace TankSurvival.Camera с полями target/offset/smoothing и LateUpdate с экспоненциальным сглаживанием позиции (поворот не меняется).
Блокер: —
