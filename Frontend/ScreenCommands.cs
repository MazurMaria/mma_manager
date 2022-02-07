using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace mma_manager.Frontend
{
    /// <summary>
    /// Класс для работы с отрисовкой и заполнением буферов экрана
    /// </summary>
    public static class ScreenCommands
    {
        // ************************************************************************************************************
        // Импорт библиотек для разворачивания окна консоли в полный экран и отображения окна по центру экрана
        // https://www.c-sharpcorner.com/code/448/code-to-auto-maximize-console-application-according-to-screen-width-in-c-sharp.aspx
        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        private const int MAXIMIZE = 3;
        // ************************************************************************************************************

        /// <summary>
        /// Коллекция, хранящая характеристики всех доступных областей экрана консоли
        /// </summary>
        public static List<Area> ScreenAreasCollection;

        /// <summary>
        /// Статический конструктор данного класса работы с отрисовкой областей экрана
        /// (вызывается автоматически при первом обращении к данному статическому классу)
        /// </summary>
        static ScreenCommands()
        {
            // Увеличиваем окно консоли на полный экран и центрируем его
            SetConsoleWindowToFullScreen();

            // Помещаем все созданные в классе Screen области в коллекцию
            ScreenAreasCollection = new List<Area>
            {
                Screen.HeaderArea,
                Screen.CatalogsTreeArea,
                Screen.InfoArea,
                Screen.UserInputArea
            };
        }

        /// <summary>
        /// Разворачивает окно консоли в полный экран и центрирует его
        /// </summary>
        /// <param name="hideRolls">Требуется ли скрывать полосы прокрутки, по умолчанию true</param>
        private static void SetConsoleWindowToFullScreen(bool hideRolls = true)
        {
            // Разворачиваем окно консоли по размеру экрана
            Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);

            // Если требуется убрать полосы прокрутки, 
            // то устанавливаем размер буфера консоли равным размеру окна
            if (hideRolls)
            {
                Console.SetBufferSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
            }

            // функция из системной библиотеки "kernel32.dll"
            // возвращает указатель на активное окно консоли
            IntPtr trhisConsole = GetConsoleWindow();

            // Функция из системной библиотеки "user32.dll", 
            // отображает окно по его указателю с параметром "развернуть на весь экран (MAXIMIZE = 3)"
            ShowWindow(trhisConsole, MAXIMIZE);

        }

        /// <summary>
        /// Отрисовка всех областей окна консоли
        /// </summary>
        public static void DrawAllAreas()
        {
            foreach (Area area in ScreenAreasCollection)
            {
                area.Draw();
            }
        }

        /// <summary>
        /// Очищение консоли и перерисовка всех областей
        /// </summary>
        public static void Clear()
        {
            // Очистка всех буферов строк 
            foreach (var area in ScreenAreasCollection)
            {
                area.RowsCollection.Clear();
            }

            Console.Clear();

            DrawAllAreas();
        }

        /// <summary>
        /// Вывод ошибок в определенной области экрана
        /// </summary>
        public static void PrintErrorInSpecifiedScreenArea(Area screenArea, Exception error)
        {
            screenArea.WriteLine(message: $"Ошибка: {error.Message} {error.InnerException?.Message}");
        }
    }
}
