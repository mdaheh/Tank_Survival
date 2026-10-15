---
name: Лазер
id: laser
variant: base
weapon_class: laser
delivery_module: "[[Hitscan]]"
delivery_config:
  duration: continuous
  tick_rate: fast
  onhit_tick_rate: medium
  max_targets: 1
  range: high
  damage: medium
on_hit_modules:
  - "[[Laser Focus]]"
specializations:
  - name: "[[Beam]]"
    unlock_kills: 1500
  - name: "[[Railgun]]"
    unlock_kills: 3000
unlock_kills: 0
balance_status: draft
tags:
  - turret
  - base
  - laser
---

## Behavior

1. Найти ближайшего врага в радиусе атаки.
2. Начать непрерывный луч (см. [[Hitscan]]).
3. Каждые `tick_rate` секунд наносить урон цели.
4. **Targeting:** если появляется **более близкий** враг — переключиться на него.
5. Если цель мертва — найти новую.
6. [[Laser Focus]] накапливает урон, пока цель в луче.
7. Смена цели сбрасывает стаки Laser Focus.

## Описание

Футуристичная башня с батарейками. Непрерывный поток энергии. Урон растёт, пока цель в луче.

**Особенность:** [[Laser Focus]] — стартовый OnHit. Урон накапливается.

**Targeting-логика:** переключение на ближайшего врага. Это защита от ситуаций, когда Laser Focus фокусируется на дальнем враге, пока толпа окружила танк.