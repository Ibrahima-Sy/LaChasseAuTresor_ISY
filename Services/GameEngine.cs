using LaChasseAuTresor_ISY.Models;

public class GameEngine
{
    private readonly Map _map;

    public GameEngine(Map map)
    {
        _map = map;
    }

    public void ProcessTurn()
    {
        foreach (var adventurer in _map.Adventurers)
        {
            if (adventurer.Moves.Count > 0)
            {
                var move = adventurer.Moves.Dequeue();

                if (move == 'A') // Move forward
                {
                    MoveAdventurer(adventurer);
                }
                else if (move == 'D') // Turn right
                {
                    TurnAdventurer(adventurer, 'R');
                }
                else if (move == 'G') // Turn left
                {
                    TurnAdventurer(adventurer, 'L');
                }
            }
        }
    }

    private void MoveAdventurer(Adventurer adventurer)
    {
        var (newX, newY) = GetNewCoordinates(adventurer);

        if (IsWithinBounds(newX, newY) && !_map.Mountains.Any(m => m.X == newX && m.Y == newY))
        {
            adventurer.X = newX;
            adventurer.Y = newY;

            // Check for treasure
            var treasure = _map.Treasures.FirstOrDefault(t => t.X == newX && t.Y == newY);
            if (treasure != null && treasure.Count > 0)
            {
                adventurer.CollectedTreasures++;
                treasure.Count--;
            }
        }
    }

    private void TurnAdventurer(Adventurer adventurer, char direction)
    {
        var orientations = new[] { 'N', 'E', 'S', 'W' };
        int currentOrientationIndex = Array.IndexOf(orientations, adventurer.Orientation);

        if (direction == 'R') // Turn right
        {
            adventurer.Orientation = orientations[(currentOrientationIndex + 1) % 4];
        }
        else if (direction == 'L') // Turn left
        {
            adventurer.Orientation = orientations[(currentOrientationIndex + 3) % 4];
        }

        MoveAdventurer(adventurer);
    }

    private (int, int) GetNewCoordinates(Adventurer adventurer)
    {
        int newX = adventurer.X;
        int newY = adventurer.Y;

        // Update the coordinates based on the orientation
        switch (adventurer.Orientation)
        {
            case 'N': newY--; break;
            case 'E': newX++; break;
            case 'S': newY++; break;
            case 'W': newX--; break;
        }

        // Ensure the new coordinates are within the map boundaries
        if (newX < 0 || newX >= _map.Width || newY < 0 || newY >= _map.Height)
        {
            // If the move is out of bounds, return the original coordinates (adventurer doesn't move)
            return (adventurer.X, adventurer.Y);
        }

        return (newX, newY);
    }


    private bool IsWithinBounds(int x, int y)
    {
        return x >= 0 && x < _map.Width && y >= 0 && y < _map.Height;
    }

    public void RunGameTurns()
    {
        bool hasMovesLeft;

        do
        {
            hasMovesLeft = false;

            // Var allowing to track intended movements
            var intendedMovements = new Dictionary<Adventurer, (int newX, int newY)>();
            var occupiedPositions = new HashSet<(int x, int y)>();

            // Collect all intended movements for this round
            foreach (var adventurer in _map.Adventurers)
            {
                if (adventurer.Moves.Count > 0) // If the adventurer has moves left
                {
                    hasMovesLeft = true; // At least one adventurer has moves left

                    // Get the next move
                    var move = adventurer.Moves.Dequeue();

                    // Update orientation if needed (based on your move commands)
                    UpdateAdventurerOrientation(adventurer, move);

                    // Get new coordinates based on the current orientation
                    var (newX, newY) = GetNewCoordinates(adventurer);

                    // Store the intended movement
                    intendedMovements[adventurer] = (newX, newY);
                }
            }

            // Process movements, ensuring that only one adventurer can occupy a space
            foreach (var adventurer in _map.Adventurers)
            {
                if (intendedMovements.TryGetValue(adventurer, out var newPosition))
                {
                    var (newX, newY) = newPosition;

                    // If the position is already occupied, skip this adventurer's move
                    if (!occupiedPositions.Contains((newX, newY)))
                    {
                        // Move the adventurer to the new position
                        adventurer.X = newX;
                        adventurer.Y = newY;

                        // Check for treasure
                        var treasure = _map.Treasures.FirstOrDefault(t => t.X == newX && t.Y == newY);
                        if (treasure != null && treasure.Count > 0)
                        {
                            adventurer.CollectedTreasures++;
                            treasure.Count--;
                        }

                        // Mark this position as occupied
                        occupiedPositions.Add((newX, newY));
                    }
                }
            }

        } while (hasMovesLeft); // Continue until no adventurer has moves left
    }

    private void UpdateAdventurerOrientation(Adventurer adventurer, char move)
    {
        switch (move)
        {
            case 'G': // Turn left
                adventurer.Orientation = adventurer.Orientation switch
                {
                    'N' => 'W',
                    'W' => 'S',
                    'S' => 'E',
                    'E' => 'N',
                    _ => adventurer.Orientation
                };
                break;

            case 'D': // Turn right
                adventurer.Orientation = adventurer.Orientation switch
                {
                    'N' => 'E',
                    'E' => 'S',
                    'S' => 'W',
                    'W' => 'N',
                    _ => adventurer.Orientation
                };
                break;

            case 'A': // Advance (No orientation change)
                      // Orientation remains unchanged
                break;

            default:
                throw new InvalidOperationException($"Invalid move command: {move}");
        }
    }

    public void WriteOutputFile(string filePath)
    {
        using (var writer = new StreamWriter(filePath))
        {
            // Write the map dimensions
            writer.WriteLine($"C - {_map.Width} - {_map.Height}");

            // Write mountains
            foreach (var mountain in _map.Mountains)
            {
                writer.WriteLine($"M - {mountain.X} - {mountain.Y}");
            }

            // Write remaining treasures
            foreach (var treasure in _map.Treasures)
            {
                 writer.WriteLine($"T - {treasure.X} - {treasure.Y} - {treasure.Count}");                
            }

            // Write adventurers' final states
            foreach (var adventurer in _map.Adventurers)
            {
                writer.WriteLine($"A - {adventurer.Name} - {adventurer.X} - {adventurer.Y} - {adventurer.Orientation} - {adventurer.CollectedTreasures}");
            }
        }
    }

}
