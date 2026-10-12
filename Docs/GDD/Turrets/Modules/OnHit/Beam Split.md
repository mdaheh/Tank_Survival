---
name: Лучевая дробь
id: beam_split
module_class: on_hit
added_tags:
  - spawn
  - single
required_tags:
  - hitscan
conflicts_with: []
conflict_group: none
fields:
  split_count: 3
  split_angle: medium
  chance: 1
priority: 6
description: Луч разделяется на несколько лучей. Требует hitscan.
tags:
  - on_hit
  - module
---

## Behavior

1. Если `Random.value > chance` — выйти.
2. Создать `split_count` новых лучей из точки попадания.
3. Распределить их по `split_angle`.
4. Каждый луч наследует билд турели.