namespace Box.Loaders.BitmapFonts;

internal struct Size
{
    public static Size Empty => new();

    public int Width = 0;
    public int Height = 0;

    public Size(int width, int height)
    {
        Width = width;
        Height = height;
    }
}
