# GDD Index

Карта GDD. Открывается Архитектором при планировании фазы и при постановке
задач, которым нужен GDD-контекст. При узкой корректировке одной задачи —
не открывается.

## Как пользоваться

1. Этот файл — карта. Открывается, когда задаче нужен GDD-контекст.
2. Определи систему, нужную для задачи.
3. Открой только те `.md`, что перечислены в разделе системы.
4. Нет файла в этом индексе → вопрос человеку, а не «посмотрю, что там».
5. Папку целиком не читать, если в индексе назван конкретный файл.

Корень GDD: `Docs/GDD/`.

---

## Legend
Файл: `Docs/GDD/Legend.md`
Назначение: шкалы, тиры, множители (low/medium/high/extreme, APS, tick_rate, шкала шасси, шкала врагов).
Читать когда: нужна расшифровка тира из любого ассета (damage: medium, range: high и т.п.).

---

## Systems

### Waves
Папка: `Docs/GDD/Waves/`
Назначение: тайминг волн, формула состава, 8 сложностей и их множители.
Файлы:
- `Waves.md` — формула состава волны, спавн-батч, интервал, лимиты, боссы
- `Difficulties.md` — 8 сложностей, множители (×HP, ×Speed, ×XP, ×кол-во), таблица открытия
Читать когда: спавн, тайминг, состав волны, баланс, множители сложности, победа по волнам.

### Enemies
Папка: `Docs/GDD/Enemies/`
Назначение: 5 типов врагов, характеристики и поведение.
Файлы: `Runner.md`, `Brute.md`, `Tank.md`, `Shooter.md`, `Boss.md`
Читать когда: HP/скорость/урон/XP врага, AI-поведение, спавн конкретного типа, баланс.

### Turrets
Папка: `Docs/GDD/Turrets/`
Назначение: базовые турели, специализации, delivery- и on_hit-модули.
Подпапки:
- `Base/` — 3 базовые турели: `Flamethrower.md`, `Hammer.md`, `Laser.md`
- `Specializations/` — 8 специализаций: `Machine gun`, `Shotgun`, `Bombard`, `Piercing cannon`, `Beam`, `Railgun`, `Freezer`, `Hellfire`
- `Modules/Delivery/` — 3 модуля: `Projectile.md`, `Hitscan.md`, `Cone.md`
- `Modules/OnHit/` — 10 модулей: `Explosion`, `Burn`, `Freeze`, `Ground`, `Knockback`, `Slow`, `Pierce`, `Homing`, `Beam Split`, `Laser Focus`
Читать когда: оружие, стрельба, delivery-механика, on_hit-эффекты, специализации турелей, ДПС, синергии и конфликты модулей.

### Chassis
Папка: `Docs/GDD/Chassis/`
Назначение: 4 шасси, их статы и активные способности.
Файлы: `Standart.md`, `Fat.md`, `Fancy.md`, `Veteran.md`
Читать когда: HP/скорость/поворот шасси, способности (Boost/Nova/Phase/Shield), разблокировка по убийствам.

### Upgrades — Chassis
Папка: `Docs/GDD/Upgrades/Chassis/`
Назначение: 14 апгрейдов шасси.
Подпапки:
- `Base/` — 3 общих: `Enhanced engine`, `Improved turn`, `Reinforced hull`
- `Ability/` — 8 апгрейдов способностей: `Bigger nova`, `Faster cooldown`, `Faster nova`, `Lasting shield`, `Longer boost`, `Longer phase`, `Speed phase`, `Stronger shield`
- `Unique/` — 3 уникальных: `Elite reinforcement` (Veteran), `Iron gut` (Fat), `Unstable phase` (Fancy)
Читать когда: level-up для шасси, апгрейды способностей, уникальные модификаторы.

### Upgrades — Turrets
Папка: `Docs/GDD/Upgrades/Turrets/`
Назначение: 37 апгрейдов турелей.
Подпапки:
- `Tag/` — 16 тег-апгрейдов: `Accelerated feed`, `Accelerated ticks`, `Amplified emitter`, `Extra pellet`, `Fast charge`, `Fast focus`, `Long barrels`, `Long nozzle`, `Long optics`, `Projectile accelerator`, `Quick bolt`, `Quick discharge`, `Quick resonance`, `Reinforced barrels`, `Thick mixture`, `Wide nozzle`
- `Unique/` — 12 уникальных: `Deep freeze`, `Double shot`, `Eternal flame`, `Extra coil`, `Hellish flame`, `Hot barrel`, `Hyper piercing core`, `Piercing beam`, `Reinforced rails`, `Shrapnel bomb`, `Superconducting loop`, `Target stabilizer`
- `Module/` — 9 модульных: `Beam split upgrade`, `Cryo charge`, `Explosive rounds`, `Ground charge`, `Homing charge`, `Knockback charge`, `Piercing charge`, `Slowing charge`, `Thermite charge`
Читать когда: level-up турелей, стекирование, применение в StatBlock, wave reward.

### UI
Папка: `Docs/GDD/UI/`
Назначение: 6 экранов и их переходы.
Файлы:
- `UI — Overview.md` — карта экранов и переходов
- `UI — Main Menu.md` — главное меню
- `UI — Hangar.md` — выбор шасси и турели
- `UI — Difficulty Selection.md` — выбор сложности
- `UI — Round.md` — HUD, level-up, wave reward, пауза
- `UI — Round End.md` — итоги забега
Читать когда: экраны, кнопки, навигация, HUD, пауза, level-up UI.

### Progression / Meta Progression
Файлы:
- `Docs/GDD/Progression.md` — XP, кривая уровней, level-up, пул апгрейдов, level cap
- `Docs/GDD/Meta Progression.md` — счётчики убийств, разблокировки, сохранение, JSON-формат
Читать когда: XP-цепочка, кривая уровней, разблокировки частей/сложностей, сейв/прогресс.

### Project (общее)
Папка: `Docs/GDD/Project/`
Назначение: общие требования, платформа, монетизация, арт, звук.
Файлы:
- `Pitch.md` — концепция, USP, аудитория
- `MVP Plan.md` — MVP и v1.0
- `Technical Requirements.md` — платформа, производительность, архитектура, пулы, локализация
- `Monetization.md` — реклама, SDK
- `Audio.md`, `Visual Style.md`, `Locations.md`, `References.md`
Читать когда: общие технические ограничения (WebGL, пулы, ноль аллокаций), платформа, целевой FPS, требования к ассетам.

---

## Как читать файл системы

Один файл системы — это YAML-подобный блок (поля, статы, теги)
плюс текстовое описание Behavior/Описание.

При чтении:
- поля и тиры (`damage: medium`) расшифровывай через `Legend.md`;
- в задачах для кодеров не дублируй YAML — переводи в конкретные числа/структуры;
- если в файле есть противоречие с текущим кодом — это вопрос Архитектору,
  а не «решу по-своему».