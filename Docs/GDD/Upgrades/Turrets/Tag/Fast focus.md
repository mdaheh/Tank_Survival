---
name: Быстрая фокусировка
id: fast_focus
target: turret
upgrade_type: tag
required_tags:
  - hitscan
  - continuous

stat_changes:
  - stat: damage_growth
    target_module: "[[Laser Focus]]"
    stack_type: multiplicative
    value: 0.25

max_stacks: 3
description: "Laser Focus накапливает урон на 25% быстрее."

tags:
  - upgrade
  - tag
  - hitscan
---
