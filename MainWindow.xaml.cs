using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using Microsoft.Win32;
using System.Reflection.Metadata;
using System.Threading;

namespace FreeFileSyncApp
{
    public partial class MainWindow : Window
    {
        private string freeFileSyncPath;
        private bool isEditMode = false;
        private static IntPtr handle;
        static class NativeMethods
        {
            [DllImport("kernel32.dll")]
            static public extern IntPtr GetConsoleWindow();

            [DllImport("user32.dll")]
            static public extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

            public const int SW_HIDE = 0;
            public const int SW_SHOW = 5;
        }

        public MainWindow()
        {
            var handle = NativeMethods.GetConsoleWindow();

            NativeMethods.ShowWindow(handle, NativeMethods.SW_HIDE);

            InitializeComponent();
            Thread.Sleep(2000);
            AskForFreeFileSyncPath();

        }

        private void AskForFreeFileSyncPath()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "FreeFileSync Executable|FreeFileSync.exe",
                Title = "Выберите файл FreeFileSync.exe"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                freeFileSyncPath = openFileDialog.FileName;
                PathTextBox.Text = freeFileSyncPath;
            }
            else
            {
                MessageBox.Show("Необходимо выбрать файл FreeFileSync.exe", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                AskForFreeFileSyncPath();
            }
        }

        private void ChangePathButton_Click(object sender, RoutedEventArgs e)
        {
            if (isEditMode)
            {
                // Сохранение измененного пути
                freeFileSyncPath = PathTextBox.Text;
                PathTextBox.IsEnabled = false;
                ChangePathButton.Content = "Изменить";
                isEditMode = false;
            }
            else
            {
                // Включение режима редактирования пути
                PathTextBox.IsEnabled = true;
                ChangePathButton.Content = "Сохранить";
                isEditMode = true;
            }
        }
        private static bool isConsoleVisible = true;

        private void Show_Console(object sender, RoutedEventArgs e)
        {
            var handle = NativeMethods.GetConsoleWindow();

            if (isConsoleVisible)
            {
                NativeMethods.ShowWindow(handle, NativeMethods.SW_HIDE);
                isConsoleVisible = false;
                ShowConsoleButton.Content = "Показать консоль";
            }
            else
            {
                NativeMethods.ShowWindow(handle, NativeMethods.SW_SHOW);
                isConsoleVisible = true;
                ShowConsoleButton.Content = "Скрыть консоль";
            }
        }
        private async void CheckPathButton_Click(object sender, RoutedEventArgs e)
        {
            var handle = NativeMethods.GetConsoleWindow();
            if (!string.IsNullOrEmpty(freeFileSyncPath) && File.Exists(freeFileSyncPath))
            {
                string syncFolderPath = Path.Combine(Path.GetDirectoryName(freeFileSyncPath), "sync");
                string acfcfgFilePath = Path.Combine(Path.GetDirectoryName(freeFileSyncPath), "acfexc.cfg");
                string holdExeFilePath = Path.Combine(Path.GetDirectoryName(freeFileSyncPath), "hold.exe");

                List<string> missingFiles = new List<string>();

                if (!Directory.Exists(syncFolderPath))
                {
                    missingFiles.Add("Папка 'sync'");
                }

                if (!File.Exists(acfcfgFilePath))
                {
                    missingFiles.Add("Файл 'acfcfg.cfg'");
                }

                if (!File.Exists(holdExeFilePath))
                {
                    missingFiles.Add("Файл 'hold.exe'");
                }

                if (missingFiles.Count == 0)
                {
                    MessageBox.Show("Путь действителен.", "Проверка пути", MessageBoxButton.OK, MessageBoxImage.Information);
                    NativeMethods.ShowWindow(handle, NativeMethods.SW_SHOW);
                    await Program.FF(holdExeFilePath, acfcfgFilePath, freeFileSyncPath, TokenTextBox.Text);
                }
                else
                {
                    string missingFilesMessage = string.Join(", ", missingFiles);
                    string errorMessage = $"Неверный путь или отсутствуют следующие файлы и/или папки: {missingFilesMessage}";
                    MessageBox.Show(errorMessage, "Проверка пути", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Путь к файлу FreeFileSync.exe не задан.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AskForFreeFileSyncPath(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            AskForFreeFileSyncPath();
        }
    }
}