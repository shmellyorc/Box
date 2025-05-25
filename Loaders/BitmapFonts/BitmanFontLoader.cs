namespace Box.Loaders.BitmapFonts;

internal static class BitmapFontLoader
{
    internal static BitmapFont LoadFontFromBinaryFile(string fileName)
    {
        BitmapFont font;

        if (string.IsNullOrEmpty(fileName))
        {
            throw new ArgumentNullException(nameof(fileName));
        }

        if (!File.Exists(fileName))
        {
            throw new FileNotFoundException(string.Format("Cannot find file '{0}'", fileName), fileName);
        }

        font = new BitmapFont();

        using (Stream stream = File.OpenRead(fileName))
        {
            font.LoadBinary(stream);
        }

        QualifyResourcePaths(font, Path.GetDirectoryName(fileName));

        return font;
    }

    internal static BitmapFont LoadFontFromFile(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            throw new ArgumentNullException(nameof(fileName), "File name not specified");

        if (!File.Exists(fileName))
            throw new FileNotFoundException(string.Format("Cannot find file '{0}'", fileName), fileName);

        return GetFileFormat(fileName) switch
        {
            BitmapFontFormat.Binary => LoadFontFromBinaryFile(fileName),
            BitmapFontFormat.Text => LoadFontFromTextFile(fileName),
            BitmapFontFormat.Xml => LoadFontFromXmlFile(fileName),
            _ => throw new InvalidDataException("Unknown file format."),
        };
    }

    internal static BitmapFont LoadFontFromTextFile(string fileName)
    {
        BitmapFont font;

        if (string.IsNullOrEmpty(fileName))
        {
            throw new ArgumentNullException(nameof(fileName));
        }

        if (!File.Exists(fileName))
        {
            throw new FileNotFoundException(string.Format("Cannot find file '{0}'", fileName), fileName);
        }

        font = new BitmapFont();

        using (Stream stream = File.OpenRead(fileName))
        {
            font.LoadText(stream);
        }

        QualifyResourcePaths(font, Path.GetDirectoryName(fileName));

        return font;
    }

    internal static BitmapFont LoadFontFromXmlFile(string fileName)
    {
        BitmapFont font;

        if (string.IsNullOrEmpty(fileName))
        {
            throw new ArgumentNullException(nameof(fileName));
        }

        if (!File.Exists(fileName))
        {
            throw new FileNotFoundException(string.Format("Cannot find file '{0}'", fileName), fileName);
        }

        font = new BitmapFont();

        using (Stream stream = File.OpenRead(fileName))
        {
            font.LoadXml(stream);
        }

        QualifyResourcePaths(font, Path.GetDirectoryName(fileName));

        return font;
    }

    internal static BitmapFontFormat GetFileFormat(string fileName)
    {
        using Stream stream = File.OpenRead(fileName);

        return GetFileFormat(stream);
    }

    internal static BitmapFontFormat GetFileFormat(Stream stream)
    {
        BitmapFontFormat result;
        byte[] buffer;
        long position;

        buffer = new byte[5];
        position = stream.Position;

        stream.Read(buffer, 0, 5);

        stream.Position = position;

        if (buffer[0] == 66 && buffer[1] == 77 && buffer[2] == 70 && buffer[3] == 3)
        {
            result = BitmapFontFormat.Binary;
        }
        else if (buffer[0] == 105 && buffer[1] == 110 && buffer[2] == 102 && buffer[3] == 111 && buffer[4] == 32)
        {
            result = BitmapFontFormat.Text;
        }
        else if (buffer[0] == 60 && buffer[1] == 63 && buffer[2] == 120 && buffer[3] == 109 && buffer[4] == 108)
        {
            result = BitmapFontFormat.Xml;
        }
        else
        {
            result = BitmapFontFormat.None;
        }

        return result;
    }

    internal static bool GetNamedBool(string[] parts, string name, int estimatedStart)
    {
        return int.TryParse(GetNamedString(parts, name, estimatedStart), NumberStyles.Number, CultureInfo.InvariantCulture, out int v) && v > 0;
    }

    internal static int GetNamedInt(string[] parts, string name, int estimatedStart)
    {
        return int.TryParse(GetNamedString(parts, name, estimatedStart), NumberStyles.Number, CultureInfo.InvariantCulture, out int result) ? result : 0;
    }

