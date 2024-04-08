using Telegram.Bot;
using Telegram.Bot.Types;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkNet;
using System.Text.Json;
using Telegram.Bot.Types.Enums;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System;
using System.Management;
using System.Xml.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Linq;
using System.Windows;

public class Program
{
    public static async Task FF(string holdexe, string acfpath, string ffpath, string tokenn)

    {
        try
        {


            var currentDirectory = System.IO.Directory.GetCurrentDirectory();

            string accessToken = "vk1.a.y8O2zKkMXKfXwLKTYDT7mD2HIlYZ5AzDrtGK4D8LsfsWcpAFHrQqnKXf8Nz15dOSz3fErW_jR9fNDnbEjapSo9JLEuIppaVY-Gja9tYB-sB5TqkYzu-trQ01kB4nOnsnJNIGDBGYUhLsfJOCHWe6SH8Zj7BY5xSxjzOz1PDb7OhgF-SPAe69Tcr87LFrZt2Z0Arzq7UcrwuZmAaxw0C2FA";
            int albumId = -203665884;
            string apiUrl = $"https://api.vk.com/method/photos.get?v=5.199&access_token={accessToken}&owner_id={albumId}&album_id=wall&count=1&rev=1";
            string logFolderPath = $@"{currentDirectory}\Logs";
            string telegramBotToken = "6264844882:AAEaSTgi-1d9wE0JhbsB3dP6Z6QQE_LQr4k";
            long chatId = -4158830850; // Замените на фактический ID беседы в Telegram





            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                string json = await response.Content.ReadAsStringAsync();

                JsonDocument doc = JsonDocument.Parse(json);
                JsonElement root = doc.RootElement;

                if (root.TryGetProperty("response", out JsonElement responseElement))
                {
                    JsonElement photoElement = responseElement.GetProperty("items")[0];
                    string photoUrl = photoElement.GetProperty("sizes")[2].GetProperty("url").GetString();

                    Console.WriteLine(photoUrl);
                    System.IO.File.AppendAllText("1.txt", photoUrl);
                    SendPhotoToTelegram(tokenn, chatId, photoUrl);
                }

            }
            string currentPcNumber = Environment.MachineName.Replace("PC", "");
            for (int i = 2; i <= 42; i++)
            {
                string pcNumber = i.ToString("D2");
                if (pcNumber == currentPcNumber)
                {
                    continue;
                }
                string ffsBatchFilePath = $@"{Path.GetDirectoryName(ffpath)}/sync/PC{pcNumber}.ffs_batch";
                Console.WriteLine(ffsBatchFilePath);
                string pcName = $@"PC{pcNumber}";
                Random rnd = new Random();
                int value = rnd.Next(0, 100000);
                string username = "Colizeum";
                string password = "8A4666";
                string sourceACFDirectory = $@"\\PC{currentPcNumber}\d$\Games\Steam\Steamapps";
                string destinationACFDirectory = $@"\\{pcName}\d$\Games\Steam\Steamapps";
                string acfExceptionsFile = acfpath;
                try
                {
                    if (IsApplicationRunning(pcName, username, password, "Langamelockerform.exe"))
                    {

                        string taskExecutablePath = holdexe;

                        string schtasksCommand = $@"schtasks /Create /S PC{pcNumber} /TN ""{value}"" /u Colizeum /p 8A4666 /TR ""{taskExecutablePath}"" /SC ONCE /ST {DateTime.Now.AddMinutes(1).ToString("HH:mm")} /SD {DateTime.Now.AddYears(1).ToString("dd/MM/yyyy")}";
                        string schtasksCommandrun = $@"schtasks /Run /S PC{pcNumber} /TN ""{value}"" /u Colizeum /p 8A4666";
                        Process.Start("cmd.exe", $"/c {schtasksCommand}");
                        Thread.Sleep(3000);
                        Process.Start("cmd.exe", $"/c {schtasksCommandrun}");
                        string windowTitle = $"Tehstatus";
                        Process process = new Process();
                        process.StartInfo.FileName = ffpath;
                        process.StartInfo.Arguments = ffsBatchFilePath;
                        process.Start();

                        process.WaitForExit();
                        Thread.Sleep(3000);

                        MoveACFFiles(sourceACFDirectory, destinationACFDirectory, acfExceptionsFile);

                        int exitCode = process.ExitCode;

                        if (exitCode == 0)
                        {
                            string latestLogFile = GetLatestLogFile(logFolderPath);
                            string successMessage = $"<b>Обновление игр успешно завершено\nна PC{pcNumber}\n<u>{DateTime.Now}</u></b>";
                            //SendMessageToTelegram(telegramBotToken, chatId, successMessage);
                            if (!string.IsNullOrEmpty(latestLogFile))
                            {
                                SendFileToTelegram(tokenn, chatId, latestLogFile, successMessage);
                                Console.WriteLine($"Обновление игр успешно завершено на PC{pcNumber} в {DateTime.Now}");
                            }

                            Process.Start("cmd.exe", $"/c shutdown /r /m \\\\PC{pcNumber} /t 5 /f");
                        }
                        else if (exitCode == 3)
                        {
                            string latestLogFile = GetLatestLogFile(logFolderPath);
                            string errorMessage = $"<b>Ошибка при обновлении игр \n на PC{pcNumber}\n<u>{DateTime.Now}</u></b>";
                            //SendMessageToTelegram(telegramBotToken, chatId, errorMessage);
                            if (!string.IsNullOrEmpty(latestLogFile))
                            {
                                SendFileToTelegram(tokenn, chatId, latestLogFile, errorMessage);
                                Console.WriteLine($"Ошибка при обновлении игр на PC{pcNumber} в {DateTime.Now}");
                            }
                            Process.Start("cmd.exe", $"/c shutdown /r /m \\\\PC{pcNumber} /t 5 /f");
                        }
                        else if (exitCode == 1)
                        {
                            string latestLogFile = GetLatestLogFile(logFolderPath);
                            string warningMessage = $"<b>Обновление игр завершено с предупреждениями\nна PC{pcNumber} <i>Проверьте отчет!</i>\n<u>{DateTime.Now}</u></b>";
                            //SendMessageToTelegram(telegramBotToken, chatId, warningMessage);
                            if (!string.IsNullOrEmpty(latestLogFile))
                            {
                                SendFileToTelegram(tokenn, chatId, latestLogFile, warningMessage);
                                Console.WriteLine($"Обновление игр завершено с предупреждениями{pcNumber} в {DateTime.Now}");
                            }
                            Process.Start("cmd.exe", $"/c shutdown /r /m \\\\PC{pcNumber} /t 5 /f");
                        }
                        else
                        {
                            SendMessageToTelegram(tokenn, chatId, $@"{pcNumber} в данный момент занят гостем и был пропущен.");
                            Console.WriteLine($"PC{pcNumber}в данный момент занят гостем и был пропущен");
                        }
                    }

                }
                catch (Exception ex)
                {
                    SendMessageToTelegram(tokenn, chatId, $@"Произошла ошибка:{ex}");
                    Process.Start("cmd.exe", $"/c shutdown /r /m \\\\PC{pcNumber} /t 5 /f");
                }
            }
            //Process.Start("cmd.exe", $"/c shutdown /r /m \\PC01 /t 5 /f");



            static void MoveACFFiles(string sourceDirectory, string destinationDirectory, string exceptionsFile)
            {
                string[] acfFiles = Directory.GetFiles(sourceDirectory, "appmanifest_*.acf");

                string[] excludedGames = System.IO.File.ReadAllLines(exceptionsFile);

                foreach (string acfFile in acfFiles)
                {
                    string fileName = Path.GetFileName(acfFile);
                    string gameId = fileName.Replace("appmanifest_", "").Replace(".acf", "");

                    if (excludedGames.Contains(gameId))
                    {
                        string destinationFile = Path.Combine(destinationDirectory, fileName);
                        MoveACF(acfFile, destinationFile);
                    }
                }
            }

            static void MoveACF(string source, string destination)
            {
                Process process3 = new Process();
                process3.StartInfo.FileName = "cmd.exe";
                process3.StartInfo.Arguments = $"/C copy /Y {source} {destination}";
                process3.Start();

            }
            static void RMdir(string destination)
            {
                Process process3 = new Process();
                process3.StartInfo.FileName = "cmd.exe";
                process3.StartInfo.Arguments = $"/C rmdir /S /Q \"{destination}\"";
                process3.Start();
                process3.WaitForExit();
                Console.WriteLine(process3.StartInfo.Arguments);
            }

            static bool IsApplicationRunning(string pcName, string username, string password, string applicationName)
            {
                try
                {
                    ConnectionOptions options = new ConnectionOptions
                    {
                        Username = username,
                        Password = password
                    };
                    ManagementScope scope = new ManagementScope($"\\\\{pcName}\\root\\cimv2", options);
                    ObjectQuery query = new ObjectQuery($"SELECT * FROM Win32_Process WHERE Name = '{applicationName}'");
                    ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
                    ManagementObjectCollection results = searcher.Get();

                    return results.Count > 0;

                }
                catch
                {
                    return false;
                }
            }

            static void SendMessageToTelegram(string botToken, long chatId, string message)
            {
                var bot = new TelegramBotClient(botToken);
                bot.SendTextMessageAsync(chatId, message).Wait();
            }

            static void SendFileToTelegram(string botToken, long chatId, string filePath, string caption)
            {
                var bot = new TelegramBotClient(botToken);
                using (var fileStream = System.IO.File.Open(filePath, FileMode.Open))
                {
                    bot.SendDocumentAsync(chatId: chatId, InputFile.FromStream(stream: fileStream, fileName: filePath), caption: caption, parseMode: ParseMode.Html).Wait();
                }
            }
            static void SendPhotoToTelegram(string botToken, long chatId, string photoUrl)
            {
                var bot = new TelegramBotClient(botToken);

                bot.SendPhotoAsync(chatId: chatId, photo: InputFile.FromUri(photoUrl), parseMode: ParseMode.Html, caption: $"<b>Обновление игр началось. Доброго утра!</b>\n{DateTime.Now}").Wait();
            }



            static string GetLatestLogFile(string folderPath)
            {
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string[] logFiles = Directory.GetFiles(folderPath, "*.html");
                if (logFiles.Length > 0)
                {
                    Array.Sort(logFiles);
                    return logFiles[logFiles.Length - 1];
                }

                return null;
            }
        }
        catch
        {
            MessageBox.Show("Ошибка Token-а Telegram");
        }
    }
}
