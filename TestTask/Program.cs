using System;

using System.Collections.Generic;

namespace TestTask
{
    
    public class Program
    {
        private static readonly HashSet<char> VOWELS = new HashSet<char>(
            "aeiouyAEIOUYауоиэыяюеёАУОИЭЫЯЮЕЁ".ToCharArray());

        /// <summary>
        /// Программа принимает на входе 2 пути до файлов.
        /// Анализирует в первом файле кол-во вхождений каждой буквы (регистрозависимо). Например А, б, Б, Г и т.д.
        /// Анализирует во втором файле кол-во вхождений парных букв (не регистрозависимо). Например АА, Оо, еЕ, тт и т.д.
        /// По окончанию работы - выводит данную статистику на экран.
        /// </summary>
        /// <param name="args">Первый параметр - путь до первого файла.
        /// Второй параметр - путь до второго файла.</param>
        static void Main(string[] args)
        {
            if(args.Length != 2) throw new ArgumentException("Invalid arguments count. Example: TestTask.exe file1.txt file2.txt");

            using(IReadOnlyStream inputStream1 = GetInputStream(args[0]))
            {
                using(IReadOnlyStream inputStream2 = GetInputStream(args[1]))
                {
                    IList<LetterStats> singleLetterStats = FillSingleLetterStats(inputStream1);
                    IList<LetterStats> doubleLetterStats = FillDoubleLetterStats(inputStream2);

                    RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
                    RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);

                    Console.WriteLine("Single letter stats:");
                    PrintStatistic(singleLetterStats);

                    Console.WriteLine("\nDouble letter stats:");
                    PrintStatistic(doubleLetterStats);
                }
            }

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        /// <summary>
        /// Ф-ция возвращает экземпляр потока с уже загруженным файлом для последующего посимвольного чтения.
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        /// <returns>Поток для последующего чтения.</returns>
        private static IReadOnlyStream GetInputStream(string fileFullPath)
        {
            return new ReadOnlyStream(fileFullPath);
        }

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения каждой буквы.
        /// Статистика РЕГИСТРОЗАВИСИМАЯ!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        private static IList<LetterStats> FillSingleLetterStats(IReadOnlyStream stream)
        {
            List<LetterStats> stats = new List<LetterStats>();
            stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                char ch = stream.ReadNextChar();

                if(!Char.IsLetter(ch)) continue;

                try
                {
                    AddStatistic(stats, ch.ToString());
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Can't add statistic for char: {ex.Message}");
                }
                
            }
            return stats;
        }

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения парных букв.
        /// В статистику должны попадать только пары из одинаковых букв, например АА, СС, УУ, ЕЕ и т.д.
        /// Статистика - НЕ регистрозависимая!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        private static IList<LetterStats> FillDoubleLetterStats(IReadOnlyStream stream)
        {
            List<LetterStats> stats = new List<LetterStats>();
            stream.ResetPositionToStart();
            char headChar = Char.ToUpperInvariant(stream.ReadNextChar());
            char nextChar;
            while (!stream.IsEof)
            {
                nextChar = Char.ToUpperInvariant(stream.ReadNextChar());

                if(Char.IsLetter(headChar) == false)
                {
                    headChar = nextChar; continue;
                }
                if(Char.IsLetter(nextChar) == false) 
                {
                    headChar = Char.ToUpperInvariant(stream.ReadNextChar()); continue;
                }
                if(Char.ToUpperInvariant(headChar) != Char.ToUpperInvariant(nextChar))
                {
                    headChar = nextChar; continue;
                }
                
                try
                {
                    AddStatistic(stats, $"{headChar}{nextChar}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Can't add statistic for pair: {ex.Message}");
                }
                headChar = Char.ToUpperInvariant(stream.ReadNextChar());
            }
            return stats;
        }

        /// <summary>
        /// Ф-ция добавляет/увеличивает статистику вхождения для переданной буквы/пары букв.
        /// </summary>
        /// <param name="letters">Коллекция со статистиками вхождения букв/пар</param>
        /// <param name="letterStat">Буква/пара букв, для которой нужно добавить/увеличить статистику</param>
        private static void AddStatistic(List<LetterStats> letters, string letterStat)
        {
            if(letters.Exists(x => x.Letter == letterStat))
            {
                int index = letters
                    .FindIndex(x => String
                    .Equals(x.Letter, letterStat));
                IncStatistic(letters.ElementAt(index));
            }
            else
            {
                letters.Add(new LetterStats(letterStat, 1));
            }
        }

        /// <summary>
        /// Ф-ция перебирает все найденные буквы/парные буквы, содержащие в себе только гласные или согласные буквы.
        /// (Тип букв для перебора определяется параметром charType)
        /// Все найденные буквы/пары соответствующие параметру поиска - удаляются из переданной коллекции статистик.
        /// </summary>
        /// <param name="letters">Коллекция со статистиками вхождения букв/пар</param>
        /// <param name="charType">Тип букв для анализа</param>
        private static void RemoveCharStatsByType(IList<LetterStats> letters, CharType charType)
        {
            bool isVowelType = charType == CharType.Vowel;
            for(int i = 0; i < letters.Count; i++)
            {
                bool isVowel = VOWELS.Contains(letters[i].Letter[0]);
                if(isVowel == isVowelType)
                {
                    letters.RemoveAt(i);
                    i--;
                } 
            }
        }

        /// <summary>
        /// Ф-ция выводит на экран полученную статистику в формате "{Буква} : {Кол-во}"
        /// Каждая буква - с новой строки.
        /// Выводить на экран необходимо предварительно отсортировав набор по алфавиту.
        /// В конце отдельная строчка с ИТОГО, содержащая в себе общее кол-во найденных букв/пар
        /// </summary>
        /// <param name="letters">Коллекция со статистикой</param>
        private static void PrintStatistic(IEnumerable<LetterStats> letters)
        {
            letters
                .OrderBy(x => x.Letter)
                .ToList()
                .ForEach(x => Console.WriteLine($"{x.Letter} : {x.Count}"));
        }

        /// <summary>
        /// Метод увеличивает счётчик вхождений по переданной структуре.
        /// </summary>
        /// <param name="letterStats"></param>
        private static void IncStatistic(LetterStats letterStats)
        {
            letterStats.Count++;
        }
    }
}
