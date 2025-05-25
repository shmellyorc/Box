namespace Box.Loaders.Maps.Settings;

/// <summary>
/// Represents a setting associated with a map element.
/// </summary>
public class MapSetting : IMapSetting
{
    /// <summary>
    /// Gets the value of the map setting.
    /// </summary>
    public object Value { get; }


    internal MapSetting(object value) => Value = value;


    /// <summary>
    /// Determines whether a specified setting exists within the provided settings dictionary.
    /// </summary>
    /// <param name="settings">A dictionary containing map settings.</param>
    /// <param name="name">The name of the setting to check for.</param>
    /// <returns>True if the setting exists; otherwise, false.</returns>
    public static bool HasSetting(Dictionary<string, MapSetting> settings, string name)
        => settings.ContainsKey(name) && settings[name] is not null;



    /// <summary>
    /// Replaces or adds a map setting in the provided dictionary of settings.
    /// </summary>
    /// <param name="settings">The dictionary containing the map settings.</param>
    /// <param name="name">The name of the setting to replace or add.</param>
    /// <param name="setting">The new map setting to replace or add.</param>
    /// <returns>True if the setting was successfully replaced; otherwise, false.</returns>
    public static bool ReplaceSetting(Dictionary<string, MapSetting> settings, string name, MapSetting setting)
    {
        if (!HasSetting(settings, name))
            return false;

        settings[name] = setting;

        return true;
    }




    /// <summary>
    /// Retrieves the setting value as a boolean.
    /// </summary>
    /// <returns>The value of the setting cast to a boolean.</returns>
    /// <exception cref="InvalidCastException">Thrown if the setting value cannot be cast to a boolean.</exception>
    public bool GetBoolSetting()
    {
        try
        {
            return (bool)Value;
        }
        catch (InvalidCastException ex)
        {
            throw new InvalidCastException("The setting value cannot be cast to a Boolean.", ex);
        }
    }

    /// <summary>
    /// Retrieves the value of the specified setting as a boolean from the provided settings dictionary.
    /// </summary>
    /// <param name="settings">A dictionary containing map settings.</param>
    /// <param name="name">The name of the setting to retrieve.</param>
    /// <returns>The value of the specified setting cast to a boolean.</returns>
    /// <exception cref="InvalidCastException">Thrown if the setting value cannot be cast to a boolean.</exception>
    public static bool GetBoolSetting(Dictionary<string, MapSetting> settings, string name)
    {
        if (!settings.TryGetValue(name, out MapSetting value))
            return default;

        return value.GetBoolSetting();
    }

    /// <summary>
    /// Attempts to retrieve a boolean setting from the provided dictionary of settings.
    /// </summary>
    /// <param name="settings">A dictionary containing the settings.</param>
    /// <param name="name">The name of the setting to retrieve.</param>
    /// <param name="value">The output boolean value of the setting if found; otherwise, null.</param>
    /// <returns>True if the setting is found and is a valid boolean; otherwise, false.</returns>
    public static bool TryGetBoolSetting(Dictionary<string, MapSetting> settings, string name, out bool? value)
    {
        value = GetBoolSetting(settings, name);

        return value.HasValue;
    }



    /// <summary>
    /// Retrieves the setting value as a Color.
    /// </summary>
    /// <returns>The value of the setting cast to a Color.</returns>
    /// <exception cref="InvalidCastException">Thrown if the setting value cannot be cast to a Color.</exception>
    public BoxColor GetColorSetting()
    {
        try
        {
            return (BoxColor)Value;
        }
        catch (InvalidCastException ex)
        {
            throw new InvalidCastException("The setting value cannot be cast to a Color.", ex);
        }
    }

    /// <summary>
    /// Retrieves the Color setting value from the specified settings dictionary.
    /// </summary>
    /// <param name="settings">A dictionary containing map settings.</param>
    /// <param name="name">The name of the setting to retrieve.</param>
    /// <returns>The Color setting value if found; otherwise, throws InvalidCastException.</returns>
    /// <exception cref="InvalidCastException">Thrown if the setting value cannot be cast to a Color.</exception>
    public static BoxColor GetColorSetting(Dictionary<string, MapSetting> settings, string name)
    {
        if (!settings.TryGetValue(name, out MapSetting value))
            return default;

        return value.GetColorSetting(); // Return default Color if setting is not found
    }

    /// <summary>
    /// Attempts to retrieve a color setting from the provided dictionary of settings.
    /// </summary>
    /// <param name="settings">A dictionary containing the settings.</param>
    /// <param name="name">The name of the setting to retrieve.</param>
    /// <param name="value">The output color value of the setting if found; otherwise, null.</param>
    /// <returns>True if the setting is found and is a valid color; otherwise, false.</returns>
    public static bool TryGetColorSetting(Dictionary<string, MapSetting> settings, string name, out BoxColor? value)
    {
        value = GetColorSetting(settings, name);

        return value.HasValue;
    }



