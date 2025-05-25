using System;
using System.Collections.Generic;

using Box.Services.Types;

namespace Box.Signals;

/// <summary>
/// Enables signal passing without direct object access, utilizing a publish/subscribe (pub/sub) pattern.
/// <para>Connect to signals and emit them from different parts of your code. Data can also be passed along with signals.</para>
/// </summary>
/// <remarks>
/// <para>Example:</para>
/// <para>Class 1: Connect("Push", OnPush);</para>
/// <para>Class 2: Emit("Push", box);</para>
/// </remarks>
public sealed class Signal : GameService
{
	private readonly Dictionary<string, Dictionary<string, Action<SignalHandle>>> _items = new();

	/// <summary>
	/// Gets the total number of connected signals in the entire project.
	/// </summary>
	public int Count => _items.Count;

	// /// <summary>
	// /// Represents the current instance of a signal.
	// /// </summary>
	// public static Signal Instance { get; private set; }

	// internal Signal() => Instance ??= this;

	internal void Clear()
	{
		if (_items.Count == 0)
			return;

		_items.Clear();
	}

	#region Connect
	/// <summary>
	/// Connects to a signal that may be emitted later in time.
	/// </summary>
	/// <param name="name">The name of the signal to connect to.</param>
	/// <param name="handle">The handle and data of the emitted signal.</param>
	public void Connect(string name, Action<SignalHandle> handle) => Connect(string.Empty, name, handle);

	/// <summary>
	/// Connects to a signal that may be emitted later in time.
	/// </summary>
	/// <param name="name">The name of the signal to connect to.</param>
	/// <param name="handle">The handle and data of the emitted signal.</param>
	public void Connect(Enum name, Action<SignalHandle> handle) => Connect(string.Empty, name.ToEnumString(), handle);

	private void Connect(string id, string name, Action<SignalHandle> handle)
	{
		if (!_items.ContainsKey(name))
			_items.Add(name, new Dictionary<string, Action<SignalHandle>>());

		if (!_items[name].ContainsKey(id))
			_items[name].Add(id, null);

		_items[name][id] += handle;
	}
	#endregion


	#region Disconnect
	/// <summary>
	/// Disconnects from an active signal.
	/// </summary>
	/// <param name="name">The name of the signal to disconnect from.</param>
	/// <param name="handle">The method or delegate that was connected to this signal.</param>
	public void Disconnect(string name, Action<SignalHandle> handle) => Disconnect(string.Empty, name, handle);

	/// <summary>
	/// Disconnects from an active signal.
	/// </summary>
	/// <param name="name">The name of the signal to disconnect from.</param>
	/// <param name="handle">The method or delegate that was connected to this signal.</param>
	public void Disconnect(Enum name, Action<SignalHandle> handle) => Disconnect(name.ToEnumString(), handle);

	private void Disconnect(string id, string name, Action<SignalHandle> handle)
	{
		if (_items.ContainsKey(name) && _items[name].ContainsKey(id))
		{
			_items[name][id] -= handle;
			if (_items[name][id] is null) _items.Remove(name);
		}
	}
	#endregion


	#region Emit
	/// <summary>
	/// Emits a signal with optional data based on a connected signal.
	/// </summary>
	/// <param name="name">The name of the signal to emit.</param>
	/// <param name="data">Optional data to pass with the signal.</param>
	public void Emit(string name, params object[] data) => Emit(string.Empty, name, data);

	/// <summary>
	/// Emits a signal with optional data based on a connected signal.
	/// </summary>
	/// <param name="name">The name of the signal to emit.</param>
	/// <param name="data">Optional data to pass with the signal.</param>
	public void Emit(Enum name, params object[] data) => Emit(name.ToEnumString(), data);

	/// <summary>
	/// Emits a delayed signal with optional data based on a connected signal.
	/// </summary>
	/// <param name="delay">The delay time in seconds before emitting the signal.</param>
	/// <param name="name">The name of the signal to emit.</param>
	/// <param name="data">Optional data to pass with the signal.</param>
	public void EmitDelayed(float delay, string name, params object[] data)
	{
		IEnumerator routine(float delay, string name, object[] data)
		{
			yield return delay;

			Emit(name, data);
		}

		GetService<Coroutine>().Run(routine(delay, name, data));
	}

	/// <summary>
	/// Emits a delayed signal with optional data based on a connected signal.
	/// </summary>
	/// <param name="delay">The delay time in seconds before emitting the signal.</param>
	/// <param name="name">The name of the signal to emit.</param>
	/// <param name="data">Optional data to pass with the signal.</param>
	public void EmitDelayed(float delay, Enum name, params object[] data)
		=> EmitDelayed(delay, name.ToEnumString(), data);

	private void Emit(string id, string name, params object[] data)
	{
		if (GetService<EngineSettings>().LogSignalEvents)
			Console.WriteLine($"Event Published: '{name}' and {data.Length} parameters");

		ProcessEvent(id, name, data);
	}
	#endregion

	private void ProcessEvent(string id, string name, params object[] data)
	{
		if (_items.TryGetValue(name, out var handlersForType))
		{
			if (handlersForType.TryGetValue(id, out var handlerCare))
			{
				SignalHandle handle;

				if (data.IsEmpty())
					handle = new SignalHandle(name, id, Array.Empty<object>());
				else
					handle = new SignalHandle(name, id, data);

				handlerCare.Invoke(handle);
			}

			// invoke handlers that don't care about a particular id
			if (id != string.Empty && handlersForType.TryGetValue(string.Empty, out var handlerDontCare))
			{
				SignalHandle handle;

				if (data.IsEmpty())
					handle = new SignalHandle(name, id, Array.Empty<object>());
				else
					handle = new SignalHandle(name, id, data);

				handlerDontCare.Invoke(handle);
			}
		}
	}
}
