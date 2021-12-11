namespace Shared
{
    public class Beer
    {
        private static int _counter = 0;
        public int Id { get; set; }
        public string Name { get; set; }
        public string Manufacturer { get; set; }

        public Beer()
        {
            Id = ++_counter;
        }
    }
}