    /// <summary>
    /// Retrieves the MapEntityRef setting value.
    /// </summary>
    /// <returns>The MapEntityRef setting value.</returns>
    /// <exception cref="InvalidCastException">Thrown if the setting value cannot be cast to a MapEntityRef.</exception>
    public MapEntityRef GetEntityRefSetting()
    {
        try
        {
            return (MapEntityRef)Value;
        }
        catch (InvalidCastException ex)
        {
            throw new InvalidCastException("The setting value cannot be cast to a MapEntityRef.", ex);
        }
    }

    /// <summary>
    /// Retrieves the MapEntityRef setting value from the specified settings dictionary.
    /// </summary>
    /// <param name="settings">A dictionary containing map settings.</param>
    /// <param name="name">The name of the setting to retrieve.</param>
    /// <returns>The MapEntityRef setting value if found; otherwise, throws KeyNotFoundException.</returns>
    /// <exception cref="InvalidCastException">Thrown if the setting value cannot be cast to a MapEntityRef.</exception>
    public static MapEntityRef GetEntityRefSetting(Dictionary<string, MapSetting> settings, string name)
    {
        if (!settings.TryGetValue(name, out MapSetting value))
            return default;

        return value.GetEntityRefSetting();
    }

    /// <summary>
    /// Attempts to retrieve an entity reference setting from the provided dictionary of settings.
    /// </summary>
    /// <param name="settings">A dictionary containing the settings.</param>
    /// <param name="name">The name of the setting to retrieve.</param>
    /// <param name="value">The output entity reference value of the setting if found.</param>
    /// <returns>True if the setting is found and is a valid entity reference; otherwise, false.</returns>
    public static bool TryGetEntityRefSetting(Dictionary<string, MapSetting> settings, string name, out MapEntityRef? value)
    {
        value = GetEntityRefSetting(settings, name);

        return value.HasValue && !value.Value.IsEmpty;
    }



    /// <summary>
    /// Retrieves the enum setting value of type T.
    /// </summary>
    /// <typeparam name="T">The enum type to retrieve.</typeparam>
    /// <returns>The enum setting value of type T.</returns>
    /// <exception cref="InvalidCastException">Thrown if the setting value cannot be cast to the specified enum type T.</exception>
    public T GetEnumSetting<T>() where T : struct
    {
        try
        {
            return Enum.Parse<T>((string)Value, true);
        }
        catch (InvalidCastException ex)
        {
            throw new InvalidCastException("The setting value cannot be cast to a Enum.", ex);
        }
    }

    /// <summary>
    /// Retrieves the enum setting value of type T from the specified settings dictionary.
    /// </summary>
    /// <typeparam name="T">The enum type to retrieve.</typeparam>
    /// <param name="settings">A dictionary containing map settings.</param>
    /// <param name="name">The name of the setting to retrieve.</param>
    /// <returns>The enum setting value of type T if found; otherwise, throws KeyNotFoundException.</returns>
    /// <exception cref="InvalidCastException">Thrown if the setting value cannot be cast to the specified enum type T.</exception>
    public static T GetEnumSetting<T>(Dictionary<string, MapSetting> settings, string name) where T : struct
    {
        if (!settings.TryGetValue(name, out MapSetting value))
            return default;

        return value.GetEnumSetting<T>();
    }

    /// <summary>
    /// Attempts to retrieve an enum setting of type <typeparamref name="T"/> from the provided dictionary of settings.
    /// </summary>
    /// <typeparam name="T">The enum type to retrieve.</typeparam>
    /// <param name="settings">A dictionary containing the settings.</param>
    /// <param name="name">The name of the setting to retrieve.</param>
    /// <param name="value">The output enum value of the setting if found; otherwise, null.</param>
    /// <returns>True if the setting is found and is a valid enum of type <typeparamref name="T"/>; otherwise, false.</returns>
    public static bool TryGetEnumSetting<T>(Dictionary<string, MapSetting> settings, string name, out T? value) where T : struct
    {
        value = GetEnumSetting<T>(settings, name);

        return value.HasValue && value.Value is Enum;
    }




    /// <summary>
    /// Retrieves the filepath setting value as a string.
    /// </summary>
    /// <returns>The filepath setting value as a string.</returns>
    /// <exception cref="InvalidCastException">Thrown if the setting value cannot be cast to a string.</exception>
    public string GetFilepathSetting()
    {
        try
        {
            return (string)Value;
        }
        catch (InvalidCastException ex)
        {
            throw new InvalidCastException("The setting value cannot be cast to a Filepath.", ex);
        }
    }

    /// <summary>
    /// Retrieves the filepath setting value as a string from the specified settings dictionary.
    /// </summary>
    /// <param name="settings">A dictionary containing map settings.</param>
    /// <param name="name">The name of the setting to retrieve.</param>
    /// <returns>The filepath setting value as a string if found; otherwise, throws KeyNotFoundException.</returns>
    /// <exception cref="InvalidCastException">Thrown if the setting value cannot be cast to a string.</exception>
    public static string GetFilepathSetting(Dictionary<string, MapSetting> settings, string name)
    {
        if (!settings.TryGetValue(name, out MapSetting value))
            return default;

        return value.GetFilepathSetting();
    }

