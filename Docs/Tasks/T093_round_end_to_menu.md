# T093: RoundEndUI — кнопка ведёт в Dev Menu (не рестарт)
- Status: [x]
- Phase: 10.8
- Files:
  - Assets/Scripts/UI/RoundEndUI.cs

## Задача
Изменить кнопку на экране RoundEnd: вместо перезапуска раунда (`roundController.Restart()`) она должна завершать экран и вести в Dev Menu. Связь с Dev Menu — через событие (класс `RoundEndUI` в релиз-слое не может ссылаться на dev-типы).

`Assets/Scripts/UI/RoundEndUI.cs`:
1. Добавить событие `public event System.Action OnContinue;`.
2. В `Awake()` заменить `restartButton.onClick.AddListener(Restart);` на `restartButton.onClick.AddListener(GoToMenu);`.
3. Заменить метод `Restart()` на:
   ```csharp
   private void GoToMenu()
   {
       Time.timeScale = 1f;
       panel.SetActive(false);
       OnContinue?.Invoke();
   }
   ```
   (вызов `roundController.Restart()` удалить — раунд завершён, переходим в Dev Menu).
Поле `restartButton` НЕ переименовывать (иначе придётся перепривязывать в инспекторе).

## Ограничения
- Не менять `HandlePhaseChanged`, `Show`, поля `roundController`/`panel`/`resultText`.
- Без `SceneManager.LoadScene` и без `roundController.StartRound()`/`Restart()`.
- Победа/поражение (`Win`/`Lose`) и показ `RoundEndUI` остаются без изменений.

## Unity (человеку)
- (Опционально) на кнопке `RestartButton` сменить текст дочернего `Text` с «Рестарт» на «Меню».

## Проверка человеком
- Проект компилируется без ошибок и предупреждений.
- Play Mode → пройти раунд до победы или поражения → `RoundEndPanel` активна («Победа»/«Поражение», `Time.timeScale = 0`).
- Клик по кнопке → панель скрывается, `Time.timeScale = 1`. Новый раунд автоматически НЕ запускается (Dev Menu появится в T094).

## Отчёт Исполнителя
Готово: Assets/Scripts/UI/RoundEndUI.cs: добавлено событие `public event System.Action OnContinue;`; в `Awake()` подписка заменена на `GoToMenu`; метод `Restart()` заменён на `GoToMenu()` (`Time.timeScale = 1f`, `panel.SetActive(false)`, `OnContinue?.Invoke()`), вызов `roundController.Restart()` удалён; поле `restartButton` не переименовано.
Блокер: —
