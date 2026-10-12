---
name: Заморозка
id: freeze
module_class: on_hit
added_tags:
  - control
  - dot
required_tags: []
conflicts_with:
  - burn
conflict_group: element
fields:
  duration: medium
  slow_amount: high
  freeze_threshold: 3
  chance: 1
priority: 5
description: Замораживает врага. Сначала замедляет, потом останавливает.
tags:
  - on_hit
  - module
---

## Behavior

1. Если `Random.value > chance` — выйти.
2. Наложить на врага статус `freeze`.
3. Замедлить врага на `slow_amount`.
4. Если враг получил `freeze_threshold` стаков — заморозить полностью.
5. Через `duration` секунд снять статус.