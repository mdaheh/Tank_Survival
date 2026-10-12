---
name: Laser Focus
id: laser_focus
module_class: on_hit
added_tags:
  - single
  - dot
required_tags:
  - hitscan
conflicts_with: []
conflict_group: none
fields:
  damage_growth: 0.1
  max_stacks: 5
  chance: 1
  stack_reset_on_switch: 1
priority: 9
description: Урон луча растёт, пока цель в луче. Работает только с continuous hitscan.
tags:
  - on_hit
  - module
---
## Behavior
1. Если delivery не в режиме `continuous` — выйти (модуль не срабатывает).
2. Если `Random.value > chance` — выйти.
3. Увеличить урон луча на `damage_growth`.
4. Если урон достиг `max_stacks` — остановиться.
5. Урон растёт, пока цель в луче.
6. При смене цели — сбросить стаки.