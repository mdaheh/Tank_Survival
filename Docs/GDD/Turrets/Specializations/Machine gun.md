---
name: Пулемёт
id: machine_gun
variant: specialized
weapon_class: cannon
base_turret: "[[Hammer]]"
delivery_module: "[[Projectile]]"
delivery_config:
  aps: very_fast
  speed: medium
  range: medium
  damage: low
  pellet_count: 1
  spread_angle: none
  tick_rate: none
  onhit_tick_rate: none
on_hit_modules: []
unlock_kills: 1500
balance_status: draft
tags:
  - turret
  - specialized
  - cannon
---

## Behavior

1. Найти ближайшего врага.
2. Выстрелить снарядом (см. [[Projectile]]).
3. Снаряд летит в цель и наносит урон при контакте.
4. Высокая скорострельность → частые OnHit-срабатывания.
5. Специализация: скорость стрельбы.

## Описание

Внешне похож на Молоток, но имеет два ствола (косметика). Высокая скорострельность, низкий урон.

**Роль:** билд на частые срабатывания OnHit-модулей.