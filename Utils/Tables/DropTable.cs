using System.Collections.Generic;

namespace Box.Utils.Tables;

//Eg:
//		foreach (var item in CreateSlimeTable().RollLoot())
// 			GameData.AddToInventory(Item);

/// <summary>
/// Represents a single drop entry in a drop table, including item ID, drop chance, and quantity range.
/// </summary>
public readonly struct DropEntry
{
	/// <summary>
	/// Gets the ID of the item to be dropped.
	/// </summary>
	public string ItemId { get; }

	/// <summary>
	/// Gets the chance (0 to 1) for this item to drop.
	/// </summary>
	public float Chance { get; }

	/// <summary>
	/// Gets the minimum amount of the item to drop.
	/// </summary>
	public int MinAmount { get; }

	/// <summary>
	/// Gets the maximum amount of the item to drop.
	/// </summary>
	public int MaxAmount { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="DropEntry"/> struct.
	/// </summary>
	/// <param name="itemId">The ID of the item to drop.</param>
	/// <param name="chance">The drop chance, clamped between 0 and 1.</param>
	/// <param name="min">The minimum quantity to drop. Must not be greater than <paramref name="max"/>.</param>
	/// <param name="max">The maximum quantity to drop.</param>
	/// <exception cref="ArgumentException">
	/// Thrown when <paramref name="min"/> is greater than <paramref name="max"/>.
	/// </exception>
	public DropEntry(string itemId, float chance, int min = 1, int max = 1)
	{
		if (min > max)
			throw new ArgumentException($"MinAmount ({min}) cannot be greater than MaxAmount ({max}).");

		ItemId = itemId;
		Chance = Math.Clamp(chance, 0f, 1f);
		MinAmount = min;
		MaxAmount = max;
	}
}

/// <summary>
/// Represents the result of a drop roll, including the item ID and quantity.
/// </summary>
public readonly struct DropEntryResult
{
	/// <summary>
	/// Gets the ID of the dropped item.
	/// </summary>
	public string ItemId { get; }

	/// <summary>
	/// Gets the quantity of the dropped item.
	/// </summary>
	public int Count { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="DropEntryResult"/> struct.
	/// </summary>
	/// <param name="itemId">The ID of the dropped item.</param>
	/// <param name="count">The quantity of the dropped item. Must be greater than zero.</param>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Thrown when <paramref name="count"/> is less than 1.
	/// </exception>
	public DropEntryResult(string itemId, int count)
	{
		if (count < 1)
			throw new ArgumentOutOfRangeException(nameof(count), "Count must be at least 1.");

		ItemId = itemId;
		Count = count;
	}
}

/// <summary>
/// Represents a loot table containing multiple drop entries that can be rolled to produce loot results.
/// </summary>
public class DropTable
{
	private readonly List<DropEntry> _entries = new();

	/// <summary>
	/// Adds a new drop entry to the drop table.
	/// </summary>
	/// <param name="entry">The <see cref="DropEntry"/> to add to the drop table.</param>
	public void Add(DropEntry entry) => _entries.Add(entry);

	/// <summary>
	/// Adds a new drop entry to the table.
	/// </summary>
	/// <param name="itemId">The ID of the item to drop.</param>
	/// <param name="chance">The chance for the item to drop (0 to 1).</param>
	/// <param name="min">The minimum quantity to drop.</param>
	/// <param name="max">The maximum quantity to drop.</param>
	public void Add(string itemId, float chance, int min = 1, int max = 1)
		=> Add(new DropEntry(itemId, chance, min, max));

	/// <summary>
	/// Adds a range of drop entries to the table.
	/// </summary>
	/// <param name="entries">The array of drop entries to add.</param>
	public void Add(params DropEntry[] entries) => _entries.AddRange(entries);

	/// <summary>
	/// Rolls the drop table and returns a list of items that were successfully dropped.
	/// </summary>
	/// <returns>A list of <see cref="DropEntryResult"/> representing the dropped items.</returns>
	public List<DropEntryResult> RollLoot()
	{
		var loot = new List<DropEntryResult>();

		foreach (var entry in _entries)
		{
			if (FastRandom.Instance.NextFloat() < entry.Chance)
			{
				int amount = FastRandom.Instance.Range(entry.MinAmount, entry.MaxAmount);

				loot.Add(new DropEntryResult(entry.ItemId, amount));
			}
		}



		return loot;
	}
}
