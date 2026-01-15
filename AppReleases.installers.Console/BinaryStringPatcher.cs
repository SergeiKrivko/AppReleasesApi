using System.Text;

namespace AppReleases.installers.Console;

public static class BinaryPatcher
{
    // Кодировка строк в .NET (UTF-16BE)
    private static readonly Encoding Encoding = Encoding.BigEndianUnicode;

    /// <summary>
    /// Заменяет строку в бинарном файле
    /// </summary>
    /// <param name="source">Исходные данные</param>
    /// <param name="oldString">Исходная строка (должна точно соответствовать)</param>
    /// <param name="newString">Новая строка (не должна быть длиннее исходной)</param>
    /// <returns>True если успешно</returns>
    public static byte[] PatchString(this byte[] source, string oldString, string newString)
    {
        if (newString.Length > oldString.Length)
        {
            throw new ArgumentException($"Новая строка не должна быть длиннее исходной. Максимум: {oldString.Length}");
        }

        var oldBytes = Encoding.GetBytes(oldString);
        var newBytes = Encoding.GetBytes(newString);
        System.Console.WriteLine(BitConverter.ToString(oldBytes));

        // Добиваем новую строку нулями до длины старой
        if (newBytes.Length < oldBytes.Length)
        {
            var paddedBytes = new byte[oldBytes.Length];
            Array.Copy(newBytes, paddedBytes, newBytes.Length);
            // Остальные байты уже нулевые (по умолчанию)
            newBytes = paddedBytes;
        }

        try
        {
            // Ищем вхождения старой строки
            var positions = FindAllOccurrences(source, oldBytes);

            if (positions.Count == 0)
            {
                System.Console.WriteLine($"Строка '{oldString}' не найдена в файле.");
                return source;
            }

            System.Console.WriteLine($"Найдено {positions.Count} вхождений строки.");

            var result = source.ToArray();
            // Заменяем все вхождения
            foreach (var pos in positions)
            {
                Array.Copy(newBytes, 0, result, pos, newBytes.Length);
                System.Console.WriteLine($"Заменено вхождение по смещению 0x{pos:X8}");
            }

            return result;
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Ошибка: {ex.Message}");
            return source;
        }
    }

    /// <summary>
    /// Находит все вхождения байтового массива в файле
    /// </summary>
    private static List<int> FindAllOccurrences(byte[] source, byte[] pattern)
    {
        var positions = new List<int>();

        for (int i = 0; i <= source.Length - pattern.Length; i++)
        {
            if (IsMatch(source, i, pattern))
            {
                positions.Add(i);
                // Пропускаем длину паттерна для непересекающихся поисков
                i += pattern.Length - 1;
            }
        }

        return positions;
    }

    private static bool IsMatch(byte[] source, int start, byte[] pattern)
    {
        if (start + pattern.Length > source.Length)
            return false;

        for (int i = 0; i < pattern.Length; i++)
        {
            if (source[start + i] != pattern[i])
                return false;
        }

        return true;
    }
}