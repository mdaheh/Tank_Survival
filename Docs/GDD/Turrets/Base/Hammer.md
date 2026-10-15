---
name: Молоток
id: hammer
variant: base
weapon_class: cannon
delivery_module: "[[Projectile]]"
delivery_config:
  aps: medium
  speed: medium
  range: medium
  damage: medium
  pellet_count: 1
  spread_angle: none
  tick_rate: none
  onhit_tick_rate: none
on_hit_modules: []
specializations:
  - name: "[[Machine gun]]"
    unlock_kills: 1500
  - name: "[[Shotgun]]"
    unlock_kills: 2000
  - name: "[[Bombard]]"
    unlock_kills: 2500
  - name: "[[Piercing cannon]]"
    unlock_kills: 3000
unlock_kills: 0
balance_status: draft
tags:
  - turret
  - base
  - cannon
---

## Behavior

1. Найти ближайшего врага.
2. Выстрелить снарядом (см. [[Projectile]]).
3. Снаряд летит в цель и наносит урон при контакте.
4. OnHit-модули отсутствуют (базовая версия).

## Описание

Классическая башня танка с одной пушкой. Универсал. Простой, понятный, надёжный.