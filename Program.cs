// Вывод дерева файловой системы с условием “пейджинга”
// ls C:\Source - p 2
// Копирование каталога
// cp C:\Source D:\Target
// Копирование файла
// cp C:\source.txt D:\target.txt
// Удаление каталога рекурсивно
// rm C:\Source
// Удаление файла
// rm C:\source.txt
// Вывод информации
// file C:\source.txt

using mma_manager.Backend;
using mma_manager.Frontend;
using System.IO;

namespace mma_manager
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Получаем записанный в настройках каталог пользователя, сохраненный при предыдущем запуске программы
            string lastUserCatalog = Properties.Settings.Default.LastUserCatalog;

            // Если он не пуст, то устанавливаем текущий каталог в полученный из настроек, и продолжаем работу
            if (string.IsNullOrEmpty(lastUserCatalog) == false)
            {
                Directory.SetCurrentDirectory(lastUserCatalog);
            }

            // Отрисовка всех областей окна консоли
            ScreenCommands.DrawAllAreas();

            // Ожидание пользовательского ввода 
            CommandHandlers commandHandlers = new CommandHandlers();
            commandHandlers.WaitForUserInput();
        }
    }
}
