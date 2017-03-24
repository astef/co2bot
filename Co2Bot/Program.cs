using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using Microsoft.Extensions.Configuration;

namespace Co2Bot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var cfg = new ConfigurationBuilder().AddJsonFile("options.json").Build();
            var options = new Options().Configure(cfg);

            var tempNotifier = new Notifier(options.ShortRepeatCount, TimeSpan.FromMinutes(options.LongRepeatDelayMin));
            tempNotifier.Message += m => Send(m, options);
            var noairNotifier = new Notifier(options.ShortRepeatCount, TimeSpan.FromMinutes(options.LongRepeatDelayMin));
            noairNotifier.Message += m => Send(m, options);
            var greet = false;
            while (true)
            {
                var now = DateTime.Now;
                try
                {
                    string[] lines;
                    try
                    {
                        lines =
                            File.ReadAllLines(
                                $"{options.Co2DataPath}/{now.Year}/{now.ToString("MM")}/{now.ToString("dd")}.CSV")
                                .Last()
                                .Split(',');
                    }
                    catch (Exception e)
                    {
                        Log($"Exception reading file. Do not start. {e.Message}");
                        continue;
                    }
                    if (lines.Length < 2)
                    {
                        Log("Not enough lines. Do not start.");
                        continue;
                    }
                    //13:56:03,726,25.54,0.00
                    var date = DateTime.ParseExact(lines[0], "HH:mm:ss", CultureInfo.InvariantCulture);
                    if (now - date > TimeSpan.FromMilliseconds(options.Frequency))
                    {
                        Log($"Old data. Do not start. {now - date} > {options.Frequency} ms");
                        continue;
                    }
                    //
                    var morning = DateTime.ParseExact(options.MorningTime, "HH:mm", CultureInfo.InvariantCulture);
                    var evening = DateTime.ParseExact(options.EveningTime, "HH:mm", CultureInfo.InvariantCulture);
                    if (now < morning || now > evening)
                    {
                        continue;
                    }
                    //
                    var co2 = int.Parse(lines[1], CultureInfo.InvariantCulture);
                    var temp = double.Parse(lines[2], CultureInfo.InvariantCulture);
                    if (!greet)
                    {
                        Send(
                            $"Бот проверки качества воздуха запущен на машине {Environment.MachineName}. Текущие параметры воздуха в кабинете: CO2 = {co2} ppm; температура = {temp} C",
                            options);
                        greet = true;
                    }
                    if (co2 > options.RedCo2)
                    {
                        noairNotifier.Warn($"Внимание! Опасная концентрация CO2: {co2} ppm");
                    }
                    else if (co2 > options.YellowCo2)
                    {
                        noairNotifier.Warn($"Высокая концентрация CO2: {co2} ppm");
                    }
                    else
                    {
                        noairNotifier.Ok($"Концентрация CO2 вернулась в норму: {co2} ppm");
                    }
                    if (temp < options.BlueTemp)
                    {
                        tempNotifier.Warn($"Низкая температура: {temp} C");
                    }
                    else if (temp > options.RedTemp)
                    {
                        tempNotifier.Warn($"Высокая температура: {temp} C");
                    }
                    else
                    {
                        tempNotifier.Ok($"Температура вернулась в норму: {temp} C");
                    }
                }
                catch (Exception ee)
                {
                    Log($"Exception.Message: {ee.Message}");
                    Log($"Exception.ToString: {ee}");
                }
                finally
                {
                    System.Threading.Thread.Sleep(Math.Max(options.Frequency, 1000));
                }
            }
        }

        private static void Send(string msg, Options opt)
        {
            if (opt.SendToConsole)
            {
                Console.WriteLine(msg);
            }
            else
            {
                var res = new HttpClient().GetAsync($"{opt.HttpUrl}{msg}").Result;
                Log($"I've sent notification. Result code: {res.StatusCode}");
            }
        }

        private static readonly object LastLogSyncRoot = new object();

        private static string _lastLog;

        private static void Log(string msg, bool allowRepeat = false)
        {
            lock (LastLogSyncRoot)
            {
                if (!allowRepeat && _lastLog == msg)
                    return;

                // delete 1-week ago log
                var now = DateTime.Now;
                File.Delete(MakeLogFileName(now.AddDays(-7.0)));
                //
                var log = $"{now.ToString("HH:mm:ss.fff")}: {msg}";
                File.AppendAllLines(MakeLogFileName(now), new[] { log });
                Console.WriteLine(log);

                _lastLog = msg;
            }
        }

        private static string MakeLogFileName(DateTime dt)
        {
            return $"log_{dt.ToString("yy-MM-dd")}.txt";
        }
    }
}