    /// <summary>
    /// Tries to retrieve a filepath setting from the provided dictionary of settings.
    /// </summary>
    /// <param name="settings">A dictionary containing the settings.</param>
    /// <param name="name">The name of the setting to retrieve.</param>
    /// <param name="value">The output filepath value of the setting if found.</param>
    /// <returns>True if the setting is found and the filepath is valid; otherwise, false.</returns>
    public static bool TryGetFilepathSetting(Dictionary<string, MapSetting> settings, string name, out string value)
    {
        value = GetFilepathSetting(settings, name);

        return value is not null;
    }




    /// <summary>
    /// Retrieves the float setting value as a single-precision floating point number.
    /// </summary>
    /// <returns>The float setting value as a single-precision floating point number.</returns>
    /// <exception cref="InvalidCastException">Thrown if the setting value cannot be cast to a float.</exception>
    public float GetFloatSetting()
    {
        try
        {
            return (float)Value;
        }
        catch (InvalidCastException ex)
        {
            throw new InvalidCastException("The setting value cannot be cast to a Float.", ex);
        }
    }

    /// <summary>
    /// Retrieves the float setting value as a single-precision floating point number from the specified settings dictionary.
    /// </summary>
    /// <param name="settings">A dictionary containing map settings.</param>
    /// <param name="name">The name of the setting to retrieve.</param>
    /// <returns>The float setting value as a single-precision floating point number if found; otherwise, throws KeyNotFoundException.</returns>
    /// <exception cref="InvalidCastException">Thrown if the setting value cannot be cast to a float.</exception>
    public static float GetFloatSetting(Dictionary<string, MapSetting> settings, string name)
    {
        if (!settings.TryGetValue(name, out MapSetting value))
            return default;

        return value.GetFloatSetting();
    }

    /// <summary>
    /// Tries to retrieve a float setting from the provided dictionary of settings.
    /// </summary>
    /// <param name="settings">A dictionary containing the settings.</param>
    /// <param name="name">The name of the setting to retrieve.</param>
    /// <param name="value">The output float value of the setting if found; otherwise, null.</param>
    /// <returns>True if the setting is found and is a valid float; otherwise, false.</returns>
    public static bool TryGetFloatSetting(Dictionary<string, MapSetting> settings, string name, out float? value)
    {
        value = GetFloatSetting(settings, name);

        return value.HasValue;
    }



    /// <summary>
    /// Retrieves the integer setting value as a 32-bit signed integer.
    /// </summary>
    /// <returns>The integer setting value as a 32-bit signed integer.</returns>
    /// <exception cref="InvalidCastException">Thrown if the setting value cannot be cast to an integer.</exception>
    public int GetIntSetting()
    {
        try
        {
            return (int)Value;
        }
        catch (InvalidCastException ex)
        {
            throw new InvalidCastException("The setting value cannot be cast to a Int.", ex);
        }
    }

    /// <summary>
    /// Retrieves the integer setting value as a 32-bit signed integer from the specified settings dictionary.
    /// </summary>
    /// <param name="settings">A dictionary containing map settings.</param>
    /// <param name="name">The name of the setting to retrieve.</param>
    /// <returns>The integer setting value as a 32-bit signed integer if found; otherwise, throws KeyNotFoundException.</returns>
    /// <exception cref="InvalidCastException">Thrown if the setting value cannot be cast to an integer.</exception>
    public static int GetIntSetting(Dictionary<string, MapSetting> settings, string name)
    {
        if (!settings.TryGetValue(name, out MapSetting value))
            return default;

        return value.GetIntSetting();
    }

    /// <summary>
    /// Tries to retrieve an integer setting from the provided dictionary of settings.
    /// </summary>
    /// <param name="settings">A dictionary containing the settings.</param>
    /// <param name="name">The name of the setting to retrieve.</param>
    /// <param name="value">The output integer value of the setting if found; otherwise, null.</param>
    /// <returns>True if the setting is found and is a valid integer; otherwise, false.</returns>
    public static bool TryGetIntSetting(Dictionary<string, MapSetting> settings, string name, out int? value)
    {
        value = GetIntSetting(settings, name);

        return value.HasValue;
    }




    /// <summary>
    /// Retrieves the string setting value as a string.
    /// </summary>
    /// <returns>The string setting value.</returns>
    /// <exception cref="InvalidCastException">Thrown if the setting value cannot be cast to a string.</exception>on>
    public string GetStringSetting()
    {
        try
        {
            return (string)Value;
        }
        catch (InvalidCastException ex)
        {
            throw new InvalidCastException("The setting value cannot be cast to a String.", ex);
        }
    }

