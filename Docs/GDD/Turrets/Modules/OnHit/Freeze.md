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
priority: 5
description: Замораживает врага. Сначала замедляет, потом останавливает.
tags:
  - on_hit
  - module
---

## Behavior

1. Наложить на врага статус `freeze`.
2. Замедлить врага на `slow_amount`.
3. Если враг получил `freeze_threshold` стаков — заморозить полностью.
4. Через `duration` секунд снять статус.