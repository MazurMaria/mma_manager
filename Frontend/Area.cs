using System;
using System.Collections.Generic;

namespace mma_manager.Frontend
{

    /// <summary>
    /// Класс, описывающий области окна консоли, использующийся для отображения какой-либо информации.
    /// </summary>
    public class Area
    {
        /// <summary>
        /// Конструктор класса, описывающего область печати консоли
        /// </summary>
        /// <param name="areaTop">Верхняя граница области</param>
        /// <param name="areaBottom">Нижняя граница области</param>
        /// <param name="rowsCollection"> Если коллекция строк для печати в данной области. Если не передана, то создастся пустая коллекция.</param>
        /// <param name="needTopBorder">Необходимо ли печатать верхнюю границу области</param>
        /// <param name="needBottomBorder">Необходимо ли печатать нижнюю границу области</param>
        public Area(int areaTop, int areaBottom, bool needTopBorder,
                          bool needBottomBorder, string headerMessage = "",
                          List<string> rowsCollection = null,
                          char borderTopDelimiterSymbol = '-', char borderBottomDelimiterSymbol = '-')
        {
            BorderTop = areaTop;
            NeedTopBorder = needTopBorder;
            BorderBottom = areaBottom;
            NeedBottomBorder = needBottomBorder;
            BorderTopDelimiterSymbol = borderTopDelimiterSymbol;
            BorderBottomDelimiterSymbol = borderBottomDelimiterSymbol;
            RowsCollection = rowsCollection ?? new List<string>();
            HeaderMessage = headerMessage;

            // Запоминаем начальную позицию курсора
            LastCursorLeftPosition = 0;
            LastCursorTopPosition = GetFirstRowNumber();
        }

        /// <summary>
        /// Последняя позиция курсора в данной области экрана (левая координата)
        /// Запоминается при покидании области, 
        /// при возвращении в область курсор устанавливается в запомненную позицию
        /// </summary>
        public int LastCursorLeftPosition { get; set; }

        /// <summary>
        /// Последняя позиция курсора в данной области экрана (левая координата)
        /// Запоминается при покидании области, 
        /// при возвращении в область курсор устанавливается в запомненную позицию
        /// </summary>
        public int LastCursorTopPosition { get; set; }

        /// <summary>
        /// Верхняя координата границы области окна консоли
        /// </summary>
        public int BorderTop { get; set; }

        /// <summary>
        /// Символ для печати верхней границы
        /// </summary>
        public char BorderTopDelimiterSymbol { get; set; }

        /// <summary>
        /// Необходимо ли отрисовывать верхнюю границу
        /// </summary>
        public bool NeedTopBorder { get; set; }

        /// <summary>
        /// Нижняя координата области окна консоли
        /// </summary>
        public int BorderBottom { get; set; }

        /// <summary>
        /// Символ для печати нижней границы
        /// </summary>
        public char BorderBottomDelimiterSymbol { get; set; }

        /// <summary>
        /// Необходимо ли отрисовывать нижнюю границу
        /// </summary>
        public bool NeedBottomBorder { get; set; }

        /// <summary>
        /// Сообщение, выводимое в шапке окна
        /// </summary>
        public string HeaderMessage { get; set; }

        /// <summary>
        /// Коллекция строк для вывода в данной области окна
        /// </summary>
        public List<string> RowsCollection { get; set; }

        /// <summary>
        /// Отрисовка границ области, и печать шапки окна, если нужно
        /// </summary>
        public void Draw()
        {
            string delimiterString;
            if (NeedTopBorder)
            {
                // Строим строку для отрисовки (заполняем пустую строку символами-разделителями по ширине окна)
                delimiterString = string.Empty.PadRight(Console.WindowWidth, BorderTopDelimiterSymbol);

                // Устанавливаем курсор в требуемую для отрисовки позицию
                Console.SetCursorPosition(0, BorderTop);

                Console.WriteLine(delimiterString);

                if (string.IsNullOrEmpty(HeaderMessage))
                {
                    Console.WriteLine(HeaderMessage);
                }
            }
            if (NeedBottomBorder)
            {
                // Строим строку для отрисовки (заполняем пустую строку символами-разделителями по ширине окна)
                delimiterString = string.Empty.PadRight(Console.WindowWidth, BorderBottomDelimiterSymbol);

                Console.SetCursorPosition(0, BorderBottom);

                Console.Write(delimiterString);
            }
            if (string.IsNullOrEmpty(HeaderMessage) == false)
            {
                int headerRowNumber = BorderTop + 1;

                if (BorderTop == 0 && NeedTopBorder == false)
                {
                    headerRowNumber = 0;
                }

                Console.SetCursorPosition(0, headerRowNumber);

                Console.Write(HeaderMessage);
            }

            SetCursorToTop();
        }

