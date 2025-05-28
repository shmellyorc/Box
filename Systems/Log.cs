using Box.Services.Types;

namespace Box.Systems;

/// <summary>
/// Provides logging functionality to write messages to a file.
/// </summary>
public sealed class Log
{
	private bool _exited;
	private readonly StreamWriter _writer;
	private readonly string _path;

	/// <summary>
	/// Singleton instance of the Log class.
	/// </summary>
	public static Log Instance { get; private set; }

	internal Log()
	{
		Instance ??= this;

		_path = Path.Combine(
			FileHelpers.GetApplicationDataPath(),
			EngineSettings.Instance.AppLogRoot,
			$"{DateTime.Now.ToShortDateString().Replace("/", "-")}.txt"
		);

		string pathOnly = Path.GetDirectoryName(_path);

		if (!File.Exists(pathOnly))
			Directory.CreateDirectory(pathOnly);

		_writer = new StreamWriter(_path, true);

		AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
		Engine.Instance.OnExiting += OnExiting;

		Trace.Listeners.Add(new TextWriterTraceListener(_writer));
		Trace.AutoFlush = true;

		WriteMessge($"\n\n\n{EngineSettings.Instance.AppName} started...");
	}

	private void OnExiting(Engine engine) => Exit();

	/// <summary>
	/// Finalizes an instance of the Log class.
	/// </summary>
	~Log() => Exit();

	internal void Exit()
	{
		if (!_exited)
		{
			_writer.Flush();
			_writer.Close();

			_exited = true;
		}
	}

	/// <summary>
	/// Prints a string value.
	/// </summary>
	/// <param name="value">The string value to print.</param>
	public void Print(string value) => WriteMessge(value);

	/// <summary>
	/// Prints an object's string representation.
	/// </summary>
	/// <param name="value">The object whose string representation will be printed.</param>
	public void Print(object value) => WriteMessge(value);

	/// <summary>
	/// Prints an object's title and associated text.
	/// </summary>
	/// <param name="title">The title of the object.</param>
	/// <param name="text">The text associated with the object.</param>
	public void PrintObjcet(object title, string text)
		=> WriteMessge($"[{title.GetType().Name}]: {text}");


	/// <summary>
	/// Prints multiple objects in a tabular format.
	/// </summary>
	/// <param name="values">Objects to print in tabular format.</param>
	public void PrintTabbed(params object[] values)
		=> WriteMessge($"{string.Join('\t', values)}");

	/// <summary>
	/// Prints multiple values.
	/// </summary>
	/// <param name="values">Values to print.</param>
	public void PrintMany(params object[] values)
		=> WriteMessge($"{string.Join(' ', values)}");


	/// <summary>
	/// Checks for a condition; if the condition is true, display a message that shows the call stack.
	/// </summary>
	/// <param name="condition">true if passed, false if failed</param>
	/// <param name="message">the message of your assert</param>
	public void Assert(bool condition, string message = "")
	{
		if (condition)
		{
			if (message.IsEmpty())
				throw new Exception();
			else
				throw new Exception(message);
		}
	}

	private void WriteMessge(object message)
	{
		if (EngineSettings.Instance.LogDateTime)
		{
			var dt = DateTime.Now;

			Trace.WriteLine($"[{dt.ToShortDateString()} {dt.ToShortTimeString()}]: {message}");
		}
		else
			Trace.WriteLine(message);
	}

	private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
	{
		Exception error = (Exception)e.ExceptionObject;

		Task.Run(() => EngineSettings.Instance.OnError?.Invoke(Engine.Instance, error));

		string message = $"   {error.Message}";
		string exception = error.InnerException is null ? "   Null" : $"   {error.InnerException}";
		string stack = error.StackTrace is null ? "   Null" : $"{error.StackTrace}";

		Print(
			$"Crash:\n{message}\n\nException:\n{exception}\n\nStack:\n{stack}"
		);

		Exit();
	}
}
