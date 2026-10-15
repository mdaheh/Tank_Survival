---
name: Луч
id: beam
variant: specialized
weapon_class: laser
base_turret: "[[Laser]]"
delivery_module: "[[Hitscan]]"
delivery_config:
  duration: instant
  aps: fast
  tick_rate: none
  onhit_tick_rate: none
  max_targets: 1
  range: extreme
  damage: high
on_hit_modules: []
unlock_kills: 1500
balance_status: draft
tags:
  - turret
  - specialized
  - laser
---

## Behavior

1. Найти ближайшего врага.
2. Каждые `1/aps` секунд сделать мгновенный выстрел.
3. Мгновенно нанести урон цели (см. [[Hitscan]]).
4. OnHit-модули отсутствуют (базовая версия).

## Описание

Мгновенный луч. Короткий разовый выстрел. Всегда попадает. Высокая дальность, высокий урон, хорошая скорострельность.

**Отличие от Projectile:** мгновенная доставка. Снаряд может промахнуться, луч — нет.

**Синергия:** С [[Beam Split]] — луч разделяется на несколько лучей. С [[Pierce]] не работает (требует projectile).