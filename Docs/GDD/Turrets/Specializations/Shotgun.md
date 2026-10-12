---
name: Дробовик
id: shotgun
variant: specialized
weapon_class: cannon
base_turret: "[[Hammer]]"
delivery_module: "[[Projectile]]"
delivery_config:
  aps: slow
  speed: medium
  range: low
  damage: low
  pellet_count: 5
  spread_angle: medium
  tick_rate: none
  onhit_tick_rate: none
on_hit_modules: []
unlock_kills: 2000
balance_status: draft
tags:
  - turret
  - specialized
  - cannon
---

## Behavior

1. Найти ближайшего врага.
2. Создать 5 снарядов (см. [[Projectile]]).
3. Распределить их веером по `spread_angle`.
4. Каждый pellet наносит урон и вызывает OnHit независимо.
5. Специализация: веер снарядов.

## Описание

Короткий ствол. Выстреливает веер снарядов. Каждый pellet — отдельный снаряд.

**Роль:** билд на массовые OnHit-срабатывания. Каждый pellet может поджечь, заморозить, взорвать.

**ДПС:** `low (1) × 5 (pellet) × slow (1) = 5` — но каскадный урон по толпе.