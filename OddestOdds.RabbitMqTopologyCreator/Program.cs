namespace OddestOdds.RabbitMqTopologyCreator;

public static class Program
{
    public static void Main(string[] args)
    {
        string hostName = "localhost";
        string userName = "guest";
        string password = "guest";
        int port = 5672;

        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--host":
                    if (i + 1 < args.Length) hostName = args[++i];
                    break;
                case "--userName":
                    if (i + 1 < args.Length) userName = args[++i];
                    break;
                case "--password":
                    if (i + 1 < args.Length) password = args[++i];
                    break;
                case "--port":
                    if (i + 1 < args.Length) port = int.Parse(args[++i]);
                    break;
            }
        }

        Messaging.Helper.RabbitMqTopologyCreator rabbitMqTopologyCreator =
            new Messaging.Helper.RabbitMqTopologyCreator(hostName, userName, password, port);
        Console.WriteLine("Setup Started..");
        rabbitMqTopologyCreator.Setup();
        Console.WriteLine("RabbitMq topology is ready.");
    }
}