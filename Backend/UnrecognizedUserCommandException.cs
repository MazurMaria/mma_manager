using System;

namespace mma_manager.Backend
{
    /// <summary>
    /// Класс, содержащий ошибку, возникающую при нераспознанном вводе пользователя
    /// https://docs.microsoft.com/ru-ru/dotnet/standard/exceptions/how-to-create-localized-exception-messages
    /// </summary>
    [Serializable]
    public class UnrecognizedUserCommandException : Exception
    {

        public UnrecognizedUserCommandException() { }

        public UnrecognizedUserCommandException(string message)
            : base(message) { }

        public UnrecognizedUserCommandException(string message, Exception inner)
            : base(message, inner) { }

    }
}
