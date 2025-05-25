using System.Runtime.CompilerServices;

using Box.Graphics.Batch;

namespace Box.Services.Types;

/// <summary>
/// Base class for all engine service modules. Provides centralized access to core engine systems
/// such as rendering, audio, assets, input, and more.
/// </summary>
public class GameService
{
	/// <summary>
	/// Determines whether the service is enabled and should be updated.
	/// </summary>
	public bool Enabled { get; set; } = true;

	#region Property Helpers

	/// <summary>
	/// Gets the singleton instance of the core engine.
	/// </summary>
	protected Engine Engine => GetService<Engine>();

	/// <summary>
	/// Gets the coroutine manager for async and delayed operations.
	/// </summary>
	protected Coroutine Coroutine => GetService<Coroutine>();

	/// <summary>
	/// Gets the screen manager responsible for scene stack and transitions.
	/// </summary>
	protected ScreenManager ScreenManager => GetService<ScreenManager>();

	/// <summary>
	/// Gets the asset manager for loading and accessing game content.
	/// </summary>
	protected Assets Assets => GetService<Assets>();

	/// <summary>
	/// Gets the sound manager for handling sound effects and music playback.
	/// </summary>
	protected SoundManager SoundManager => GetService<SoundManager>();

	/// <summary>
	/// Gets the renderer used for all drawing and batching operations.
	/// </summary>
	protected Renderer Renderer => GetService<Renderer>();

	/// <summary>
	/// Gets the signal/event system for decoupled communication.
	/// </summary>
	protected Signal Signal => GetService<Signal>();

	/// <summary>
	/// Gets the current input map from the engine (keyboard, gamepad, etc.).
	/// </summary>
	protected InputMap Input => Engine.Input;

	/// <summary>
	/// Gets the engine's clock, used for tracking time and delta updates.
	/// </summary>
	protected Clock Clock => GetService<Clock>();

	/// <summary>
	/// Gets the engine's logging system.
	/// </summary>
	protected Log Log => GetService<Log>();

	/// <summary>
	/// Gets the engine's random number generator.
	/// </summary>
	protected Rand Rand => Engine.GetService<Rand>();

	/// <summary>
	/// Retrieves a registered engine service of the specified type.
	/// </summary>
	/// <typeparam name="T">The type of the service to retrieve.</typeparam>
	/// <returns>The singleton instance of the service, or <c>null</c> if not registered.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected T GetService<T>() where T : GameService => Engine.GetService<T>();

	#endregion

	/// <summary>
	/// Called once to initialize the service. Override to implement startup logic.
	/// </summary>
	public virtual void Initialize() { }
}
