namespace Компилятор
{
    public struct TextPosition
    {
        private uint _lineNumber;
        private byte _charNumber;

        public uint LineNumber
        {
            get => _lineNumber;
            set => _lineNumber = value;
        }

        public byte CharNumber
        {
            get => _charNumber;
            set => _charNumber = value;
        }

        public TextPosition(uint ln = 0, byte c = 0)
        {
            _lineNumber = ln;
            _charNumber = c;
        }
    }

    public struct Err
    {
        private TextPosition _errorPosition;
        private byte _errorCode;

        public TextPosition ErrorPosition
        {
            get => _errorPosition;
            set => _errorPosition = value;
        }

        public byte ErrorCode
        {
            get => _errorCode;
            set => _errorCode = value;
        }

        public Err(TextPosition pos, byte code)
        {
            _errorPosition = pos;
            _errorCode = code;
        }
    }

    public static class InputOutput
    {
        private const byte ErrMax = 9;
        private static StreamReader file;
        private static string line;
        private static byte lastInLine;
        private static uint errorCount = 0;
        private static TextPosition positionNowField;

        public static char Ch { get; private set; }

        public static TextPosition PositionNow
        {
            get => positionNowField;
            private set => positionNowField = value;
        }

        public static List<Err> Errors { get; private set; }

        public static void Init(string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException($"Файл {fileName} не найден.");
            }

            file = new StreamReader(fileName);
            Errors = new List<Err>();
            PositionNow = new TextPosition(1, 0);
            ReadNextLine();

            if (line != null && line.Length > 0)
            {
                Ch = line[0];
            }
        }

        public static void Close()
        {
            file?.Close();
            Console.WriteLine($"\nКомпиляция завершена: ошибок — {errorCount}!");
        }

        private static void ReadNextLine()
        {
            if (!file.EndOfStream)
            {
                line = file.ReadLine();
                lastInLine = (byte)(line.Length - 1);
                Errors = new List<Err>();
            }
            else
            {
                line = null;
                lastInLine = 0;
                End();
            }
        }

        private static void ListThisLine()
        {
            if (line != null)
            {
                Console.WriteLine(line);
            }
        }

        private static void ListErrors()
        {
            int pos = 6 - $"{PositionNow.LineNumber} ".Length;

            foreach (Err e in Errors)
            {
                errorCount++;
                string s = "**";

                if (errorCount < 10)
                {
                    s += "0";
                }

                s += $"{errorCount}**";

                while (s.Length - 1 < pos + e.ErrorPosition.CharNumber)
                {
                    s += " ";
                }

                string errorMessage = Компилятор.Errors.GetMessage(e.ErrorCode);
                s += $"^ ошибка код {e.ErrorCode}: {errorMessage}";
                Console.WriteLine(s);
            }
        }

        private static void End()
        {
            if (line != null)
            {
                ListThisLine();
            }

            if (Errors.Count > 0)
            {
                ListErrors();
            }

            Close();
            Environment.Exit(0);
        }

        public static void NextCh()
        {
            if (PositionNow.CharNumber == lastInLine)
            {
                ListThisLine();

                if (Errors.Count > 0)
                {
                    ListErrors();
                }

                ReadNextLine();

                if (line == null)
                {
                    return;
                }

                PositionNow = new TextPosition(PositionNow.LineNumber + 1, 0);
            }
            else
            {
                PositionNow = new TextPosition(PositionNow.LineNumber, (byte)(PositionNow.CharNumber + 1));
            }

            Ch = line[PositionNow.CharNumber];
        }

        public static void Error(byte errorCode, TextPosition position)
        {
            if (Errors.Count <= ErrMax)
            {
                Errors.Add(new Err(position, errorCode));
            }
        }

        public static void SetCharNumber(byte newCharNumber)
        {
            PositionNow = new TextPosition(PositionNow.LineNumber, newCharNumber);
        }

        public static void IncrementCharNumber()
        {
            PositionNow = new TextPosition(PositionNow.LineNumber, (byte)(PositionNow.CharNumber + 1));
        }

        public static void DecrementCharNumber()
        {
            PositionNow = new TextPosition(PositionNow.LineNumber, (byte)(PositionNow.CharNumber - 1));
        }
    }
}