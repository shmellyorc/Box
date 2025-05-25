namespace Box.Loaders.BitmapFonts;

internal readonly struct Kerning : IEquatable<Kerning>
{
    private readonly int _amount;
    private readonly char _firstCharacter;
    private readonly char _secondCharacter;

    public Kerning(char firstCharacter, char secondCharacter, int amount) : this()
    {
        _firstCharacter = firstCharacter;
        _secondCharacter = secondCharacter;
        _amount = amount;
    }


    public int Amount
    {
        get { return _amount; }
    }

    public char FirstCharacter
    {
        get { return _firstCharacter; }
    }

    public char SecondCharacter
    {
        get { return _secondCharacter; }
    }

    public override bool Equals(object obj)
    {
        if (obj is null) return false;
        if (obj.GetType() != typeof(Kerning)) return false;

        return Equals((Kerning)obj);
    }

    public bool Equals(Kerning other)
    {
        return _firstCharacter == other._firstCharacter && _secondCharacter == other._secondCharacter;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return _firstCharacter << 16 | _secondCharacter;
        }
    }

    public override string ToString()
    {
        return string.Format("{0} to {1} = {2}", _firstCharacter, _secondCharacter, _amount);
    }

    public static bool operator ==(Kerning left, Kerning right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Kerning left, Kerning right)
    {
        return !(left == right);
    }
}
