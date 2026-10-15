---
name: Толстяк
id: chassis_fat
variant: base
stats:
  health: very_high
  speed: very_low
  turn: low
ability:
  name: Взрыв
  id: ability_nova
  type: active
  cooldown: 15
  radius: 5
  damage: 50
unlock_kills: 5000
balance_status: draft
tags:
  - chassis
  - base
---

## Ability: Nova

1. Активируется по нажатию.
2. Взрыв вокруг танка в радиусе `radius`.
3. Наносит `damage` всем врагам в радиусе.
4. Перезарядка `cooldown` секунд.

## Описание

Медленное, но крайне живучее шасси. Подходит для билдов «засяду в углу и пережду».

**Роль:** выживание через HP.