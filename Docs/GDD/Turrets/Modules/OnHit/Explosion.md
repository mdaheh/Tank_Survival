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
  chance: 1
  explosion_vfx: GameObject
priority: 10
description: При попадании создаёт взрыв по площади.
tags:
  - on_hit
  - module
---

## Behavior

1. Если `depth >= max_depth` — выйти.
2. Если `Random.value > chance` — выйти.
3. Создать взрыв в точке попадания.
4. Нанести урон всем врагам в радиусе `radius`.
5. Вызвать `OnHit` у всех OnHit-модулей для каждого врага.
6. Передать `depth + 1`.