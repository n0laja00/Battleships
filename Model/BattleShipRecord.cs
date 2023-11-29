namespace Battleships.Model;
/// <summary>
/// Class record of list of battleships.
/// </summary>
/// <param name="battleships"></param>
public record class BattleshipRecord(List<Battleship> battleships)
{
    public BattleshipRecord() : this(new List<Battleship>()) { }


    public void PrintBattleshipListCoords()
    {
        foreach (var ship in this.battleships)
        {
            ship.printshipcoord();
        }
    }

    /// <summary>
    /// takes in a string and compares it to coordinates on ships. 
    /// If coord exists on one of the ships, it takes a hit.
    /// </summary>
    /// <param name="coord">int list of coordinates on board, "row col" "2 1"</param>
    /// <returns>bool</returns>
    public bool HitFunction(List<int> coord)
    {
        var index = this.battleships.FindIndex(ship => ship.Coordinates.Any(c => c.SequenceEqual(coord)));

        if (index == -1)
        {
            Console.WriteLine("Miss!!");
            return false;
        }

        if (battleships[index].Hitpoints == 0)
        {
            Console.WriteLine("You've already hit this ship!!");
            return false;
        }
        else
        {
            battleships[index].Hitpoints = battleships[index].Hitpoints - 1;
            if (battleships[index].Hitpoints == 0)
            {
                Console.WriteLine("You sunk my battleship!!");
                return true;
            }
            else
            {
                Console.WriteLine("Hit!!");
                return true;
            }

        }
    }
    /// <summary>
    /// Prints remaining battleships with more than 0 hitpoints
    /// </summary>
    public void printRemainingBattleships()
    {
        List<Battleship> remainingBattleships = battleships.FindAll(s => s.Hitpoints > 0);

        for (int i = 0; i < remainingBattleships.Count(); i++)
        {
            Console.WriteLine(remainingBattleships[i].SizeOnGrid.ToString());
        }
    }

}