    /// <summary>
    /// Retrieves the string setting value from the specified settings dictionary.
    /// </summary>
    /// <param name="settings">A dictionary containing map settings.</param>
    /// <param name="name">The name of the setting to retrieve.</param>
    /// <returns>The string setting value if found; otherwise, throws KeyNotFoundException.</returns>
    /// <exception cref="InvalidCastException">Thrown if the setting value cannot be cast to a string.</exception>
    public static string GetStringSetting(Dictionary<string, MapSetting> settings, string name)
    {
        if (!settings.TryGetValue(name, out MapSetting value))
            return default;

        return value.GetStringSetting();
    }

    /// <summary>
    /// Attempts to retrieve a string setting from the provided dictionary of settings.
    /// </summary>
    /// <param name="settings">A dictionary containing the settings.</param>
    /// <param name="name">The name of the setting to retrieve.</param>
    /// <param name="value">The output string value of the setting if found; otherwise, null.</param>
    /// <returns>True if the setting is found and is a valid string; otherwise, false.</returns>
    public static bool TryGetStringSetting(Dictionary<string, MapSetting> settings, string name, out string value)
    {
        value = GetStringSetting(settings, name);

        return value is not null;
    }



    /// <summary>
    /// Retrieves the point setting value as a Vect2.
    /// </summary>
    /// <returns>The point setting value as a Vect2.</returns>
    /// <exception cref="InvalidCastException">Thrown if the setting value cannot be cast to a Vect2.</exception>
    public Vect2 GetPointSetting()
    {
        try
        {
            return (Vect2)Value;
        }
        catch (InvalidCastException ex)
        {
            throw new InvalidCastException("The setting value cannot be cast to a Vect2.", ex);
        }
    }

    /// <summary>
    /// Retrieves the point setting value from the specified settings dictionary.
    /// </summary>
    /// <param name="settings">A dictionary containing map settings.</param>
    /// <param name="name">The name of the setting to retrieve.</param>
    /// <returns>The point setting value as a Vect2 if found; otherwise, throws KeyNotFoundException.</returns>
    /// <exception cref="InvalidCastException">Thrown if the setting value cannot be cast to a Vect2.</exception>
    public static Vect2 GetPointSetting(Dictionary<string, MapSetting> settings, string name)
    {
        if (!settings.TryGetValue(name, out MapSetting value))
            return default;

        return value.GetPointSetting();
    }

    /// <summary>
    /// Attempts to retrieve a tile setting from the provided dictionary of settings.
    /// </summary>
    /// <param name="settings">A dictionary containing the settings.</param>
    /// <param name="name">The name of the setting to retrieve.</param>
    /// <param name="value">The output tile value of the setting if found; otherwise, null.</param>
    /// <returns>True if the setting is found and is a valid tile; otherwise, false.</returns>
    public static bool TryGetPointSetting(Dictionary<string, MapSetting> settings, string name, out Vect2? value)
    {
        value = GetPointSetting(settings, name);

        return value.HasValue;
    }



    /// <summary>
    /// Retrieves the tile setting value as a MapTile.
    /// </summary>
    /// <returns>The tile setting value as a MapTile.</returns>
    /// <exception cref="InvalidCastException">Thrown if the setting value cannot be cast to a MapTile.</exception>
    public MapTile GeTileSetting()
    {
        try
        {
            return (MapTile)Value;
        }
        catch (InvalidCastException ex)
        {
            throw new InvalidCastException("The setting value cannot be cast to a MapTile.", ex);
        }
    }

    /// <summary>
    /// Retrieves the tile setting value from the specified settings dictionary.
    /// </summary>
    /// <param name="settings">A dictionary containing map settings.</param>
    /// <param name="name">The name of the setting to retrieve.</param>
    /// <returns>The tile setting value as a MapTile if found; otherwise, throws KeyNotFoundException.</returns>
    /// <exception cref="InvalidCastException">Thrown if the setting value cannot be cast to a MapTile.</exception>
    public static MapTile GeTileSetting(Dictionary<string, MapSetting> settings, string name)
    {
        if (!settings.TryGetValue(name, out MapSetting value))
            return default;

        return value.GeTileSetting();
    }

    /// <summary>
    /// Attempts to retrieve a tile setting from the provided dictionary of settings.
    /// </summary>
    /// <param name="settings">A dictionary containing the settings.</param>
    /// <param name="name">The name of the setting to retrieve.</param>
    /// <param name="value">The output tile value of the setting if found; otherwise, null.</param>
    /// <returns>True if the setting is found and is a valid tile; otherwise, false.</returns>
    public static bool TryGeTileSetting(Dictionary<string, MapSetting> settings, string name, out MapTile? value)
    {
        value = GeTileSetting(settings, name);

        return value.HasValue && !value.Value.IsEmpty;
    }









    /// <summary>
    /// Retrieves the boolean array setting value as an IEnumerable of bool.
    /// </summary>
    /// <returns>The boolean array setting value as an IEnumerable of bool.</returns>
    /// <exception cref="InvalidCastException">Thrown if the setting value cannot be cast to an IEnumerable of bool.</exception>
    public IEnumerable<bool> GetBoolArraySetting() //=> (IEnumerable<bool>)Value;
    {
        try
        {
            return (IEnumerable<bool>)Value;
        }
        catch (InvalidCastException ex)
        {
            throw new InvalidCastException("The setting value cannot be cast to a BoolArray.", ex);
        }
    }

