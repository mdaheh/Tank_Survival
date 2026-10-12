---
name: Глубокая заморозка
id: deep_freeze
target: turret
upgrade_type: unique
target_turret: "[[Freezer]]"

stat_changes:
  - stat: freeze_duration
    target_module: "[[Freeze]]"
    stack_type: additive
    value: 1

max_stacks: 3
description: "Заморозка длится на 1 секунду дольше."

tags:
  - upgrade
  - unique
  - flamethrower
---
