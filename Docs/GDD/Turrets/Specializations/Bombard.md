---
name: Бомбарда
id: bombard
variant: specialized
weapon_class: cannon
base_turret: "[[Hammer]]"
delivery_module: "[[Projectile]]"
delivery_config:
  aps: slow
  speed: low
  range: medium
  damage: medium
  pellet_count: 1
  spread_angle: none
  tick_rate: none
  onhit_tick_rate: none
on_hit_modules:
  - "[[Explosion]]"
unlock_kills: 2500
balance_status: draft
tags:
  - turret
  - specialized
  - cannon
---

## Behavior

1. Найти ближайшего врага.
2. Выстрелить снарядом (см. [[Projectile]]).
3. Снаряд летит медленно.
4. При попадании — [[Explosion]] создаёт взрыв по площади.
5. Специализация: взрыв.

## Описание

Короткий и толстый ствол. Медленный снаряд, который взрывается при контакте.

**Роль:** билд на взрывы. Медленно, но больно. Каждый выстрел накрывает толпу.