    /// <summary>
    /// Retrieves the boolean array setting value from the specified settings dictionary.
    /// </summary>
    /// <param name="settings">A dictionary containing map settings.</param>
    /// <param name="name">The name of the setting to retrieve.</param>
    /// <returns>The boolean array setting value as an IEnumerable of bool if found; otherwise, throws KeyNotFoundException.</returns>
    /// <exception cref="InvalidCastException">Thrown if the setting value cannot be cast to an IEnumerable of bool.</exception>
    public static IEnumerable<bool> GetBoolArraySetting(Dictionary<string, MapSetting> settings, string name)
    {
        if (!settings.TryGetValue(name, out MapSetting value))
            return default;

        return value.GetBoolArraySetting();
    }

    /// <summary>
    /// Tries to retrieve a boolean array setting from the provided dictionary of settings.
    /// </summary>
    /// <param name="settings">A dictionary containing the settings.</param>
    /// <param name="name">The name of the setting to retrieve.</param>
    /// <param name="values">The output enumerable of boolean values if found.</param>
    /// <returns>True if the setting is found and contains valid boolean values; otherwise, false.</returns>
    public static bool TryGetBoolArraySetting(Dictionary<string, MapSetting> settings, string name, out IEnumerable<bool> values)
    {
        values = GetBoolArraySetting(settings, name);

        return !values.IsEmpty();
    }



    /// <summary>
    /// Retrieves the color array setting value as an IEnumerable of Color.
    /// </summary>
    /// <returns>The color array setting value as an IEnumerable of Color.</returns>
    /// <exception cref="InvalidCastException">Thrown if the setting value cannot be cast to an IEnumerable of Color.</exception>
    public IEnumerable<BoxColor> GetColorArraySetting() //=> (IEnumerable<Color>)Value;
    {
        try
        {
            return (IEnumerable<BoxColor>)Value;
        }
        catch (InvalidCastException ex)
        {
            throw new InvalidCastException("The setting value cannot be cast to a ColorArray.", ex);
        }
    }

    /// <summary>
    /// Retrieves the color array setting value from the specified settings dictionary.
    /// </summary>
    /// <param name="settings">A dictionary containing map settings.</param>
    /// <param name="name">The name of the setting to retrieve.</param>
    /// <returns>The color array setting value as an IEnumerable of Color if found; otherwise, throws KeyNotFoundException.</returns>
    /// <exception cref="InvalidCastException">Thrown if the setting value cannot be cast to an IEnumerable of Color.</exception>
    public static IEnumerable<BoxColor> GetColorArraySetting(Dictionary<string, MapSetting> settings, string name)
    {
        if (!settings.TryGetValue(name, out MapSetting value))
            return default;

        return value.GetColorArraySetting();
    }

    /// <summary>
    /// Tries to retrieve a color array setting from the provided dictionary of settings.
    /// </summary>
    /// <param name="settings">A dictionary containing the settings.</param>
    /// <param name="name">The name of the setting to retrieve.</param>
    /// <param name="values">The output enumerable of Color values if found.</param>
    /// <returns>True if the setting is found and contains valid Color values; otherwise, false.</returns>
    public static bool TryGetColorArraySetting(Dictionary<string, MapSetting> settings, string name, out IEnumerable<BoxColor> values)
    {
        values = GetColorArraySetting(settings, name);

        return !values.IsEmpty();
    }



    /// <summary>
    /// Retrieves the array of MapEntityRef setting value as an IEnumerable of MapEntityRef.
    /// </summary>
    /// <returns>The array of MapEntityRef setting value as an IEnumerable of MapEntityRef.</returns>
    /// <exception cref="InvalidCastException">Thrown if the setting value cannot be cast to an IEnumerable of MapEntityRef.</exception>
    public IEnumerable<MapEntityRef> GetEntityRefArraySetting() // => (IEnumerable<MapEntityRef>)Value;
    {
        try
        {
            return (IEnumerable<MapEntityRef>)Value;
        }
        catch (InvalidCastException ex)
        {
            throw new InvalidCastException("The setting value cannot be cast to a EntityRefArra.", ex);
        }
    }

    /// <summary>
    /// Retrieves the array of MapEntityRef setting value from the specified settings dictionary.
    /// </summary>
    /// <param name="settings">A dictionary containing map settings.</param>
    /// <param name="name">The name of the setting to retrieve.</param>
    /// <returns>The array of MapEntityRef setting value as an IEnumerable of MapEntityRef if found; otherwise, throws KeyNotFoundException.</returns>
    /// <exception cref="InvalidCastException">Thrown if the setting value cannot be cast to an IEnumerable of MapEntityRef.</exception>
    public static IEnumerable<MapEntityRef> GetEntityRefArraySetting(Dictionary<string, MapSetting> settings, string name)
    {
        if (!settings.TryGetValue(name, out MapSetting value))
            return default;

        return value.GetEntityRefArraySetting();
    }

