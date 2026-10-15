---
name: Laser Focus
id: laser_focus
module_class: on_hit
added_tags:
  - single
  - dot
required_tags:
  - hitscan
  - continuous
conflicts_with: []
conflict_group: none
fields:
  damage_growth: 0.1
  max_stacks: 5
  stack_reset_on_switch: 1
priority: 9
description: Урон луча растёт, пока цель в луче. Работает только с continuous hitscan.
tags:
  - on_hit
  - module
---
## Behavior
1. Если delivery не в режиме `continuous` — выйти (модуль не срабатывает).
2. Увеличить урон луча на `damage_growth`.
3. Если урон достиг `max_stacks` — остановиться.
4. Урон растёт, пока цель в луче.
5. При смене цели — сбросить стаки.