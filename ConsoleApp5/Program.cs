using System;
using System.IO;

namespace Компилятор
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=================" +
                "=======================");
            Console.WriteLine("ТЕСТИРОВАНИЕ " +
                "МОДУЛЯ ВВОДА-ВЫВОДА");
            Console.WriteLine("Тестирование" +
                " функции NextCh()");
            Console.WriteLine("==================" +
                "======================\n");

            string currentDir = Directory.GetCurrentDirectory();
            string testFile = Path.Combine(currentDir, "test.pas");

            Console.WriteLine($"Текущая директория: {currentDir}");
            Console.WriteLine($"Путь к файлу: {testFile}\n");
            Console.WriteLine(new string('-', 60));

            try
            {
                if (!File.Exists(testFile))
                {
                    Console.WriteLine($"ОШИБКА: Файл test.pas" +
                        $" не найден в текущей директории!");
                    Console.WriteLine("\nСоздайте файл test.pas" +
                        " с таким содержимым:");
                    Console.WriteLine("program Test;");
                    Console.WriteLine("var");
                    Console.WriteLine("  x: integer;");
                    Console.WriteLine("begin");
                    Console.WriteLine("  x := 9999999999;" +
                        "  // эта строка вызовет ошибку 203");
                    Console.WriteLine("  ch := 'a;     " +
                        "    // эта строка вызовет ошибку 205");
                    Console.WriteLine("end.");
                    return;
                }

                Console.WriteLine("Файл найден! Начинаем чтение\n");

                InputOutput.Init(testFile);

                Console.WriteLine("Содержимое файла:");
                Console.WriteLine(new string('-', 40));

                bool errorAddedAtLine2 = false;
                bool errorAddedAtLine5 = false;
                bool errorAddedAtLine6 = false;

                uint currentLine;
                byte currentChar;
                while (true)
                {
                    currentLine = 
                        InputOutput.PositionNow.LineNumber;
                    currentChar =
                        InputOutput.PositionNow.CharNumber;

                    // Строка 2: ошибка 1 — недопустимый символ
                    if (currentLine == 2 &&
                        !errorAddedAtLine2 &&
                        currentChar >= 5)
                    {
                        InputOutput.Error(1, new TextPosition(2, 5));
                        errorAddedAtLine2 = true;
                        Console.WriteLine($"  >>> Добавлена ошибка " +
                            $"1 на строке 2, позиция 5");
                    }

                    // Строка 5: ошибка 203 — константа превышает диапазон
                    if (currentLine == 5 &&
                        !errorAddedAtLine5 &&
                        currentChar >= 10)
                    {
                        InputOutput.Error(203, new TextPosition(5, 10));
                        errorAddedAtLine5 = true;
                        Console.WriteLine($"  >>> Добавлена" +
                            $" ошибка 203 на строке 5, позиция 10");
                    }

                    // Строка 6: ошибка 205 — незакрытая символьная константа
                    if (currentLine == 6 &&
                        !errorAddedAtLine6 &&
                        currentChar >= 10)
                    {
                        InputOutput.Error(205, new TextPosition(6, 10));
                        errorAddedAtLine6 = true;
                        Console.WriteLine($"  >>> Добавлена" +
                            $" ошибка 205 на строке 6, позиция 10");
                    }

                    InputOutput.NextCh();

                    if (InputOutput.Ch == '\0')
                    {
                        break;
                    }
                }
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"\nОШИБКА: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nНепредвиденная " +
                    $"ошибка: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                InputOutput.Close();
            }

            Console.WriteLine("\n================" +
                "========================");
            Console.WriteLine("ТЕСТИРОВАНИЕ ЗАВЕРШЕНО");
            Console.WriteLine("==================" +
                "======================");
        }
    }
}