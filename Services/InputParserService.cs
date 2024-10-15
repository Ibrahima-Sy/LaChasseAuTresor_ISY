using LaChasseAuTresor_ISY.Models;
namespace LaChasseAuTresor_ISY.Services
{
    public class InputParserService
    {
        public Map ParseInputFile(IFormFile inputFile)
        {
            Map map = null;
            bool hasAdventurer = false;

            using (var reader = new StreamReader(inputFile.OpenReadStream()))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith("#")) continue;

                    var parts = line.Split(" - ");

                    switch (parts[0])
                    {
                        case "C":  // Map definition
                            if (map != null) throw new Exception("Please define one single map");

                            map = new Map
                            {
                                Width = int.Parse(parts[1]),
                                Height = int.Parse(parts[2])
                            };
                            break;

                        case "M":  // Mountain definition
                            if (map == null) throw new Exception("Define the map before adding elements");

                            ValidateCoordinates(int.Parse(parts[1]), int.Parse(parts[2]), map);

                            map.Mountains.Add(new Mountain
                            {
                                X = int.Parse(parts[1]),
                                Y = int.Parse(parts[2])
                            });
                            break;

                        case "T":  // Treasure definition
                            if (map == null) throw new Exception("Define the map before adding elements");

                            ValidateCoordinates(int.Parse(parts[1]), int.Parse(parts[2]), map);

                            map.Treasures.Add(new Treasure
                            {
                                X = int.Parse(parts[1]),
                                Y = int.Parse(parts[2]),
                                Count = int.Parse(parts[3])
                            });
                            break;

                        case "A":  // Adventurer definition
                            if (map == null) throw new Exception("Define the map before adding elements");

                            ValidateCoordinates(int.Parse(parts[2]), int.Parse(parts[3]), map);

                            var moves = parts[5].ToCharArray();
                            if (moves.Length == 0) throw new Exception($"The adventurer {parts[1]} has no movement sequence");

                            var adventurer = new Adventurer
                            {
                                Name = parts[1],
                                X = int.Parse(parts[2]),
                                Y = int.Parse(parts[3]),
                                Orientation = char.Parse(parts[4])
                            };

                            foreach (var move in moves)
                            {
                                adventurer.Moves.Enqueue(move);
                            }

                            map.Adventurers.Add(adventurer);
                            hasAdventurer = true;
                            break;

                        default:
                            throw new Exception("Invalid line format.");
                    }
                }
            }

            if (map == null) throw new Exception("There was no map in the input file");
            if (!hasAdventurer) throw new Exception("There was no adventurer in the input file");

            return map;
        }

        private void ValidateCoordinates(int x, int y, Map map)
        {
            if (x < 0 || x >= map.Width || y < 0 || y >= map.Height)
            {
                throw new Exception($"Your adventurer needs to start inside the map");
            }
        }
    }
}
