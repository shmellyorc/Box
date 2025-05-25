# BoxGameEngine

BoxEngine is a simple game engine for building 2D games in .NET.

## Features

- Easy to use
- High performance
- Cross-platform
- Does not require an editor to create entities
- And much more...

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
        using var game = new Engine(new EngineSettings
        {
            Window = new Vect2(1920, 1080),
            Viewport = new(320, 180),
            Screens = [new BootScreen()],
            UseTextureHalfOffset = true,
            DebugDraw = false,
        });

        game.Start();
    }

    StartGame();
    ```

## License

MIT