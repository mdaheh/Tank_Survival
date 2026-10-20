# Phase 8: Враги v1.0 — Brute, Shooter, Boss
- Status: in-progress
- Цель: расширить состав врагов до полного набора GDD (Runner/Brute/Tank/
  Shooter/Boss) и добавить первый ranged-пайплайн: Shooter стреляет
  вражеским снарядом в игрока; Brute и Boss — данные поверх готового
  melee-пайплайна.

## Задачи (порядок = приоритет)
- T052: `EnemyData` — поля стрельбы (`shootRange`, `shootInterval`).
- T053: `EnemyProjectile` — вражеский снаряд (пул), урон игроку.
- T054: `EnemyController` — ranged-ветка (стрельба по игроку).
- T055: `GameBootstrap` — prewarm вражеских снарядов.
- T056: Shooter — data-only (SO + префаб + состав в `WaveData`).
- T057: Brute — data-only (SO + префаб + состав в `WaveData`).
- T058: Boss — data-only (SO + префаб + состав в `WaveData`).

## Smoke-тест (проверяет человек)
- [ ] Проект компилируется без ошибок и предупреждений.
- [ ] Shooter спавнится в волне, идёт к игроку, на дистанции `shootRange`
  останавливается и стреляет вражеским снарядом в игрока; снаряд наносит
  урон `PlayerHealth`.
- [ ] При контакте игрок получает melee-урон от Shooter (как Runner/Tank).
- [ ] Brute спавнится в волне, идёт к игроку и бьёт melee (HP 40 / Speed
  6.0 / Damage 10 / XP 3).
- [ ] Boss спавнится в волне, идёт к игроку и бьёт melee (HP 160 / Speed
  3.0 / Damage 40 / XP 50).
- [ ] Все 5 врагов умирают от урона и возвращаются в пул; число объектов в
  иерархии не растёт; вражеские снаряды возвращаются в пул.
