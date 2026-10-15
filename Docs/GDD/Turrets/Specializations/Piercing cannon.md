---
name: Пробивная пушка
id: piercing_cannon
variant: specialized
weapon_class: cannon
base_turret: "[[Hammer]]"
delivery_module: "[[Projectile]]"
delivery_config:
  aps: slow
  speed: high
  range: high
  damage: high
  pellet_count: 1
  spread_angle: none
  tick_rate: none
  onhit_tick_rate: none
on_hit_modules:
  - "[[Pierce]]"
unlock_kills: 3000
balance_status: draft
tags:
  - turret
  - specialized
  - cannon
---

## Behavior

1. Найти ближайшего врага.
2. Выстрелить снарядом (см. [[Projectile]]).
3. При попадании — [[Pierce]] уменьшает счётчик пробитий.
4. Снаряд летит дальше, пока счётчик > 0.
5. Каждый контакт вызывает OnHit у всех OnHit-модулей.
6. Специализация: пробитие.

## Описание

Большой ствол. Один мощный выстрел, который пробивает толпу насквозь.