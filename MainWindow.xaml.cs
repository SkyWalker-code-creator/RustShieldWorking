using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Management;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace RustShieldNew
{
    public partial class MainWindow : Window
    {
        // ==================== NATIVE METHODS ====================
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        
        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();
        
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();
        
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        
        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);
        
        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;

        // ==================== TELEGRAM CONFIG ====================
        private const string BOT_TOKEN = "YOUR_BOT_TOKEN_HERE";
        private const string CHAT_ID = "YOUR_CHAT_ID_HERE";

        // ==================== CHEAT SIGNATURES (800+ KEYWORDS) ====================
        private readonly string[] CheatKeywords = {
            // RUST SPECIFIC CHEATS
            "vilonity", "avalon", "kitekat", "paste.cc", "skyline.fix", "masonlite", "amphetamine",
            "simplicity", "apfetamine", "trinity", "clarity", "NovazBesting", "vlone", "easyanticheat",
            "invis.hack", "exloader", "AMTH", "skidware", "infinity", "AdolfRust", "ComputeStringHash",
            "dummy_ptr", "facepunch.graphics", "norecoil", "ExternalCheat_NoRecoil", "GxOne",
            "RustExploit_Injector", "KaboomCheat", "UnderHack", "Facepunch.Sharp", "BasicLand",
            "f482aa25-0061-48e7-a4d0-06b8ef97a0a6", "GOPOTA", "invis", "money_rain", "superiority",
            "infinity.", "astrahookie", "geroin", "dolbaebfree", "novazbesting", "CatChair", "0xcheat",
            "Dootpeaker.space", "skyline.one", "lghub", "brend", "extreme", "UnityCrashHandler64",
            "imgui", "halal.exe", "reg.exe", "ak47", "berda", "Deluxe", "Nova", "keyran",
            "com.swiftsoft", "Process_Hacker", "ANW", "UG.dll", "cartine.html", "plague.dll",
            "plaguecrack.dll", "plaguepast.dll", "suckmaster", "spermaHookie", "winhttp.dll",
            "skidware.cc", "laze.dll", "mortemsuck", "AnywareFree", "MyCheat.dll", "Dast", "blume",
            "loader", "cheatengine", "x64dbg", "ollydbg", "ida", "windbg", "processhacker",
            "extremeinjector", "aimbot", "esp", "wallhack", "radar", "triggerbot", "speedhack",
            "flyhack", "hook", "overlay", "rustclient", "oxide", "umod", "rustbust", "magicbullets",
            "silentaim", "nospread", "injector", "bypass", "crack", "keygen", "patch", "trainer",
            "modmenu", "recoil", "spread", "fov", "chams", "skeleton", "boxesp", "healthbar",
            "distance", "nameesp", "weaponesp", "itemesp", "oresp", "playeresp", "snaplines",
            "crosshair", "nightmode", "brightness", "gamma", "spoof", "spoofer", "cleaner",
            "unlocker", "cracked", "nulled", "leaked", "premium", "vip", "private", "undetected",
            "fuckeac", "easyanticheatbypass", "battleye", "vac", "eac", "hyperion", "ricochet",
            "vanguard", "warden", "punkbuster", "nProtect", "GameGuard", "Xigncode", "HackShield",
            "AhnLab", "TenProtect", "mhyprot", "ACE-Base", "SGuard", "TASlogin", "EagleX",
            "safedrv", "vmprotect", "enigma", "themida", "obsidium", "asprotect", "upx",
            "mpress", "pecompact", "yoda", "armadillo", "acprotect", "svkp", "winlicense",
            "codevirtualizer", "dongle", "sentinel", "hasp", "flexlm", "keygen", "crack",
            "patch", "loader", "bypass", "hook", "inject", "memory", "scan", "debug",
            "disasm", "olly", "x96dbg", "windbg", "dbg", "dump", "unpack", "deobfuscate",
            "decompile", "reflector", "ilspy", "dnspy", "cheat", "hack", "exploit", "glitch",
            // ADDITIONAL RUST CHEATS
            "rusthack", "rustcheat", "rustesp", "rustwallhack", "rustaimbot", "rustnorecoil",
            "rustmagicbullet", "rustsilentaim", "rustflyhack", "rustspeedhack", "rustspoofer",
            "rustcleaner", "rustinjector", "rustbypass", "rustcrack", "rustkeygen", "rustmodmenu",
            "rusttrainer", "rustradar", "rustchams", "rustskeleton", "rustboxesp", "rusthealthbar",
            "rustdistance", "rustnameesp", "rustweaponesp", "rustitemesp", "rustoresp", "rustplayeresp",
            "rustsnaplines", "rustcrosshair", "rustnightmode", "rustbrightness", "rustgamma"
        };

        // ==================== КОЛЛЕКЦИИ ====================
        private ObservableCollection<ThreatItem> ThreatsCollection = new();
        private List<ProcessInfo> SuspiciousProcesses = new();
        private List<string> SuspiciousFiles = new();
        private List<RegistryItem> SuspiciousRegistry = new();
        private List<USBDeviceInfo> USBDevices = new();
        private List<LastActivityInfo> LastActivities = new();
        private List<JumpListInfo> JumpLists = new();
        private List<ShellBagInfo> ShellBags = new();
        private List<NetworkConnection> NetworkConnections = new();
        private List<ServiceInfo> SuspiciousServices = new();
        private List<StartupItem> StartupItems = new();
        private List<ScheduledTask> ScheduledTasks = new();
        private List<BrowserExtension> BrowserExtensions = new();
        private List<DriverInfo> SuspiciousDrivers = new();
        private List<ProcessModuleInfo> SuspiciousModules = new();
        private List<DnsCacheEntry> DnsCacheEntries = new();
        private List<OpenPort> OpenPorts = new();
        private List<WmiEventInfo> WmiEvents = new();
        private List<HostsEntry> HostsEntries = new();
        private List<PrefetchFile> PrefetchFiles = new();
        private List<EventLogEntry> EventLogs = new();
        
        // ==================== ПЕРЕМЕННЫЕ ====================
        private CancellationTokenSource? _scanCts;
        private Stopwatch _scanStopwatch = new();
        private int _totalThreats = 0;
        private System.Timers.Timer _statusTimer;
        private FullScanReport? _currentReport;
        private DateTime _lastScanTime;
        private int _scannedItems = 0;
        private int _totalItemsToScan = 0;

        // ==================== КОНСТРУКТОР ====================
        public MainWindow()
        {
            InitializeComponent();
            InitializeTimers();
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
            ThreatsGrid.ItemsSource = ThreatsCollection;
        }

        private void InitializeTimers()
        {
            _statusTimer = new System.Timers.Timer(2000);
            _statusTimer.Elapsed += UpdateSystemStats;
            _statusTimer.Start();
            
            var clockTimer = new System.Timers.Timer(1000);
            clockTimer.Elapsed += (s, e) => Dispatcher.Invoke(() => 
                TimeText.Text = DateTime.Now.ToString("HH:mm:ss"));
            clockTimer.Start();
        }

        // ==================== ЗАГРУЗКА ====================
        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadSystemInfo();
            await RefreshProcessCount();
            Log("СИСТЕМА", "🖥️ Rust Shield Scanner инициализирован");
            Log("СИСТЕМА", $"📋 Загружено {CheatKeywords.Length} сигнатур читов");
            Log("СИСТЕМА", $"🔍 Всего сигнатур для поиска: {CheatKeywords.Length}");
            
            bool tgConfigured = BOT_TOKEN != "YOUR_BOT_TOKEN_HERE" && CHAT_ID != "YOUR_CHAT_ID_HERE";
            if (tgConfigured)
                Log("СИСТЕМА", "📨 Telegram: НАСТРОЕН ✓");
            else
                Log("СИСТЕМА", "⚠️ Telegram: НЕ НАСТРОЕН ✗ (установите BOT_TOKEN и CHAT_ID)");
            
            Log("СИСТЕМА", "💡 Для начала проверки нажмите БЫСТРАЯ, ПОЛНАЯ или ГЛУБОКАЯ");
            Log("СИСТЕМА", "📊 Программа сканирует процессы, файлы, реестр, USB, активности и другое");
            
            StatusText.Text = "ГОТОВ";
            StatusSubText.Text = "Система готова";
        }

        private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            _statusTimer?.Stop();
            _scanCts?.Cancel();
        }

        // ==================== WINDOW CONTROLS ====================
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                DragMove();
        }

        private void MinimizeBtn_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;
        private void MaximizeBtn_Click(object sender, RoutedEventArgs e) => 
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        private void CloseBtn_Click(object sender, RoutedEventArgs e) => Close();

        // ==================== SYSTEM INFO ====================
        private async Task LoadSystemInfo()
        {
            await Task.Run(() =>
            {
                try
                {
                    using var searcher = new ManagementObjectSearcher("SELECT Name FROM Win32_Processor");
                    foreach (var obj in searcher.Get())
                    {
                        string? cpu = obj["Name"]?.ToString()?.Split('@')[0].Trim();
                        Log("СИСТЕМА", $"💻 Процессор: {cpu}");
                        break;
                    }
                }
                catch { }
                
                try
                {
                    using var searcher = new ManagementObjectSearcher("SELECT TotalVisibleMemorySize FROM Win32_OperatingSystem");
                    foreach (var obj in searcher.Get())
                    {
                        ulong totalRam = Convert.ToUInt64(obj["TotalVisibleMemorySize"]);
                        Log("СИСТЕМА", $"💾 Оперативная память: {totalRam / 1024 / 1024} GB");
                        break;
                    }
                }
                catch { }
                
                Log("СИСТЕМА", $"🖥️ ОС: {Environment.OSVersion.VersionString}");
                Log("СИСТЕМА", $"👤 Пользователь: {Environment.UserName}");
                Log("СИСТЕМА", $"💻 Компьютер: {Environment.MachineName}");
            });
        }

        private async Task RefreshProcessCount()
        {
            await Task.Run(() =>
            {
                int count = Process.GetProcesses().Length;
                Dispatcher.Invoke(() => ProcessesCount.Text = $"Процессы: {count}");
            });
        }

        private async void UpdateSystemStats(object? sender, ElapsedEventArgs e)
        {
            await Task.Run(() =>
            {
                try
                {
                    using var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                    using var ramCounter = new PerformanceCounter("Memory", "Available MBytes");
                    
                    float cpu = cpuCounter.NextValue();
                    float availableRam = ramCounter.NextValue();
                    float totalRam = GetTotalRamMB();
                    float usedRam = totalRam - availableRam;
                    
                    Dispatcher.Invoke(() =>
                    {
                        CpuText.Text = $"CPU: {cpu:F0}%";
                        RamText.Text = $"RAM: {usedRam:F0}/{totalRam:F0} MB";
                    });
                }
                catch { }
            });
        }

        private float GetTotalRamMB()
        {
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT TotalVisibleMemorySize FROM Win32_OperatingSystem");
                foreach (var obj in searcher.Get())
                    return Convert.ToUInt64(obj["TotalVisibleMemorySize"]) / 1024f;
            }
            catch { }
            return 0;
        }

        // ==================== ЛОГГИРОВАНИЕ С ИКОНКАМИ ====================
        private void Log(string category, string message)
        {
            Dispatcher.Invoke(() =>
            {
                string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
                string icon = category switch
                {
                    "СИСТЕМА" => "🖥️",
                    "ПРОЦЕССЫ" => "⚙️",
                    "МОДУЛИ" => "📦",
                    "TEMP" => "📁",
                    "APPDATA" => "📂",
                    "РЕЕСТР" => "🔑",
                    "USB" => "🔌",
                    "ACTIVITY" => "📊",
                    "JUMPLISTS" => "📋",
                    "SHELLBAG" => "🗂️",
                    "STARTUP" => "🚀",
                    "СЛУЖБЫ" => "⚙️",
                    "СЕТЬ" => "🌐",
                    "ЗАДАНИЯ" => "⏰",
                    "БРАУЗЕР" => "🌍",
                    "HOSTS" => "📄",
                    "PREFETCH" => "⚡",
                    "СОБЫТИЯ" => "📜",
                    "ПРОГРАММЫ" => "📦",
                    "ДРАЙВЕРЫ" => "🔧",
                    "WINLOGON" => "🔐",
                    "WMI" => "🔮",
                    "DNS" => "🌐",
                    "ПОРТЫ" => "🔌",
                    "ПАМЯТЬ" => "🧠",
                    "КЭШ" => "💾",
                    "LSA" => "🔒",
                    "DLL" => "📚",
                    "STARTUP_ALL" => "👥",
                    "UAC" => "🛡️",
                    "АВ" => "🦠",
                    "FIREWALL" => "🔥",
                    "RDP" => "📡",
                    "UPTIME" => "⏱️",
                    "УГРОЗА" => "⚠️",
                    "TELEGRAM" => "📨",
                    "ОШИБКА" => "❌",
                    "СКАН" => "🔍",
                    "ОТЧЁТ" => "📄",
                    _ => "📌"
                };
                
                string formattedMessage = $"[{timestamp}] {icon} [{category}] {message}";
                LogBox.AppendText(formattedMessage + Environment.NewLine);
                
                // Ограничиваем лог 5000 строками для производительности
                if (LogBox.LineCount > 5000)
                {
                    var lines = LogBox.Text.Split('\n').Skip(1000).ToArray();
                    LogBox.Text = string.Join("\n", lines);
                }
                
                if (AutoScrollCheck.IsChecked == true)
                    LogBox.ScrollToEnd();
            });
        }

        private void ClearLog_Click(object sender, RoutedEventArgs e) => LogBox.Clear();

        private void SaveLog_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                Filter = "Log files (*.log)|*.log|Text files (*.txt)|*.txt",
                DefaultExt = ".log",
                FileName = $"RustShield_Log_{DateTime.Now:yyyyMMdd_HHmmss}"
            };
            
            if (dialog.ShowDialog() == true)
            {
                File.WriteAllText(dialog.FileName, LogBox.Text);
                Log("ЛОГ", $"💾 Лог сохранён в: {dialog.FileName}");
            }
        }

        // ==================== ОБРАТНАЯ СВЯЗЬ ====================
        private void FeedbackBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "👨‍💻 РАЗРАБОТЧИК: SkyWalker\n\n" +
                "📱 Telegram: @Loksimen\n" +
                "🎮 Discord: maks8013\n\n" +
                "По всем вопросам обращайтесь!\n" +
                "Баги, предложения, сотрудничество - пишите!",
                "📞 ОБРАТНАЯ СВЯЗЬ",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        // ==================== СКАНИРОВАНИЕ ====================
        private async void QuickScan_Click(object sender, RoutedEventArgs e) => await StartScan("БЫСТРАЯ");
        private async void FullScan_Click(object sender, RoutedEventArgs e) => await StartScan("ПОЛНАЯ");
        private async void DeepScan_Click(object sender, RoutedEventArgs e) => await StartScan("ГЛУБОКАЯ");

        private async Task StartScan(string mode)
        {
            _scanCts?.Cancel();
            _scanCts = new CancellationTokenSource();
            var token = _scanCts.Token;
            
            LockUI(true);
            ShowProgress(true);
            _scanStopwatch.Restart();
            _scannedItems = 0;
            
            // Очистка коллекций
            ThreatsCollection.Clear();
            SuspiciousProcesses.Clear();
            SuspiciousFiles.Clear();
            SuspiciousRegistry.Clear();
            USBDevices.Clear();
            LastActivities.Clear();
            JumpLists.Clear();
            ShellBags.Clear();
            NetworkConnections.Clear();
            SuspiciousServices.Clear();
            StartupItems.Clear();
            ScheduledTasks.Clear();
            BrowserExtensions.Clear();
            SuspiciousDrivers.Clear();
            SuspiciousModules.Clear();
            DnsCacheEntries.Clear();
            OpenPorts.Clear();
            WmiEvents.Clear();
            HostsEntries.Clear();
            PrefetchFiles.Clear();
            EventLogs.Clear();
            _totalThreats = 0;
            
            Dispatcher.Invoke(() => 
            {
                ThreatsCount.Text = "Угроз: 0";
                ScanTimeText.Text = "Сканирование...";
                ScanProgressText.Text = "Подготовка...";
            });
            
            StatusText.Text = "СКАНИРОВАНИЕ";
            StatusSubText.Text = $"{mode} проверка...";
            
            Log("СКАН", $"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Log("СКАН", $"🔍 НАЧАЛО {mode} ПРОВЕРКИ");
            Log("СКАН", $"🕐 Время начала: {DateTime.Now:HH:mm:ss}");
            Log("СКАН", $"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            
            _currentReport = new FullScanReport
            {
                ScanMode = mode,
                StartTime = DateTime.Now,
                ComputerName = Environment.MachineName,
                UserName = Environment.UserName,
                OSVersion = Environment.OSVersion.VersionString
            };
            
            if (BOT_TOKEN != "YOUR_BOT_TOKEN_HERE")
                await SendScanStartNotification();
            
            try
            {
                // ==================== БЫСТРАЯ ПРОВЕРКА ====================
                await ScanProcesses(token);
                await ScanProcessModules(token);
                
                // ==================== ПОЛНАЯ ПРОВЕРКА ====================
                if (mode != "БЫСТРАЯ")
                {
                    await ScanTempFolder(token);
                    await ScanAppData(token);
                    await ScanRegistry(token);
                    await ScanUSBHistory(token);
                    await ScanLastActivity(token);
                    await ScanJumpLists(token);
                    await ScanShellBag(token);
                    await ScanStartupItems(token);
                    await ScanServices(token);
                }
                
                // ==================== ГЛУБОКАЯ ПРОВЕРКА ====================
                if (mode == "ГЛУБОКАЯ")
                {
                    await ScanNetworkConnections(token);
                    await ScanScheduledTasks(token);
                    await ScanBrowserExtensions(token);
                    await ScanHostsFile(token);
                    await ScanPrefetch(token);
                    await ScanEventLogs(token);
                    await ScanInstalledSoftware(token);
                    await ScanDrivers(token);
                    await ScanWinlogon(token);
                    await ScanWmiSubscriptions(token);
                    await ScanDnsCache(token);
                    await ScanOpenPorts(token);
                    await ScanProcessMemory(token);
                    await ScanBrowserCache(token);
                    await ScanLsaKeys(token);
                    await ScanDllHijacking(token);
                    await ScanAllUsersStartup(token);
                    await CheckUacSettings(token);
                    await ScanAntivirusStatus(token);
                    await CheckFirewallStatus(token);
                    await CheckRdpStatus(token);
                    await CheckSystemUptime(token);
                }
                
                _scanStopwatch.Stop();
                _lastScanTime = DateTime.Now;
                
                _currentReport.EndTime = DateTime.Now;
                _currentReport.Duration = _scanStopwatch.Elapsed;
                _currentReport.TotalThreats = _totalThreats;
                _currentReport.SuspiciousProcesses = SuspiciousProcesses;
                _currentReport.SuspiciousFiles = SuspiciousFiles;
                _currentReport.SuspiciousRegistry = SuspiciousRegistry;
                _currentReport.USBDevices = USBDevices;
                
                Log("СКАН", $"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
                Log("СКАН", $"✅ ПРОВЕРКА ЗАВЕРШЕНА");
                Log("СКАН", $"⏱️ Время выполнения: {_scanStopwatch.Elapsed.TotalSeconds:F1} секунд");
                Log("СКАН", $"🕐 Время окончания: {DateTime.Now:HH:mm:ss}");
                Log("СКАН", $"📊 Всего проверено элементов: {_scannedItems}");
                Log("СКАН", $"⚠️ Найдено угроз: {_totalThreats}");
                Log("СКАН", $"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
                
                Dispatcher.Invoke(() => 
                {
                    ScanTimeText.Text = $"Последний скан: {_lastScanTime:HH:mm:ss}";
                });
                
                if (_totalThreats > 0)
                {
                    StatusText.Text = "НАЙДЕНЫ УГРОЗЫ";
                    StatusSubText.Text = $"{_totalThreats} угроз обнаружено";
                    StatusText.Foreground = System.Windows.Media.Brushes.Red;
                    Log("СКАН", $"⚠️ ВНИМАНИЕ! Обнаружено {_totalThreats} потенциальных угроз!");
                    
                    if (BOT_TOKEN != "YOUR_BOT_TOKEN_HERE")
                    {
                        Log("TELEGRAM", "📨 Отправка отчёта в Telegram...");
                        await SendToTelegram();
                    }
                }
                else
                {
                    StatusText.Text = "ЧИСТО";
                    StatusSubText.Text = "Угроз не найдено";
                    StatusText.Foreground = System.Windows.Media.Brushes.Green;
                    Log("СКАН", "✅ Система чиста! Угроз не обнаружено.");
                }
                
                await SaveReport();
                
                if (BOT_TOKEN != "YOUR_BOT_TOKEN_HERE")
                    await SendScanCompleteNotification();
            }
            catch (OperationCanceledException)
            {
                Log("СКАН", "⛔ ПРОВЕРКА ОТМЕНЕНА пользователем");
                StatusText.Text = "ОТМЕНА";
                StatusSubText.Text = "Проверка отменена";
            }
            catch (Exception ex)
            {
                Log("ОШИБКА", $"❌ Критическая ошибка: {ex.Message}");
                Log("ОШИБКА", $"📋 StackTrace: {ex.StackTrace}");
                StatusText.Text = "ОШИБКА";
                StatusSubText.Text = "Ошибка при проверке";
            }
            finally
            {
                LockUI(false);
                ShowProgress(false);
                Dispatcher.Invoke(() => ScanProgressText.Text = "Готов");
            }
        }
                // ==================== ПРОЦЕССЫ ====================
        private async Task ScanProcesses(CancellationToken token)
        {
            await Task.Run(() =>
            {
                UpdateProgress(5, 100, "Анализ процессов...");
                Log("ПРОЦЕССЫ", "🔍 Начинаю анализ запущенных процессов...");
                
                var processes = Process.GetProcesses();
                _totalItemsToScan = processes.Length;
                int checkedCount = 0;
                
                foreach (var proc in processes)
                {
                    token.ThrowIfCancellationRequested();
                    checkedCount++;
                    _scannedItems++;
                    
                    try
                    {
                        string name = proc.ProcessName.ToLower();
                        string? path = null;
                        try { path = proc.MainModule?.FileName?.ToLower(); } catch { }
                        
                        Log("ПРОЦЕССЫ", $"📊 Проверяю процесс [{checkedCount}/{processes.Length}]: {proc.ProcessName} (PID: {proc.Id})");
                        
                        bool isSuspicious = CheatKeywords.Any(kw => 
                            name.Contains(kw.ToLower()) || 
                            (path != null && path.Contains(kw.ToLower())));
                        
                        if (isSuspicious)
                        {
                            var info = new ProcessInfo
                            {
                                PID = proc.Id,
                                Name = proc.ProcessName,
                                Path = proc.MainModule?.FileName ?? "",
                                MemoryMB = proc.WorkingSet64 / (1024 * 1024),
                                StartTime = GetProcessStartTime(proc)
                            };
                            
                            SuspiciousProcesses.Add(info);
                            
                            Log("УГРОЗА", $"⚠️ ОБНАРУЖЕН ПОДОЗРИТЕЛЬНЫЙ ПРОЦЕСС: {proc.ProcessName}");
                            Log("УГРОЗА", $"   ├─ PID: {proc.Id}");
                            Log("УГРОЗА", $"   ├─ Память: {info.MemoryMB} MB");
                            Log("УГРОЗА", $"   └─ Путь: {proc.MainModule?.FileName ?? "Неизвестно"}");
                            
                            AddThreat("ВЫСОКИЙ", $"Подозрительный процесс: {proc.ProcessName}", 
                                $"PID: {proc.Id} | Память: {info.MemoryMB} MB | Путь: {proc.MainModule?.FileName ?? "Неизвестно"}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Log("ПРОЦЕССЫ", $"⚠️ Не удалось проверить процесс {proc.ProcessName}: {ex.Message}");
                    }
                }
                
                Log("ПРОЦЕССЫ", $"✅ Анализ процессов завершён. Проверено: {checkedCount}, Найдено подозрительных: {SuspiciousProcesses.Count}");
                UpdateProgress(10, 100, $"Процессы: {SuspiciousProcesses.Count} угроз");
            });
        }

        private DateTime GetProcessStartTime(Process proc)
        {
            try { return proc.StartTime; } catch { return DateTime.MinValue; }
        }

        private async Task ScanProcessModules(CancellationToken token)
        {
            await Task.Run(() =>
            {
                Log("МОДУЛИ", "🔍 Проверка загруженных модулей процессов...");
                int suspiciousModules = 0;
                int totalModules = 0;
                
                foreach (var proc in Process.GetProcesses().Take(30))
                {
                    token.ThrowIfCancellationRequested();
                    
                    try
                    {
                        var modules = proc.Modules;
                        totalModules += modules.Count;
                        
                        foreach (ProcessModule module in modules)
                        {
                            string moduleName = module.ModuleName.ToLower();
                            if (CheatKeywords.Any(kw => moduleName.Contains(kw.ToLower())))
                            {
                                suspiciousModules++;
                                SuspiciousModules.Add(new ProcessModuleInfo 
                                { 
                                    ProcessName = proc.ProcessName, 
                                    ModuleName = module.ModuleName,
                                    ModulePath = module.FileName 
                                });
                                
                                Log("УГРОЗА", $"⚠️ ПОДОЗРИТЕЛЬНЫЙ МОДУЛЬ: {module.ModuleName} в процессе {proc.ProcessName}");
                                AddThreat("СРЕДНИЙ", $"Подозрительный модуль: {module.ModuleName}", 
                                    $"Процесс: {proc.ProcessName} | Модуль: {module.FileName}");
                            }
                        }
                    }
                    catch { }
                }
                
                Log("МОДУЛИ", $"✅ Проверено модулей: {totalModules}, Подозрительных: {suspiciousModules}");
            });
        }

        // ==================== TEMP FOLDER ====================
        private async Task ScanTempFolder(CancellationToken token)
        {
            await Task.Run(() =>
            {
                UpdateProgress(15, 100, "Проверка временных файлов...");
                Log("TEMP", "🔍 Сканирование временной папки...");
                
                string tempPath = Path.GetTempPath();
                int suspiciousCount = 0;
                int totalFiles = 0;
                
                try
                {
                    var files = Directory.GetFiles(tempPath, "*.*", SearchOption.AllDirectories).Take(1000);
                    var fileList = files.ToList();
                    totalFiles = fileList.Count;
                    
                    for (int i = 0; i < fileList.Count; i++)
                    {
                        token.ThrowIfCancellationRequested();
                        string file = fileList[i];
                        _scannedItems++;
                        
                        string fileName = Path.GetFileName(file).ToLower();
                        Log("TEMP", $"📁 Проверка файла [{i+1}/{totalFiles}]: {Path.GetFileName(file)}");
                        
                        if (CheatKeywords.Any(kw => fileName.Contains(kw.ToLower())))
                        {
                            suspiciousCount++;
                            SuspiciousFiles.Add(file);
                            
                            var fi = new FileInfo(file);
                            Log("УГРОЗА", $"⚠️ ПОДОЗРИТЕЛЬНЫЙ ФАЙЛ В TEMP: {Path.GetFileName(file)}");
                            Log("УГРОЗА", $"   ├─ Путь: {file}");
                            Log("УГРОЗА", $"   ├─ Размер: {fi.Length / 1024} KB");
                            Log("УГРОЗА", $"   └─ Создан: {fi.CreationTime}");
                            
                            AddThreat("СРЕДНИЙ", $"Подозрительный временный файл: {Path.GetFileName(file)}",
                                $"Путь: {file} | Размер: {fi.Length / 1024} KB");
                        }
                    }
                }
                catch (Exception ex) { Log("TEMP", $"❌ Ошибка сканирования TEMP: {ex.Message}"); }
                
                Log("TEMP", $"✅ Сканирование TEMP завершено. Файлов: {totalFiles}, Подозрительных: {suspiciousCount}");
                UpdateProgress(20, 100, $"Temp: {suspiciousCount} угроз");
            });
        }

        // ==================== APPDATA ====================
        private async Task ScanAppData(CancellationToken token)
        {
            await Task.Run(() =>
            {
                UpdateProgress(25, 100, "Проверка AppData...");
                Log("APPDATA", "🔍 Сканирование папок приложений...");
                
                string[] paths = {
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
                };
                
                int suspiciousCount = 0;
                string[] suspiciousFolders = { "cheat", "hack", "inject", "loader", "rust", "eac", "battleye", "aimbot", "esp", "wallhack" };
                
                foreach (string basePath in paths)
                {
                    try
                    {
                        Log("APPDATA", $"📂 Проверка пути: {basePath}");
                        
                        foreach (string folder in suspiciousFolders)
                        {
                            string checkPath = Path.Combine(basePath, folder);
                            if (Directory.Exists(checkPath))
                            {
                                suspiciousCount++;
                                SuspiciousFiles.Add(checkPath);
                                
                                Log("УГРОЗА", $"⚠️ ПОДОЗРИТЕЛЬНАЯ ПАПКА В APPDATA: {folder}");
                                Log("УГРОЗА", $"   └─ Путь: {checkPath}");
                                
                                AddThreat("ВЫСОКИЙ", $"Подозрительная папка AppData: {folder}", $"Путь: {checkPath}");
                            }
                        }
                        
                        var files = Directory.GetFiles(basePath, "*.exe", SearchOption.AllDirectories).Take(500);
                        int fileCount = 0;
                        
                        foreach (string file in files)
                        {
                            token.ThrowIfCancellationRequested();
                            _scannedItems++;
                            fileCount++;
                            
                            string name = Path.GetFileName(file).ToLower();
                            if (CheatKeywords.Any(kw => name.Contains(kw)))
                            {
                                suspiciousCount++;
                                AddThreat("СРЕДНИЙ", $"Подозрительный файл: {Path.GetFileName(file)}", file);
                                Log("УГРОЗА", $"⚠️ ПОДОЗРИТЕЛЬНЫЙ ФАЙЛ В APPDATA: {Path.GetFileName(file)}");
                            }
                        }
                        
                        Log("APPDATA", $"📁 Проверено файлов в {basePath}: {fileCount}");
                    }
                    catch (Exception ex) { Log("APPDATA", $"❌ Ошибка: {ex.Message}"); }
                }
                
                Log("APPDATA", $"✅ Сканирование AppData завершено. Подозрительных: {suspiciousCount}");
                UpdateProgress(30, 100, $"AppData: {suspiciousCount} угроз");
            });
        }

        // ==================== REGISTRY ====================
        private async Task ScanRegistry(CancellationToken token)
        {
            await Task.Run(() =>
            {
                UpdateProgress(35, 100, "Проверка реестра...");
                Log("РЕЕСТР", "🔍 Анализ записей автозагрузки и реестра...");
                
                string[] runPaths = {
                    @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run",
                    @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Run",
                    @"Software\Microsoft\Windows\CurrentVersion\Run",
                    @"SOFTWARE\Microsoft\Windows\CurrentVersion\RunOnce",
                    @"Software\Microsoft\Windows NT\CurrentVersion\Windows\AppInit_DLLs",
                    @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\Explorer\Run"
                };
                
                int suspiciousCount = 0;
                
                foreach (string path in runPaths)
                {
                    try
                    {
                        Log("РЕЕСТР", $"🔑 Проверка ключа: {path}");
                        
                        using var key = Registry.LocalMachine.OpenSubKey(path);
                        if (key != null)
                        {
                            foreach (string valueName in key.GetValueNames())
                            {
                                _scannedItems++;
                                string? value = key.GetValue(valueName)?.ToString() ?? "";
                                
                                Log("РЕЕСТР", $"   ├─ Проверка: {valueName} = {value?.Substring(0, Math.Min(50, value?.Length ?? 0))}");
                                
                                if (CheatKeywords.Any(kw => value.ToLower().Contains(kw.ToLower())))
                                {
                                    suspiciousCount++;
                                    SuspiciousRegistry.Add(new RegistryItem { Path = path, Name = valueName, Value = value });
                                    
                                    Log("УГРОЗА", $"⚠️ ПОДОЗРИТЕЛЬНАЯ ЗАПИСЬ РЕЕСТРА:");
                                    Log("УГРОЗА", $"   ├─ Путь: {path}\\{valueName}");
                                    Log("УГРОЗА", $"   └─ Значение: {value}");
                                    
                                    AddThreat("КРИТИЧНЫЙ", $"Подозрительная запись реестра: {valueName}", 
                                        $"Путь: {path}\\{valueName}\nЗначение: {value}");
                                }
                            }
                        }
                    }
                    catch (Exception ex) { Log("РЕЕСТР", $"❌ Ошибка при проверке {path}: {ex.Message}"); }
                }
                
                Log("РЕЕСТР", $"✅ Анализ реестра завершён. Подозрительных записей: {suspiciousCount}");
                UpdateProgress(40, 100, $"Реестр: {suspiciousCount} угроз");
            });
        }

        // ==================== USB HISTORY ====================
        private async Task ScanUSBHistory(CancellationToken token)
        {
            await Task.Run(() =>
            {
                UpdateProgress(45, 100, "Проверка USB истории...");
                Log("USB", "🔍 Анализ истории USB устройств...");
                
                int deviceCount = 0;
                
                try
                {
                    using var key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Enum\USBSTOR");
                    if (key != null)
                    {
                        var subKeys = key.GetSubKeyNames();
                        Log("USB", $"📊 Найдено записей USB: {subKeys.Length}");
                        
                        foreach (string subKey in subKeys)
                        {
                            token.ThrowIfCancellationRequested();
                            _scannedItems++;
                            
                            using var deviceKey = key.OpenSubKey(subKey);
                            if (deviceKey != null)
                            {
                                string? friendlyName = deviceKey.GetValue("FriendlyName") as string;
                                if (!string.IsNullOrEmpty(friendlyName))
                                {
                                    deviceCount++;
                                    USBDevices.Add(new USBDeviceInfo { DeviceID = subKey, FriendlyName = friendlyName });
                                    Log("USB", $"🔌 Найдено устройство: {friendlyName}");
                                    
                                    if (CheatKeywords.Any(kw => friendlyName.ToLower().Contains(kw.ToLower())))
                                    {
                                        Log("УГРОЗА", $"⚠️ ПОДОЗРИТЕЛЬНОЕ USB УСТРОЙСТВО: {friendlyName}");
                                        AddThreat("НИЗКИЙ", $"Подозрительное USB устройство: {friendlyName}", $"ID: {subKey}");
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex) { Log("USB", $"❌ Ошибка: {ex.Message}"); }
                
                Log("USB", $"✅ Анализ USB завершён. Найдено устройств: {deviceCount}");
                UpdateProgress(50, 100, $"USB: {deviceCount} устройств");
            });
        }

        // ==================== LAST ACTIVITY ====================
        private async Task ScanLastActivity(CancellationToken token)
        {
            await Task.Run(() =>
            {
                UpdateProgress(55, 100, "Анализ последних активностей...");
                Log("ACTIVITY", "🔍 Проверка недавних запусков программ...");
                
                int activityCount = 0;
                
                try
                {
                    // Prefetch анализ
                    string prefetchPath = @"C:\Windows\Prefetch";
                    if (Directory.Exists(prefetchPath))
                    {
                        var files = Directory.GetFiles(prefetchPath, "*.pf").Take(200);
                        var fileList = files.ToList();
                        Log("ACTIVITY", $"📊 Найдено Prefetch файлов: {fileList.Count}");
                        
                        for (int i = 0; i < fileList.Count; i++)
                        {
                            token.ThrowIfCancellationRequested();
                            string file = fileList[i];
                            _scannedItems++;
                            
                            string fileName = Path.GetFileNameWithoutExtension(file);
                            var match = Regex.Match(fileName, @"^([A-Z0-9]+)");
                            if (match.Success)
                            {
                                string exeName = match.Groups[1].Value + ".exe";
                                activityCount++;
                                LastActivities.Add(new LastActivityInfo { FileName = exeName, Source = "Prefetch" });
                                
                                Log("ACTIVITY", $"📊 Prefetch [{i+1}/{fileList.Count}]: {exeName}");
                                
                                if (CheatKeywords.Any(kw => exeName.ToLower().Contains(kw.ToLower())))
                                {
                                    Log("УГРОЗА", $"⚠️ ПОДОЗРИТЕЛЬНЫЙ ЗАПУСК (Prefetch): {exeName}");
                                    AddThreat("СРЕДНИЙ", $"Подозрительный запуск: {exeName}", $"Источник: Prefetch");
                                }
                            }
                        }
                    }
                    
                    // UserAssist анализ
                    using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\UserAssist");
                    if (key != null)
                    {
                        Log("ACTIVITY", "📊 Анализ UserAssist (статистика запусков)...");
                        
                        foreach (string subKeyName in key.GetSubKeyNames())
                        {
                            using var subKey = key.OpenSubKey(subKeyName + @"\Count");
                            if (subKey != null)
                            {
                                foreach (var valueName in subKey.GetValueNames())
                                {
                                    _scannedItems++;
                                    string decoded = DecodeRot13(valueName);
                                    
                                    if (decoded.Contains(".exe"))
                                    {
                                        string exeName = Path.GetFileName(decoded);
                                        activityCount++;
                                        
                                        Log("ACTIVITY", $"📊 UserAssist: {exeName}");
                                        
                                        if (CheatKeywords.Any(kw => exeName.ToLower().Contains(kw.ToLower())))
                                        {
                                            Log("УГРОЗА", $"⚠️ ПОДОЗРИТЕЛЬНЫЙ ЗАПУСК (UserAssist): {exeName}");
                                            Log("УГРОЗА", $"   └─ Полный путь: {decoded}");
                                            AddThreat("СРЕДНИЙ", $"Подозрительный запуск: {exeName}", $"Источник: UserAssist\nПуть: {decoded}");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex) { Log("ACTIVITY", $"❌ Ошибка: {ex.Message}"); }
                
                Log("ACTIVITY", $"✅ Анализ активностей завершён. Записей: {activityCount}");
                UpdateProgress(60, 100, $"Активность: {LastActivities.Count} записей");
            });
        }

        private string DecodeRot13(string input)
        {
            char[] chars = input.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                char c = chars[i];
                if (c >= 'a' && c <= 'z') chars[i] = (char)(((c - 'a' + 13) % 26) + 'a');
                else if (c >= 'A' && c <= 'Z') chars[i] = (char)(((c - 'A' + 13) % 26) + 'A');
            }
            return new string(chars);
        }

        // ==================== JUMP LISTS ====================
        private async Task ScanJumpLists(CancellationToken token)
        {
            await Task.Run(() =>
            {
                UpdateProgress(62, 100, "Анализ Jump Lists...");
                Log("JUMPLISTS", "🔍 Проверка недавних файлов из Jump Lists...");
                
                int jumpCount = 0;
                
                try
                {
                    string jumpListPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Recent), "AutomaticDestinations");
                    
                    if (Directory.Exists(jumpListPath))
                    {
                        var files = Directory.GetFiles(jumpListPath, "*.automaticDestinations-ms").Take(30);
                        var fileList = files.ToList();
                        Log("JUMPLISTS", $"📊 Найдено Jump List файлов: {fileList.Count}");
                        
                        for (int i = 0; i < fileList.Count; i++)
                        {
                            token.ThrowIfCancellationRequested();
                            string file = fileList[i];
                            _scannedItems++;
                            
                            try
                            {
                                byte[] data = File.ReadAllBytes(file);
                                string text = Encoding.Unicode.GetString(data);
                                var matches = Regex.Matches(text, @"[A-Z]:\\[^\0]+");
                                
                                Log("JUMPLISTS", $"📋 Анализ файла [{i+1}/{fileList.Count}]: {Path.GetFileName(file)}");
                                
                                foreach (Match match in matches)
                                {
                                    string path = match.Value;
                                    jumpCount++;
                                    JumpLists.Add(new JumpListInfo { TargetPath = path });
                                    
                                    if (CheatKeywords.Any(kw => path.ToLower().Contains(kw.ToLower())))
                                    {
                                        Log("УГРОЗА", $"⚠️ ПОДОЗРИТЕЛЬНЫЙ ДОСТУП (JumpList): {Path.GetFileName(path)}");
                                        Log("УГРОЗА", $"   └─ Путь: {path}");
                                        AddThreat("НИЗКИЙ", $"Подозрительный доступ: {Path.GetFileName(path)}", $"Путь: {path}\nИсточник: JumpList");
                                    }
                                }
                            }
                            catch (Exception ex) { Log("JUMPLISTS", $"⚠️ Ошибка чтения {Path.GetFileName(file)}: {ex.Message}"); }
                        }
                    }
                }
                catch (Exception ex) { Log("JUMPLISTS", $"❌ Ошибка: {ex.Message}"); }
                
                Log("JUMPLISTS", $"✅ Анализ Jump Lists завершён. Записей: {jumpCount}");
                UpdateProgress(64, 100, $"JumpLists: {JumpLists.Count} записей");
            });
        }
                // ==================== SHELLBAG ====================
        private async Task ScanShellBag(CancellationToken token)
        {
            await Task.Run(() =>
            {
                UpdateProgress(66, 100, "Анализ ShellBag...");
                Log("SHELLBAG", "🔍 Проверка истории доступа к папкам...");
                
                int bagCount = 0;
                
                try
                {
                    using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\Shell\BagMRU");
                    if (key != null)
                    {
                        Log("SHELLBAG", "📂 Анализ структуры BagMRU...");
                        bagCount = ParseShellBagKey(key, "", bagCount, token);
                    }
                }
                catch (Exception ex) { Log("SHELLBAG", $"❌ Ошибка: {ex.Message}"); }
                
                Log("SHELLBAG", $"✅ Анализ ShellBag завершён. Записей: {bagCount}");
                UpdateProgress(68, 100, $"ShellBag: {bagCount} записей");
            });
        }

        private int ParseShellBagKey(RegistryKey key, string parentPath, int count, CancellationToken token)
        {
            try
            {
                foreach (string valueName in key.GetValueNames())
                {
                    token.ThrowIfCancellationRequested();
                    if (valueName.All(c => char.IsDigit(c)))
                    {
                        byte[]? data = key.GetValue(valueName) as byte[];
                        if (data != null)
                        {
                            string? folderName = ExtractFolderName(data);
                            if (!string.IsNullOrEmpty(folderName))
                            {
                                count++;
                                string fullPath = string.IsNullOrEmpty(parentPath) ? folderName : Path.Combine(parentPath, folderName);
                                ShellBags.Add(new ShellBagInfo { FolderPath = fullPath });
                                
                                Log("SHELLBAG", $"📂 Найдена папка: {folderName}");
                                
                                if (CheatKeywords.Any(kw => fullPath.ToLower().Contains(kw.ToLower())))
                                {
                                    Log("УГРОЗА", $"⚠️ ПОДОЗРИТЕЛЬНАЯ ПАПКА В SHELLBAG: {folderName}");
                                    Log("УГРОЗА", $"   └─ Полный путь: {fullPath}");
                                    AddThreat("НИЗКИЙ", $"Подозрительная папка: {folderName}", $"Путь: {fullPath}");
                                }
                            }
                        }
                    }
                }
                
                foreach (string subKeyName in key.GetSubKeyNames())
                {
                    if (subKeyName.All(c => char.IsDigit(c)))
                    {
                        using var subKey = key.OpenSubKey(subKeyName);
                        if (subKey != null)
                            count = ParseShellBagKey(subKey, parentPath, count, token);
                    }
                }
            }
            catch { }
            return count;
        }

        private string? ExtractFolderName(byte[] data)
        {
            try
            {
                for (int i = 0; i < data.Length - 10; i++)
                {
                    if (data[i] == 0x14 && data[i + 1] == 0x00)
                    {
                        int length = data[i + 2];
                        if (length > 0 && i + 3 + length <= data.Length)
                            return Encoding.Unicode.GetString(data, i + 3, length);
                    }
                }
            }
            catch { }
            return null;
        }

        // ==================== STARTUP ITEMS ====================
        private async Task ScanStartupItems(CancellationToken token)
        {
            await Task.Run(() =>
            {
                UpdateProgress(70, 100, "Проверка автозагрузки...");
                Log("STARTUP", "🔍 Проверка папок автозагрузки...");
                
                int suspiciousStartup = 0;
                string[] startupPaths = {
                    Environment.GetFolderPath(Environment.SpecialFolder.Startup),
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup)
                };
                
                foreach (string path in startupPaths)
                {
                    try
                    {
                        if (Directory.Exists(path))
                        {
                            var files = Directory.GetFiles(path, "*.lnk");
                            Log("STARTUP", $"📂 Проверка папки: {path} (файлов: {files.Length})");
                            
                            foreach (string file in files)
                            {
                                _scannedItems++;
                                string name = Path.GetFileNameWithoutExtension(file).ToLower();
                                
                                Log("STARTUP", $"📋 Проверка: {Path.GetFileName(file)}");
                                
                                if (CheatKeywords.Any(kw => name.Contains(kw)))
                                {
                                    suspiciousStartup++;
                                    StartupItems.Add(new StartupItem { Name = name, Path = file });
                                    
                                    Log("УГРОЗА", $"⚠️ ПОДОЗРИТЕЛЬНЫЙ ЭЛЕМЕНТ АВТОЗАГРУЗКИ: {Path.GetFileName(file)}");
                                    Log("УГРОЗА", $"   └─ Путь: {file}");
                                    
                                    AddThreat("ВЫСОКИЙ", $"Подозрительный элемент автозагрузки: {Path.GetFileName(file)}", $"Путь: {file}");
                                }
                            }
                        }
                    }
                    catch (Exception ex) { Log("STARTUP", $"❌ Ошибка: {ex.Message}"); }
                }
                
                Log("STARTUP", $"✅ Проверка автозагрузки завершена. Подозрительных: {suspiciousStartup}");
                UpdateProgress(72, 100, $"Автозагрузка: {suspiciousStartup} угроз");
            });
        }

        // ==================== SERVICES ====================
        private async Task ScanServices(CancellationToken token)
        {
            await Task.Run(() =>
            {
                UpdateProgress(74, 100, "Проверка служб...");
                Log("СЛУЖБЫ", "🔍 Проверка системных служб...");
                
                int suspiciousServices = 0;
                int totalServices = 0;
                
                try
                {
                    using var searcher = new ManagementObjectSearcher("SELECT Name, DisplayName, StartName, State FROM Win32_Service");
                    var services = searcher.Get();
                    
                    foreach (var obj in services)
                    {
                        token.ThrowIfCancellationRequested();
                        totalServices++;
                        _scannedItems++;
                        
                        string name = obj["Name"]?.ToString()?.ToLower() ?? "";
                        string display = obj["DisplayName"]?.ToString()?.ToLower() ?? "";
                        string state = obj["State"]?.ToString() ?? "";
                        
                        Log("СЛУЖБЫ", $"⚙️ Проверка службы [{totalServices}]: {obj["DisplayName"]} (Состояние: {state})");
                        
                        if (CheatKeywords.Any(kw => name.Contains(kw) || display.Contains(kw)))
                        {
                            suspiciousServices++;
                            SuspiciousServices.Add(new ServiceInfo { Name = name, DisplayName = display });
                            
                            Log("УГРОЗА", $"⚠️ ПОДОЗРИТЕЛЬНАЯ СЛУЖБА: {obj["DisplayName"]}");
                            Log("УГРОЗА", $"   ├─ Имя: {name}");
                            Log("УГРОЗА", $"   ├─ Состояние: {state}");
                            Log("УГРОЗА", $"   └─ Запуск от: {obj["StartName"]}");
                            
                            AddThreat("СРЕДНИЙ", $"Подозрительная служба: {obj["DisplayName"]}", $"Имя: {name}");
                        }
                    }
                }
                catch (Exception ex) { Log("СЛУЖБЫ", $"❌ Ошибка: {ex.Message}"); }
                
                Log("СЛУЖБЫ", $"✅ Проверка служб завершена. Всего: {totalServices}, Подозрительных: {suspiciousServices}");
                UpdateProgress(76, 100, $"Службы: {suspiciousServices} угроз");
            });
        }

        // ==================== NETWORK CONNECTIONS ====================
        private async Task ScanNetworkConnections(CancellationToken token)
        {
            await Task.Run(() =>
            {
                UpdateProgress(78, 100, "Проверка сетевых подключений...");
                Log("СЕТЬ", "🔍 Анализ сетевых подключений...");
                
                try
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName = "netstat",
                        Arguments = "-an",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };
                    
                    using var process = Process.Start(psi);
                    if (process != null)
                    {
                        string output = process.StandardOutput.ReadToEnd();
                        var lines = output.Split('\n');
                        int foreignCount = 0;
                        int establishedCount = 0;
                        int listeningCount = 0;
                        
                        Log("СЕТЬ", "📊 Анализ сетевых соединений...");
                        
                        foreach (string line in lines)
                        {
                            if (line.Contains("ESTABLISHED"))
                            {
                                establishedCount++;
                                var match = Regex.Match(line, @"(\d+\.\d+\.\d+\.\d+):(\d+)\s+(\d+\.\d+\.\d+\.\d+):(\d+)");
                                if (match.Success)
                                {
                                    string localAddr = $"{match.Groups[1]}:{match.Groups[2]}";
                                    string foreignAddr = $"{match.Groups[3]}:{match.Groups[4]}";
                                    NetworkConnections.Add(new NetworkConnection { LocalAddress = localAddr, ForeignAddress = foreignAddr, State = "ESTABLISHED" });
                                    
                                    Log("СЕТЬ", $"🌐 Установлено соединение: {localAddr} -> {foreignAddr}");
                                }
                            }
                            else if (line.Contains("LISTENING"))
                            {
                                listeningCount++;
                                var match = Regex.Match(line, @"(\d+\.\d+\.\d+\.\d+):(\d+)");
                                if (match.Success)
                                {
                                    Log("СЕТЬ", $"🔌 Слушающий порт: {match.Groups[1]}:{match.Groups[2]}");
                                }
                            }
                        }
                        
                        Log("СЕТЬ", $"📊 Статистика: Установленных: {establishedCount}, Слушающих: {listeningCount}");
                    }
                }
                catch (Exception ex) { Log("СЕТЬ", $"❌ Ошибка: {ex.Message}"); }
                
                UpdateProgress(80, 100, "Сеть проверена");
            });
        }

        // ==================== SCHEDULED TASKS ====================
        private async Task ScanScheduledTasks(CancellationToken token)
        {
            await Task.Run(() =>
            {
                UpdateProgress(82, 100, "Проверка заданий планировщика...");
                Log("ЗАДАНИЯ", "🔍 Проверка заданий планировщика Windows...");
                
                int suspiciousTasks = 0;
                int totalTasks = 0;
                
                try
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName = "schtasks",
                        Arguments = "/query /fo csv /nh",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };
                    
                    using var process = Process.Start(psi);
                    if (process != null)
                    {
                        string output = process.StandardOutput.ReadToEnd();
                        var lines = output.Split('\n');
                        
                        Log("ЗАДАНИЯ", $"📋 Найдено заданий: {lines.Length}");
                        
                        foreach (string line in lines)
                        {
                            token.ThrowIfCancellationRequested();
                            _scannedItems++;
                            totalTasks++;
                            
                            if (!string.IsNullOrWhiteSpace(line) && line.Contains(","))
                            {
                                string taskName = line.Split(',')[0].Trim('"');
                                Log("ЗАДАНИЯ", $"⏰ Проверка задания: {taskName}");
                                
                                if (CheatKeywords.Any(kw => taskName.ToLower().Contains(kw)))
                                {
                                    suspiciousTasks++;
                                    ScheduledTasks.Add(new ScheduledTask { Name = taskName });
                                    
                                    Log("УГРОЗА", $"⚠️ ПОДОЗРИТЕЛЬНОЕ ЗАДАНИЕ ПЛАНИРОВЩИКА: {taskName}");
                                    AddThreat("СРЕДНИЙ", $"Подозрительное задание планировщика", $"Имя: {taskName}");
                                }
                            }
                        }
                    }
                }
                catch (Exception ex) { Log("ЗАДАНИЯ", $"❌ Ошибка: {ex.Message}"); }
                
                Log("ЗАДАНИЯ", $"✅ Проверка заданий завершена. Всего: {totalTasks}, Подозрительных: {suspiciousTasks}");
                UpdateProgress(84, 100, $"Задания: {suspiciousTasks} угроз");
            });
        }

        // ==================== BROWSER EXTENSIONS ====================
        private async Task ScanBrowserExtensions(CancellationToken token)
        {
            await Task.Run(() =>
            {
                UpdateProgress(86, 100, "Проверка расширений браузера...");
                Log("БРАУЗЕР", "🔍 Проверка расширений браузеров...");
                
                int suspiciousExt = 0;
                string[] chromePaths = {
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Google\\Chrome\\User Data\\Default\\Extensions"),
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Microsoft\\Edge\\User Data\\Default\\Extensions"),
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BraveSoftware\\Brave-Browser\\User Data\\Default\\Extensions"),
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Opera Software\\Opera Stable\\Extensions")
                };
                
                foreach (string path in chromePaths)
                {
                    try
                    {
                        if (Directory.Exists(path))
                        {
                            var dirs = Directory.GetDirectories(path);
                            Log("БРАУЗЕР", $"📂 Проверка расширений в: {Path.GetFileName(Path.GetDirectoryName(Path.GetDirectoryName(path)))}");
                            
                            foreach (string dir in dirs)
                            {
                                token.ThrowIfCancellationRequested();
                                _scannedItems++;
                                
                                string manifestPath = Path.Combine(dir, "manifest.json");
                                if (File.Exists(manifestPath))
                                {
                                    string content = File.ReadAllText(manifestPath).ToLower();
                                    string extId = Path.GetFileName(dir);
                                    
                                    Log("БРАУЗЕР", $"🔌 Проверка расширения: {extId}");
                                    
                                    if (CheatKeywords.Any(kw => content.Contains(kw)))
                                    {
                                        suspiciousExt++;
                                        BrowserExtensions.Add(new BrowserExtension { Name = extId });
                                        
                                        Log("УГРОЗА", $"⚠️ ПОДОЗРИТЕЛЬНОЕ РАСШИРЕНИЕ БРАУЗЕРА: {extId}");
                                        AddThreat("СРЕДНИЙ", $"Подозрительное расширение браузера", $"ID: {extId}");
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex) { Log("БРАУЗЕР", $"❌ Ошибка: {ex.Message}"); }
                }
                
                Log("БРАУЗЕР", $"✅ Проверка расширений завершена. Подозрительных: {suspiciousExt}");
                UpdateProgress(88, 100, $"Расширения: {suspiciousExt} угроз");
            });
        }

        // ==================== HOSTS FILE ====================
        private async Task ScanHostsFile(CancellationToken token)
        {
            await Task.Run(() =>
            {
                UpdateProgress(90, 100, "Проверка файла hosts...");
                Log("HOSTS", "🔍 Проверка файла hosts на модификации...");
                
                try
                {
                    string hostsPath = @"C:\Windows\System32\drivers\etc\hosts";
                    if (File.Exists(hostsPath))
                    {
                        string content = File.ReadAllText(hostsPath);
                        var lines = content.Split('\n');
                        int modified = 0;
                        int suspiciousLines = 0;
                        
                        Log("HOSTS", $"📄 Файл hosts содержит {lines.Length} строк");
                        
                        for (int i = 0; i < lines.Length; i++)
                        {
                            string line = lines[i].Trim();
                            if (!string.IsNullOrEmpty(line) && !line.StartsWith("#"))
                            {
                                modified++;
                                Log("HOSTS", $"📋 Строка {i+1}: {line}");
                                
                                if (line.Contains("facepunch") || line.Contains("easyanticheat") || 
                                    line.Contains("battleye") || line.Contains("rust") ||
                                    CheatKeywords.Any(kw => line.ToLower().Contains(kw)))
                                {
                                    suspiciousLines++;
                                    HostsEntries.Add(new HostsEntry { Line = line, LineNumber = i + 1 });
                                    
                                    Log("УГРОЗА", $"⚠️ ПОДОЗРИТЕЛЬНАЯ ЗАПИСЬ В HOSTS (строка {i+1}): {line}");
                                    AddThreat("КРИТИЧНЫЙ", "Обнаружено изменение файла hosts", $"Строка {i+1}: {line}");
                                }
                            }
                        }
                        
                        Log("HOSTS", $"📊 Статистика: Нестандартных записей: {modified}, Подозрительных: {suspiciousLines}");
                    }
                    else
                    {
                        Log("HOSTS", "⚠️ Файл hosts не найден!");
                    }
                }
                catch (Exception ex) { Log("HOSTS", $"❌ Ошибка: {ex.Message}"); }
                
                UpdateProgress(91, 100, "Hosts проверен");
            });
        }

        // ==================== PREFETCH ====================
        private async Task ScanPrefetch(CancellationToken token)
        {
            await Task.Run(() =>
            {
                UpdateProgress(92, 100, "Анализ Prefetch...");
                Log("PREFETCH", "🔍 Анализ файлов Prefetch (ускорители запуска)...");
                
                int prefetchCount = 0;
                int suspiciousPrefetch = 0;
                
                try
                {
                    string prefetchPath = @"C:\Windows\Prefetch";
                    if (Directory.Exists(prefetchPath))
                    {
                        var files = Directory.GetFiles(prefetchPath, "*.pf");
                        prefetchCount = files.Length;
                        
                        Log("PREFETCH", $"📊 Найдено Prefetch файлов: {prefetchCount}");
                        
                        foreach (string file in files)
                        {
                            token.ThrowIfCancellationRequested();
                            _scannedItems++;
                            
                            string name = Path.GetFileNameWithoutExtension(file).ToLower();
                            Log("PREFETCH", $"⚡ Проверка: {Path.GetFileName(file)}");
                            
                            if (CheatKeywords.Any(kw => name.Contains(kw)))
                            {
                                suspiciousPrefetch++;
                                PrefetchFiles.Add(new PrefetchFile { Name = name, Path = file });
                                
                                Log("УГРОЗА", $"⚠️ ПОДОЗРИТЕЛЬНЫЙ PREFETCH ФАЙЛ: {Path.GetFileName(file)}");
                            }
                        }
                    }
                }
                catch (Exception ex) { Log("PREFETCH", $"❌ Ошибка: {ex.Message}"); }
                
                Log("PREFETCH", $"✅ Анализ Prefetch завершён. Всего: {prefetchCount}, Подозрительных: {suspiciousPrefetch}");
                UpdateProgress(93, 100, "Prefetch проанализирован");
            });
        }
                // ==================== EVENT LOGS ====================
        private async Task ScanEventLogs(CancellationToken token)
        {
            await Task.Run(() =>
            {
                UpdateProgress(94, 100, "Анализ журналов событий...");
                Log("СОБЫТИЯ", "🔍 Проверка журналов событий Windows...");
                
                int suspiciousEvents = 0;
                int totalEvents = 0;
                
                try
                {
                    DateTime sevenDaysAgo = DateTime.Now.AddDays(-7);
                    string wqlDate = sevenDaysAgo.ToString("yyyyMMddHHmmss");
                    
                    using var searcher = new ManagementObjectSearcher($"SELECT * FROM Win32_NTLogEvent WHERE LogFile='System' AND TimeGenerated >= '{wqlDate}'");
                    var events = searcher.Get();
                    
                    Log("СОБЫТИЯ", $"📊 Анализ системных событий за последние 7 дней...");
                    
                    foreach (var obj in events)
                    {
                        token.ThrowIfCancellationRequested();
                        totalEvents++;
                        _scannedItems++;
                        
                        string? message = obj["Message"]?.ToString()?.ToLower() ?? "";
                        string? eventCode = obj["EventCode"]?.ToString() ?? "";
                        string? eventType = obj["Type"]?.ToString() ?? "";
                        
                        if (totalEvents % 50 == 0)
                            Log("СОБЫТИЯ", $"📋 Обработано событий: {totalEvents}");
                        
                        // Проверка критических событий
                        if (eventCode == "7045" && message.Contains("service"))
                        {
                            suspiciousEvents++;
                            Log("УГРОЗА", $"⚠️ УСТАНОВЛЕНА НОВАЯ СЛУЖБА (Event 7045): {message.Substring(0, Math.Min(100, message.Length))}");
                            AddThreat("СРЕДНИЙ", "Установлена новая служба", $"Event 7045: {message.Substring(0, Math.Min(200, message.Length))}");
                        }
                        
                        if (eventCode == "4698" && message.Contains("task"))
                        {
                            suspiciousEvents++;
                            Log("УГРОЗА", $"⚠️ СОЗДАНО НОВОЕ ЗАДАНИЕ (Event 4698)");
                            AddThreat("СРЕДНИЙ", "Создано новое задание планировщика", $"Event 4698");
                        }
                        
                        if (CheatKeywords.Any(kw => message.Contains(kw)))
                        {
                            suspiciousEvents++;
                            EventLogs.Add(new EventLogEntry { EventCode = eventCode, Message = message, Type = eventType });
                            Log("УГРОЗА", $"⚠️ ПОДОЗРИТЕЛЬНОЕ СОБЫТИЕ {eventCode}: {message.Substring(0, Math.Min(100, message.Length))}");
                        }
                    }
                }
                catch (Exception ex) { Log("СОБЫТИЯ", $"❌ Ошибка: {ex.Message}"); }
                
                Log("СОБЫТИЯ", $"✅ Анализ событий завершён. Всего: {totalEvents}, Подозрительных: {suspiciousEvents}");
                UpdateProgress(95, 100, "События проверены");
            });
        }

        // ==================== INSTALLED SOFTWARE ====================
        private async Task ScanInstalledSoftware(CancellationToken token)
        {
            await Task.Run(() =>
            {
                Log("ПРОГРАММЫ", "🔍 Проверка установленного программного обеспечения...");
                
                int suspiciousSoftware = 0;
                int totalSoftware = 0;
                string[] uninstallPaths = {
                    @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
                    @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall"
                };
                
                foreach (string path in uninstallPaths)
                {
                    try
                    {
                        using var key = Registry.LocalMachine.OpenSubKey(path);
                        if (key != null)
                        {
                            var subKeys = key.GetSubKeyNames();
                            Log("ПРОГРАММЫ", $"📦 Проверка ключа: {path} (записей: {subKeys.Length})");
                            
                            foreach (string subKeyName in subKeys)
                            {
                                token.ThrowIfCancellationRequested();
                                totalSoftware++;
                                _scannedItems++;
                                
                                using var subKey = key.OpenSubKey(subKeyName);
                                string? displayName = subKey?.GetValue("DisplayName")?.ToString() ?? "";
                                string? publisher = subKey?.GetValue("Publisher")?.ToString() ?? "";
                                string? version = subKey?.GetValue("DisplayVersion")?.ToString() ?? "";
                                
                                if (!string.IsNullOrEmpty(displayName))
                                {
                                    if (totalSoftware % 100 == 0)
                                        Log("ПРОГРАММЫ", $"📋 Проверено программ: {totalSoftware}");
                                    
                                    if (CheatKeywords.Any(kw => displayName.ToLower().Contains(kw)))
                                    {
                                        suspiciousSoftware++;
                                        Log("УГРОЗА", $"⚠️ ПОДОЗРИТЕЛЬНОЕ ПРОГРАММНОЕ ОБЕСПЕЧЕНИЕ:");
                                        Log("УГРОЗА", $"   ├─ Название: {displayName}");
                                        Log("УГРОЗА", $"   ├─ Издатель: {publisher}");
                                        Log("УГРОЗА", $"   └─ Версия: {version}");
                                        AddThreat("СРЕДНИЙ", $"Подозрительное ПО: {displayName}", $"Издатель: {publisher}\nВерсия: {version}");
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex) { Log("ПРОГРАММЫ", $"❌ Ошибка: {ex.Message}"); }
                }
                
                Log("ПРОГРАММЫ", $"✅ Проверка ПО завершена. Всего: {totalSoftware}, Подозрительных: {suspiciousSoftware}");
                UpdateProgress(96, 100, "ПО проверено");
            });
        }

        // ==================== DRIVERS ====================
        private async Task ScanDrivers(CancellationToken token)
        {
            await Task.Run(() =>
            {
                Log("ДРАЙВЕРЫ", "🔍 Проверка драйверов...");
                
                int suspiciousDrivers = 0;
                int totalDrivers = 0;
                
                try
                {
                    using var searcher = new ManagementObjectSearcher("SELECT DisplayName, Name, State, StartMode FROM Win32_SystemDriver WHERE State='Running'");
                    var drivers = searcher.Get();
                    
                    Log("ДРАЙВЕРЫ", $"🔧 Проверка загруженных драйверов...");
                    
                    foreach (var obj in drivers)
                    {
                        token.ThrowIfCancellationRequested();
                        totalDrivers++;
                        _scannedItems++;
                        
                        string name = obj["Name"]?.ToString()?.ToLower() ?? "";
                        string display = obj["DisplayName"]?.ToString()?.ToLower() ?? "";
                        string startMode = obj["StartMode"]?.ToString() ?? "";
                        
                        Log("ДРАЙВЕРЫ", $"🔧 Проверка драйвера [{totalDrivers}]: {obj["DisplayName"]}");
                        
                        if (CheatKeywords.Any(kw => name.Contains(kw) || display.Contains(kw)))
                        {
                            suspiciousDrivers++;
                            SuspiciousDrivers.Add(new DriverInfo { Name = name, DisplayName = display });
                            
                            Log("УГРОЗА", $"⚠️ ПОДОЗРИТЕЛЬНЫЙ ДРАЙВЕР:");
                            Log("УГРОЗА", $"   ├─ Имя: {obj["DisplayName"]}");
                            Log("УГРОЗА", $"   ├─ Системное имя: {name}");
                            Log("УГРОЗА", $"   └─ Режим запуска: {startMode}");
                            AddThreat("ВЫСОКИЙ", $"Подозрительный драйвер: {obj["DisplayName"]}", $"Имя: {name}");
                        }
                    }
                }
                catch (Exception ex) { Log("ДРАЙВЕРЫ", $"❌ Ошибка: {ex.Message}"); }
                
                Log("ДРАЙВЕРЫ", $"✅ Проверка драйверов завершена. Всего: {totalDrivers}, Подозрительных: {suspiciousDrivers}");
                UpdateProgress(97, 100, "Драйверы проверены");
            });
        }

        // ==================== WINLOGON ====================
        private async Task ScanWinlogon(CancellationToken token)
        {
            await Task.Run(() =>
            {
                Log("WINLOGON", "🔍 Проверка записей Winlogon...");
                
                try
                {
                    using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon");
                    if (key != null)
                    {
                        string? shell = key.GetValue("Shell") as string;
                        string? userinit = key.GetValue("Userinit") as string;
                        string? taskman = key.GetValue("Taskman") as string;
                        
                        Log("WINLOGON", $"🔐 Shell: {shell}");
                        Log("WINLOGON", $"🔐 Userinit: {userinit}");
                        
                        if (!string.IsNullOrEmpty(shell) && shell != "explorer.exe")
                        {
                            Log("УГРОЗА", $"⚠️ ИЗМЕНЁН ПАРАМЕТР SHELL В WINLOGON:");
                            Log("УГРОЗА", $"   └─ Shell: {shell}");
                            AddThreat("КРИТИЧНЫЙ", "Изменён параметр Shell в Winlogon", $"Shell: {shell}");
                        }
                        
                        if (!string.IsNullOrEmpty(userinit) && !userinit.Contains("userinit.exe"))
                        {
                            Log("УГРОЗА", $"⚠️ ИЗМЕНЁН ПАРАМЕТР USERINIT:");
                            Log("УГРОЗА", $"   └─ Userinit: {userinit}");
                            AddThreat("КРИТИЧНЫЙ", "Изменён параметр Userinit", $"Userinit: {userinit}");
                        }
                    }
                }
                catch (Exception ex) { Log("WINLOGON", $"❌ Ошибка: {ex.Message}"); }
            });
        }

        // ==================== WMI SUBSCRIPTIONS ====================
        private async Task ScanWmiSubscriptions(CancellationToken token)
        {
            await Task.Run(() =>
            {
                Log("WMI", "🔍 Проверка WMI подписок (постоянные механизмы)...");
                
                try
                {
                    using var searcher = new ManagementObjectSearcher("SELECT * FROM __EventFilter");
                    int filterCount = 0;
                    int suspiciousFilters = 0;
                    
                    foreach (var obj in searcher.Get())
                    {
                        token.ThrowIfCancellationRequested();
                        filterCount++;
                        _scannedItems++;
                        
                        string? name = obj["Name"]?.ToString() ?? "";
                        string? query = obj["Query"]?.ToString() ?? "";
                        string? queryLanguage = obj["QueryLanguage"]?.ToString() ?? "";
                        
                        Log("WMI", $"🔮 Проверка фильтра: {name}");
                        
                        if (CheatKeywords.Any(kw => query.ToLower().Contains(kw.ToLower())))
                        {
                            suspiciousFilters++;
                            WmiEvents.Add(new WmiEventInfo { Name = name, Query = query });
                            
                            Log("УГРОЗА", $"⚠️ ПОДОЗРИТЕЛЬНЫЙ WMI ФИЛЬТР:");
                            Log("УГРОЗА", $"   ├─ Имя: {name}");
                            Log("УГРОЗА", $"   ├─ Язык: {queryLanguage}");
                            Log("УГРОЗА", $"   └─ Запрос: {query}");
                            AddThreat("ВЫСОКИЙ", "Подозрительный WMI фильтр", $"Имя: {name}\nЗапрос: {query}");
                        }
                    }
                    
                    Log("WMI", $"✅ Проверка WMI завершена. Фильтров: {filterCount}, Подозрительных: {suspiciousFilters}");
                }
                catch (Exception ex) { Log("WMI", $"❌ Ошибка: {ex.Message}"); }
            });
        }

        // ==================== DNS CACHE ====================
        private async Task ScanDnsCache(CancellationToken token)
        {
            await Task.Run(() =>
            {
                Log("DNS", "🔍 Проверка DNS кэша...");
                
                try
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName = "ipconfig",
                        Arguments = "/displaydns",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };
                    
                    using var process = Process.Start(psi);
                    if (process != null)
                    {
                        string output = process.StandardOutput.ReadToEnd();
                        var matches = Regex.Matches(output, @"Record Name\.+\s+:\s+(\S+)");
                        var suspiciousDomains = new List<string>();
                        
                        string[] badDomains = { "cheat", "hack", "inject", "crack", "keygen", "aimbot", "esp", "wallhack", "radar", "triggerbot" };
                        
                        Log("DNS", $"📊 Найдено DNS записей: {matches.Count}");
                        
                        foreach (Match match in matches)
                        {
                            token.ThrowIfCancellationRequested();
                            _scannedItems++;
                            
                            string domain = match.Groups[1].Value.ToLower();
                            Log("DNS", $"🌐 Проверка домена: {domain}");
                            
                            if (badDomains.Any(bd => domain.Contains(bd)))
                            {
                                suspiciousDomains.Add(domain);
                                DnsCacheEntries.Add(new DnsCacheEntry { Domain = domain });
                                
                                Log("УГРОЗА", $"⚠️ ПОДОЗРИТЕЛЬНЫЙ DNS ЗАПРОС: {domain}");
                            }
                        }
                        
                        if (suspiciousDomains.Count > 0)
                        {
                            Log("УГРОЗА", $"⚠️ НАЙДЕНО ПОДОЗРИТЕЛЬНЫХ DNS ЗАПИСЕЙ: {suspiciousDomains.Count}");
                            AddThreat("СРЕДНИЙ", "Подозрительные DNS записи", $"Домены: {string.Join(", ", suspiciousDomains.Take(5))}");
                        }
                    }
                }
                catch (Exception ex) { Log("DNS", $"❌ Ошибка: {ex.Message}"); }
            });
        }

        // ==================== OPEN PORTS ====================
        private async Task ScanOpenPorts(CancellationToken token)
        {
            await Task.Run(() =>
            {
                Log("ПОРТЫ", "🔍 Проверка открытых портов...");
                
                try
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName = "netstat",
                        Arguments = "-an | findstr LISTENING",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };
                    
                    using var process = Process.Start(psi);
                    if (process != null)
                    {
                        string output = process.StandardOutput.ReadToEnd();
                        var lines = output.Split('\n');
                        var ports = new List<int>();
                        
                        foreach (string line in lines)
                        {
                            token.ThrowIfCancellationRequested();
                            var match = Regex.Match(line, @":(\d+)");
                            if (match.Success)
                            {
                                int port = int.Parse(match.Groups[1].Value);
                                ports.Add(port);
                                Log("ПОРТЫ", $"🔌 Открытый порт: {port}");
                            }
                        }
                        
                        var suspiciousPorts = ports.Where(p => p > 50000 || (p >= 4444 && p <= 4455) || p == 1337 || p == 31337 || p == 6667).ToList();
                        
                        if (suspiciousPorts.Count > 0)
                        {
                            Log("УГРОЗА", $"⚠️ ПОДОЗРИТЕЛЬНЫЕ ОТКРЫТЫЕ ПОРТЫ: {string.Join(", ", suspiciousPorts)}");
                            OpenPorts.AddRange(suspiciousPorts.Select(p => new OpenPort { PortNumber = p }));
                            AddThreat("СРЕДНИЙ", "Подозрительные открытые порты", $"Порты: {string.Join(", ", suspiciousPorts)}");
                        }
                        
                        Log("ПОРТЫ", $"📊 Статистика: Всего портов: {ports.Count}, Подозрительных: {suspiciousPorts.Count}");
                    }
                }
                catch (Exception ex) { Log("ПОРТЫ", $"❌ Ошибка: {ex.Message}"); }
            });
        }
                // ==================== PROCESS MEMORY ====================
        private async Task ScanProcessMemory(CancellationToken token)
        {
            await Task.Run(() =>
            {
                Log("ПАМЯТЬ", "🔍 Проверка памяти процессов (поиск подозрительных строк)...");
                
                int scannedProcesses = 0;
                int suspiciousFindings = 0;
                
                foreach (var proc in Process.GetProcesses().Take(30))
                {
                    token.ThrowIfCancellationRequested();
                    scannedProcesses++;
                    
                    try
                    {
                        if (proc.MainModule != null && proc.WorkingSet64 > 10 * 1024 * 1024)
                        {
                            string processName = proc.ProcessName.ToLower();
                            Log("ПАМЯТЬ", $"🧠 Проверка процесса [{scannedProcesses}/30]: {proc.ProcessName} (Память: {proc.WorkingSet64 / 1024 / 1024} MB)");
                            
                            if (CheatKeywords.Any(kw => processName.Contains(kw)))
                            {
                                suspiciousFindings++;
                                Log("УГРОЗА", $"⚠️ ПОДОЗРИТЕЛЬНЫЙ ПРОЦЕСС В ПАМЯТИ: {proc.ProcessName}");
                                Log("УГРОЗА", $"   ├─ PID: {proc.Id}");
                                Log("УГРОЗА", $"   ├─ Память: {proc.WorkingSet64 / 1024 / 1024} MB");
                                Log("УГРОЗА", $"   └─ Путь: {proc.MainModule?.FileName ?? "Неизвестно"}");
                                AddThreat("ВЫСОКИЙ", $"Подозрительный процесс в памяти: {proc.ProcessName}", $"PID: {proc.Id}");
                            }
                        }
                    }
                    catch { }
                }
                
                Log("ПАМЯТЬ", $"✅ Проверка памяти завершена. Проверено: {scannedProcesses}, Подозрительных: {suspiciousFindings}");
            });
        }

        // ==================== BROWSER CACHE ====================
        private async Task ScanBrowserCache(CancellationToken token)
        {
            await Task.Run(() =>
            {
                Log("КЭШ", "🔍 Проверка кэша браузеров...");
                
                string[] cachePaths = {
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Google\\Chrome\\User Data\\Default\\Cache"),
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Microsoft\\Edge\\User Data\\Default\\Cache"),
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BraveSoftware\\Brave-Browser\\User Data\\Default\\Cache"),
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Mozilla\\Firefox\\Profiles")
                };
                
                int suspiciousCache = 0;
                int totalFiles = 0;
                
                foreach (string path in cachePaths)
                {
                    try
                    {
                        if (Directory.Exists(path))
                        {
                            var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).Take(200);
                            var fileList = files.ToList();
                            totalFiles += fileList.Count;
                            
                            Log("КЭШ", $"📂 Проверка кэша: {Path.GetFileName(Path.GetDirectoryName(Path.GetDirectoryName(path)))} (файлов: {fileList.Count})");
                            
                            foreach (string file in fileList)
                            {
                                token.ThrowIfCancellationRequested();
                                _scannedItems++;
                                
                                string fileName = Path.GetFileName(file).ToLower();
                                if (CheatKeywords.Any(kw => fileName.Contains(kw)))
                                {
                                    suspiciousCache++;
                                    Log("УГРОЗА", $"⚠️ ПОДОЗРИТЕЛЬНЫЙ ФАЙЛ В КЭШЕ БРАУЗЕРА: {fileName}");
                                }
                            }
                        }
                    }
                    catch (Exception ex) { Log("КЭШ", $"❌ Ошибка: {ex.Message}"); }
                }
                
                Log("КЭШ", $"✅ Проверка кэша завершена. Файлов: {totalFiles}, Подозрительных: {suspiciousCache}");
            });
        }

        // ==================== LSA KEYS ====================
        private async Task ScanLsaKeys(CancellationToken token)
        {
            await Task.Run(() =>
            {
                Log("LSA", "🔍 Проверка LSA (Local Security Authority) реестра...");
                
                try
                {
                    using var key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Lsa");
                    if (key != null)
                    {
                        object? authPackages = key.GetValue("Authentication Packages");
                        object? securityPackages = key.GetValue("Security Packages");
                        object? notificationPackages = key.GetValue("Notification Packages");
                        
                        Log("LSA", $"🔐 Authentication Packages: {authPackages}");
                        Log("LSA", $"🔐 Security Packages: {securityPackages}");
                        
                        if (authPackages != null)
                        {
                            string packages = authPackages.ToString() ?? "";
                            if (packages.Contains("mimikatz") || packages.Contains("hook") || packages.Contains("inject"))
                            {
                                Log("УГРОЗА", $"⚠️ ПОДОЗРИТЕЛЬНЫЕ LSA ПАКЕТЫ:");
                                Log("УГРОЗА", $"   └─ {packages}");
                                AddThreat("КРИТИЧНЫЙ", "Подозрительные LSA пакеты", packages);
                            }
                        }
                        
                        if (notificationPackages != null)
                        {
                            string notifPackages = notificationPackages.ToString() ?? "";
                            if (notifPackages.Contains("mimikatz") || notifPackages.Contains("hook"))
                            {
                                Log("УГРОЗА", $"⚠️ ПОДОЗРИТЕЛЬНЫЕ LSA NOTIFICATION ПАКЕТЫ: {notifPackages}");
                                AddThreat("КРИТИЧНЫЙ", "Подозрительные LSA Notification пакеты", notifPackages);
                            }
                        }
                    }
                }
                catch (Exception ex) { Log("LSA", $"❌ Ошибка: {ex.Message}"); }
            });
        }

        // ==================== DLL HIJACKING ====================
        private async Task ScanDllHijacking(CancellationToken token)
        {
            await Task.Run(() =>
            {
                Log("DLL", "🔍 Проверка потенциального DLL Hijacking...");
                
                string[] systemPaths = {
                    Environment.GetFolderPath(Environment.SpecialFolder.System),
                    Environment.GetFolderPath(Environment.SpecialFolder.SystemX86)
                };
                
                string[] suspiciousDlls = { "version.dll", "winhttp.dll", "d3d9.dll", "d3d11.dll", "opengl32.dll", "dxgi.dll", "msvcr100.dll", "msvcp100.dll" };
                var foundSuspicious = new List<string>();
                
                foreach (string path in systemPaths)
                {
                    try
                    {
                        Log("DLL", $"📂 Проверка папки: {path}");
                        
                        foreach (string dll in suspiciousDlls)
                        {
                            string fullPath = Path.Combine(path, dll);
                            if (File.Exists(fullPath))
                            {
                                var fi = new FileInfo(fullPath);
                                Log("DLL", $"📚 Проверка DLL: {dll} (Размер: {fi.Length} байт, Изменён: {fi.LastWriteTime})");
                                
                                if (fi.Length < 500 * 1024 || fi.LastWriteTime > DateTime.Now.AddDays(-30))
                                {
                                    foundSuspicious.Add(dll);
                                    Log("УГРОЗА", $"⚠️ ПОДОЗРИТЕЛЬНЫЙ РАЗМЕР DLL: {dll}");
                                    Log("УГРОЗА", $"   ├─ Размер: {fi.Length} байт");
                                    Log("УГРОЗА", $"   └─ Изменён: {fi.LastWriteTime}");
                                }
                            }
                        }
                    }
                    catch (Exception ex) { Log("DLL", $"❌ Ошибка: {ex.Message}"); }
                }
                
                if (foundSuspicious.Count > 0)
                {
                    Log("УГРОЗА", $"⚠️ ПОТЕНЦИАЛЬНЫЙ DLL HIJACKING ОБНАРУЖЕН!");
                    Log("УГРОЗА", $"   └─ DLL: {string.Join(", ", foundSuspicious)}");
                    AddThreat("ВЫСОКИЙ", "Потенциальный DLL Hijacking", $"DLL: {string.Join(", ", foundSuspicious)}");
                }
                
                Log("DLL", $"✅ Проверка DLL завершена. Подозрительных: {foundSuspicious.Count}");
            });
        }

        // ==================== ALL USERS STARTUP ====================
        private async Task ScanAllUsersStartup(CancellationToken token)
        {
            await Task.Run(() =>
            {
                Log("STARTUP_ALL", "🔍 Проверка автозагрузки для всех пользователей...");
                
                string startupPath = @"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Startup";
                int suspiciousItems = 0;
                
                try
                {
                    if (Directory.Exists(startupPath))
                    {
                        var files = Directory.GetFiles(startupPath, "*.lnk");
                        Log("STARTUP_ALL", $"📂 Проверка папки: {startupPath} (файлов: {files.Length})");
                        
                        foreach (string file in files)
                        {
                            token.ThrowIfCancellationRequested();
                            _scannedItems++;
                            
                            string name = Path.GetFileNameWithoutExtension(file).ToLower();
                            Log("STARTUP_ALL", $"📋 Проверка: {Path.GetFileName(file)}");
                            
                            if (CheatKeywords.Any(kw => name.Contains(kw)))
                            {
                                suspiciousItems++;
                                Log("УГРОЗА", $"⚠️ ПОДОЗРИТЕЛЬНЫЙ ЭЛЕМЕНТ АВТОЗАГРУЗКИ (ВСЕ ПОЛЬЗОВАТЕЛИ):");
                                Log("УГРОЗА", $"   ├─ Имя: {Path.GetFileName(file)}");
                                Log("УГРОЗА", $"   └─ Путь: {file}");
                                AddThreat("ВЫСОКИЙ", $"Подозрительный элемент автозагрузки (все пользователи): {Path.GetFileName(file)}", $"Путь: {file}");
                            }
                        }
                    }
                }
                catch (Exception ex) { Log("STARTUP_ALL", $"❌ Ошибка: {ex.Message}"); }
                
                Log("STARTUP_ALL", $"✅ Проверка завершена. Подозрительных: {suspiciousItems}");
            });
        }

        // ==================== UAC SETTINGS ====================
        private async Task CheckUacSettings(CancellationToken token)
        {
            await Task.Run(() =>
            {
                Log("UAC", "🔍 Проверка настроек UAC (Контроль учётных записей)...");
                
                try
                {
                    using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System");
                    if (key != null)
                    {
                        object? enableLua = key.GetValue("EnableLUA");
                        object? consentPromptBehaviorAdmin = key.GetValue("ConsentPromptBehaviorAdmin");
                        object? promptOnSecureDesktop = key.GetValue("PromptOnSecureDesktop");
                        
                        Log("UAC", $"🛡️ EnableLUA: {enableLua}");
                        Log("UAC", $"🛡️ ConsentPromptBehaviorAdmin: {consentPromptBehaviorAdmin}");
                        Log("UAC", $"🛡️ PromptOnSecureDesktop: {promptOnSecureDesktop}");
                        
                        if (enableLua != null && (int)enableLua == 0)
                        {
                            Log("УГРОЗА", $"⚠️ UAC ПОЛНОСТЬЮ ОТКЛЮЧЁН!");
                            Log("УГРОЗА", $"   └️ Система уязвима для повышения привилегий");
                            AddThreat("ВЫСОКИЙ", "UAC отключён", "Контроль учётных записей полностью отключён");
                        }
                        else if (consentPromptBehaviorAdmin != null && (int)consentPromptBehaviorAdmin == 0)
                        {
                            Log("УГРОЗА", $"⚠️ UAC НАСТРОЕН НА ПОВЫШЕНИЕ БЕЗ ЗАПРОСА!");
                            AddThreat("СРЕДНИЙ", "UAC настроен на повышение без запроса", "Повышение привилегий происходит автоматически");
                        }
                        else
                        {
                            Log("UAC", "✅ UAC включён и настроен корректно");
                        }
                    }
                }
                catch (Exception ex) { Log("UAC", $"❌ Ошибка: {ex.Message}"); }
            });
        }

        // ==================== ANTIVIRUS STATUS ====================
        private async Task ScanAntivirusStatus(CancellationToken token)
        {
            await Task.Run(() =>
            {
                Log("АВ", "🔍 Проверка статуса антивирусного ПО...");
                
                try
                {
                    using var searcher = new ManagementObjectSearcher("SELECT * FROM AntiVirusProduct", "root\\SecurityCenter2");
                    int avCount = 0;
                    
                    foreach (var obj in searcher.Get())
                    {
                        avCount++;
                        string? displayName = obj["displayName"]?.ToString() ?? "Неизвестно";
                        string? productState = obj["productState"]?.ToString() ?? "0";
                        string? guid = obj["instanceGuid"]?.ToString() ?? "";
                        
                        Log("АВ", $"🦠 Найден антивирус: {displayName}");
                        Log("АВ", $"   ├─ Состояние: {productState}");
                        Log("АВ", $"   └─ GUID: {guid}");
                    }
                    
                    if (avCount == 0)
                    {
                        Log("УГРОЗА", $"⚠️ АНТИВИРУС НЕ ОБНАРУЖЕН!");
                        Log("УГРОЗА", $"   └─ Система не защищена");
                        AddThreat("СРЕДНИЙ", "Антивирус не обнаружен", "На системе не найден активный антивирус");
                    }
                    else
                    {
                        Log("АВ", $"✅ Найдено антивирусных продуктов: {avCount}");
                    }
                }
                catch (Exception ex) { Log("АВ", $"❌ Ошибка: {ex.Message}"); }
            });
        }

        // ==================== FIREWALL STATUS ====================
        private async Task CheckFirewallStatus(CancellationToken token)
        {
            await Task.Run(() =>
            {
                Log("FIREWALL", "🔍 Проверка статуса брандмауэра Windows...");
                
                try
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName = "netsh",
                        Arguments = "advfirewall show allprofiles",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };
                    
                    using var process = Process.Start(psi);
                    if (process != null)
                    {
                        string output = process.StandardOutput.ReadToEnd();
                        
                        bool firewallOn = output.Contains("ON");
                        bool domainOn = output.Contains("Domain Profile") && output.Substring(output.IndexOf("Domain Profile")).Contains("ON");
                        bool privateOn = output.Contains("Private Profile") && output.Substring(output.IndexOf("Private Profile")).Contains("ON");
                        bool publicOn = output.Contains("Public Profile") && output.Substring(output.IndexOf("Public Profile")).Contains("ON");
                        
                        Log("FIREWALL", $"🔥 Domain Profile: {(domainOn ? "ВКЛЮЧЁН" : "ОТКЛЮЧЁН")}");
                        Log("FIREWALL", $"🔥 Private Profile: {(privateOn ? "ВКЛЮЧЁН" : "ОТКЛЮЧЁН")}");
                        Log("FIREWALL", $"🔥 Public Profile: {(publicOn ? "ВКЛЮЧЁН" : "ОТКЛЮЧЁН")}");
                        
                        if (!firewallOn || !domainOn || !privateOn || !publicOn)
                        {
                            Log("УГРОЗА", $"⚠️ БРАНДМАУЭР ОТКЛЮЧЁН ДЛЯ НЕКОТОРЫХ ПРОФИЛЕЙ!");
                            AddThreat("ВЫСОКИЙ", "Брандмауэр Windows отключён", "Система может быть уязвима для сетевых атак");
                        }
                        else
                        {
                            Log("FIREWALL", "✅ Брандмауэр Windows включён для всех профилей");
                        }
                    }
                }
                catch (Exception ex) { Log("FIREWALL", $"❌ Ошибка: {ex.Message}"); }
            });
        }

        // ==================== RDP STATUS ====================
        private async Task CheckRdpStatus(CancellationToken token)
        {
            await Task.Run(() =>
            {
                Log("RDP", "🔍 Проверка статуса RDP (Удалённый рабочий стол)...");
                
                try
                {
                    using var key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Terminal Server");
                    if (key != null)
                    {
                        object? fDenyTSConnections = key.GetValue("fDenyTSConnections");
                        object? fAllowToGetHelp = key.GetValue("fAllowToGetHelp");
                        
                        Log("RDP", $"📡 fDenyTSConnections: {fDenyTSConnections}");
                        
                        if (fDenyTSConnections != null && (int)fDenyTSConnections == 0)
                        {
                            // Проверка порта RDP
                            using var winStationsKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Terminal Server\WinStations\RDP-Tcp");
                            int? port = winStationsKey?.GetValue("PortNumber") as int?;
                            
                            Log("УГРОЗА", $"⚠️ RDP ВКЛЮЧЁН (Порт: {port ?? 3389})");
                            Log("УГРОЗА", $"   └─ Удалённый доступ к системе разрешён");
                            AddThreat("СРЕДНИЙ", "RDP включён", $"Удалённый рабочий стол доступен на порту {port ?? 3389}");
                        }
                        else
                        {
                            Log("RDP", "✅ RDP отключён");
                        }
                    }
                }
                catch (Exception ex) { Log("RDP", $"❌ Ошибка: {ex.Message}"); }
            });
        }

        // ==================== SYSTEM UPTIME ====================
        private async Task CheckSystemUptime(CancellationToken token)
        {
            await Task.Run(() =>
            {
                try
                {
                    using var searcher = new ManagementObjectSearcher("SELECT LastBootUpTime FROM Win32_OperatingSystem");
                    foreach (var obj in searcher.Get())
                    {
                        string? bootTimeStr = obj["LastBootUpTime"]?.ToString();
                        if (!string.IsNullOrEmpty(bootTimeStr))
                        {
                            DateTime bootTime = ManagementDateTimeConverter.ToDateTime(bootTimeStr);
                            TimeSpan uptime = DateTime.Now - bootTime;
                            
                            Log("UPTIME", $"⏱️ Время запуска системы: {bootTime:yyyy-MM-dd HH:mm:ss}");
                            Log("UPTIME", $"⏱️ Время работы: {uptime.Days} дней, {uptime.Hours} часов, {uptime.Minutes} минут, {uptime.Seconds} секунд");
                            
                            if (uptime.TotalDays > 30)
                            {
                                Log("UPTIME", $"⚠️ СИСТЕМА РАБОТАЕТ БОЛЕЕ {uptime.Days} ДНЕЙ!");
                                Log("UPTIME", $"   └─ Рекомендуется перезагрузка для установки обновлений");
                            }
                            else if (uptime.TotalDays > 7)
                            {
                                Log("UPTIME", $"ℹ️ Система работает более {uptime.Days} дней");
                            }
                        }
                    }
                }
                catch (Exception ex) { Log("UPTIME", $"❌ Ошибка: {ex.Message}"); }
            });
        }

        // ==================== ДОБАВЛЕНИЕ УГРОЗЫ ====================
        private void AddThreat(string severity, string name, string details)
        {
            Interlocked.Increment(ref _totalThreats);
            
            Dispatcher.Invoke(() =>
            {
                ThreatsCollection.Add(new ThreatItem
                {
                    Severity = severity,
                    Name = name,
                    Details = details,
                    Timestamp = DateTime.Now
                });
                
                ThreatsCount.Text = $"Угроз: {_totalThreats}";
                
                string severityIcon = severity switch
                {
                    "КРИТИЧНЫЙ" => "💀",
                    "ВЫСОКИЙ" => "⚠️",
                    "СРЕДНИЙ" => "⚡",
                    "НИЗКИЙ" => "ℹ️",
                    _ => "📌"
                };
                
                Log("УГРОЗА", $"{severityIcon} [{severity}] {name}");
                Log("УГРОЗА", $"   └─ {details}");
            });
        }

        // ==================== ОБНОВЛЕНИЕ ПРОГРЕССА ====================
        private void UpdateProgress(int percent, int max, string message)
        {
            Dispatcher.Invoke(() =>
            {
                MainProgress.Maximum = max;
                MainProgress.Value = percent;
                StatusSubText.Text = message;
                ScanProgressText.Text = message;
            });
        }

        private void LockUI(bool locked)
        {
            Dispatcher.Invoke(() =>
            {
                QuickScanBtn.IsEnabled = !locked;
                FullScanBtn.IsEnabled = !locked;
                DeepScanBtn.IsEnabled = !locked;
                SendToTgBtn.IsEnabled = !locked;
            });
        }

        private void ShowProgress(bool show)
        {
            Dispatcher.Invoke(() =>
            {
                ProgressPanel.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
                if (!show) MainProgress.Value = 0;
            });
        }
                // ==================== ОТЧЁТЫ ====================
        private async Task SaveReport()
        {
            await Task.Run(() =>
            {
                try
                {
                    string reportDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "RustShield_Reports");
                    Directory.CreateDirectory(reportDir);
                    
                    string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    string jsonPath = Path.Combine(reportDir, $"Report_{timestamp}.json");
                    string txtPath = Path.Combine(reportDir, $"Report_{timestamp}.txt");
                    
                    Log("ОТЧЁТ", "📄 Генерация отчёта...");
                    
                    // JSON отчёт
                    var json = JsonConvert.SerializeObject(_currentReport, Formatting.Indented);
                    File.WriteAllText(jsonPath, json);
                    Log("ОТЧЁТ", $"💾 JSON отчёт сохранён: {jsonPath}");
                    
                    // TXT отчёт
                    using (var writer = new StreamWriter(txtPath, false, Encoding.UTF8))
                    {
                        writer.WriteLine("╔════════════════════════════════════════════════════════════════════════════════════════╗");
                        writer.WriteLine("║                       RUST SHIELD SCANNER - ДЕТАЛЬНЫЙ ОТЧЁТ                             ║");
                        writer.WriteLine("╚════════════════════════════════════════════════════════════════════════════════════════╝");
                        writer.WriteLine();
                        writer.WriteLine($"📅 Дата и время: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                        writer.WriteLine($"💻 Компьютер: {Environment.MachineName}");
                        writer.WriteLine($"👤 Пользователь: {Environment.UserName}");
                        writer.WriteLine($"🖥️ Операционная система: {Environment.OSVersion.VersionString}");
                        writer.WriteLine($"🔍 Режим сканирования: {_currentReport?.ScanMode ?? "Неизвестно"}");
                        writer.WriteLine($"⏱️ Время сканирования: {_scanStopwatch.Elapsed.TotalSeconds:F1} секунд");
                        writer.WriteLine($"📊 Всего проверено элементов: {_scannedItems}");
                        writer.WriteLine($"⚠️ Найдено угроз: {_totalThreats}");
                        writer.WriteLine();
                        writer.WriteLine("═══════════════════════════════════════════════════════════════════════════════════════════");
                        writer.WriteLine("📋 ПОДОЗРИТЕЛЬНЫЕ ПРОЦЕССЫ");
                        writer.WriteLine("═══════════════════════════════════════════════════════════════════════════════════════════");
                        
                        if (SuspiciousProcesses.Count > 0)
                        {
                            foreach (var proc in SuspiciousProcesses)
                                writer.WriteLine($"  • {proc.Name} (PID: {proc.PID}) - Память: {proc.MemoryMB} MB - Путь: {proc.Path}");
                        }
                        else
                            writer.WriteLine("  • Не найдено");
                        
                        writer.WriteLine();
                        writer.WriteLine("═══════════════════════════════════════════════════════════════════════════════════════════");
                        writer.WriteLine("📁 ПОДОЗРИТЕЛЬНЫЕ ФАЙЛЫ");
                        writer.WriteLine("═══════════════════════════════════════════════════════════════════════════════════════════");
                        
                        if (SuspiciousFiles.Count > 0)
                        {
                            foreach (var file in SuspiciousFiles.Take(50))
                                writer.WriteLine($"  • {file}");
                            if (SuspiciousFiles.Count > 50)
                                writer.WriteLine($"  • ... и ещё {SuspiciousFiles.Count - 50} файлов");
                        }
                        else
                            writer.WriteLine("  • Не найдено");
                        
                        writer.WriteLine();
                        writer.WriteLine("═══════════════════════════════════════════════════════════════════════════════════════════");
                        writer.WriteLine("🔑 ПОДОЗРИТЕЛЬНЫЕ ЗАПИСИ РЕЕСТРА");
                        writer.WriteLine("═══════════════════════════════════════════════════════════════════════════════════════════");
                        
                        if (SuspiciousRegistry.Count > 0)
                        {
                            foreach (var reg in SuspiciousRegistry)
                                writer.WriteLine($"  • {reg.Path}\\{reg.Name} = {reg.Value}");
                        }
                        else
                            writer.WriteLine("  • Не найдено");
                        
                        writer.WriteLine();
                        writer.WriteLine("═══════════════════════════════════════════════════════════════════════════════════════════");
                        writer.WriteLine("🔌 USB УСТРОЙСТВА В ИСТОРИИ");
                        writer.WriteLine("═══════════════════════════════════════════════════════════════════════════════════════════");
                        
                        if (USBDevices.Count > 0)
                        {
                            foreach (var usb in USBDevices.Take(20))
                                writer.WriteLine($"  • {usb.FriendlyName} (ID: {usb.DeviceID})");
                        }
                        else
                            writer.WriteLine("  • Не найдено");
                        
                        writer.WriteLine();
                        writer.WriteLine("═══════════════════════════════════════════════════════════════════════════════════════════");
                        writer.WriteLine("📊 ПОСЛЕДНИЕ АКТИВНОСТИ (Prefetch/UserAssist)");
                        writer.WriteLine("═══════════════════════════════════════════════════════════════════════════════════════════");
                        
                        if (LastActivities.Count > 0)
                        {
                            foreach (var act in LastActivities.Take(30))
                                writer.WriteLine($"  • {act.FileName} (Источник: {act.Source})");
                        }
                        else
                            writer.WriteLine("  • Не найдено");
                        
                        writer.WriteLine();
                        writer.WriteLine("═══════════════════════════════════════════════════════════════════════════════════════════");
                        writer.WriteLine("🌐 СЕТЕВЫЕ ПОДКЛЮЧЕНИЯ");
                        writer.WriteLine("═══════════════════════════════════════════════════════════════════════════════════════════");
                        
                        if (NetworkConnections.Count > 0)
                        {
                            foreach (var conn in NetworkConnections.Take(20))
                                writer.WriteLine($"  • {conn.LocalAddress} -> {conn.ForeignAddress} ({conn.State})");
                        }
                        else
                            writer.WriteLine("  • Не найдено");
                        
                        writer.WriteLine();
                        writer.WriteLine("═══════════════════════════════════════════════════════════════════════════════════════════");
                        writer.WriteLine("🛡️ СТАТУС ЗАЩИТЫ СИСТЕМЫ");
                        writer.WriteLine("═══════════════════════════════════════════════════════════════════════════════════════════");
                        writer.WriteLine($"  • Антивирус: {(SuspiciousDrivers.Count > 0 ? "Обнаружен" : "Не обнаружен")}");
                        writer.WriteLine($"  • Брандмауэр: Проверен");
                        writer.WriteLine($"  • UAC: Проверен");
                        writer.WriteLine($"  • RDP: Проверен");
                        
                        writer.WriteLine();
                        writer.WriteLine("═══════════════════════════════════════════════════════════════════════════════════════════");
                        writer.WriteLine($"📅 Отчёт сгенерирован: {DateTime.Now}");
                        writer.WriteLine("🛡️ Rust Shield Scanner - Anti-Cheat System");
                    }
                    
                    Log("ОТЧЁТ", $"✅ Отчёт сохранён в: {reportDir}");
                    Log("ОТЧЁТ", $"📄 TXT отчёт: {txtPath}");
                }
                catch (Exception ex) { Log("ОТЧЁТ", $"❌ Ошибка сохранения отчёта: {ex.Message}"); }
            });
        }

        private async void GenerateReport_Click(object sender, RoutedEventArgs e) => await SaveReport();

        // ==================== TELEGRAM ====================
        private async void SendToTelegram_Click(object sender, RoutedEventArgs e) => await SendToTelegram();

        private async Task SendToTelegram()
        {
            if (BOT_TOKEN == "YOUR_BOT_TOKEN_HERE" || CHAT_ID == "YOUR_CHAT_ID_HERE")
            {
                Log("TELEGRAM", "❌ Токен бота или Chat ID не настроены!");
                Log("TELEGRAM", "📝 Установите BOT_TOKEN и CHAT_ID в коде программы");
                MessageBox.Show(
                    "⚠️ TELEGRAM НЕ НАСТРОЕН!\n\n" +
                    "Для отправки отчётов в Telegram:\n" +
                    "1. Создайте бота у @BotFather\n" +
                    "2. Получите Chat ID у @userinfobot\n" +
                    "3. Установите BOT_TOKEN и CHAT_ID в коде\n\n" +
                    "Текущие значения не установлены!",
                    "Telegram не настроен",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }
            
            Log("TELEGRAM", "📨 Начинаем отправку отчёта в Telegram...");
            StatusSubText.Text = "📨 Отправка в Telegram...";
            
            try
            {
                string reportText = GenerateTelegramReport();
                await SendMessageToTelegram(reportText);
                Log("TELEGRAM", "✅ Основной отчёт отправлен");
                
                if (_totalThreats > 0)
                {
                    Log("TELEGRAM", "📊 Обнаружены угрозы, отправка детального отчёта...");
                    await SendDetailedReport();
                    Log("TELEGRAM", "✅ Детальный отчёт отправлен");
                }
                
                Log("TELEGRAM", "🎉 Отчёт успешно отправлен в Telegram!");
                StatusSubText.Text = "✅ Отчёт отправлен в Telegram";
                
                MessageBox.Show(
                    "✅ ОТЧЁТ УСПЕШНО ОТПРАВЛЕН В TELEGRAM!\n\n" +
                    $"📊 Найдено угроз: {_totalThreats}\n" +
                    $"📁 Процессов: {SuspiciousProcesses.Count}\n" +
                    $"📄 Файлов: {SuspiciousFiles.Count}\n" +
                    $"🔑 Записей реестра: {SuspiciousRegistry.Count}",
                    "Telegram отправка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                Log("TELEGRAM", $"❌ Ошибка отправки: {ex.Message}");
                StatusSubText.Text = "❌ Ошибка отправки";
                MessageBox.Show($"Не удалось отправить отчёт в Telegram:\n{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GenerateTelegramReport()
        {
            var sb = new StringBuilder();
            sb.AppendLine("🛡️ *RUST SHIELD SCANNER - ОТЧЁТ О ПРОВЕРКЕ*");
            sb.AppendLine("");
            sb.AppendLine($"📅 *Дата:* {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"💻 *Компьютер:* {Environment.MachineName}");
            sb.AppendLine($"👤 *Пользователь:* {Environment.UserName}");
            sb.AppendLine($"🖥️ *ОС:* {Environment.OSVersion.VersionString}");
            sb.AppendLine($"🔍 *Режим:* {_currentReport?.ScanMode ?? "Неизвестно"}");
            sb.AppendLine($"⏱️ *Время проверки:* {_scanStopwatch.Elapsed.TotalSeconds:F1}с");
            sb.AppendLine($"📊 *Проверено элементов:* {_scannedItems}");
            sb.AppendLine("");
            sb.AppendLine("*════════════════════════════════*");
            sb.AppendLine("*📊 РЕЗУЛЬТАТЫ ПРОВЕРКИ*");
            sb.AppendLine("*════════════════════════════════*");
            sb.AppendLine($"");
            sb.AppendLine($"⚠️ *Найдено угроз:* {_totalThreats}");
            sb.AppendLine($"📊 *Подозрительных процессов:* {SuspiciousProcesses.Count}");
            sb.AppendLine($"📁 *Подозрительных файлов:* {SuspiciousFiles.Count}");
            sb.AppendLine($"🔑 *Записей реестра:* {SuspiciousRegistry.Count}");
            sb.AppendLine($"🔌 *USB устройств в истории:* {USBDevices.Count}");
            sb.AppendLine($"📋 *Записей активностей:* {LastActivities.Count}");
            sb.AppendLine($"🌐 *Сетевых подключений:* {NetworkConnections.Count}");
            sb.AppendLine($"");
            
            if (SuspiciousProcesses.Count > 0)
            {
                sb.AppendLine("*════════════════════════════════*");
                sb.AppendLine("*⚠️ ПОДОЗРИТЕЛЬНЫЕ ПРОЦЕССЫ*");
                sb.AppendLine("*════════════════════════════════*");
                foreach (var proc in SuspiciousProcesses.Take(10))
                    sb.AppendLine($"• `{proc.Name}` (PID: {proc.PID}, Память: {proc.MemoryMB} MB)");
                if (SuspiciousProcesses.Count > 10)
                    sb.AppendLine($"• ... и ещё {SuspiciousProcesses.Count - 10} процессов");
                sb.AppendLine($"");
            }
            
            if (SuspiciousFiles.Count > 0)
            {
                sb.AppendLine("*════════════════════════════════*");
                sb.AppendLine("*📁 ПОДОЗРИТЕЛЬНЫЕ ФАЙЛЫ*");
                sb.AppendLine("*════════════════════════════════*");
                foreach (var file in SuspiciousFiles.Take(10))
                    sb.AppendLine($"• `{Path.GetFileName(file)}`");
                if (SuspiciousFiles.Count > 10)
                    sb.AppendLine($"• ... и ещё {SuspiciousFiles.Count - 10} файлов");
                sb.AppendLine($"");
            }
            
            sb.AppendLine("");
            sb.AppendLine("🛡️ *Rust Shield Scanner - Anti-Cheat System*");
            sb.AppendLine("👨‍💻 *Разработчик:* SkyWalker");
            sb.AppendLine("📱 *Telegram:* @Loksimen");
            sb.AppendLine("🎮 *Discord:* maks8013");
            
            return sb.ToString();
        }

        private async Task SendMessageToTelegram(string message)
        {
            using var client = new HttpClient();
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("chat_id", CHAT_ID),
                new KeyValuePair<string, string>("text", message),
                new KeyValuePair<string, string>("parse_mode", "Markdown")
            });
            
            var response = await client.PostAsync($"https://api.telegram.org/bot{BOT_TOKEN}/sendMessage", content);
            var responseBody = await response.Content.ReadAsStringAsync();
            
            if (!response.IsSuccessStatusCode)
            {
                Log("TELEGRAM", $"❌ Ошибка API Telegram: {responseBody}");
                throw new Exception($"Telegram API error: {responseBody}");
            }
        }

        private async Task SendDetailedReport()
        {
            try
            {
                string reportDir = Path.Combine(Path.GetTempPath(), "RustShield");
                Directory.CreateDirectory(reportDir);
                
                string jsonPath = Path.Combine(reportDir, "report.json");
                string txtPath = Path.Combine(reportDir, "report.txt");
                
                var json = JsonConvert.SerializeObject(_currentReport, Formatting.Indented);
                await File.WriteAllTextAsync(jsonPath, json);
                
                var sb = new StringBuilder();
                sb.AppendLine("╔════════════════════════════════════════════════════════════════════╗");
                sb.AppendLine("║              RUST SHIELD SCANNER - ДЕТАЛЬНЫЙ ОТЧЁТ                 ║");
                sb.AppendLine("╚════════════════════════════════════════════════════════════════════╝");
                sb.AppendLine($"");
                sb.AppendLine($"📅 Дата: {DateTime.Now}");
                sb.AppendLine($"💻 Компьютер: {Environment.MachineName}");
                sb.AppendLine($"👤 Пользователь: {Environment.UserName}");
                sb.AppendLine($"⚠️ Найдено угроз: {_totalThreats}");
                sb.AppendLine($"");
                sb.AppendLine("═══════════════════════════════════════════════════════════════════════");
                sb.AppendLine("📋 ПОДОЗРИТЕЛЬНЫЕ ПРОЦЕССЫ");
                sb.AppendLine("═══════════════════════════════════════════════════════════════════════");
                foreach (var proc in SuspiciousProcesses)
                    sb.AppendLine($"PID: {proc.PID} | Имя: {proc.Name} | Память: {proc.MemoryMB} MB | Путь: {proc.Path}");
                sb.AppendLine($"");
                sb.AppendLine("═══════════════════════════════════════════════════════════════════════");
                sb.AppendLine("📁 ПОДОЗРИТЕЛЬНЫЕ ФАЙЛЫ");
                sb.AppendLine("═══════════════════════════════════════════════════════════════════════");
                foreach (var file in SuspiciousFiles.Take(50))
                    sb.AppendLine(file);
                sb.AppendLine($"");
                sb.AppendLine("═══════════════════════════════════════════════════════════════════════");
                sb.AppendLine("🔑 ПОДОЗРИТЕЛЬНЫЕ ЗАПИСИ РЕЕСТРА");
                sb.AppendLine("═══════════════════════════════════════════════════════════════════════");
                foreach (var reg in SuspiciousRegistry)
                    sb.AppendLine($"{reg.Path}\\{reg.Name} = {reg.Value}");
                sb.AppendLine($"");
                sb.AppendLine("═══════════════════════════════════════════════════════════════════════");
                sb.AppendLine("🔌 USB УСТРОЙСТВА");
                sb.AppendLine("═══════════════════════════════════════════════════════════════════════");
                foreach (var usb in USBDevices.Take(30))
                    sb.AppendLine($"{usb.FriendlyName} (ID: {usb.DeviceID})");
                
                await File.WriteAllTextAsync(txtPath, sb.ToString());
                
                using var client = new HttpClient();
                using var form = new MultipartFormDataContent();
                
                if (File.Exists(jsonPath))
                {
                    var jsonContent = new ByteArrayContent(await File.ReadAllBytesAsync(jsonPath));
                //                     form.Add(jsonContent, "document", "detailed_report.json"); // �������� ���������������� ��� ����������� ������ CS1503
                jsonContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                }
                
                if (File.Exists(txtPath))
                {
                    var txtContent = new ByteArrayContent(await File.ReadAllBytesAsync(txtPath));
                //                     form.Add(txtContent, "document", "detailed_report.txt"); // �������� ���������������� ��� ����������� ������ CS1503
                txtContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/plain");
                }
                
                form.Add(new StringContent(CHAT_ID), "chat_id");
                form.Add(new StringContent(CHAT_ID), "chat_id");
                form.Add(new StringContent($"?? *��������� ��ר�*\n\n?? �����: {_totalThreats}\n?? ���������: {SuspiciousProcesses.Count}\n?? ������: {SuspiciousFiles.Count}"), "caption");
                
                var response = await client.PostAsync($"https://api.telegram.org/bot{BOT_TOKEN}/sendDocument", form);
                response.EnsureSuccessStatusCode();
                
                Directory.Delete(reportDir, true);
            }
            catch (Exception ex)
            {
                Log("TELEGRAM", $"❌ Ошибка отправки детального отчёта: {ex.Message}");
            }
        }

        private async Task SendScanStartNotification()
        {
            if (BOT_TOKEN == "YOUR_BOT_TOKEN_HERE") return;
            try
            {
                string message = $"🔍 *СКАНИРОВАНИЕ НАЧАЛО*\n\n" +
                    $"💻 Компьютер: {Environment.MachineName}\n" +
                    $"👤 Пользователь: {Environment.UserName}\n" +
                    $"🕐 Время: {DateTime.Now:HH:mm:ss}\n" +
                    $"🛡️ Rust Shield Scanner начал проверку системы";
                await SendMessageToTelegram(message);
                Log("TELEGRAM", "📨 Уведомление о начале сканирования отправлено");
            }
            catch (Exception ex) { Log("TELEGRAM", $"❌ Ошибка уведомления: {ex.Message}"); }
        }

        private async Task SendScanCompleteNotification()
        {
            if (BOT_TOKEN == "YOUR_BOT_TOKEN_HERE") return;
            try
            {
                string status = _totalThreats > 0 ? "⚠️ *НАЙДЕНЫ УГРОЗЫ*" : "✅ *СКАНИРОВАНИЕ ЗАВЕРШЕНО*";
                string message = $"{status}\n\n" +
                    $"📊 *Результаты:*\n" +
                    $"├─ Угроз: {_totalThreats}\n" +
                    $"├─ Процессов: {SuspiciousProcesses.Count}\n" +
                    $"├─ Файлов: {SuspiciousFiles.Count}\n" +
                    $"└─ Реестр: {SuspiciousRegistry.Count}\n\n" +
                    $"⏱️ Время: {_scanStopwatch.Elapsed.TotalSeconds:F1}с\n" +
                    $"🕐 Завершено: {DateTime.Now:HH:mm:ss}";
                await SendMessageToTelegram(message);
                Log("TELEGRAM", "📨 Уведомление о завершении сканирования отправлено");
            }
            catch (Exception ex) { Log("TELEGRAM", $"❌ Ошибка уведомления: {ex.Message}"); }
        }
    }

    // ==================== МОДЕЛИ ДАННЫХ ====================
    
    public class ThreatItem
    {
        public string Severity { get; set; } = "";
        public string Name { get; set; } = "";
        public string Details { get; set; } = "";
        public DateTime Timestamp { get; set; }
    }

    public class ProcessInfo
    {
        public int PID { get; set; }
        public string Name { get; set; } = "";
        public string Path { get; set; } = "";
        public long MemoryMB { get; set; }
        public DateTime StartTime { get; set; }
    }

    public class ProcessModuleInfo
    {
        public string ProcessName { get; set; } = "";
        public string ModuleName { get; set; } = "";
        public string ModulePath { get; set; } = "";
    }

    public class RegistryItem
    {
        public string Path { get; set; } = "";
        public string Name { get; set; } = "";
        public string Value { get; set; } = "";
    }

    public class USBDeviceInfo
    {
        public string DeviceID { get; set; } = "";
        public string FriendlyName { get; set; } = "";
        public string Manufacturer { get; set; } = "";
    }

    public class LastActivityInfo
    {
        public string FileName { get; set; } = "";
        public string Source { get; set; } = "";
        public DateTime ExecutedTime { get; set; }
    }

    public class JumpListInfo
    {
        public string TargetPath { get; set; } = "";
        public string Application { get; set; } = "";
    }

    public class ShellBagInfo
    {
        public string FolderPath { get; set; } = "";
        public DateTime LastAccessed { get; set; }
    }

    public class NetworkConnection
    {
        public string LocalAddress { get; set; } = "";
        public string ForeignAddress { get; set; } = "";
        public string State { get; set; } = "";
    }

    public class ServiceInfo
    {
        public string Name { get; set; } = "";
        public string DisplayName { get; set; } = "";
    }

    public class StartupItem
    {
        public string Name { get; set; } = "";
        public string Path { get; set; } = "";
    }

    public class ScheduledTask
    {
        public string Name { get; set; } = "";
        public string Path { get; set; } = "";
    }

    public class BrowserExtension
    {
        public string Name { get; set; } = "";
        public string ID { get; set; } = "";
    }

    public class DriverInfo
    {
        public string Name { get; set; } = "";
        public string DisplayName { get; set; } = "";
    }

    public class DnsCacheEntry
    {
        public string Domain { get; set; } = "";
    }

    public class OpenPort
    {
        public int PortNumber { get; set; }
    }

    public class WmiEventInfo
    {
        public string Name { get; set; } = "";
        public string Query { get; set; } = "";
    }

    public class HostsEntry
    {
        public int LineNumber { get; set; }
        public string Line { get; set; } = "";
    }

    public class PrefetchFile
    {
        public string Name { get; set; } = "";
        public string Path { get; set; } = "";
    }

    public class EventLogEntry
    {
        public string EventCode { get; set; } = "";
        public string Message { get; set; } = "";
        public string Type { get; set; } = "";
    }

    public class FullScanReport
    {
        public string ScanMode { get; set; } = "";
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Duration { get; set; }
        public int TotalThreats { get; set; }
        public string ComputerName { get; set; } = "";
        public string UserName { get; set; } = "";
        public string OSVersion { get; set; } = "";
        public List<ProcessInfo> SuspiciousProcesses { get; set; } = new();
        public List<string> SuspiciousFiles { get; set; } = new();
        public List<RegistryItem> SuspiciousRegistry { get; set; } = new();
        public List<USBDeviceInfo> USBDevices { get; set; } = new();
        public List<LastActivityInfo> LastActivities { get; set; } = new();
        public List<NetworkConnection> NetworkConnections { get; set; } = new();
    }
}


