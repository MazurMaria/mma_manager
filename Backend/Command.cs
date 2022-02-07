namespace mma_manager.Backend
{
    /// <summary>
    /// Класс, описывающий пользовательскую команду
    /// </summary>
    public class Command
    {
        public Command(string alias, string description, string commandHandlerName)
        {
            Alias = alias;
            Description = description;
            CommandHandlerName = commandHandlerName;
        }

        /// <summary>
        /// "Псевдоним" команды.
        /// Ввод, ожидаемый от пользователя для исполнения этой команды.
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Описание команды
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Наименование метода, отвечающего за обработку этой команды
        /// Используются делегаты
        /// </summary>
        public string CommandHandlerName { get; set; }
    }
}
