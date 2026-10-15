---
name: Огнемёт
id: flamethrower
variant: base
weapon_class: flamethrower

delivery_module: "[[Cone]]"
delivery_config:
  duration: continuous
  angle: medium
  range: medium
  damage: low
  tick_rate: fast
  onhit_tick_rate: medium
  cone_vfx: GameObject

on_hit_modules: []

specializations:
  - name: "[[Freezer]]"
    unlock_kills: 1500
  - name: "[[Hellfire]]"
    unlock_kills: 2500

unlock_kills: 0
balance_status: draft
tags: [turret, base, flamethrower]
---

## Behavior

1. Найти направление на ближайшего врага.
2. Нанести урон всем врагам в конусе (`angle`, `range`).
3. Урон наносится каждые `tick_rate` секунд (см. [[Cone]]).
4. OnHit-модули отсутствуют (базовая версия).

## Описание

Классический огнемёт. Струя пламени бьёт по площади в конусе. Наносит периодический урон всем врагам в зоне.