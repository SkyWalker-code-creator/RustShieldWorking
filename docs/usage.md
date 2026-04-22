# Руководство по использованию RustShieldWorking

## Быстрый старт

1. Запустите программу от имени администратора
2. Выберите тип сканирования:
   - Быстрое (5-10 минут)
   - Полное (20-30 минут)
   - Выборочное (выберите компоненты)

3. Нажмите "Начать сканирование"

## Компоненты сканирования

### Системные процессы
- Поиск подозрительных процессов
- Проверка цифровых подписей
- Анализ потребления ресурсов

### Файловая система
- Сканирование Program Files
- Проверка временных папок
- Поиск скрытых файлов

### Реестр Windows
- Ключи автозагрузки
- Shell команд
- AppInit_DLLs

### Сеть
- Открытые порты
- DNS кэш
- Hosts файл

## Форматы отчетов

### JSON
```json
{
  "scan_time": "2024-01-01T12:00:00",
  "threats_found": 3,
  "details": [...]
}

HTML
Веб-отчет с подсветкой угроз

CSV
Для анализа в Excel

Telegram бот
Создайте бота у @BotFather

Получите токен и chat_id

Настройте в программе

Отчеты будут отправляться автоматически

Советы
Запускайте сканирование после подозрительной активности

Сохраняйте отчеты для анализа трендов

Регулярно обновляйте программу


### **RustShieldWorking.csproj**
```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net6.0-windows</TargetFramework>
    <Nullable>enable</Nullable>
    <UseWPF>true</UseWPF>
    <UseWindowsForms>true</UseWindowsForms>
    <ApplicationIcon>Resources/icon.ico</ApplicationIcon>
    <AssemblyName>RustShieldWorking</AssemblyName>
    <RootNamespace>RustShieldWorking</RootNamespace>
    <Version>1.0.0</Version>
    <Authors>RustShieldTeam</Authors>
    <Company>RustShield</Company>
    <Product>RustShield Working Anti-Cheat</Product>
    <Description>Комплексная система обнаружения читов на компьютере</Description>
    <PackageProjectUrl>https://github.com/yourusername/RustShieldWorking</PackageProjectUrl>
    <RepositoryUrl>https://github.com/yourusername/RustShieldWorking</RepositoryUrl>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="System.Management" Version="7.0.2" />
    <PackageReference Include="Microsoft.Win32.Registry" Version="5.0.0" />
    <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
    <PackageReference Include="Telegram.Bot" Version="19.0.0" />
    <PackageReference Include="Microsoft.Extensions.Logging" Version="7.0.0" />
    <PackageReference Include="System.Security.Cryptography.ProtectedData" Version="7.0.1" />
  </ItemGroup>

  <ItemGroup>
    <Compile Update="Properties\Settings.Designer.cs">
      <DesignTimeSharedInput>True</DesignTimeSharedInput>
      <AutoGen>True</AutoGen>
      <DependentUpon>Settings.settings</DependentUpon>
    </Compile>
  </ItemGroup>

</Project>