        /// <summary>
        /// Очищение области от напечатанных строк
        /// Устанавливаем курсор построчно от верхней границы области до нижней,
        /// И затираем эти строки пробелами по всей ширине
        /// Потом устанавливаем курсор в начало области для печати
        /// </summary>
        public void ClearAndSetCursorToTop()
        {
            // Формируем строку пробелов, равную ширине экрана.
            string CleanString = string.Empty.PadLeft(Console.BufferWidth, ' ');

            // Устанавливаем курсор в каждую строку области печати (для данной области),
            // и стираем написанное на этой строке с помощью строки пробелов
            for (int rowNumber = GetFirstRowNumber();
                rowNumber <= GetLastRowNumber(); rowNumber++)
            {
                Console.SetCursorPosition(0, rowNumber);
                Console.Write(CleanString);
            }

            // Установить позицию курсора в начало области печати
            SetCursorToTop();

        }

        /// <summary>
        /// Получить номер первой строки области, доступной для печати 
        /// Устанавливается на 1 больше верхней границы области, если шапка области пуста,
        /// И на 2 больше верхней границы, если шапка области не пуста
        /// </summary>
        public int GetFirstRowNumber()
        {
            if (GetMaxRowsNumber() == 1)
            {
                return BorderTop;
            }

            if (string.IsNullOrEmpty(HeaderMessage))
            {
                return BorderTop + 1;
            }

            return BorderTop + 2;
        }

        /// <summary>
        /// Получить номер последней строки области, доступной для печати 
        /// Устанавливается на 1 меньше верхней границы области
        /// </summary>
        public int GetLastRowNumber()
        {
            return BorderBottom - 1;
        }

        /// <summary>
        /// Получить максимальное число строк, доступное для записи в данной области окна
        /// </summary>
        public int GetMaxRowsNumber()
        {
            return BorderBottom - BorderTop - 1;
        }

        /// <summary>
        /// Метод, сохраняющий последнюю позицию курсора в данной области
        /// Если позиция курсора не передана, то запоминается текущая позиция курсора
        /// </summary>
        public void SaveCursorPosition()
        {
            LastCursorLeftPosition = Console.CursorLeft;
            LastCursorTopPosition = Console.CursorTop;
        }

        /// <summary>
        /// Метод, сохраняющий последнюю позицию курсора в данной области
        /// Если позиция курсора передана, то запоминается она
        /// </summary>
        public void SaveCursorPosition(int lastCursorLeftPosition, int lastCursorTopPosition)
        {
            LastCursorLeftPosition = lastCursorLeftPosition;
            LastCursorTopPosition = lastCursorTopPosition;
        }

        /// <summary>
        /// Метод, устанавливающий курсор в верхнюю позицию в данной области для печати
        /// и сохраняющий позицию курсора
        /// </summary>
        public void SetCursorToTop()
        {
            Console.SetCursorPosition(0, GetFirstRowNumber());

            // Сохраняем позицию курсора после установки
            SaveCursorPosition();
        }

        /// <summary>
        /// Метод, устанавливающий курсор в ранее запомненную позицию
        /// </summary>
        public void SetCursorToLastPosition()
        {
            Console.SetCursorPosition(LastCursorLeftPosition, LastCursorTopPosition);
        }

        /// <summary>
        /// Метод, проверяющий, находится ли курсор в данной области печати (не включая границы)
        /// </summary>
        /// <returns>true если курсор в данной области и false если нет</returns>
        public bool IsActiveArea()
        {
            return Console.CursorTop > BorderTop
                && Console.CursorTop < BorderBottom;
        }

        /// <summary>
        /// Проверка переполнения области (перед печатью строки)
        /// </summary>
        private void CheckOverflow()
        {
            // Устанавливаем позицию курсора в данной области в запомненную ранее
            SetCursorToLastPosition();

            // Если подошли к границе области печати, то очищаем область печати
            if (Console.CursorTop >= GetLastRowNumber())
            {
                // Очищаем область печати
                ClearAndSetCursorToTop();
            }
        }

        /// <summary>
        /// Печать строки в данной области с переводом строки на следующую
        /// Отпечатанная строка сохраняется в буфере данной области
        /// Позиция курсора в данной области сохраняется после печати
        /// </summary>
        public void WriteLine(string message = null)
        {
            CheckOverflow();

            Console.WriteLine(message);

            // Сохраняем позицию курсора в данной области
            SaveCursorPosition();

            // Сохраняем напечатанную строку в буфере области
            RowsCollection.Add(message);
        }

        /// <summary>
        /// Печать строки в данной области без перевода строки на следующую 
        /// (например, для печати в области ввода пользователя - с приглашением)
        /// Отпечатанная строка сохраняется в буфере данной области
        /// Позиция курсора в данной области сохраняется после печати
        /// </summary>
        public void Write(string message = null)
        {
            CheckOverflow();

            Console.Write(message);

            // Сохраняем позицию курсора в данной области, как будто строка перенесена
            // так организуем, например, ввод пользователя
            // (для печати текущего каталога перед ожиданием команды пользователя)
            SaveCursorPosition(0, LastCursorTopPosition + 1);

            // Сохраняем напечатанную строку в буфере области
            RowsCollection.Add(message);
        }
    }
}
