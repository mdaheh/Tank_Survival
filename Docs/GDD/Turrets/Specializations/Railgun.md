---
name: Рейлган
id: railgun
variant: specialized
weapon_class: laser
base_turret: "[[Laser]]"
delivery_module: "[[Hitscan]]"
delivery_config:
  duration: instant
  aps: very_slow
  tick_rate: none
  onhit_tick_rate: none
  max_targets: 5
  range: extreme
  damage: extreme
on_hit_modules: []
unlock_kills: 3000
balance_status: draft
tags:
  - turret
  - specialized
  - laser
---

## Behavior

1. Найти ближайшего врага.
2. Каждые `1/aps` секунд сделать мгновенный выстрел.
3. Мгновенно нанести урон `max_targets` целям на линии.
4. OnHit-модули отсутствуют (базовая версия).

## Описание

Рельсотрон. Сокрушительный выстрел, который пробивает всё на линии. Редкий, но убийственный.

**Роль:** oneshot-билд. Один выстрел — линия трупов.

**Отличие от Piercing cannon:** Piercing cannon стреляет снарядом (летит, может промахнуться). Railgun — мгновенный (всегда попадает). У Piercing cannon выше aps, у Railgun — выше урон и дальность.

**Синергия:** С [[Beam Split]] — выстрел разделяется на несколько лучей. Это подчёркивает разницу между Railgun и Piercing cannon: Piercing cannon работает через Pierce, Railgun — через Beam Split.

**ДПС:** `extreme (27) × very_slow (0.5) = 13.5` на цель × 5 целей = **67.5** общего урона за выстрел.