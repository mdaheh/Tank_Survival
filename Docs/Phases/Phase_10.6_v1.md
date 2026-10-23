# Phase 10.6: Dev Tools — стабильность
- Status: in-progress
- Цель: правки параметров в Dev Menu действуют только внутри текущей сессии Play Mode и не переживают выход из Play Mode при отключённом Domain/Scene Reload.

## Задачи (порядок = приоритет)
- T082: `DevParam` — снапшот исходного значения с диска (`SerializedObject`) + автооткат всех параметров при выходе из Play Mode (`EditorApplication.playModeStateChanged`).
- T083: `DevParamRegistry` — устранить неоднозначность `SessionState` (CS0104) полным именем `TankSurvival.Core.SessionState`.

## Setup for smoke test (human)
1. Убедиться, что Project Settings → Editor → Enter Play Mode Options: включены «Do not reload Domain» и «Do not reload Scene» (настройка уже стоит; менять не нужно).

## Smoke-тест (проверяет человек)
- [x] Проект компилируется без ошибок и предупреждений.
- [x] В Edit Mode в инспекторе ассета ChassisData записать значение `Speed`.
- [x] Play Mode → F1 → в Dev Menu слайдер `speed` (группа `ChassisData…`) показывает это значение; изменить его (например, в 2 раза).
- [x] Stop → снова Play Mode → F1 → слайдер `speed` снова показывает исходное значение (не правку).
- [x] Инспектор ассета ChassisData в Edit Mode после цикла показывает исходное `Speed` (диск не изменён).
- [x] Повторить «правка → Stop → вход» для `EnemyData` (health): после перезахода слайдер снова равен диску.
