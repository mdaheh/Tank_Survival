---
name: Конус
id: cone
module_class: delivery
module_type: cone
added_tags:
  - cone
fields:
  angle: medium
  range: medium
  damage: low
  duration: continuous
  tick_rate: none
  cone_vfx: GameObject
  onhit_tick_rate: none
description: Наносит урон всем врагам в конусе перед турелью.
tags:
  - delivery
  - module
---

## Behavior

1. Определить направление на ближайшего врага.
2. Найти всех врагов в конусе (`angle`, `range`).
3. Для каждого врага:
   - Если `tick_rate = none` — нанести урон один раз.
   - Если `tick_rate` задан — наносить урон каждые `tick_rate` секунд, пока враг в конусе.
   - Вызвать `OnHit` у всех OnHit-модулей.
4. Проиграть `cone_vfx`.