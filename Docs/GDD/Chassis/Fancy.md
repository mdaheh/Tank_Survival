---
name: Модник
id: chassis_fancy
variant: base
stats:
  health: medium
  speed: high
  turn: high
ability:
  name: Фазовый сдвиг
  id: ability_phase
  type: active
  cooldown: 20
  duration: 3
  speed_bonus: 0
unlock_kills: 8000
balance_status: draft
tags:
  - chassis
  - base
---

## Ability: Phase

1. Активируется по нажатию.
2. Танк становится полупрозрачным на `duration` секунд.
3. Танк **не получает урон**.
4. Танк **проходит сквозь врагов**.
5. Перезарядка `cooldown` секунд.

## Описание

Футуристичное шасси. Сбалансированные параметры. Акцент на мобильности и активном использовании способности.

**Роль:** универсал с ультимативной способностью выхода из окружения.

**Особенность:** `speed_bonus` — параметр для апгрейда (по умолчанию 0).