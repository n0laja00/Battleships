namespace Battleships.Model;
public class GameState(int ValidPlacements, int NumberOfShips)
{

    public int ValidPlacements { get; set; }
    public int NumberOfShips { get; set; }


    public GameState() : this(0, 0)
    {
    }
}


