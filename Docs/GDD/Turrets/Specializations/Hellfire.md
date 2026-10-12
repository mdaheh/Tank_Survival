---
name: Hellfire
id: hellfire
variant: specialized
weapon_class: flamethrower
base_turret: "[[Flamethrower]]"
delivery_module: "[[Cone]]"
delivery_config:
  duration: continuous
  angle: high
  range: medium
  damage: medium
  tick_rate: fast
  onhit_tick_rate: medium
  cone_vfx: GameObject
on_hit_modules:
  - "[[Burn]]"
unlock_kills: 2500
balance_status: draft
tags:
  - turret
  - specialized
  - flamethrower
---

## Behavior

1. Найти направление на ближайшего врага.
2. Нанести урон всем врагам в конусе (`angle`, `range`).
3. Урон наносится каждые `tick_rate` секунд.
4. При каждом OnHit-тике — [[Burn]] поджигает врагов.
5. Подожжённые враги получают периодический урон.

## Описание

Адское пламя. Расширенный конус, повышенный урон, поджигает всех врагов в зоне.

**Конфликт:** Не совместим с `Freeze` (одна группа `element`).