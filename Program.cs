using System;
using Serializer;

namespace HttpClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string url = "http://127.0.0.1";
            int port = GetPort();


            Client client = new Client(url, port);

            while (!await client.Ping())
            {
                Thread.Sleep(1000);
            };

            Input? inpt = null;
            while(inpt is null)
            {
                Thread.Sleep(1000);
                inpt = await client.GetInputData();
                if (inpt is null)
                {
                    Console.WriteLine("Input Data has not been received");
                }
            }

            Console.WriteLine("Input Data has been received");
            Output oupt = Serializer.Serializer.GetOutputObject(inpt);


            while (!await client.WriteAnswer(oupt))
            {
                Thread.Sleep(1000);
                Console.WriteLine("Answer has not been sent");
            };
            Console.WriteLine("Answer has been sent");
        }

        private static int GetPort()
        {
            System.Console.WriteLine("Enter a port (0..65536): ");

            int port = 0;
            while (!int.TryParse(Console.ReadLine(), out port))
            {
                System.Console.WriteLine("Wrong format. Try again. It should be 0..65536)");
            }
            if (port < 0 || port > 65535)
            {
                GetPort();
            }

            return port;
        }
    }
}
