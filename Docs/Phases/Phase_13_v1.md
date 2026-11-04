# Phase 13: Стрельба v2 — логический слой lifecycle
- Status: in-progress
- Цель: поверх фундамента Phase 12 ввести поведенческий слой lifecycle-модели
  (`DeliveryConfig`, `DamageModule`, `ShotFactory`, контроллеры Projectile/Hitscan/Cone)
  как изолированный код, не меняя поведение текущей стрельбы.

## Задачи (порядок = приоритет)
- T118: `DeliveryConfig` + поля `config` в `SpawnRequest`/`ShotContext`.
- T119: `DamageModule` — единственная точка нанесения урона через `IDamageable`.
- T120: `ShotFactory` — сборка `SpawnRequest`/`BuildSnapshot` из `DeliveryModuleData`.
- T121: `ProjectileController` — снаряд (Terminal Hit, swept).
- T122: `HitscanController` — мгновенный/непрерывный луч (Terminal/Periodic).
- T123: `ConeController` — конус (Periodic Hit).

## Smoke-тест (проверяет человек)
- [ ] Проект компилируется без ошибок и предупреждений.
- [ ] Текущая стрельба (Hammer/Beam/Laser/Cone + on-hit) работает как до фазы
      (фаза изолированная — поведение не меняется).
- [ ] В `Assets/Scripts/Combat/Shooting/` появились `DeliveryConfig.cs`, `DamageModule.cs`,
      `ShotFactory.cs`, `ProjectileController.cs`, `HitscanController.cs`, `ConeController.cs`.

## Примечания (контекст рефакторинга)
- AoE-контроллер и миграция on-hit (Explosion/Ground/статусы) — в следующих фазах (14–15);
  здесь только прямой урон через `DamageModule`.
- Путь данных конфига: `DeliveryModuleData → ShotFactory → SpawnRequest.config`; копирование
  `SpawnRequest.config → ShotContext.config` — на стадии Spawn (ShotSystem) в Phase 14.
- Интеграция в игру (ShotView, ShotSystem, `TurretController → ShotFactory`) — Phase 14.
- Дисциплина namespace: все новые файлы в `TankSurvival.Combat.Shooting`; ссылки на типы из
  других namespace — только с явным `using` (см. `Files`/`Ограничения` каждой задачи). Это
  предотвращает повторение блокера T113 (`IDamageable` в `TankSurvival.Core`).
