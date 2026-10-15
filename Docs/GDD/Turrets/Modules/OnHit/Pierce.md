---
name: Пробитие
id: pierce
module_class: on_hit
added_tags:
  - single
required_tags:
  - projectile
conflicts_with: []
conflict_group: none
fields:
  max_pierce_count: 3
  damage_falloff: 0.8
priority: 2
description: Снаряд пробивает врагов насквозь. Требует projectile.
tags:
  - on_hit
  - module
---

## Behavior

1. Уменьшить счётчик пробитий.
2. Если счётчик > 0 — снаряд продолжает лететь.
3. Если счётчик = 0 — уничтожить снаряд.
4. Урон следующей цели = `урон × damage_falloff`.