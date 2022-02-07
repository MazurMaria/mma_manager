using mma_manager.Backend;
using System;
using System.Collections.Generic;

namespace mma_manager.Frontend
{
    /// <summary>
    /// Класс для определения всех доступных областей экрана
    /// </summary>
    public static class Screen
    {
        /// <summary>
        /// Статический конструктор (Определение характеристик областей экрана)
        /// </summary>
        static Screen()
        {
            // Определяем области экрана
            int delimiterPosition_1 = 0;
            int delimiterPosition_2 = 1;
            int delimiterPosition_3 = Console.WindowHeight - Console.WindowHeight / 4 - 5;
            int delimiterPosition_4 = Console.WindowHeight - Console.WindowHeight / 8;
            int delimiterPosition_5 = Console.WindowHeight - 1;

            HeaderArea = new Area(areaTop: delimiterPosition_1, areaBottom: delimiterPosition_2,
                                        needTopBorder: false, needBottomBorder: true,
                                        headerMessage: $"Для вывода подсказки наберите {Commands.Help.Alias}");

            CatalogsTreeArea = new Area(areaTop: delimiterPosition_2, areaBottom: delimiterPosition_3,
                                              needTopBorder: false, needBottomBorder: false);

            InfoArea = new Area(areaTop: delimiterPosition_3, areaBottom: delimiterPosition_4,
                                      needTopBorder: true, needBottomBorder: true);

            UserInputArea = new Area(areaTop: delimiterPosition_4, areaBottom: delimiterPosition_5,
                                           needTopBorder: true, needBottomBorder: false);

            AllScreenAreas = new List<Area>
            {
                HeaderArea, CatalogsTreeArea, InfoArea, UserInputArea
            };
        }

        /// <summary>
        /// Область "Шапки" окна консоли
        /// </summary>
        public static readonly Area HeaderArea;

        /// <summary>
        /// Область для вывода дерева каталогов
        /// </summary>
        public static readonly Area CatalogsTreeArea;

        /// <summary>
        /// Область для вывода информации о файле
        /// </summary>
        public static readonly Area InfoArea;

        /// <summary>
        /// Область для ввода команд пользователя
        /// </summary>
        public static readonly Area UserInputArea;

        /// <summary>
        /// Коллекция, содержащая все все определенные на экране области
        /// </summary>
        public static readonly List<Area> AllScreenAreas;
    }
}
