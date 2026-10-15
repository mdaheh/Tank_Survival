---
name: Взрыв
id: explosion
module_class: on_hit
added_tags:
  - aoe
  - spawn
required_tags: []
conflicts_with: []
conflict_group: none
fields:
  radius: medium
  damage: high
  max_depth: 3
  explosion_vfx: GameObject
priority: 10
description: При попадании создаёт взрыв по площади.
tags:
  - on_hit
  - module
---

## Behavior

1. Если `depth >= max_depth` — выйти.
2. Создать взрыв в точке попадания.
3. Нанести урон всем врагам в радиусе `radius`.
4. Вызвать `OnHit` у всех OnHit-модулей для каждого врага.
5. Передать `depth + 1`.