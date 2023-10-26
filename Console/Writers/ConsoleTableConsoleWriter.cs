namespace Console.Writers;

public class ConsoleTableConsoleWriter
{
    private const char horizontalSeparator = '|';
    private const char verticalSeparator = '-';
    private const ConsoleColor defaultRowColor = ConsoleColor.Gray;
    private readonly string rowSeparator;
    private readonly List<Row> rows = new();
    private readonly List<string> columnFormats = new();

    public ConsoleTableConsoleWriter(params Column[] columns)
    {
        foreach (var column in columns)
        {
            var width = column.Width;
            var cellFormat = $"{{0,-{width}}}";
            columnFormats.Add(cellFormat);
        }
        
        var columnsCount = columns.Length;
        var columnsWidthSum = columns.Sum(x => x.Width);
        rowSeparator = string.Join("", Enumerable.Range(0, columnsWidthSum + columnsCount + 1).Select(_ => verticalSeparator));
        
        AppendRow(columns.Select(x => new Cell(x.Name, defaultRowColor)).ToArray());
    }

    public void AppendRow(params Cell[] cells)
    {
        rows.Add(new Row(cells));
    }

    public void Write()
    {
        foreach (var row in rows)
        {
            SetDefaultConsoleForeground();
            System.Console.WriteLine(rowSeparator);
            var cells = row.Cells;
            var cellsCount = cells.Count;
            for(var cellNumber = 0; cellNumber < cellsCount; cellNumber++) 
            {
                var cell = cells[cellNumber];
                SetDefaultConsoleForeground();
                System.Console.Write(horizontalSeparator);
                ChangeConsoleForeground(cell.Color);
                System.Console.Write(columnFormats[cellNumber], cell.Value);
            }
            SetDefaultConsoleForeground();
            System.Console.Write(horizontalSeparator);
            System.Console.WriteLine();
        }
        System.Console.WriteLine(rowSeparator);
    }

    private static void SetDefaultConsoleForeground()
    {
        ChangeConsoleForeground(defaultRowColor);
    }
    
    private static void ChangeConsoleForeground(ConsoleColor consoleColor)
    {
        System.Console.ForegroundColor = consoleColor;
    }

    public readonly struct Column
    {
        public string Name { get; }
        public int Width { get; }

        public Column(string name, int width)
        {
            Name = name;
            Width = width;
        }
        public Column(string name)
        {
            Name = name;
            Width = name.Length;
        }
    }

    public readonly struct Row
    {
        public Row(IEnumerable<Cell> cells)
        {
            Cells = new List<Cell>(cells);
        }

        public IList<Cell> Cells { get; } = new List<Cell>();
    }

    public readonly struct Cell
    {
        public Cell(object value, ConsoleColor color)
        {
            Color = color;
            Value = value;
        }

        public Cell(object value)
        {
            Color = defaultRowColor;
            Value = value;
        }

        public ConsoleColor Color { get; }

        public object Value { get; }

        public override string? ToString() => Value.ToString();
    }
}