---
name: Холодильник
id: freezer
variant: specialized
weapon_class: flamethrower
base_turret: "[[Flamethrower]]"
delivery_module: "[[Cone]]"
delivery_config:
  duration: continuous
  angle: medium
  range: medium
  damage: low
  tick_rate: fast
  onhit_tick_rate: medium
  cone_vfx: GameObject
on_hit_modules:
  - "[[Freeze]]"
unlock_kills: 1500
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
4. При каждом OnHit-тике — [[Freeze]] замедляет врагов.
5. При накоплении стаков — враги замораживаются.

## Описание

Конусный замораживатель. Вместо огня — струя холода. Замедляет врагов в конусе, при накоплении стаков — замораживает.

**Роль:** контроль. Игрок может безопасно стоять в центре толпы, замораживая всех вокруг.

**Конфликт:** Не совместим с `Burn` (одна группа `element`).