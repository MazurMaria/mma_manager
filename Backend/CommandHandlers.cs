using mma_manager.Frontend;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace mma_manager.Backend
{
    /// <summary>
    /// Класс, содержащий обработчики команд пользователя
    /// Поиск подходящего обработчика выполняется в методе CommandParser
    /// </summary>
    public class CommandHandlers
    {
        /// <summary>
        /// При построении дерева каталогов храним построенное дерево в этой коллекции
        /// </summary>
        private static List<string> CatalogTreeCollection;

        /// <summary>
        /// Для запоминания номера последнего напечатанного из дерева каталогов элемента
        /// Для постраничного вывода дерева каталогов
        /// </summary>
        private static int? LastPrintedCatalogTreeIndex;

        private static char[] СommandSeparator;
        private static char[] ParameterSeparators;

        /// <summary>
        ///  Количество строк, выводимое на одной странице при печати дерева каталогов
        ///  Берется из файла настроек приложения
        /// </summary>
        private static int CatalogsTreeElementsPerPage;

        /// <summary>
        /// Статический конструктор данного класса 
        /// (вызывается автоматически при первом обращении к данному статическому классу)
        /// </summary>
        public CommandHandlers()
        {
            CatalogTreeCollection = new List<string>();

            LastPrintedCatalogTreeIndex = null;

            СommandSeparator = new char[] { ' ' };
            ParameterSeparators = new char[] { ' ', '"' };

            // Получаем из настроек количество элементов на одной странице
            // при выводе на экран дерева каталогов
            // Если он равен 0, то устанавливаем его как "десять элементов на странице"
            int elementsPerPage = Properties.Settings.Default.CatalogsTreeElementsPerPage;
            CatalogsTreeElementsPerPage = elementsPerPage == 0 ? 10 : elementsPerPage;
        }

        /// <summary>
        /// Выход из программы
        /// </summary>
        public static void ProgramExit()
        {
            // Сохранение настроек приложения
            Properties.Settings.Default.LastUserCatalog = Directory.GetCurrentDirectory();
            Properties.Settings.Default.Save();

            // Выход из программы
            Environment.Exit(0);
        }

        /// <summary>
        /// Рекурсивно обходит все файлы и подкаталоги в каталоге, и вычисляет суммарный размер каталога
        /// </summary>
        /// <param name="path">каталог, размер которого вычисляем</param>
        /// <returns></returns>
        private static long GetDirectorySize(string path)
        {
            try
            {
                if (string.IsNullOrEmpty(path))
                {
                    path = Directory.GetCurrentDirectory();
                }

                string[] files = Directory.GetFiles(path);

                long currentDirectorySize = 0;

                foreach (string file in files)
                {
                    FileInfo fileInfo = new FileInfo(file);
                    currentDirectorySize += fileInfo.Length;
                }

                var subDirectories = Directory.GetDirectories(path);

                long subDirSize = 0;

                foreach (string subDir in subDirectories)
                {
                    subDirSize += GetDirectorySize(subDir);
                }

                return currentDirectorySize + subDirSize;
            }
            catch (Exception e)
            {
                ScreenCommands.PrintErrorInSpecifiedScreenArea(screenArea: Screen.InfoArea, e);
                return 0;
            }
        }

        /// <summary>
        /// Вывод информации о файле 
        /// </summary>
        /// <param name="path">Путь к файлу или каталогу</param>
        public static void PrintInfo(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    FileInfo fileInfo = new FileInfo(path);

                    string creationTime = File.GetCreationTime(path).ToString().PadRight(25, ' ') + '|';

                    string lastWriteTime = File.GetLastWriteTime(path).ToString().PadRight(25, ' ') + '|';

                    string fileSize = fileInfo.Length.ToString().PadRight(25, ' ') + '|';

                    Screen.InfoArea.ClearAndSetCursorToTop();

                    Screen.InfoArea.WriteLine($"Информация о файле {fileInfo.FullName}:");

                    Screen.InfoArea.WriteLine($"********************************************************************************");

                    Screen.InfoArea.WriteLine($"     Размер, байт        |     Дата создания       |     Дата посл. записи   |");

                    Screen.InfoArea.WriteLine($"********************************************************************************");

                    Screen.InfoArea.WriteLine($"{fileSize}{creationTime}{lastWriteTime}");

                    Screen.InfoArea.WriteLine($"********************************************************************************");
                }
                else if (Directory.Exists(path))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(path);

                    string creationTime = directoryInfo.CreationTime.ToString().PadRight(25, ' ') + '|';

                    string lastWriteTime = directoryInfo.LastWriteTime.ToString().PadRight(25, ' ') + '|';

                    string directorySize = GetDirectorySize(path).ToString().PadRight(25, ' ') + '|';

                    Screen.InfoArea.ClearAndSetCursorToTop();

                    Screen.InfoArea.WriteLine($"Информация о каталоге {directoryInfo.FullName}:");

                    Screen.InfoArea.WriteLine($"********************************************************************************");

                    Screen.InfoArea.WriteLine($"     Размер, байт        |     Дата создания       |     Дата посл. записи   |");

                    Screen.InfoArea.WriteLine($"********************************************************************************");

                    Screen.InfoArea.WriteLine($"{directorySize}{creationTime}{lastWriteTime}");

                    Screen.InfoArea.WriteLine($"********************************************************************************");
                }
                else
                {
                    throw new FileNotFoundException("Не существует указанного файла или каталога.");
                }
            }

            catch (Exception e)
            {
                ScreenCommands.PrintErrorInSpecifiedScreenArea(screenArea: Screen.InfoArea, e);
            }
        }

        /// <summary>
        /// Удаление файла или рекурсивное удаление каталога 
        /// </summary>
        /// <param name="source">Что удаляем</param>
        public static void DeleteCatalogOrFile(string source, bool isCallingByUser = true)
        {
            try
            {
                if (isCallingByUser)
                {
                    Screen.InfoArea.ClearAndSetCursorToTop();
                }

                // Если пользователь ввел имя каталога, то удаляем каталог рекурсивно (со всеми подкаталогами)
                if (Directory.Exists(source))
                {
                    string[] subDirectories = Directory.GetDirectories(source);

                    string[] files = Directory.GetFiles(source);

                    foreach (string file in files)
                    {
                        File.Delete(file);
                    }

                    foreach (string subDirectory in subDirectories)
                    {
                        DeleteCatalogOrFile(subDirectory);
                    }

                    if (Directory.GetDirectories(source).Length == 0
                        && Directory.GetFiles(source).Length == 0)
                    {
                        Directory.Delete(source);
                        Screen.InfoArea.WriteLine(message: $"{source} => удален.");
                    }
                }
                // Если пользователь ввел имя файла, то удаляем файл
                else if (File.Exists(source))
                {
                    File.Delete(source);
                    Screen.InfoArea.WriteLine(message: "Выполнено.");
                }
                else
                {
                    Screen.UserInputArea.WriteLine(message: "Указанного файла или каталога не существует.");
                }
            }
            catch (Exception e)
            {
                ScreenCommands.PrintErrorInSpecifiedScreenArea(screenArea: Screen.InfoArea, e);
            }
        }

        /// <summary>
        /// Рекурсивное копирование каталога 
        /// </summary>
        /// <param name="source">Что копируем</param>
        /// <param name="target">Куда копируем</param>
        /// <param name="overwriteAll">Если пользователь выберет "заменить все"</param>
        public static void CopyCatalogOrFile(string source, string target, bool isCallingByUser = true)
        {
            try
            {
                if (isCallingByUser)
                {
                    Screen.InfoArea.ClearAndSetCursorToTop();
                }
                // Если пользователь ввел имя каталога, то копируем каталог рекурсивно (со всеми подкаталогами)
                if (Directory.Exists(source))
                {
                    Directory.CreateDirectory(target);

                    string[] files = Directory.GetFiles(source);

                    foreach (string file in files)
                    {
                        string fileName = Path.GetFileName(file);
                        string destinationFileName = Path.Combine(target, fileName);
                        File.Copy(file, destinationFileName, true);
                        Screen.InfoArea.WriteLine(message: $"{file} => {destinationFileName}");
                    }

                    // Проходим все подкаталоги текущего каталога,
                    // для каждого из них выполняем рекурсивный вызов для копирования подкаталога и его файлов
                    string[] subDirectories = Directory.GetDirectories(source);

                    foreach (string subDirectory in subDirectories)
                    {
                        string subDirectoryName = new DirectoryInfo(subDirectory).Name;

                        string newSource = Path.Combine(source, subDirectoryName);

                        string newTarget = Path.Combine(target, subDirectoryName);

                        CopyCatalogOrFile(newSource, newTarget, false);
                    }
                }
                // Если пользователь ввел имя файла, то копируем файл
                else if (File.Exists(source))
                {
                    bool overwriteTarget = false;
                    if (File.Exists(target))
                    {
                        Screen.UserInputArea.WriteLine(
                            message: "Такой файл уже существует. Чтобы заменить файл, нажмите 'y'."
                                     + " Чтобы пропустить операцию, нажмите любую клавишу.");

                        overwriteTarget = Console.ReadKey().KeyChar == 'y';
                    }
                    File.Copy(source, target, overwriteTarget);
                    Screen.UserInputArea.WriteLine(message: "Выполнено.");
                }
                else
                {
                    Screen.UserInputArea.WriteLine(message: "Указанного файла или каталога не существует.");
                }
            }
            catch (Exception e)
            {
                ScreenCommands.PrintErrorInSpecifiedScreenArea(screenArea: Screen.InfoArea, e);
            }
        }

        /// <summary>
        /// Построение дерева каталогов, рекурсивно
        /// Строки, находимые этим методом, заносятся в коллекцию
        /// </summary>
        /// <param name="dir">Путь к каталогу, "родитель" которого ищется в настоящий момент</param>
        /// <param name="isCallingByUser">Признак того, что метод вызван пользователем, по умолчанию true</param>
        private static void BuildCatalogsTree(string path = null, int currentLevel = 0, bool isCallingByUser = true)
        {
            DirectoryInfo dir;

            try
            {
                // Проверку на существование каталога делаем только первый раз,
                // когда процедура вызывается пользователем.
                // Также только в этом случае очищаем коллекцию, хранящую дерево каталогов.
                // В последующих вызовах (из самой себя) эта проверка не нужна
                if (isCallingByUser)
                {
                    // Если путь не передан, то начнем вывод дерева c текущего каталога пользователя
                    if (string.IsNullOrEmpty(path))
                    {
                        path = Directory.GetCurrentDirectory();
                    }

                    else
                    {
                        CheckCatalogOrFileIsExists(path);
                    }

                    // Очищаем коллекцию для хранения дерева каталогов и номер последнего напечатанного элемента.
                    CatalogTreeCollection.Clear();
                    LastPrintedCatalogTreeIndex = null;
                }

                dir = new DirectoryInfo(path);

                // Формируем линии для отображения дерева
                string indentation = string.Empty;

                for (var i = 0; i < currentLevel; i++)
                {
                    indentation += "\t";
                }

                indentation += "|___";

                // Сохраняем построенную строку в коллекцию,
                // для последующего постраничного вывода на экран
                CatalogTreeCollection.Add($"{indentation}{dir.Name}");

                // Проходим все подкатологи текущего каталога,
                // для каждого из них выполняем рекурсивный вызов для построения следующего уровня дерева

                DirectoryInfo[] subDirectories = dir.GetDirectories();

                int nextLevel = currentLevel + 1;

                foreach (DirectoryInfo subDirectory in subDirectories)
                {
                    BuildCatalogsTree(path: subDirectory.FullName, currentLevel: nextLevel, isCallingByUser: false);
                }
            }
            catch (Exception e)
            {
                ScreenCommands.PrintErrorInSpecifiedScreenArea(screenArea: Screen.InfoArea, e);
            }
        }

        /// <summary>
        /// Печать помощи
        /// </summary>
        public static void PrintHelp()
        {
            Screen.InfoArea.ClearAndSetCursorToTop();

            Screen.InfoArea.WriteLine("** Имена каталогов и файлов в параметрах команд лучше указывать в кавычках. **");
            Screen.InfoArea.WriteLine();

            int commandNumber = 1;

            foreach (Command userCommand in Commands.AllCommands)
            {
                Screen.InfoArea.WriteLine($"{commandNumber++}. {userCommand.Alias}: {userCommand.Description}");
            }
        }

        /// <summary>
        /// Проверка существования каталога или файла
        /// Если каталога или файла не существует, то выбросить ошибку
        /// </summary>
        private static void CheckCatalogOrFileIsExists(string path)
        {
            if (Directory.Exists(path) == false && File.Exists(path) == false)
            {
                throw new FileNotFoundException("Не существует такого каталога или файла.");
            }
        }

        /// <summary>
        /// Обработчик команды смены рабочего каталога
        /// </summary>
        /// <param name="path"></param>
        public static void ChangeWorkDirectory(string newWorkCatalogPath)
        {
            try
            {
                if (string.IsNullOrEmpty(newWorkCatalogPath) == false)
                {
                    CheckCatalogOrFileIsExists(newWorkCatalogPath);

                    Directory.SetCurrentDirectory(newWorkCatalogPath);
                }
            }
            catch (Exception e)
            {
                ScreenCommands.PrintErrorInSpecifiedScreenArea(screenArea: Screen.InfoArea, e);
            }
        }

        /// <summary>
        /// Постраничный вывод на экран построенного дерева каталогов
        /// </summary>
        private static void PrintCatalogTree()
        {
            // Очищаем область печати дерева каталогов
            Screen.CatalogsTreeArea.ClearAndSetCursorToTop();

            // Первый индекс для печати (последний отпечатанный + 1 или 0)
            int firsIndexForPrint = LastPrintedCatalogTreeIndex ?? 0;

            for (int i = firsIndexForPrint;
                i < (firsIndexForPrint + CatalogsTreeElementsPerPage)
                     && i < CatalogTreeCollection.Count
                ; i++)
            {
                Screen.CatalogsTreeArea.WriteLine(CatalogTreeCollection[i]);
                LastPrintedCatalogTreeIndex = i;
            }

            Screen.CatalogsTreeArea.WriteLine();

            if (LastPrintedCatalogTreeIndex >= CatalogTreeCollection.Count - 1)
            {
                Screen.CatalogsTreeArea.WriteLine("Последняя страница.");
            }
            else
            {
                Screen.CatalogsTreeArea.WriteLine("Для вывода следующей страницы нажмите пробел.");

                Screen.CatalogsTreeArea.WriteLine("Для завершения вывода и перехода к вводу команд нажмите любую клавишу.");

                if (Console.ReadKey().Key == ConsoleKey.Spacebar)
                {
                    PrintCatalogTree();
                }
            }
        }

        /// <summary>
        /// Последовательность действий для построения и вывода на экран дерева каталогов
        /// </summary>
        public static void BuildAndPrintCatalogTree(string path = "")
        {

            // Строим дерево каталогов и сохраняем его в коллекцию CatalogTreeCollection
            BuildCatalogsTree(path);

            // Выводим построенное дерево каталогов на печать, постранично
            PrintCatalogTree();
        }

        /// <summary>
        /// Обработка ввода пользователя. 
        /// При успешной обработке вызывается необходимая команда.
        /// При ошибке создастся исключение UnrecognizedUserCommandException
        /// После завершения, возвращается к ожиданию ввода пользователя
        /// </summary>
        /// <param name="userInput">Строка, введенная пользователем.</param>
        private void CommandParser(string userInput)
        {
            try
            {
                // Если введенная строка пуста, то возвращаемся к ожиданию ввода пользователя
                // (сработает блок finally)
                if (string.IsNullOrEmpty(userInput) || string.IsNullOrWhiteSpace(userInput))
                {
                    return;
                }

                // Обработка ввода пользователя. 

                // Первое разделение - отделяем команду пользователя от параметров
                string[] arrayCommandAndParameters = userInput.Split(СommandSeparator, 2
                                                      , StringSplitOptions.RemoveEmptyEntries);

                string possibleCommandAlias = arrayCommandAndParameters[0];

                // Если первый элемент полученного массива не содержит команду,
                // то сразу возвращаемся к ожиданию ввода 
                // (сработает блок finally)

                if (Commands.AllCommandAliases.Contains(possibleCommandAlias) == false)
                {
                    return;
                }

                // Определяем команду пользователя
                // Для этого в коллекции доступных команд ищем первую команду,
                // Alias которой равен предполагаемой команде пользователя
                Command command = Commands.AllCommands.First(comm => comm.Alias.ToLower() == possibleCommandAlias.ToLower());

                // Получаем имя метода, который должна выполнять данная команда пользователя, и вызываем его.
                // Использовала информацию отсюда:
                // https://metanit.com/sharp/tutorial/14.1.php
                // https://docs.microsoft.com/ru-ru/dotnet/api/system.reflection.methodbase.invoke?view=net-6.0

                // Получаем тип (собственно этот класс)
                Type commandHandlersType = this.GetType();

                // Получаем метод (обработчик команды пользователя) по его имени. 
                // Имя метода беру из свойства CommandHandlerName полученной команды
                MethodInfo commandHandler = commandHandlersType.GetMethod(command.CommandHandlerName);

                // Получаю параметры найденного метода-обработчика
                ParameterInfo[] commandHandlerParameters = commandHandler.GetParameters();

                List<object> commandParameters = null;

                // Если найденный метод требует ввода параметров,
                // то пробуем найти их в вводе пользователя.
                // Они содержатся во второй части массива arrayCommandAndParameters
                if (commandHandlerParameters.Length > 0)
                {
                    // инициализируем массив параметров для передачи в метод
                    commandParameters = new List<object>();

                    //// Если массив состоит только из одной части, то пользователь не ввел параметры,
                    //// и надо попробовать сформировать параметры по умолчанию для данного метода-обработчика
                    if (arrayCommandAndParameters.Length == 1)
                    {
                        foreach (ParameterInfo param in commandHandlerParameters)
                        {
                            commandParameters.Add(param.DefaultValue);
                        }
                    }
                    else
                    {
                        // Пробуем разобрать вторую часть массива на параметры
                        // Пользователь мог ввести их с кавычками и без кавычек.
                        List<string> arrayParameters = new List<string>();

                        // Заменяем кавычки и пробел между ними на звездочку
                        string paramsString = arrayCommandAndParameters[1].Replace("\" \"", "*");

                        // Если звездочки нет, но строка содержит кавычки,
                        // то возможно параметр у нас один, или они введены неверно.
                        if (paramsString.Contains('*') == false && paramsString.Contains('"'))
                        {
                            arrayParameters.Add(paramsString);
                        }
                        else if (paramsString.Contains('*') == false && paramsString.Contains('"') == false)
                        {
                            arrayParameters.AddRange(paramsString.Split(' ').ToList());
                        }

                        // Если звездочка есть, то разделяем строку по ней на отдельные параметры,
                        // в каждом из них убираем кавычки, и добавляем этот массив для передачи в метод-обработчик
                        else
                        {
                            arrayParameters.AddRange(paramsString.Split('*').ToList());
                        }

                        // Убираем кавычки из найденных параметров
                        List<string> tempArrayParameters = new List<string>();
                        foreach (string param in arrayParameters)
                        {
                            tempArrayParameters.Add(param.Replace("\"", ""));
                        }

                        arrayParameters = tempArrayParameters;

                        // Добавляем найденные параметры для передачи в метод-обработчик
                        commandParameters.AddRange(arrayParameters);

                        // Если число параметров пользователя меньше, чем нужно в процедуре, 
                        // то пробуем заполнить оставшиеся значениями по умолчанию
                        for (int i = 0; i < commandHandlerParameters.Length; i++)
                        {
                            if (i > arrayParameters.Count - 1)
                            {
                                commandParameters.Add(commandHandlerParameters[i].DefaultValue);
                            }
                        }
                    }
                }

                // Вызываем найденный метод, с передачей найденных параметров
                // (или null если они ему не требуются)
                commandHandler.Invoke(this, commandParameters?.ToArray());

            }
            catch (Exception e)
            {
                ScreenCommands.PrintErrorInSpecifiedScreenArea(screenArea: Screen.InfoArea, e);
            }
            finally
            {
                // Продолжаем ожидать ввод пользователя.
                WaitForUserInput();
            }
        }

        /// <summary>
        /// Очищение консоли и перерисовка всех областей
        /// </summary>
        public static void ClearScreen()
        {
            ScreenCommands.Clear();
        }

        /// <summary>
        /// Разбор ввода пользователя
        /// </summary>
        public void WaitForUserInput()
        {
            // Печать текущего каталога с добавлением ">" в конце строки (строка приглашения)
            Screen.UserInputArea.Write(message: Directory.GetCurrentDirectory() + ">");

            // Получаем ввод пользователя (приводя его к виду lowercase)
            string userInput = Console.ReadLine().ToLower();

            CommandParser(userInput);
        }
    }
}
