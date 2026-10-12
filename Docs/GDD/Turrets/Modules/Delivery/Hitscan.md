---
name: Хитскан
id: hitscan
module_class: delivery
module_type: hitscan
added_tags:
  - hitscan
fields:
  duration: instant
  tick_rate: none
  max_targets: 1
  range: medium
  damage: medium
  onhit_tick_rate: none
  aps: medium
presets:
  beam:
    duration: instant
    tick_rate: none
    max_targets: 1
    range: high
    damage: high
    onhit_tick_rate: none
    aps: fast
    added_tags: "[hitscan, instant]"
  laser:
    duration: continuous
    tick_rate: fast
    max_targets: 1
    range: medium
    damage: low
    onhit_tick_rate: medium
    aps: none
    added_tags: "[hitscan, continuous]"
  railgun:
    duration: instant
    tick_rate: none
    max_targets: 5
    range: extreme
    damage: extreme
    onhit_tick_rate: none
    aps: very_slow
    added_tags: "[hitscan, instant]"
description: Мгновенный или непрерывный луч. Поведение зависит от duration и max_targets.
tags:
  - delivery
  - module
---

## Behavior
**Если `duration = instant`:**
1. Каждые `1/aps` секунд сделать raycast в сторону ближайшего врага.
2. Нанести урон `max_targets` целям на линии.
3. Вызвать `OnHit` у всех OnHit-модулей для каждой цели.
**Если `duration = continuous`:**
4. Запустить корутину.
5. Каждые `tick_rate` секунд делать raycast и вызывать `OnHit`.
6. Остановиться, когда цель мертва или появилась более близкая.
7. Остановиться через `duration` секунд (если задано).