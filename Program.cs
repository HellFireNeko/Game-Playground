using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Debug()
    .WriteTo.File("Logs/Log.txt")
    .CreateLogger();

using var game = new GamePlayground.Game1();
game.Run();