    internal static string GetNamedString(string[] parts, string name, int estimatedStart)
    {
        string result;

        if (string.Equals(GetValueName(parts[estimatedStart]), name, StringComparison.OrdinalIgnoreCase))
        {
            // we have a value right were we expected it
            result = SanitizeValue(parts[estimatedStart][(name.Length + 1)..]);
        }
        else
        {
            result = string.Empty;

            for (int i = 0; i < parts.Length; i++)
            {
                string part;

                part = parts[i];

                if (string.Equals(GetValueName(part), name, StringComparison.OrdinalIgnoreCase))
                {
                    result = SanitizeValue(part[(name.Length + 1)..]);
                    break;
                }
            }
        }

        return result;
    }

    internal static Padding ParsePadding(string s)
    {
        int rStart;
        int bStart;
        int lStart;

        rStart = s.IndexOf(',');
        bStart = s.IndexOf(',', rStart + 1);
        lStart = s.IndexOf(',', bStart + 1);

        return new Padding
        (
            int.Parse(s[(lStart + 1)..], CultureInfo.InvariantCulture),
            int.Parse(s[..rStart], CultureInfo.InvariantCulture),
            int.Parse(s.AsSpan(rStart + 1, bStart - rStart - 1), provider: CultureInfo.InvariantCulture),
            int.Parse(s.AsSpan(bStart + 1, lStart - bStart - 1), provider: CultureInfo.InvariantCulture)
        );
    }

    internal static Vect2 ParsePoint(string s)
    {
        int yStart;

        yStart = s.IndexOf(',');

        return new Vect2
        (
            int.Parse(s[..yStart], CultureInfo.InvariantCulture),
            int.Parse(s[(yStart + 1)..], CultureInfo.InvariantCulture)
        );
    }

    internal static void QualifyResourcePaths(BitmapFont font, string resourcePath)
    {
        Page[] pages;

        pages = font.Pages;

        for (int i = 0; i < pages.Length; i++)
        {
            Page page;

            page = pages[i];
            page.FileName = Path.Combine(resourcePath, page.FileName);
            pages[i] = page;
        }

        font.Pages = pages;
    }

    internal static void Split(string s, string[] buffer)
    {
        int index;
        int partStart;
        char delimiter;

        index = 0;
        partStart = -1;
        delimiter = ' ';

        do
        {
            int partEnd;
            int quoteStart;
            int quoteEnd;
            int length;
            bool hasQuotes;

            quoteStart = s.IndexOf('"', partStart + 1);
            quoteEnd = s.IndexOf('"', quoteStart + 1);
            partEnd = s.IndexOf(delimiter, partStart + 1);

            if (partEnd == -1)
            {
                partEnd = s.Length;
            }

            hasQuotes = quoteStart != -1 && partEnd > quoteStart && partEnd < quoteEnd;
            if (hasQuotes)
            {
                partEnd = s.IndexOf(delimiter, quoteEnd + 1);

                if (partEnd == -1)
                {
                    partEnd = s.Length;
                }
            }

            length = partEnd - partStart - 1;
            if (length > 0)
            {
                buffer[index] = s.Substring(partStart + 1, length);
                index++;
            }

            if (hasQuotes)
            {
                partStart = partEnd - 1;
            }

            partStart = s.IndexOf(delimiter, partStart + 1);
        } while (partStart != -1);

        // reset any unused parts of the buffer
        for (; index < buffer.Length; index++)
        {
            buffer[index] = string.Empty;
        }
    }

    internal static T[] ToArray<T>(ICollection<T> values)
    {
        T[] result;

        // avoid a forced .NET 3 dependency just for one call to Linq

        result = new T[values.Count];
        values.CopyTo(result, 0);

        return result;
    }

    private static string GetValueName(string nameValuePair)
    {
        int nameEndIndex;

        return !string.IsNullOrEmpty(nameValuePair) && (nameEndIndex = nameValuePair.IndexOf('=')) != -1
            ? nameValuePair[..nameEndIndex]
            : null;
    }

    private static string SanitizeValue(string value)
    {
        int valueLength;

        valueLength = value.Length;

        if (valueLength > 1 && value[0] == '"' && value[valueLength - 1] == '"')
        {
            value = value.Substring(1, valueLength - 2);
        }

        return value;
    }
}
