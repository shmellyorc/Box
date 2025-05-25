namespace Box.Loaders.BitmapFonts;

internal struct Padding
{
    public int Bottom { get; set; }
    public int Left { get; set; }
    public int Right { get; set; }
    public int Top { get; set; }

    public Padding(int left, int top, int right, int bottom)
      : this()
    {
        Top = top;
        Left = left;
        Right = right;
        Bottom = bottom;
    }

    public override string ToString()
    {
        return string.Format("{0}, {1}, {2}, {3}", Left, Top, Right, Bottom);
    }
}
