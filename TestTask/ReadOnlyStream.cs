using System;
using System.IO;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream
    {
        private Stream _localStream;
        private StreamReader _localStreamReader;

        ~ReadOnlyStream()
        {
            Dispose();
        }

        /// <summary>
        /// Конструктор класса. 
        /// Т.к. происходит прямая работа с файлом, необходимо 
        /// обеспечить ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с таковым!
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyStream(string fileFullPath)
        {
            try
            {
                _localStream = File.OpenRead(fileFullPath);
                _localStreamReader = new StreamReader(_localStream);
            }
            catch (Exception ex)
            {
                throw new Exception($"Can't open file {fileFullPath}", ex);
            }
        }
                
        /// <summary>
        /// Флаг окончания файла.
        /// </summary>
        public bool IsEof
        {
            get;
            private set;
        }

        /// <summary>
        /// Ф-ция чтения следующего символа из потока.
        /// Если произведена попытка прочитать символ после достижения конца файла, метод 
        /// должен бросать соответствующее исключение
        /// </summary>
        /// <returns>Считанный символ.</returns>
        public char ReadNextChar()
        {
                var ch = _localStreamReader.Read();
                if(ch == -1)
                {
                    IsEof = true;
                    return '\0';
                }
                return (char)ch;
        }

        /// <summary>
        /// Сбрасывает текущую позицию потока на начало.
        /// </summary>
        public void ResetPositionToStart()
        {
            if (_localStreamReader == null)
            {
                IsEof = true;
                return;
            }

            _localStream.Position = 0;
            IsEof = false;
        }

        /// <summary>
        /// Освобождает ресурсы, используемые экземпляром ReadOnlyStream
        /// </summary>
        public void Dispose()
        {
            _localStream.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
