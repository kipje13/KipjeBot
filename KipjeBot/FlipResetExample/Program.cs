using System.IO;

using RLBotDotNet;

using KipjeBot;

namespace FlipResetExample
{
    class Program
    {
        static void Main(string[] args)
        {
            // Read the port from port.cfg.
            const string file = "port.cfg";
            string text = File.ReadAllLines(file)[0];
            int port = int.Parse(text);

            Physics.LoadMapGeometry("soccar.dat");

            // BotManager is a generic which takes in your bot as its T type.
            BotManager<FlipResetExample> botManager = new BotManager<FlipResetExample>();
            // Start the server on the port given in the port.cfg file.
            botManager.Start(port);
        }
    }
}
