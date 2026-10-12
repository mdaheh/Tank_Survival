---
name: Дополнительная дробинка
id: extra_pellet
target: turret
upgrade_type: tag
required_tags:
  - projectile

stat_changes:
  - stat: pellet_count
    stack_type: additive
    value: 1

max_stacks: 5
description: "Добавляет +1 pellet к каждому выстрелу. Работает только если pellet_count > 1."

tags:
  - upgrade
  - tag
  - projectile
---
