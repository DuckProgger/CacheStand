using System.Text;

namespace Console.Writers;

public class ConsoleTableStringBuilder
{
    private readonly StringBuilder stringBuilder = new();
    private readonly string rowFormat;
    private readonly string rowSeparator;

    //public ConsoleTableStringBuilder(int columns, int columnWidth)
    //{
    //    const char horizontalSeparator = '|';
    //    const char verticalSeparator = '-';
    //    rowSeparator = string.Join("", Enumerable.Range(0, (columns + 1) * columnWidth).Select(_ => verticalSeparator));
    //    rowFormat = string.Join(horizontalSeparator, Enumerable.Range(0, columns).Select(x => $"{{{x},{-columnWidth}}}"));
    //    rowFormat = $"{horizontalSeparator}{rowFormat}{horizontalSeparator}";
    //    stringBuilder.AppendLine(rowSeparator);
    //}

    public ConsoleTableStringBuilder(params ColumnInfo[] columns)
    {
        const char horizontalSeparator = '|';
        const char verticalSeparator = '-';
        var columnsCount = columns.Length;
        var columnsWidthSum = columns.Sum(x => x.Width);
        rowSeparator = string.Join("", Enumerable.Range(0, columnsWidthSum + columnsCount + 1).Select(_ => verticalSeparator));
        int columnNumber = 0;
        rowFormat = string.Join(horizontalSeparator, columns.Select(x => $"{{{columnNumber++},{-x.Width}}}"));
        rowFormat = $"{horizontalSeparator}{rowFormat}{horizontalSeparator}";
        stringBuilder.AppendLine(rowSeparator);
        AppendRow(columns.Select(x => x.Name).ToArray());
    }

    public void AppendRow(params object?[] columnValues)
    {
        stringBuilder.AppendLine(string.Format(rowFormat, columnValues));
        stringBuilder.AppendLine(rowSeparator);
    }
    
    public override string ToString() => stringBuilder.ToString();

    public struct ColumnInfo
    {
        public string Name { get; }
        public int Width { get; }

        public ColumnInfo(string name, int width)
        {
            Name = name;
            Width = width;
        }
    }
}