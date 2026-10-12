---
name: Замедление
id: slow
module_class: on_hit
added_tags:
  - control
required_tags: []
conflicts_with: []
conflict_group: none
fields:
  duration: medium
  slow_amount: medium
  chance: 1
priority: 4
description: Замедляет врага.
tags:
  - on_hit
  - module
---

## Behavior

1. Если `Random.value > chance` — выйти.
2. Замедлить врага на `slow_amount`.
3. Через `duration` секунд снять эффект.