namespace TestTask
{
    /// <summary>
    /// Статистика вхождения буквы/пары букв
    /// </summary>
    public class LetterStats
    {
        /// <summary>
        /// Буква/Пара букв для учёта статистики.
        /// </summary>
        public string Letter;

        /// <summary>
        /// Кол-во вхождений буквы/пары.
        /// </summary>
        public int Count;

        /// <summary>
        /// Создает экземпляр LetterStats.
        /// </summary>
        /// <param name="letter">Буква/Пара букв для учёта статистики.</param>
        /// <param name="count">Кол-во вхождений буквы/пары.</param>
        public LetterStats(string Letter, int Count = 0){
            this.Letter = Letter;
            this.Count = Count;
        }
    }
}
