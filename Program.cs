//Rework idea: Make ClearFogOfWarFromCoord work with string coords instead of ints. This may increase number of lines. 
using Battleships.Model;

int boardSize = 0;
int shipQtt = 0;
int battleshipSizeOnGrid = 0;
bool gameEnd = false;
GameState gameState = new GameState();
Board board;
Random rnd = new Random();
BattleshipRecord battleshipRecord = new BattleshipRecord([]);

Console.WriteLine("The Mini World of Mini Battleships");

while (boardSize == 0)
{
    boardSize = SetUpGameFunction("How big is your battlefield? (1-5):");
}


while (shipQtt == 0 || shipQtt > boardSize)
{
    shipQtt = SetUpGameFunction("How many ships do you want? (Can't have more than the width of the grid):");
}

board = new Board(boardSize);

for (int i = 0; i < shipQtt; i++)
{
    battleshipSizeOnGrid = rnd.Next(1, boardSize + 1);
    Battleship ship = new Battleship(battleshipSizeOnGrid);
    battleshipRecord.battleships.Add(ship);
}

board.PopulateBoard(battleshipRecord);

Console.WriteLine("""
    ////////////////////////////////////////////////
    /// BOARD HAS BEEN SET!                      ///
    ////////////////////////////////////////////////
    """);

while (!gameEnd)
{
    string? command = string.Empty;
    List<int> fireTarget = new List<int>();
    bool success;
    bool validTargetCell;
    bool hit = false;
    int[] numbers = [];

    //board.PrintBoard();
    battleshipRecord.printRemainingBattleships();
    battleshipRecord.PrintBattleshipListCoords();

    board.PrintFogOfWarBoard();

    validTargetCell = false;
    while (!validTargetCell)
    {
        Console.WriteLine("\n\n\nCaptain, what row and column would you like to shoot, Captain? (example: 2 3)");
        success = false;
        while (!success)
        {
            numbers = ValidateInputForRowAndColumn(boardSize);
            if (numbers.Length > 0)
            {
                success = true;
            }
        }

        validTargetCell = board.validateHit(numbers[0], numbers[1]);
        if (!validTargetCell)
        {
            Console.WriteLine("You've already hit that cell!");
        }
    }

    fireTarget.Add(numbers[0]);
    fireTarget.Add(numbers[1]);

    Console.WriteLine("\n\n");

    hit = battleshipRecord.HitFunction(fireTarget);
    board.ClearFogOfWarFromCoord(fireTarget[0], fireTarget[1], hit);

    Console.WriteLine(("").PadRight(24, '-'));
    Console.WriteLine("Ships remaining");
    battleshipRecord.printRemainingBattleships();
    Console.WriteLine(("").PadRight(24, '-'));

    Console.WriteLine("\n");
    gameState.ValidPlacements = board.CalculateValidPlacements(battleshipRecord);
    gameState.NumberOfShips = battleshipRecord.battleships.Where(s => s.Hitpoints > 0).Count();
    switch (gameState.ValidPlacements)
    {
        case 0:
            Console.WriteLine("No more valid placements left! You know where they are!");
            break;
        case -1:
            Console.WriteLine("Possible cheating detected!");
            break;
        default:
            Console.WriteLine($"The number of remaining placements is {gameState.ValidPlacements}");
            break;
    }

    if (gameState.NumberOfShips == 0)
    {
        gameEnd = true;
        Console.WriteLine("""
        ////////////////////////////////////////////////
        /// You beat the game!!!                     ///
        ////////////////////////////////////////////////
        """);
    }

    Console.WriteLine("\n");
}

/// <summary>
/// Sets up an aspect of the game
/// </summary>
static int SetUpGameFunction(string message)
{
    bool success = false;
    int number = 0;

    while (!success)
    {
        Console.Write(message);
        success = int.TryParse(Console.ReadLine(), out number);
        if (!success)
        {
            Console.WriteLine("Something went wrong with input!");
        }
        else if (number > 5 || number < 1)
        {
            success = false;
            Console.WriteLine("Your number didn't match the range!");
        }
    }
    return number;
}

/// <summary>
/// Validates row and column
/// </summary>
static int[] ValidateInputForRowAndColumn(int boardSize)
{
    string? command = string.Empty;
    int[] numbers = [];

    command = Console.ReadLine();
    command = command.Trim();

    if (!string.IsNullOrWhiteSpace(command))
    {
        command = string.Join("-", command.ToCharArray().Where(Char.IsDigit));
        numbers = command.Split('-').Select(int.Parse).ToArray();
    }
    else
    {
        Console.WriteLine("Something went wrong with input!");
        return [];
    }

    if (numbers.Length < 2)
    {
        Console.WriteLine("Please insert a column number!");
        return [];
    }
    else if (numbers.Any(x => x > boardSize) || numbers.Any(x => x < 1))
    {
        Console.WriteLine("out of bounds!");
        return [];
    }
    else
    {
        for (int x = 0; x < numbers.Length; x++)
        {
            numbers[x] = numbers[x] - 1;
        }
        return numbers;
    }
}






