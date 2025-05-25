# Box Engine Features

An ultra‑efficient, code‑first C# game engine built to give you complete control and blazing performance. Every core system—from rendering and audio to input handling and UI—is defined programmatically, removing any reliance on visual editors or external tools. Box Engine’s architecture is optimized for real‑time responsiveness, seamless updates, and easy integration into your existing workflows—all through pure C# code.

> **Note:** Many features are currently in alpha and subject to change as the engine evolves.

## Core Systems

* **Service Host**: All engine subsystems run as services under a central host—rendering, audio, input, coroutines, signals, and more. You can attach your own services (persisting across the app’s lifecycle) to manage game data, settings, or custom workflows—no need to wrap your data around engine APIs.
* **Pub/Sub**: A built-in signal/event bus that enables true loose coupling. Systems can emit and subscribe to events without direct references to each other, keeping code modular and testable.
* **Advanced Coroutines**: Sequencing, nested routines, and update-driven tasks with minimal boilerplate.
* **Easings & Tweens**: Built-in library of easing functions and animation tools.
* **Lightweight Coroutines**: Frame-based cooperative routines that can pause and resume logic across updates, enabling non-blocking, sequential workflows with minimal boilerplate.

## Rendering & Performance

* **GPU-Accelerated Rendering**: Complete use of vertex buffers, draw commands, and dynamic batching for minimal draw calls and maximum throughput.
* **Lazy Loading & Smooth Framerates**: Assets, pathfinding data, and simulation components load on demand to minimize startup overhead and maintain consistently smooth frame rates throughout gameplay.
* **Advanced Culling & LOD**: View frustum and occlusion culling, plus level-of-detail support to optimize rendering workloads.
* **Dynamic Resolution Scaling**: Automatically adjust render resolution at runtime to maintain target FPS under load.
* **Graph-Based Pathfinding**: Customizable modes, weights, and heuristics for AI navigation.
* **Fast Random Utilities**: High-performance RNG with range methods for int, float, and double.
* **Performance-First Architecture**: Continuous profiling hooks, low-allocation design, and optimization opportunities for critical code paths.## Content & Asset Management
* **File System Abstraction**: Flexible content folders (Assets, Content, or custom) and extension-less loading.
* **Cross-Platform Saves**: Binary save system with AppData support for Windows, macOS, and Linux.
* **Built-In Loaders**: FNT fonts, Aseprite spritesheets, and LDTK map importer (data-only).

## Audio & Input

* **Sound Manager**: Play music, SFX, and sound banks with channel routing.
* **Input Support**: Keyboard, mouse, controllers, and Steam API bindings.

## UI & Entities

* **Entity Framework**: Entities and UI share the same system. Create panels, grid layouts, list views, sprites, nine-patches, labels, color rects, and more—all in code.
* **XML Injection**: Define and load entity and scene layouts through XML files. Simply author your entities, components, and properties in XML, and the engine will parse and instantiate them at startup or runtime for immediate use.
* **DSL Support (Coming Soon)**: Define entities and scenes programmatically with a dedicated domain-specific language. Future enhancements include a visual scene editor and live-reload for rapid iteration.

## Utilities & Extensions

* **Extension Methods**: String, numeric, enumerable, coroutine, and more helpers.
* **Loot & Drop Tables**: Built-in utility features for loot generation and value ranges.

## Debugging & Tools

* **Built-In Profiler & Metrics**: Frame timing, memory usage, and custom metric hooks to identify hotspots in real time.
* **Runtime Console & Logging**: On-screen console for commands and live log output with configurable verbosity levels.


## Getting Started

1. Install the package:
    ```sh
    dotnet add package BoxEngine
    ```

2. Usage example:
    ```csharp
    using Box;

    [STAThread]
    static void StartGame()
    {
        var settings = new EngineSettings()
            .WithAppName("BoxGame")
            .WithAppTitle("Box Game")
            .WithAppVersion("1.0a")
            .WithWindow(1280, 720)
            .WithViewport(320, 180)
            .WithMaxDrawCalls(1024)
            .WithScreens(new BootScreen())
            .WithServices(new GameData(), new SaveData())
            .Build();

        using var game = new Engine(settings);

        game.Start();
    }

    StartGame();;
    ```

## License

MIT
