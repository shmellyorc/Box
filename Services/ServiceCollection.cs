using Box.Services.Types;

namespace Box.Services;

/// <summary>
/// Represents a collection of services that can be added and accessed within the application.
/// </summary>
/// <remarks>
/// This class implements the <see cref="GameService"/> and acts as a container for storing services.
/// It allows services to be added dynamically and provides access to the collection of registered services.
/// </remarks>
public sealed class ServiceCollection : GameService
{
	// Improving to get O(1) lookups:
	private readonly Dictionary<Type, GameService> _services = new();
	private readonly List<UpdatableService> _updatableService = new();

	/// <summary>
	/// Gets a read-only list of all currently registered game services.
	/// </summary>
	/// <remarks>
	/// This provides a snapshot of the service collection at the time of access.
	/// Each service in the list derives from <see cref="GameService"/>.
	/// </remarks>
	public IReadOnlyList<GameService> Services => _services.Values.ToList();

	/// <summary>
	/// Registers a <see cref="GameService"/> instance by its concrete type. 
	/// Throws an exception if the service is <c>null</c> or if a service of the same type is already registered.
	/// </summary>
	/// <param name="service">The service instance to register.</param>
	/// <exception cref="ArgumentNullException">Thrown if the service instance is <c>null</c>.</exception>
	/// <exception cref="InvalidOperationException">Thrown if a service of the same type is already registered.</exception>
	public void RegisterService(GameService service)
	{
		if (service == null)
			throw new ArgumentNullException(nameof(service), "Cannot register a null service instance.");

		var type = service.GetType();
		if (_services.TryGetValue(type, out _))
			throw new InvalidOperationException($"A service of type '{type.FullName}' is already registered.");

		_services[service.GetType()] = service;

		if (service is UpdatableService udService)
			_updatableService.Add(udService);

		service.Initialize();
	}

	public void RegisterManyServices(params GameService[] services)
	{
		if (services.IsEmpty())
			return;

		for (int i = 0; i < services.Length; i++)
		{
			if (services[i] == null)
				throw new ArgumentNullException($"{services[i].GetType().Name} is null, cannot register a null service instance.");

			var type = services[i].GetType();
			if (_services.TryGetValue(type, out _))
				throw new InvalidOperationException($"A service of type '{type.FullName}' is already registered.");

			_services[type] = services[i];

			if (services[i] is UpdatableService udService)
				_updatableService.Add(udService);
		}

		// initialize after inserted, to make sure those services don't rely on each other:
		for (int i = 0; i < services.Length; i++)
			services[i].Initialize();
	}

	/// <summary>
	/// Determines whether a <see cref="GameService"/> of the specified type is currently registered.
	/// </summary>
	/// <typeparam name="T">The concrete type of the service to check.</typeparam>
	/// <returns><c>true</c> if a service of type <typeparamref name="T"/> is registered; otherwise, <c>false</c>.</returns>
	public bool HasService<T>() where T : GameService => _services.ContainsKey(typeof(T));

	/// <summary>
	/// Attempts to retrieve a registered service of the specified type.
	/// </summary>
	/// <typeparam name="T">The type of <see cref="GameService"/> to retrieve.</typeparam>
	/// <param name="service">
	/// When this method returns, contains the requested service instance if found; otherwise, <c>null</c>.
	/// </param>
	/// <returns><c>true</c> if the service was found and assigned; otherwise, <c>false</c>.</returns>
	public bool TryGetService<T>(out T service) where T : GameService
	{
		service = GetService<T>();

		return service != null;
	}

	/// <summary>
	/// Retrieves a registered service of the specified type.
	/// </summary>
	/// <typeparam name="T">The concrete type of the <see cref="GameService"/> to retrieve.</typeparam>
	/// <returns>
	/// The registered service instance if found; otherwise, <c>null</c>.
	/// </returns>
	public T GetService<T>() where T : GameService =>
		_services.TryGetValue(typeof(T), out var obj) && obj is T casted
			? casted : default;

	internal void Update()
	{
		if (_updatableService.Count == 0)
			return;

		for (int i = _updatableService.Count - 1; i >= 0; i--)
		{
			if (!_updatableService[i].Enabled)
				continue;

			_updatableService[i].Update();
		}
	}
}
