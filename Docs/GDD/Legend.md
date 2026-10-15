Единый справочник **игровых правил** проекта.

---

## 1. Шкалы

### 1.1 Множители
| Тир | Число |
|---|---|
| none | 0 |
| low | 1 |
| medium | 3 |
| high | 9 |
| extreme | 27 |

**Используется для:** `damage`, `range`, `radius`, `angle`, `speed`.

### 1.2 Скорострельность (APS)
| Тир | APS |
|---|---|
| very_slow | 0.5 |
| slow | 1.0 |
| medium | 2.0 |
| fast | 4.0 |
| very_fast | 8.0 |

**Используется для:** `aps`.

### 1.3 Частота тиков
| Тир | Тиков/сек |
|---|---|
| none | — |
| very_fast | 8 |
| fast | 4 |
| medium | 2 |
| slow | 1 |

**Используется для:** `tick_rate`, `refresh_rate`, `onhit_tick_rate`.

### 1.4 Шкала шасси
| Тир | Множитель |
|---|---|
| very_low | 0.5 |
| low | 0.75 |
| medium | 1.0 |
| high | 1.5 |
| very_high | 2.0 |

**Используется для:** `health`, `speed`, `turn` шасси.

### 1.5 Шкала врагов
| Тир | Множитель | HP (база 20) | Speed (база 3.0) |
|---|---|---|---|
| very_low | 0.5 | 10 | 1.5 |
| low | 1.0 | 20 | 3.0 |
| medium | 2.0 | 40 | 6.0 |
| high | 4.0 | 80 | 12.0 |
| extreme | 8.0 | 160 | 24.0 |

**Используется для:** `health`, `speed`, `damage` врагов.

**Правило:** Не смешивать шкалы.

---

## 2. Базовые значения

Вводятся на этапе прототипа.

### 2.1 Турели
| Стат | База | Единица |
|---|---|---|
| damage | 4 | урон |
| range | 2.0 | юниты |
| radius | 1.5 | юниты |
| angle | 45 | градусы |
| speed | 5.0 | юниты/сек |

### 2.2 Шасси
| Стат | База | Единица |
|---|---|---|
| chassis_hp | 100 | HP |
| chassis_speed | 5.0 | юниты/сек |
| chassis_turn | 180 | градусы/сек |
| ability_cooldown | 10.0 | секунды |
| ability_duration | 3.0 | секунды |

### 2.3 Враги
| Стат | База | Единица |
|---|---|---|
| enemy_hp | 20 | HP |
| enemy_speed | 3.0 | юниты/сек |
| enemy_damage | 5 | урон |

**Формула:** `Итог = База × Множитель`

**Правило:** Меняете базу — всё пересчитывается. Модули не трогаете.

---

## 3. Модули

### 3.1 Два класса
| Класс | Роль | Сколько на турель |
|---|---|---|
| delivery | Как турель стреляет | Ровно 1 |
| on_hit | Что при попадании | 0 или больше |

**Правило:** Турель без delivery невалидна.

### 3.2 Delivery-модули
| Модуль | `module_type` | `added_tags` |
|---|---|---|
| Projectile | projectile | projectile |
| Hitscan | hitscan | hitscan |
| Cone | cone | cone |

### 3.3 OnHit-модули
| Модуль | Теги | `required_tags` |
|---|---|---|
| Explosion | aoe, spawn | — |
| Ground | ground, aoe, dot, spawn | — |
| Burn | dot | — |
| Freeze | control, dot | — |
| Slow | control | — |
| Knockback | control | — |
| Pierce | single | projectile |
| Homing | single | projectile |
| Beam Split | spawn, single | hitscan |
| Laser Focus | single, dot | hitscan |

### 3.4 Правила модулей
| Правило | Значение |
|---|---|
| max_depth | 5 |
| max_total_entities | 500 |
| Порядок OnHit | По priority |
| Порядок расчёта | additive → multiplicative |
| Наследование | Снаряд наследует весь билд турели |
| Конфликты | Через conflict_group |
| Требования | Через required_tags |

### 3.5 Конфликтные группы
| Группа | Теги |
|---|---|
| element | burn, freeze |

**Правило:** Внутри группы активен только один тег.

### 3.6 Hitscan: instant vs continuous
| Режим | duration | Активное поле | added_tags |
|---|---|---|---|
| Beam | instant | aps | hitscan, instant |
| Laser | continuous | tick_rate | hitscan, continuous |
| Railgun | instant | aps | hitscan, instant |

**Правило:** В `instant` используется `aps`.
**Правило:** В `continuous` используется `tick_rate`, `onhit_tick_rate`.
**Правило:** `onhit_tick_rate` не может быть чаще `tick_rate`.

### 3.7 Поля-исключения (не тиры)
| Поле | Модуль | Формат |
|---|---|---|
| damage_falloff | Pierce | Число (0.8 = -20%) |
| chance | Все OnHit | Число (0..1) |
| stack_reset_on_switch | Laser Focus | Число (1.0 = сброс всех) |

---

## 4. Турели

### 4.1 Варианты
| Значение | Что означает |
|---|---|
| base | Базовая турель |
| specialized | Специализация |

**Правило:** Одна база → несколько специализаций.
**Правило:** Специализация наследует Delivery от базы.
**Правило:** Убийства копятся на классе, а не на турели.

### 4.2 Классы оружия
| Класс | База | Delivery | Специализации |
|---|---|---|---|
| cannon | Молоток | Projectile | Пулемёт, Дробовик, Бомбарда, Пробивная пушка |
| flamethrower | Огнемёт | Cone | Freezer, Hellfire |
| laser | Лазер | Hitscan | Луч, Рейлган |
| tesla | Тесла | (отложен) | (позже) |

