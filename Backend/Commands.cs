using System.Collections.Generic;

namespace mma_manager.Backend
{
    /// <summary>
    /// Статический класс, содержит доступные пользавателю команды
    /// </summary>
    public static class Commands
    {
        /// Доступные пользователю команды
        public static readonly Command ChangeWorkDirectory;
        public static readonly Command List;
        public static readonly Command Copy;
        public static readonly Command Remove;
        public static readonly Command Info;
        public static readonly Command Help;
        public static readonly Command Clear;
        public static readonly Command Exit;

        /// <summary>
        /// Доступные пользовательские команды.
        /// </summary>
        public static readonly List<Command> AllCommands;

        /// <summary>
        /// Все псевдонимы команд, для более удобного поиска подходящей команды
        /// </summary>
        public static readonly List<string> AllCommandAliases;


        static Commands()
        {
            /// Доступные пользователю команды
            /// Псевдонимы команд (доступные для ввода пользователя) 
            /// специально выбраны не совпадающими с командами консоли Windows
            /// Если нужно изменить псевдоним команды, то достаточно поменять его в этом классе.

            ChangeWorkDirectory = new Command
            (
                alias: "cdc",
                description: " (catalog name) - Смена текущего каталога. Параметр обязательный.",
                commandHandlerName: "ChangeWorkDirectory"
            );

            List = new Command
            (
                alias: "lst",
                description: " [catalog] - Вывод дерева каталогов. Параметр необязателен."
                                      + " Если исходный каталог не указан, то выводится дерево текущего рабочего каталога",
                commandHandlerName: "BuildAndPrintCatalogTree"
            );

            Copy = new Command
            (
                alias: "cop",
                description: " (source) (destination) - Копирование каталога или файла. Параметры обязательны.",
                commandHandlerName: "CopyCatalogOrFile"
            );

            Remove = new Command
            (
                alias: "rmc",
                description: " (file or catalog name) - Удаление файла или рекурсивное удаление каталога. Параметр обязателен.",
                commandHandlerName: "DeleteCatalogOrFile"
            );

            Info = new Command
            (
                alias: "info",
                description: " (file or folder name) - Вывод информации о файле или каталоге. Параметр обязателен.",
                commandHandlerName: "PrintInfo"
            );

            Help = new Command
            (
                alias: "help",
                description: "Вывод справки",
                commandHandlerName: "PrintHelp"
            );

            Clear = new Command
            (
                alias: "clc",
                description: "Очистить все области экрана",
                commandHandlerName: "ClearScreen"
            );

            Exit = new Command
            (
                alias: "exit",
                description: "Выйти из программы",
                commandHandlerName: "ProgramExit"
            );

            // Заполняем список доступных пользователю команд
            AllCommands = new List<Command>
            {
                ChangeWorkDirectory,
                List,
                Copy,
                Remove,
                Info,
                Help,
                Clear,
                Exit
            };

            AllCommandAliases = new List<string>();

            foreach (Command command in AllCommands)
            {
                AllCommandAliases.Add(command.Alias);
            }
        }
    }
}
