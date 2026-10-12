---
name: Поверхность
id: ground
module_class: on_hit
added_tags:
  - ground
  - aoe
  - dot
  - spawn
required_tags: []
conflicts_with: []
conflict_group: none
fields:
  radius: medium
  duration: medium
  tick_damage: low
  tick_rate: medium
  ground_vfx: GameObject
priority: 7
description: Создаёт зону на земле, которая наносит периодический урон.
tags:
  - on_hit
  - module
---

## Behavior

1. Создать зону на земле в точке попадания.
2. Зона существует `duration` секунд.
3. Каждые `tick_rate` секунд наносить `tick_damage` всем врагам в радиусе `radius`.
4. Вызвать `OnHit` у всех OnHit-модулей для каждого врага.