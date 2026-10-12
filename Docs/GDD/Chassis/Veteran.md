---
name: Ветеран
id: chassis_veteran
variant: base
stats:
  health: high
  speed: medium
  turn: high
ability:
  name: Щит
  id: ability_shield
  type: active
  cooldown: 12
  duration: 5
  shield_hp: 100
unlock_kills: 15000
balance_status: draft
tags:
  - chassis
  - base
---

## Ability: Shield

1. Активируется по нажатию.
2. На `duration` секунд даёт `shield_hp` дополнительного здоровья.
3. Щит поглощает урон **до** основного HP.
4. Перезарядка `cooldown` секунд.

## Описание

Награда за долгую игру. Характеристики выше остальных. Универсал без слабых мест.

**Роль:** награда за прогресс. Сильная вариация шасси.

**Особенность:** Не заменяет Fancy (мобильность) или Fat (HP-танк). Просто **сильнее** по цифрам.