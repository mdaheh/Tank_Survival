---
name: Автонаведение
id: homing
module_class: on_hit
added_tags:
  - single
required_tags:
  - projectile
conflicts_with: []
conflict_group: none
fields:
  homing_strength: medium
  homing_radius: medium
  chance: 1
priority: 1
description: Снаряд самонаводится на ближайшего врага. Требует projectile.
tags:
  - on_hit
  - module
---

## Behavior

1. Если `Random.value > chance` — выйти.
2. Снаряд меняет направление на ближайшего врага.
3. Сила наведения = `homing_strength`.
4. Радиус поиска = `homing_radius`.