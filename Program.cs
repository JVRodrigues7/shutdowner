using System.Diagnostics;
using System.Text.RegularExpressions;

class Program {
  static bool endApp = false;
  static DateTime[] dateTimes = new DateTime[10];
  static string? message = null;

  static void Main() {
    while (!endApp) {
      var ans = ReadKeys(DrawMainMenu);
      int? opt;

      try {
        opt = Convert.ToInt32(ans);
      }
      catch {
        opt = null;
      }

      switch (opt) {
        case 1:
          message = null;
          TimeMenu();
          break;
        case 2:
          message = null;
          CountdownMenu();
          break;
        case 3:
          message = null;
          CancelShutdown();
          break;
        case 0:
          Environment.Exit(0);
          return;
      }
    }
  }

  static void CountdownMenu() {
    while (1 == 1) {
      var ans = ReadKeys(DrawCountdownMenu);
      int? opt;
      try {
        opt = Convert.ToInt32(ans.FirstOrDefault().ToString());
      }
      catch {
        opt = null;
      }

      switch (opt) {
        case 1:
          Shutdown(5);
          break;
        case 2:
          Shutdown(10);
          break;
        case 3:
          Shutdown(15);
          break;
        case 4:
          Shutdown(20);
          break;
        case 5:
          Shutdown(30);
          break;
        case 6:
          Shutdown(45);
          break;
        case 7:
          Shutdown(60);
          break;
        case 8:
          Shutdown(90);
          break;
        case 9:
          if (CustomCountdown()) return;
          break;
        case 0:
          return;
      }
    }
  }

  static bool CustomCountdown() {
    var ret = false;
    while (!ret) {
      var ans = ReadKeys(DrawCustomCountdown);
      if (ans == "") return false;

      var hoursString = Regex.IsMatch(ans, "\\d{0,2}h") ? Regex.Match(ans, "\\d{0,2}h").Value.Replace("h", "") : "0";
      var minutesString = Regex.IsMatch(ans, "\\d{0,2}m") ? Regex.Match(ans, "\\d{0,2}m").Value.Replace("m", "") : "0";
      var hours = Convert.ToInt32(hoursString);
      var minutes = Convert.ToInt32(minutesString);
      var shutdownTime = (hours * 60) + minutes;

      if (shutdownTime > 1) {
        Shutdown(shutdownTime);
        return true;
      }
    }
    return ret;
  }

  static void TimeMenu() {
    dateTimes[1] = DateTime.Now.AddMinutes(5);
    dateTimes[2] = DateTime.Now.AddMinutes(10);
    dateTimes[3] = DateTime.Now.AddMinutes(15);
    dateTimes[4] = DateTime.Now.AddMinutes(20);
    dateTimes[5] = DateTime.Now.AddMinutes(30);
    dateTimes[6] = DateTime.Now.AddMinutes(45);
    dateTimes[7] = DateTime.Now.AddMinutes(60);
    dateTimes[8] = DateTime.Now.AddMinutes(90);
    dateTimes[9] = DateTime.Now.AddMinutes(120);

    var ret = false;

    while (!ret) {
      var ans = ReadKeys(DrawTimeMenu);
      int? opt;
      try {
        opt = Convert.ToInt32(ans.FirstOrDefault().ToString());
      }
      catch {
        opt = null;
      }

      if (opt == 0) return;

      if (opt > 0 && opt <= 9) {
        int shutdownTime = (dateTimes[(int)opt] - DateTime.Now).Minutes;
        if (shutdownTime > 1) {
          Shutdown(shutdownTime);
          ret = true;
        }
      }
    }
  }

  static void Shutdown(int timeInMinutes) {
    int timeInSeconds = timeInMinutes * 60;
    var process = new Process() {
      StartInfo = new ProcessStartInfo("shutdown", String.Concat(" /s /t ", timeInSeconds.ToString())) {
        CreateNoWindow = true,
        UseShellExecute = false
      }
    };
    process.Start();
    process.WaitForExit();
    switch (process.ExitCode) {
      case 0:
        message = string.Concat("The system will shutdown at ", DateTime.Now.AddMinutes(timeInMinutes).ToShortTimeString());
        break;
      case 1190:
        message = "A shutdown was already scheduled";
        break;
    }
  }

  static void CancelShutdown() {
    var process = new Process() {
      StartInfo = new ProcessStartInfo("shutdown", "/a") {
        CreateNoWindow = true,
        UseShellExecute = false
      }
    };
    process.Start();
    process.WaitForExit();
    if (process.ExitCode == 0)
      message = "The system shutdown was canceled";
  }

