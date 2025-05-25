namespace Box.Helpers;

public static class HashHelpers
{
	public static uint Hash32(string data)
	{
		const uint offset = 2166136261;
		const uint prime = 16777619;

		uint hash = offset;
		foreach (byte b in Encoding.UTF8.GetBytes(data))
		{
			hash ^= b;
			hash *= prime;
		}
		return hash;
	}
}
