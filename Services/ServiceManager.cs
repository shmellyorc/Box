using Box.Services.Types;

namespace Box.Services;

/// <summary>
/// Represents a collection of services that can be added and accessed within the application.
/// </summary>
/// <remarks>
/// This class implements the <see cref="ServiceManager"/> and acts as a container for storing services.
/// It allows services to be added dynamically and provides access to the collection of registered services.
/// </remarks>
public sealed class ServiceManager
{
	public static ServiceManager Instance { get; private set; }

	// Improving to get O(1) lookups:
	private readonly Dictionary<Type, GameService> _services = new();
	private readonly List<UpdatableService> _updatableServices = new();

	/// <summary>
	/// Gets a read-only list of all currently registered game services.
	/// </summary>
	/// <remarks>
	/// This provides a snapshot of the service collection at the time of access.
	/// Each service in the list derives from <see cref="GameService"/>.
	/// </remarks>
	public IEnumerable<GameService> Services => _services.Values;


	internal ServiceManager() => Instance ??= this;


	/// <summary>
	/// Registers a <see cref="GameService"/> instance by its concrete type. 
	/// Throws an exception if the service is <c>null</c> or if a service of the same type is already registered.
	/// </summary>
	/// <param name="service">The service instance to register.</param>
	/// <exception cref="ArgumentNullException">Thrown if the service instance is <c>null</c>.</exception>
	/// <exception cref="InvalidOperationException">Thrown if a service of the same type is already registered.</exception>
	public void RegisterService<T>(T service) where T : GameService
	{
		if (service is null)
			throw new ArgumentNullException(nameof(service));
		if (_services.ContainsKey(typeof(T)))
			throw new InvalidOperationException($"Service {typeof(T).Name} already registered.");

		_services[typeof(T)] = service;

		if (service is UpdatableService u)
			_updatableServices.Add(u);

		service.Initialize();
	}

	public void RegisterManyServices(params GameService[] services)
	{
		if (services == null || services.Length == 0)
			return;

		foreach (var service in services)
		{
			if (service is null)
				throw new ArgumentNullException(nameof(service));
			if (_services.ContainsKey(service.GetType()))
				throw new InvalidOperationException($"Service {service.GetType().Name} already registered.");

			_services[service.GetType()] = service;
		}

		foreach (var service in services)
			service.Initialize();
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
		if (_services.TryGetValue(typeof(T), out var gs) && gs is T casted)
		{
			service = casted;
			return true;
		}
		service = null;
		return false;
	}

	/// <summary>
	/// Retrieves a registered GameService of the specified type, or null if none is found.
	/// </summary>
	/// <typeparam name="T">The concrete service type to retrieve.</typeparam>
	/// <returns>The service instance, or null.</returns>
	public T? GetService<T>() where T : GameService
		=> _services.TryGetValue(typeof(T), out var svc)
			 ? svc as T
			 : null;

	internal void Update(float dt)
	{
		foreach (var svc in _updatableServices)
			if (svc.Enabled) svc.Update(dt);
	}
}