  static string ReadKeys(Action drawMethod) {
    string pressedKeys = "";
    var obj = new ConsoleKeyInfo();

    bool ret = false;
    while (!ret) {
      drawMethod();

      switch (obj.Key) {
        case ConsoleKey.C:
        case ConsoleKey.Escape:
          pressedKeys = "";
          ret = true;
          break;

        case ConsoleKey.Backspace:
          if (pressedKeys.Length > 0) {
            pressedKeys = pressedKeys.Substring(0, pressedKeys.Length - 1);
          }
          Console.Write(pressedKeys);
          obj = Console.ReadKey(true);
          break;

        case ConsoleKey.Enter:
          if (!string.IsNullOrEmpty(pressedKeys)) {
            ret = true;
            break;
          }
          obj = new ConsoleKeyInfo('\0', ConsoleKey.None, false, false, false);
          break;

        default:
          if (obj.Key != ConsoleKey.None && !String.IsNullOrEmpty(obj.KeyChar.ToString())) {
            pressedKeys += obj.KeyChar.ToString();
          }
          Console.Write(pressedKeys);
          obj = Console.ReadKey(true);
          break;
      }
    }
    return pressedKeys;
  }

  static void DrawTitle() {
    Console.Clear();
    Console.WriteLine(
  @"      _             _        _                                     
 ___ | |__   _   _ | |_   __| |  ___  __      __ _ __    ___  _ __ 
/ __|| '_ \ | | | || __| / _` | / _ \ \ \ /\ / /| '_ \  / _ \| '__|
\__ \| | | || |_| || |_ | (_| || (_) | \ V  V / | | | ||  __/| |   
|___/|_| |_| \__,_| \__| \__,_| \___/   \_/\_/  |_| |_| \___||_|   
                                                    by: JVRodrigues
"
    );
  }

  static void DrawMainMenu() {
    DrawTitle();
    Console.WriteLine("Select an option:");
    Console.WriteLine();
    Console.WriteLine("1 - Set a time to shutdown");
    Console.WriteLine("2 - Set a countdown to shutdown");
    Console.WriteLine("3 - Cancel shutdown");
    Console.WriteLine();
    Console.WriteLine("0 - Exit");
    Console.WriteLine(message);
  }

  static void DrawCountdownMenu() {
    DrawTitle();
    Console.WriteLine("Select an option:");
    Console.WriteLine();
    Console.WriteLine("1 - 05 Minutes");
    Console.WriteLine("2 - 10 Minutes");
    Console.WriteLine("3 - 15 Minutes");
    Console.WriteLine("4 - 20 Minutes");
    Console.WriteLine("5 - 30 Minutes");
    Console.WriteLine("6 - 45 Minutes");
    Console.WriteLine("7 - 01 Hour");
    Console.WriteLine("8 - 01 Hour and 30 Minutes");
    Console.WriteLine("9 - Custom");
    Console.WriteLine();
    Console.WriteLine("0 - Back to Menu");
    Console.WriteLine();
  }

  static void DrawCustomCountdown() {
    DrawTitle();
    Console.WriteLine();
    Console.WriteLine("Insert a time using the following format: 99h99m");
    Console.WriteLine("Press 'c' to cancel");
    Console.WriteLine("");
  }

  static void DrawTimeMenu() {
    DrawTitle();
    Console.WriteLine("Select an option:");
    Console.WriteLine();
    Console.WriteLine(String.Concat("1 - ", dateTimes[1].ToShortTimeString()));
    Console.WriteLine(String.Concat("2 - ", dateTimes[2].ToShortTimeString()));
    Console.WriteLine(String.Concat("3 - ", dateTimes[3].ToShortTimeString()));
    Console.WriteLine(String.Concat("4 - ", dateTimes[4].ToShortTimeString()));
    Console.WriteLine(String.Concat("5 - ", dateTimes[5].ToShortTimeString()));
    Console.WriteLine(String.Concat("6 - ", dateTimes[6].ToShortTimeString()));
    Console.WriteLine(String.Concat("7 - ", dateTimes[7].ToShortTimeString()));
    Console.WriteLine(String.Concat("8 - ", dateTimes[8].ToShortTimeString()));
    Console.WriteLine(String.Concat("9 - ", dateTimes[9].ToShortTimeString()));
    Console.WriteLine();
    Console.WriteLine("0 - Back to Menu");
    Console.WriteLine();
  }
}