namespace LaChasseAuTresor_ISY.Models
{
    public class Map
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public List<Mountain> Mountains { get; set; } = new();
        public List<Treasure> Treasures { get; set; } = new();
        public List<Adventurer> Adventurers { get; set; } = new();
    }
}