    /// <summary>
    /// Tries to retrieve an array of entity reference settings from the provided dictionary of settings.
    /// </summary>
    /// <param name="settings">A dictionary containing the settings.</param>
    /// <param name="name">The name of the setting to retrieve.</param>
    /// <param name="values">The output enumerable of MapEntityRef values if found.</param>
    /// <returns>True if the setting is found and contains valid MapEntityRef values; otherwise, false.</returns>
    public static bool TryGetEntityRefArraySetting(Dictionary<string, MapSetting> settings, string name, out IEnumerable<MapEntityRef> values)
    {
        values = GetEntityRefArraySetting(settings, name);

        return !values.IsEmpty();
    }



    /// <summary>
    /// Retrieves the array of enum setting value as an IEnumerable of the specified enum type T.
    /// </summary>
    /// <typeparam name="T">The type of the enum.</typeparam>
    /// <returns>The array of enum setting value as an IEnumerable of T.</returns>
    /// <exception cref="InvalidCastException">Thrown if the setting value cannot be cast to an IEnumerable of the specified enum type T.</exception>
    public IEnumerable<T> GetEnumArraySetting<T>() where T : struct
    {
        List<string> array = (List<string>)Value;
        List<T> result = new();

        foreach (var item in array)
        {
            try
            {
                result.Add(Enum.Parse<T>(item, true));
            }
            catch (InvalidCastException ex)
            {
                throw new InvalidCastException("The setting value cannot be cast to a EnumArray.", ex);
            }
        }

        return result;
    }

    /// <summary>
    /// Retrieves the array of enum setting value from the specified settings dictionary.
    /// </summary>
    /// <typeparam name="T">The type of the enum.</typeparam>
    /// <param name="settings">A dictionary containing map settings.</param>
    /// <param name="name">The name of the setting to retrieve.</param>
    /// <returns>The array of enum setting value as an IEnumerable of T if found; otherwise, throws KeyNotFoundException.</returns>
    /// <exception cref="InvalidCastException">Thrown if the setting value cannot be cast to an IEnumerable of the specified enum type T.</exception>
    public static IEnumerable<T> GetEnumArraySetting<T>(Dictionary<string, MapSetting> settings, string name) where T : struct
    {
        if (!settings.TryGetValue(name, out MapSetting value))
            return default;

        return value.GetEnumArraySetting<T>();
    }

    /// <summary>
    /// Tries to retrieve an array of enum settings of type <typeparamref name="T"/> from the provided dictionary of settings.
    /// </summary>
    /// <typeparam name="T">The enum type to retrieve.</typeparam>
    /// <param name="settings">A dictionary containing the settings.</param>
    /// <param name="name">The name of the setting to retrieve.</param>
    /// <param name="values">The output enumerable of enum values if found.</param>
    /// <returns>True if the setting is found and contains valid enum values of type <typeparamref name="T"/>; otherwise, false.</returns>
    public static bool TryGetEnumArraySetting<T>(Dictionary<string, MapSetting> settings, string name, out IEnumerable<T> values) where T : struct
    {
        values = GetEnumArraySetting<T>(settings, name);

        return !values.IsEmpty();
    }



    /// <summary>
    /// Retrieves the array of file path setting value as an IEnumerable of strings.
    /// </summary>
    /// <returns>The array of file path setting value as an IEnumerable of strings.</returns>
    /// <exception cref="InvalidCastException">Thrown if the setting value cannot be cast to an IEnumerable of strings.</exception>
    public IEnumerable<string> GetFilepathArraySetting()
    {
        try
        {
            return (IEnumerable<string>)Value;
        }
        catch (InvalidCastException ex)
        {
            throw new InvalidCastException("The setting value cannot be cast to a FilepathArray.", ex);
        }
    }

    /// <summary>
    /// Retrieves the array of file path setting value from the specified settings dictionary.
    /// </summary>
    /// <param name="settings">A dictionary containing map settings.</param>
    /// <param name="name">The name of the setting to retrieve.</param>
    /// <returns>The array of file path setting value as an IEnumerable of strings if found; otherwise, throws KeyNotFoundException.</returns>
    /// <exception cref="InvalidCastException">Thrown if the setting value cannot be cast to an IEnumerable of strings.</exception>
    public static IEnumerable<string> GetFilepathArraySetting(Dictionary<string, MapSetting> settings, string name)
    {
        if (!settings.TryGetValue(name, out MapSetting value))
            return default;

        return value.GetFilepathArraySetting();
    }

    /// <summary>
    /// Tries to retrieve an array of filepath settings from the provided dictionary of settings.
    /// </summary>
    /// <param name="settings">A dictionary containing the settings.</param>
    /// <param name="name">The name of the setting to retrieve.</param>
    /// <param name="values">The output enumerable of filepath values if found.</param>
    /// <returns>True if the setting is found and contains valid filepath values; otherwise, false.</returns>
    public static bool TryGetFilepathArraySetting(Dictionary<string, MapSetting> settings, string name, out IEnumerable<string> values)
    {
        values = GetFilepathArraySetting(settings, name);

        return !values.IsEmpty();
    }



