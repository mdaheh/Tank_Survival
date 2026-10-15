---
name: Горение
id: burn
module_class: on_hit
added_tags:
  - dot
required_tags: []
conflicts_with:
  - freeze
conflict_group: element
fields:
  duration: medium
  tick_damage: low
  tick_rate: medium
  max_stacks: 3
priority: 5
description: Поджигает врага. Стаки увеличивают урон.
tags:
  - on_hit
  - module
---

## Behavior

1. Наложить на врага статус `burn`.
2. Если у врага уже есть `burn` — увеличить стак (до `max_stacks`).
3. Каждые `tick_rate` секунд наносить `tick_damage × stacks`.
4. Через `duration` секунд снять статус.