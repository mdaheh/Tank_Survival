---
name: Стабилизатор цели
id: target_stabilizer
target: turret
upgrade_type: unique
target_turret: "[[Laser]]"
stat_changes:
  - stat: stack_reset_on_switch
    target_module: "[[Laser Focus]]"
    stack_type: multiplicative
    value: -0.5
max_stacks: 2
description: При смене цели Laser Focus теряет только 50% стаков вместо всех.
tags:
  - upgrade
  - unique
  - laser
---
