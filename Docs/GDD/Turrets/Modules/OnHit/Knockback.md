---
name: Отталкивание
id: knockback
module_class: on_hit
added_tags:
  - control
required_tags: []
conflicts_with: []
conflict_group: none
fields:
  force: medium
  duration: short
  damage_scaling: 0.5
priority: 3
description: Отталкивает врага. Сила зависит от урона.
tags:
  - on_hit
  - module
---

## Behavior

1. Оттолкнуть врага от точки попадания.
2. Сила = `force × damage_scaling`.
3. Длительность = `duration` секунд.