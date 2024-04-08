using System.Windows;
using System.Windows.Controls;

namespace FreeFileSyncApp.Views
{
    public partial class GameListPage : Page
    {
        public GameListPage()
        {
            InitializeComponent();

            // Загрузить список игр из базы данных или другого источника данных
          
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // Обработка события нажатия кнопки "Добавить"
            // Здесь можно добавить логику для перехода на страницу добавления новой игры или отображения модального диалога добавления новой игры.
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            // Обработка события нажатия кнопки "Изменить"
            // Здесь можно добавить логику для перехода на страницу изменения игры или отображения модального диалога изменения игры.
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Обработка события нажатия кнопки "Удалить"
            // Здесь можно добавить логику для удаления выбранной игры из списка игр.
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            // Обработка события нажатия кнопки "Начать обновление"
            // Здесь можно добавить логику для начала процесса обновления игр.
        }
    }
}