namespace LaChasseAuTresor_ISY.Models
{
    public class Adventurer
    {
        public string Name { get; set; } // Adventurer's Name
        public int X { get; set; }
        public int Y { get; set; }
        public char Orientation { get; set; }  // Set as N, S, E, W
        public Queue<char> Moves { get; set; } = new();  // Example {A, D, A, G}
        public int CollectedTreasures { get; set; } = 0;
    }
}
