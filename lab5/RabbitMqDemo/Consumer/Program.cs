using Shared;
using System;

namespace Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(" [*] Waiting for beer.");
            BeerConsumer consumer = new BeerConsumer();
            consumer.Consume(ConsumeBeer);


            Console.WriteLine(" Press any button to exit.");
            Console.ReadLine();
        }

        private static void ConsumeBeer(Beer beer)
        {
            Console.WriteLine(" [x] Beer '#{0} {1}' is served, enjoy! (Manufacturer: {2})", beer.Id, beer.Name, beer.Manufacturer);
        }
    }
}
