
namespace Battleships.Model;
public class Battleship(int sizeOnGrid)
{
    public int SizeOnGrid { get; set; } = sizeOnGrid;
    public int Hitpoints { get; set; } = sizeOnGrid;
    public List<List<int>> Coordinates { get; set; } = new List<List<int>>();

    public Battleship() : this(0) { }

    /// <summary>
    /// Updates health when ship's sizeOnGrid is changed.
    /// </summary>
    public void DecreaseHealthAndOnSizeChange()
    {
        this.SizeOnGrid--;
        this.Hitpoints--;
    }

    public void printshipcoord()
    {
        foreach (var coord in Coordinates)
        {
            foreach (var number in coord)
            {
                Console.Write($"{number + 1}|\t");
            }
            Console.WriteLine();
        }
    }
}
