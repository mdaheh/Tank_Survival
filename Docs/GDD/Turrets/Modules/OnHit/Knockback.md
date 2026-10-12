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
  duration: fast
  damage_scaling: 0.5
  chance: 1
priority: 3
description: Отталкивает врага. Сила зависит от урона и скорости атаки.
tags:
  - on_hit
  - module
---

## Behavior

1. Если `Random.value > chance` — выйти.
2. Оттолкнуть врага от точки попадания.
3. Сила = `force × damage_scaling × (1 / tick_rate)`.
4. Длительность = `duration` секунд.