    /// <summary>
    /// Retrieves the array of float setting value as an IEnumerable of floats.
    /// </summary>
    /// <returns>The array of float setting value as an IEnumerable of floats.</returns>
    /// <exception cref="InvalidCastException">Thrown if the setting value cannot be cast to an IEnumerable of floats.</exception>
    public IEnumerable<float> GetFloatArraySetting()
    {
        try
        {
            return (IEnumerable<float>)Value;
        }
        catch (InvalidCastException ex)
        {
            throw new InvalidCastException("The setting value cannot be cast to a FloatArray.", ex);
        }
    }

    /// <summary>
    /// Retrieves the array of float setting value from the specified settings dictionary.
    /// </summary>
    /// <param name="settings">A dictionary containing map settings.</param>
    /// <param name="name">The name of the setting to retrieve.</param>
    /// <returns>The array of float setting value as an IEnumerable of floats if found; otherwise, throws KeyNotFoundException.</returns>
    /// <exception cref="InvalidCastException">Thrown if the setting value cannot be cast to an IEnumerable of floats.</exception>
    public static IEnumerable<float> GetFloatArraySetting(Dictionary<string, MapSetting> settings, string name)
    {
        if (!settings.TryGetValue(name, out MapSetting value))
            return default;

        return value.GetFloatArraySetting();
    }

    /// <summary>
    /// Tries to retrieve an array of float settings from the provided dictionary of settings.
    /// </summary>
    /// <param name="settings">A dictionary containing the settings.</param>
    /// <param name="name">The name of the setting to retrieve.</param>
    /// <param name="values">The output enumerable of float values if found.</param>
    /// <returns>True if the setting is found and contains valid float values; otherwise, false.</returns>
    public static bool TryGetFloatArraySetting(Dictionary<string, MapSetting> settings, string name, out IEnumerable<float> values)
    {
        values = GetFloatArraySetting(settings, name);

        return !values.IsEmpty();
    }



    /// <summary>
    /// Retrieves the array of int setting value as an IEnumerable of ints.
    /// </summary>
    /// <returns>The array of int setting value as an IEnumerable of ints.</returns>
    /// <exception cref="InvalidCastException">Thrown if the setting value cannot be cast to an IEnumerable of ints.</exception>
    public IEnumerable<int> GetIntArraySetting()
    {
        try
        {
            return (IEnumerable<int>)Value;
        }
        catch (InvalidCastException ex)
        {
            throw new InvalidCastException("The setting value cannot be cast to a IntArray.", ex);
        }
    }

    /// <summary>
    /// Retrieves the array of int setting value from the specified settings dictionary.
    /// </summary>
    /// <param name="settings">A dictionary containing map settings.</param>
    /// <param name="name">The name of the setting to retrieve.</param>
    /// <returns>The array of int setting value as an IEnumerable of ints if found; otherwise, throws KeyNotFoundException.</returns>
    /// <exception cref="InvalidCastException">Thrown if the setting value cannot be cast to an IEnumerable of ints.</exception>
    public static IEnumerable<int> GetIntArraySetting(Dictionary<string, MapSetting> settings, string name)
    {
        if (!settings.TryGetValue(name, out MapSetting value))
            return default;

        return value.GetIntArraySetting();
    }

    /// <summary>
    /// Tries to retrieve an array of integer settings from the provided dictionary of settings.
    /// </summary>
    /// <param name="settings">A dictionary containing the settings.</param>
    /// <param name="name">The name of the setting to retrieve.</param>
    /// <param name="values">The output enumerable of integer values if found.</param>
    /// <returns>True if the setting is found and contains valid integer values; otherwise, false.</returns>
    public static bool TryGetIntArraySetting(Dictionary<string, MapSetting> settings, string name, out IEnumerable<int> values)
    {
        values = GetIntArraySetting(settings, name);

        return !values.IsEmpty();
    }



    /// <summary>
    /// Retrieves the array of string setting value as an IEnumerable of strings.
    /// </summary>
    /// <returns>The array of string setting value as an IEnumerable of strings.</returns>
    /// <exception cref="InvalidCastException">Thrown if the setting value cannot be cast to an IEnumerable of strings.</exception>
    public IEnumerable<string> GetStringArraySetting()
    {
        try
        {
            return (IEnumerable<string>)Value;
        }
        catch (InvalidCastException ex)
        {
            throw new InvalidCastException("The setting value cannot be cast to a StringArray.", ex);
        }
    }

    /// <summary>
    /// Retrieves the array of string setting value from the specified settings dictionary.
    /// </summary>
    /// <param name="settings">A dictionary containing map settings.</param>
    /// <param name="name">The name of the setting to retrieve.</param>
    /// <returns>The array of string setting value as an IEnumerable of strings if found; otherwise, throws KeyNotFoundException.</returns>
    /// <exception cref="InvalidCastException">Thrown if the setting value cannot be cast to an IEnumerable of strings.</exception>
    public static IEnumerable<string> GetStringArraySetting(Dictionary<string, MapSetting> settings, string name)
    {
        if (!settings.TryGetValue(name, out MapSetting value))
            return default;

        return value.GetStringArraySetting();
    }

