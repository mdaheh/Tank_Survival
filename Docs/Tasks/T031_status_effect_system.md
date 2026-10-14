# T031: StatusEffectSystem — тики DoT (burn)
- Status: [x]
- Phase: 5
- Files:
  - Assets/Scripts/Combat/StatusEffectSystem.cs

## Задача
Создать `StatusEffectSystem : MonoBehaviour` (namespace `TankSurvival.Combat`). Владение списком активных burn-статусов (приватный `List<BurnStatus>`). Приватный вложенный класс `BurnStatus` с полями `EnemyController enemy`, `EnemyHealth health`, `float tickDamage`, `float tickRate`, `float duration`, `float tickTimer`, `float elapsed`. Метод `public void ApplyBurn(EnemyController enemy, float tickDamage, float tickRate, float duration)`: если `enemy` null / HP ≤ 0 / объект неактивен — выйти; если статус для этого врага уже есть — обновить `tickDamage/tickRate/duration` и сбросить `elapsed/tickTimer` (стэки в MVP не используются); иначе добавить новый. В `Update` (обратная итерация): `elapsed += dt`, `tickTimer += dt`; удалить статус, если `health.currentHealth <= 0`, или `!enemy.gameObject.activeSelf`, или `elapsed >= duration`; иначе при `tickTimer >= tickRate` → `tickTimer -= tickRate` и `health.TakeDamage(tickDamage)`.

## Ограничения
- Урон только через `IDamageable` (`health.TakeDamage`). Смерть/возврат в пул определять через `EnemyHealth.currentHealth` и `enemy.gameObject.activeSelf` — без подписки на события.
- `EnemyHealth health` берётся из публичного поля `enemy.health` (без `GetComponent`).
- В `Update` нет `new`, LINQ, `GetComponent`, `Find*`; удаление — `RemoveAt(i)` при обратной итерации (без аллокаций).
- namespace `TankSurvival.Combat`; `using TankSurvival.Enemy` (EnemyController, EnemyHealth), `using System.Collections.Generic`, `using UnityEngine`.

## Unity (человеку)
1. В сцене создать пустой GameObject `StatusEffectSystem` → добавить компонент `StatusEffectSystem`.

## Проверка человеком
- Проект компилируется без ошибок.

## Отчёт Исполнителя
Готово: StatusEffectSystem.cs: создан MonoBehaviour со списком burn-статусов, ApplyBurn (обновление/добавление) и Update с обратной итерацией, RemoveAt и тиками урона через health.TakeDamage
Блокер: —
