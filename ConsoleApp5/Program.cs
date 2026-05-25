using System;
using System.IO;
using System.Text;

namespace Компилятор
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("========================================");
            Console.WriteLine("ЛЕКСИЧЕСКИЙ АНАЛИЗАТОР ПАСКАЛЯ");
            Console.WriteLine("========================================\n");

            string currentDir = Directory.GetCurrentDirectory();
            string testFile = Path.Combine(currentDir, "test.pas");

            Console.WriteLine($"Текущая директория: {currentDir}");
            Console.WriteLine($"Путь к файлу: {testFile}\n");

            if (!File.Exists(testFile))
            {
                CreateTestFile(testFile);
            }

            try
            {
                InputOutput.Init(testFile);
                LexicalAnalyzer lexer = new LexicalAnalyzer();

                Console.WriteLine("Содержимое файла:");
                Console.WriteLine(new string('-', 60));
                Console.WriteLine(File.ReadAllText(testFile));
                Console.WriteLine(new string('-', 60));
                Console.WriteLine("\nРезультаты лексического анализа:");
                Console.WriteLine(new string('=', 80));
                Console.WriteLine($"{"№",3} | {"КОД",4} | {"ТИП ТОКЕНА",-20} | {"ЛЕКСЕМА",-20} | {"ПОЗИЦИЯ"}");
                Console.WriteLine(new string('-', 80));

                int tokenCount = 0;
                StringBuilder codesOutput = new StringBuilder();
                byte symbol;

                while (true)
                {
                    symbol = lexer.NextToken();

                    if (symbol == 0)
                    {
                        Console.WriteLine("[DEBUG] Конец файла");
                        break;
                    }

                    tokenCount++;

                    string tokenType = lexer.GetTokenName(symbol);
                    string lexeme = GetLexeme(lexer, symbol);
                    string position = $"стр.{lexer.TokenPosition.LineNumber}, поз.{lexer.TokenPosition.CharNumber}";

                    Console.WriteLine($"{tokenCount,3} | {symbol,4} | {tokenType,-20} | {lexeme,-20} | {position}");

                    codesOutput.Append(symbol).Append(' ');
                }

                Console.WriteLine(new string('-', 80));
                Console.WriteLine($"Всего токенов: {tokenCount}");

                SaveTokenCodes(codesOutput.ToString(), "token_codes.txt");

                Console.WriteLine("\nПоследовательность кодов символов:");
                Console.WriteLine(codesOutput.ToString());

                InputOutput.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nНепредвиденная ошибка: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }

        static string GetLexeme(LexicalAnalyzer lexer, byte symbol)
        {
            if (symbol == LexicalAnalyzer.IDENT)
                return lexer.IdentifierName;
            if (symbol == LexicalAnalyzer.INTCONST)
                return lexer.IntValue.ToString();
            if (symbol == LexicalAnalyzer.REALCONST)
                return lexer.FloatValue.ToString(System.Globalization.CultureInfo.InvariantCulture);
            if (symbol == LexicalAnalyzer.CHARCONST)
                return $"'{lexer.CharValue}'";

            switch (symbol)
            {
                case LexicalAnalyzer.PLUS: return "+";
                case LexicalAnalyzer.MINUS: return "-";
                case LexicalAnalyzer.STAR: return "*";
                case LexicalAnalyzer.SLASH: return "/";
                case LexicalAnalyzer.EQUAL: return "=";
                case LexicalAnalyzer.ASSIGN: return ":=";
                case LexicalAnalyzer.SEMICOLON: return ";";
                case LexicalAnalyzer.COLON: return ":";
                case LexicalAnalyzer.COMMA: return ",";
                case LexicalAnalyzer.POINT: return ".";
                case LexicalAnalyzer.DOTDOT: return "..";
                case LexicalAnalyzer.LPAREN: return "(";
                case LexicalAnalyzer.RPAREN: return ")";
                case LexicalAnalyzer.LBRACKET: return "[";
                case LexicalAnalyzer.RBRACKET: return "]";
                case LexicalAnalyzer.LESS: return "<";
                case LexicalAnalyzer.GREATER: return ">";
                case LexicalAnalyzer.LESSEQUAL: return "<=";
                case LexicalAnalyzer.GREATEREQUAL: return ">=";
                case LexicalAnalyzer.NOTEQUAL: return "<>";
                case LexicalAnalyzer.CARET: return "^";
                default:
                    foreach (var kw in Keywords.GetKeywordTable())
                        if (kw.Value == symbol) return kw.Key.ToUpper();
                    return "";
            }
        }

        static void SaveTokenCodes(string codes, string filename)
        {
            try
            {
                File.WriteAllText(filename, codes);
                Console.WriteLine($"\n✓ Коды символов сохранены в файл: {filename}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении файла: {ex.Message}");
            }
        }

        static void CreateTestFile(string filename)
        {
            string testContent = @"program TestProgram;

const
    MAXVALUE = 100;
    MINVALUE = 0;

var
    x : integer;
    y : integer;

begin
    x := 50000;
    y := 60000;
    x := 65535;
end.";
            File.WriteAllText(filename, testContent);
            Console.WriteLine($"Создан тестовый файл: {filename}\n");
        }
    }
}
