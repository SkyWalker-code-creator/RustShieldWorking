# 🛡️ RustShieldWorking

[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-6.0+-purple.svg)](https://dotnet.microsoft.com/)
[![Platform](https://img.shields.io/badge/platform-Windows-blue.svg)](https://www.microsoft.com/windows)

**Анти-чит система для поиска и обнаружения читов на компьютере**

## 📋 Описание

RustShieldWorking - это комплексная система мониторинга и обнаружения читерского ПО на компьютере. Программа анализирует различные аспекты системы для выявления потенциально опасных приложений и модификаций.

## 🚀 Функциональность

### Часть 1: Базовые операции
- `using` directives и объявления
- Конструкторы и инициализация

### Часть 2: Системная информация и Логирование
- Сбор системных характеристик
- Детальное логирование всех операций

### Часть 3: Процессы, Файлы, Реестр, USB, Активности
- Мониторинг запущенных процессов
- Сканирование файловой системы
- Анализ реестра Windows
- Отслеживание USB устройств
- Журнал активностей пользователя

### Часть 4: JumpLists, ShellBag, Startup, Services, Network
- Анализ недавних документов
- Проверка автозагрузки
- Сканирование служб Windows
- Сетевые подключения

### Часть 5: Запланированные задачи, Браузеры, Hosts, Prefetch
- Анализ планировщика задач
- Проверка расширений браузеров
- Анализ файла hosts
- Prefetch файлы
- Event Logs, Drivers, Winlogon, WMI, DNS, Ports, Memory

### Часть 6: LSA, DLL Hijacking, UAC, Антивирус, Firewall, RDP
- Безопасность LSA
- Обнаружение DLL Hijacking
- Статус UAC
- Информация об антивирусе
- Статус брандмауэра
- RDP настройки
- Время работы системы

### Часть 7: Корзина, ADS, MuiCache, ShimCache, AmCache
- Анализ корзины
- Альтернативные потоки данных (ADS)
- Кэши системы
- Сетевые шары
- Правила брандмауэра

### Часть 8: Сертификаты, Bitlocker, SecureBoot, TPM, Windows Defender
- SSL/TLS сертификаты
- Статус Bitlocker
- SecureBoot настройки
- TPM модуль
- Windows Defender
- AppLocker
- PowerShell логи
- RDP логи
- Неудачные входы
- Установленные обновления

### Часть 9: Интерфейс и Отчеты
- Добавление угроз
- Обновление прогресса
- Блокировка интерфейса
- Показ прогресса
- Сохранение отчета
- Генерация отчета
- Отправка в Telegram

## 📦 Требования

- Windows 10/11
- .NET 6.0 или выше
- Права администратора (для некоторых функций)

## 🔧 Установка

1. Скачайте последний релиз из [Releases](https://github.com/yourusername/RustShieldWorking/releases)
2. Запустите `RustShieldWorking.exe` от имени администратора
3. Нажмите "Начать сканирование"

## 🖥️ Использование

```bash
# Запуск с правами администратора
RustShieldWorking.exe

# Командная строка
RustShieldWorking.exe --silent --output report.json