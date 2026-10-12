---
name: Снаряд
id: projectile
module_class: delivery
module_type: projectile
added_tags:
  - projectile
fields:
  aps: medium
  speed: medium
  range: medium
  damage: medium
  pellet_count: 1
  spread_angle: none
  tick_rate: none
  onhit_tick_rate: none
  projectile_prefab: GameObject
presets:
  default:
    aps: medium
    speed: medium
    range: medium
    damage: medium
    pellet_count: 1
    spread_angle: none
description: Создаёт летящий снаряд. Может создавать несколько снарядов (pellet-система).
tags:
  - delivery
  - module
---

## Behavior

1. Найти ближайшего врага.
2. Создать `pellet_count` снарядов из `projectile_prefab`.
3. Распределить снаряды по `spread_angle` относительно направления на цель.
4. Каждый снаряд летит со скоростью `speed` до `range`.
5. При контакте вызвать `OnHit` у всех OnHit-модулей турели.
6. Уничтожить снаряд (если нет `Pierce`).

## Pellet-система

| Значение pellet_count | Что это |
|---|---|
| 1 | Обычный снаряд |
| 2-3 | Двойной/тройной выстрел |
| 5-10 | Дробовик |

**Правило:** Каждый pellet — отдельный снаряд. Вызывает OnHit независимо.
**Правило:** max_pellet_count = 10 (защита от лагов).
**Правило:** Общий ДПС = damage × pellet_count × aps.