namespace Box.Resources;

internal sealed class ResourceLoader
{
	private const string Root = "Box.Resources.Root.";

	public static Stream GetResourceStream(string resourceName)
	{
		var assembly = Assembly.GetExecutingAssembly();

		return assembly.GetManifestResourceStream($"{Root}{resourceName}");
	}

	public static string GetResourceText(string resourceName)
	{
		using (var stream = GetResourceStream(resourceName))
		{
			if (stream == null)
				throw new FileNotFoundException("Resource not found", resourceName);

			using (var reader = new StreamReader(stream))
			{
				return reader.ReadToEnd();
			}
		}
	}

	public static byte[] GetResourceBytes(string resourceName)
	{
		using (var stream = GetResourceStream(resourceName))
		{
			if (stream == null)
				throw new FileNotFoundException("Resource not found", resourceName);

			using (var memoryStream = new MemoryStream())
			{
				stream.CopyTo(memoryStream);

				return memoryStream.ToArray();
			}
		}
	}
}