---

## 5. Апгрейды

### 5.1 Типы (для турелей)
| Тип | Поле связи | Источник |
|---|---|---|
| tag | required_tags | level_up |
| unique | target_turret | level_up |
| module | module_to_add | wave_reward (каждая 5-я волна) |

### 5.2 Типы (для шасси)
| Тип | Поле связи | Источник |
|---|---|---|
| tag | required_tags | level_up |
| ability | target_ability | level_up |
| unique | target_chassis | level_up |

### 5.3 Стекирование
| Тип | Как работает |
|---|---|
| additive | Складывается |
| multiplicative | Перемножается |

**Порядок:** Сначала additive, потом multiplicative.

### 5.4 stat_changes
Апгрейд может менять несколько статов. Каждый стат имеет свой `stack_type`.
Если стат живёт в OnHit-модуле — указывается `target_module`.

### 5.5 Модульные апгрейды
`required_tags` наследуется от `module_to_add`.

| Добавляет | `required_tags` |
|---|---|
| Explosion, Burn, Freeze, Ground, Slow, Knockback | `[]` |
| Pierce, Homing | `[projectile]` |
| Beam Split, Laser Focus | `[hitscan]` |

---

## 6. Шасси

### 6.1 Список шасси
| Шасси | HP | Speed | Turn | Способность | Открытие |
|---|---|---|---|---|---|
| Standart | medium | medium | medium | Boost | 0 |
| Fat | very_high | very_low | low | Nova | 5000 |
| Fancy | medium | high | high | Phase | 8000 |
| Veteran | high | medium | high | Shield | 15000 |

### 6.2 Способности
| Способность | Шасси | Тип |
|---|---|---|
| Boost | Standart | Активная (баф скорости и поворота) |
| Nova | Fat | Активная (взрыв вокруг) |
| Phase | Fancy | Активная (невидимость + проход) |
| Shield | Veteran | Активная (временный HP) |

### 6.3 Правила шасси
- Шасси — не конструктор. Только апгрейды.
- Ровно одна способность на шасси.
- Общие апгрейды — для всех шасси.
- Уникальные — только для конкретного.
- У Standart нет уникальных апгрейдов.

---

## 7. Враги

### 7.1 Пять базовых типов
| Враг | HP | Speed | Damage | XP | Роль |
|---|---|---|---|---|---|
| Runner | low | high | low | 1 | Пушечное мясо |
| Brute | medium | medium | medium | 3 | Средний враг |
| Tank | high | low | low | 5 | Губка |
| Shooter | low | medium | medium | 4 | Стреляет |
| Boss | extreme | low | extreme | 50 | Босс |

### 7.2 Логика поведения
- **Runner, Brute, Tank, Boss:** идут к игроку, бьют при контакте.
- **Shooter:** идёт к игроку, стреляет при дистанции, бьёт при контакте.

### 7.3 Правила врагов
- Враги используют **шкалу врагов** (не множители, не шасси).
- XP за убийство = `enemy_xp × множитель сложности`.
- Враги не имеют OnHit-модулей.
- Лимит врагов на экране — 300 (мягкий).
- Сложность масштабирует HP, Speed, количество, XP.
- Волна появления врага описана в `Waves/Waves.md`.

### 7.4 Поля врага
| Поле           | Обяз. | Что означает                           |
| -------------- | ----- | -------------------------------------- |
| display_name   | Да    | Название                               |
| id             | Да    | Идентификатор                          |
| stats          | Да    | health, speed, damage, xp_reward       |
| behavior       | Нет   | Только для ranged (по умолчанию melee) |
| balance_status | Да    | draft / testing / balanced             |
| tags           | Да    | enemy (+ boss для босса)               |

---

## 8. Заполнение полей

| Тип поля | Что писать |
|---|---|
| Шкала | Тир или `none` |
| Режим | Строка |
| Число | Число |
| Ссылка | Путь или пусто |

**Правило:** Шкалы → `none` (не пустое поле).
**Правило:** Ссылки → пусто (если нет).

---

## 9. Архитектура

| Термин | Значение |
|---|---|
| Паттерн | Component-Based Architecture (CBA) |
| Данные | ScriptableObject |
| Поведение | MonoBehaviour |
| Связь | Интерфейс IHitHandler |

**Запреты:** DOTS, Burst, async/await, Instantiate без пула.
**Обязательно:** Object Pooling, лимит сущностей, depth counter.

**Подробнее:** [[Technical Requirements]]

---

## 10. Системные файлы

| Файл | Что описывает |
|---|---|
| [[Waves/Waves]] | Система волн, состав, формулы |
| [[Waves/Difficulties]] | 8 сложностей, множители |
| [[Progression]] | Опыт, уровни, level-up |
| [[UI/UI — Overview]] | Экраны и переходы |
| [[Meta Progression]] | Открытия между забегами |
| [[Technical Requirements]] | Производительность, ограничения |

---

## 11. Отложено

- Chain как Delivery
- Melee, Aura, Wave, Drone
- Система слотов апгрейдов
- Стихийный урон
- Vulnerable, Bounce, Split
- Тесла

---

## 12. Статус

**Готово:**
- ✅ Шкалы, Базовые значения
- ✅ 3 Delivery, 10 OnHit
- ✅ 11 турелей (cannon, flamethrower, laser)
- ✅ 37 апгрейдов турелей
- ✅ 4 шасси
- ✅ 14 апгрейдов шасси
- ✅ 5 врагов
- ✅ Waves, Difficulties
- ✅ Progression
- ✅ UI (6 файлов)
- ✅ Meta Progression
- ✅ Technical Requirements
- ✅ Dashboard

**Дальше:**
- ⬜ Прототип в Unity