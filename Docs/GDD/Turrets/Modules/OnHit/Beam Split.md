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
priority: 6
description: Луч разделяется на несколько лучей. Требует hitscan.
tags:
  - on_hit
  - module
---

## Behavior

1. Создать `split_count` новых лучей из точки попадания.
2. Распределить их по `split_angle`.
3. Каждый луч наследует билд турели.