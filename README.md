# SACH — Selector Adviser for Counter Heroes

Приложение для игроков в Dota 2, которое анализирует статистику с [Stratz](https://stratz.com) и предлагает наилучших героев для пика на основе выбранных союзных и вражеских героев.

## Возможности

- Выбор героев союзной и вражеской команды
- Автоматический подбор лучших контрпиков на основе реальной статистики Stratz
- Поиск героя по имени
- Бан героев правой кнопкой мыши
- Сохранение API токена между сессиями

## Требования

- Windows 10/11
- [Hiddify](https://github.com/hiddify/hiddify-app) или другой VPN с доступом к `api.stratz.com`
- API токен Stratz — получить на [stratz.com/api](https://stratz.com/api)

## Установка и запуск

1. Скачайте последний релиз из раздела [Releases](../../releases)
2. Распакуйте архив
3. Включите VPN (Hiddify или другой с доступом к Stratz)
4. Запустите `Sach.exe`
5. При первом запуске введите API токен Stratz и нажмите **ПРИНЯТЬ**

> Приложение автоматически запустит фоновый сервер `SachServer.exe` — не закрывайте появившееся окно браузера Chromium, оно необходимо для работы приложения.

## Использование

1. В левой части экрана выберите героев **союзной команды** (синие слоты)
2. В правой части выберите героев **вражеской команды** (красные слоты)
3. Нажмите на любого героя из списка чтобы назначить его выбранному слоту
4. Приложение автоматически покажет рекомендованных героев для пика
5. Правой кнопкой мыши можно **забанить** героя — он не будет предлагаться

## Сборка из исходников

### Требования
- [.NET 7 SDK](https://dotnet.microsoft.com/download/dotnet/7.0)
- [JetBrains Rider](https://www.jetbrains.com/rider/) или Visual Studio 2022

### Шаги

1. Клонируйте репозиторий:
```bash
git clone https://github.com/ShadeRey/Sach.git
cd Sach
```

2. Откройте `Sach.sln` в Rider

3. Соберите оба проекта: `Sach` и `SachServer`

4. Скопируйте содержимое папки `SachServer/bin/Debug/net7.0/` в папку `Sach/bin/Debug/net7.0/SachServer/`

5. Запустите `Sach`

## Технологии

- [Avalonia UI](https://avaloniaui.net/) — кроссплатформенный UI фреймворк
- [ReactiveUI](https://www.reactiveui.net/) — реактивное программирование
- [Stratz GraphQL API](https://api.stratz.com/graphiql) — статистика Dota 2
- [Microsoft Playwright](https://playwright.dev/dotnet/) — обход Cloudflare защиты
- ASP.NET Core — локальный прокси-сервер

## Лицензия

MIT