    /// <summary>
    /// Tries to retrieve an array of string settings from the provided dictionary of settings.
    /// </summary>
    /// <param name="settings">A dictionary containing the settings.</param>
    /// <param name="name">The name of the setting to retrieve.</param>
    /// <param name="values">The output enumerable of string values if found.</param>
    /// <returns>True if the setting is found and contains valid string values; otherwise, false.</returns>
    public static bool TryGetStringArraySetting(Dictionary<string, MapSetting> settings, string name, out IEnumerable<string> values)
    {
        values = GetStringArraySetting(settings, name);

        return !values.IsEmpty();
    }



    /// <summary>
    /// Retrieves the array of Vect2 setting value as an IEnumerable of Vect2.
    /// </summary>
    /// <returns>The array of Vect2 setting value as an IEnumerable of Vect2.</returns>
    /// <exception cref="InvalidCastException">Thrown if the setting value cannot be cast to an IEnumerable of Vect2.</exception>
    public IEnumerable<Vect2> GetPointArraySetting()
    {
        try
        {
            return (IEnumerable<Vect2>)Value;
        }
        catch (InvalidCastException ex)
        {
            throw new InvalidCastException("The setting value cannot be cast to a PointArray.", ex);
        }
    }

    /// <summary>
    /// Retrieves the array of Vect2 setting value from the specified settings dictionary.
    /// </summary>
    /// <param name="settings">A dictionary containing map settings.</param>
    /// <param name="name">The name of the setting to retrieve.</param>
    /// <returns>The array of Vect2 setting value as an IEnumerable of Vect2 if found; otherwise, throws KeyNotFoundException.</returns>
    /// <exception cref="InvalidCastException">Thrown if the setting value cannot be cast to an IEnumerable of Vect2.</exception>
    public static IEnumerable<Vect2> GetPointArraySetting(Dictionary<string, MapSetting> settings, string name)
    {
        if (!settings.TryGetValue(name, out MapSetting value))
            return default;

        return value.GetPointArraySetting();
    }

    /// <summary>
    /// Tries to retrieve an array of Vect2 (point) settings from the provided dictionary of settings.
    /// </summary>
    /// <param name="settings">A dictionary containing the settings.</param>
    /// <param name="name">The name of the setting to retrieve.</param>
    /// <param name="values">The output enumerable of Vect2 (point) values if found.</param>
    /// <returns>True if the setting is found and contains valid Vect2 (point) values; otherwise, false.</returns>
    public static bool TryGetPointArraySetting(Dictionary<string, MapSetting> settings, string name, out IEnumerable<Vect2> values)
    {
        values = GetPointArraySetting(settings, name);

        return !values.IsEmpty();
    }



    /// <summary>
    /// Retrieves the array of MapTile setting value as an IEnumerable of MapTile.
    /// </summary>
    /// <returns>The array of MapTile setting value as an IEnumerable of MapTile.</returns>
    /// <exception cref="InvalidCastException">Thrown if the setting value cannot be cast to an IEnumerable of MapTile.</exception>
    public IEnumerable<MapTile> GeTileArraySetting()
    {
        try
        {
            return (IEnumerable<MapTile>)Value;
        }
        catch (InvalidCastException ex)
        {
            throw new InvalidCastException("The setting value cannot be cast to a TileArray.", ex);
        }
    }

    /// <summary>
    /// Retrieves the array of MapTile setting value from the specified settings dictionary.
    /// </summary>
    /// <param name="settings">A dictionary containing map settings.</param>
    /// <param name="name">The name of the setting to retrieve.</param>
    /// <returns>The array of MapTile setting value as an IEnumerable of MapTile if found; otherwise, throws KeyNotFoundException.</returns>
    /// <exception cref="InvalidCastException">Thrown if the setting value cannot be cast to an IEnumerable of MapTile.</exception>
    public static IEnumerable<MapTile> GetTileArraySetting(Dictionary<string, MapSetting> settings, string name)
    {
        if (!settings.TryGetValue(name, out MapSetting value))
            return default;

        return value.GeTileArraySetting();
    }

    /// <summary>
    /// Tries to retrieve an array of MapTile settings from the provided dictionary of settings.
    /// </summary>
    /// <param name="settings">A dictionary containing the settings.</param>
    /// <param name="name">The name of the setting to retrieve.</param>
    /// <param name="values">The output enumerable of MapTile values if found.</param>
    /// <returns>True if the setting is found and contains valid MapTile values; otherwise, false.</returns>
    public static bool TryGetTileArraySetting(Dictionary<string, MapSetting> settings, string name, out IEnumerable<MapTile> values)
    {
        values = GetTileArraySetting(settings, name);

        return !values.IsEmpty();
    }
}
