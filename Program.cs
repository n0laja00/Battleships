// See https://aka.ms/new-console-template for more information
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using static Board;
using static System.Runtime.InteropServices.JavaScript.JSType;

int boardSize = 0;
int shipQtt = 0;
int battleshipSizeOnGrid = 0;
bool gameEnd = false;
int validPlacements = 0;
Board board;

Random rnd = new Random();
BattleshipList battleshipList = new BattleshipList([]);

Console.WriteLine("The Mini World of Mini Battleships");

while (boardSize == 0)
{
    boardSize = SetUpGameFunction("How big is your battlefield? (1-5):");
}


while (shipQtt == 0 || shipQtt > boardSize)
{
    shipQtt = SetUpGameFunction("How many ships do you want? (you can't have more pieces than the size of your battlefield allows):");
}

board = new Board(boardSize);

for (int i = 0; i < shipQtt; i++)
{
    battleshipSizeOnGrid = rnd.Next(1, boardSize + 1);
    Battleship ship = new Battleship(battleshipSizeOnGrid);
    battleshipList.battleships.Add(ship);
}

board.PopulateBoard(battleshipList);

Console.WriteLine("""
    ////////////////////////////////////////////////
    /// BOARD HAS BEEN SET!                      ///
    ////////////////////////////////////////////////
    


    """);

while (!gameEnd)
{
    string? command = string.Empty;
    bool success = false;
    bool hit = false;
    int column;
    int row;
    int[] numbers = [];

    board.PrintFogOfWarBoard();
    Console.WriteLine("\n\n\nCaptain, what row and column would you like to shoot, Captain? (example: 2 3)");

    while (!success)
    {
        numbers = ValidateInput(boardSize);
        if (numbers.Length > 0)
        {
            success = true;
        }

    }

    command = string.Join(" ", numbers);
    row = numbers[0];
    column = numbers[1];

    hit = battleshipList.HitFunction(command);
    board.ClearFogOfWarFromCoord(row, column, hit);



    Console.WriteLine(("").PadRight(24, '-'));
    Console.WriteLine("Ships remaining");
    battleshipList.printRemainingBattleships();
    Console.WriteLine(("").PadRight(24, '-'));

    Console.WriteLine("\n");
    validPlacements = board.CalculateValidPlacements(battleshipList);
    switch (validPlacements)
    {
        case 0:
            Console.WriteLine("No more ships detected! You won the game!");
            gameEnd = true;
            break;
        case -1:
            Console.WriteLine("Possible cheating detected!");
            gameEnd = true;
            break;
        default:
            Console.WriteLine($"The number of remaining placements is {validPlacements}");
            break;

    }
    Console.WriteLine("\n");
}


//for (int i = 0; i < battleshipList.battleships.Count; i++)
//{
//    Console.WriteLine(battleshipList.battleships[i].sizeOnGrid);
//}


//alusten koot on x(1<x<n) Jos antaa kooksi 5x5, aluksen mahdollinen koko on 1-5

static int SetUpGameFunction(string message)
{
    bool success = false;
    int number = 0;

    while (!success)
    {
        Console.Write(message); //k
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

static int[] ValidateInput(int boardSize)
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

public class Board()
{
    int Columns { get; set; }
    int Rows { get; set; }
    List<List<string>>? Layout { get; set; }
    List<List<string>>? LayoutFogOfWar { get; set; }
    List<List<int>>? LayoutFogOfWarShipLocations { get; set; }

    public Board(int sizeOfBoard) : this()
    {
        this.Columns = sizeOfBoard;
        this.Rows = sizeOfBoard;
        this.Layout = DrawLayoutFunction();
        this.LayoutFogOfWar = DrawLayoutFogOfWarFunction();
        this.LayoutFogOfWarShipLocations = new List<List<int>>();
    }
    private List<List<string>> DrawLayoutFogOfWarFunction()
    {
        List<List<string>> board = new List<List<string>>();
        for (int i = 0; i < Rows; i++)
        {
            List<string> row = new List<string>();
            for (int y = 0; y < Columns; y++)
            {
                row.Add(".");
            }
            board.Add(row);
        }
        return board;
    }

    private List<List<string>> DrawLayoutFunction()
    {
        List<List<string>> board = new List<List<string>>();
        for (int i = 0; i < Rows; i++)
        {
            List<string> row = new List<string>();
            for (int y = 0; y < Columns; y++)
            {
                row.Add($"{i} {y}");
            }
            board.Add(row);
        }
        return board;
    }

    public void PrintBoard()
    {
        if (Layout is null)
        {
            return;
        }

        for (int y = 0; y < Rows; y++)
        {
            Console.Write($"{y + 1}: ");
            Console.WriteLine(string.Join("|", Layout[y]));
        }

    }
    public void PrintFogOfWarBoard()
    {
        if (this.LayoutFogOfWar is null)
        {
            return;
        }

        Console.WriteLine("This is what the board looks like...");

        for (int y = 0; y < Rows; y++)
        {
            Console.Write($"|{y + 1} : ");
            Console.Write(string.Join("", this.LayoutFogOfWar[y]));
            Console.WriteLine("|");
        }

    }

    public void ClearFogOfWarFromCoord(int row, int column, bool hit)
    {
        if (this.LayoutFogOfWar is null)
        {
            return;
        }

        if (this.LayoutFogOfWarShipLocations is null)
        {
            return;
        }


        if (hit)
        {
            this.LayoutFogOfWar[row][column] = "O";


            this.LayoutFogOfWarShipLocations.Add(new List<int> { row, column });

        }
        else
        {
            this.LayoutFogOfWar[row][column] = "X";
        }
    }

    public int CalculateValidPlacements(BattleshipList battleshipList)
    {
        List<List<string>>? boardToList = DrawLayoutFunction();
        string[][]? FogOfWarToArray = LayoutFogOfWar?.Select(a => a.ToArray()).ToArray();

        int validPlacements = 0;
        int HitsInFogOfWar = 0;

        if (FogOfWarToArray is null)
        {
            return -1;
        }

        for (int i = 0; i < FogOfWarToArray.Count(); i++)
        {
            for (int y = 0; y < FogOfWarToArray[i].Count(); y++)
            {
                if (FogOfWarToArray[i][y].Contains("O"))
                {
                    HitsInFogOfWar++;
                    boardToList[i][y] = string.Empty;
                }
                if (FogOfWarToArray[i][y].Contains("X"))
                {
                    boardToList[i][y] = string.Empty;
                }
            }
        }


        int battleshipCount = battleshipList.battleships.Where(s => s.Hitpoints > 0).Count();

        if (battleshipCount == 0)
        {
            return 0;
        }

        for (int battleship = 0; battleship < battleshipCount; battleship++)
        {
            List<int> verticalCells = new List<int>();
            List<int> horizontalCells = new List<int>();

            for (int i = 0; i < boardToList.Count(); i++)
            {
                int continousVertical = 0;
                for (int y = 0; y < boardToList[i].Count(); y++)
                {
                    if (boardToList[y][i] == string.Empty)
                    {
                        verticalCells.Add(continousVertical);
                        continousVertical = 0;
                    }
                    else
                    {
                        continousVertical++;
                    }
                }
                verticalCells.Add(continousVertical);
            }

            foreach (var number in verticalCells)
            {
                if (number >= battleshipList.battleships[battleship].SizeOnGrid)
                {
                    for (int i = 0; i < (number / battleshipList.battleships[battleship].SizeOnGrid); i++)
                    {
                        validPlacements++;
                    }
                }
            }

            for (int i = 0; i < boardToList.Count(); i++)
            {
                int continousHorizontal = 0;
                for (int y = 0; y < boardToList[i].Count(); y++)
                {

                    if (boardToList[i][y] == string.Empty)
                    {
                        horizontalCells.Add(continousHorizontal);
                        continousHorizontal = 0;
                    }
                    else
                    {
                        continousHorizontal++;
                    }

                    horizontalCells.Add(continousHorizontal);

                }
            }

            foreach (var number in horizontalCells)
            {
                if (number != 0 && number >= battleshipList.battleships[battleship].SizeOnGrid)
                {
                    for (int i = 0; i < (number / battleshipList.battleships[battleship].SizeOnGrid); i++)
                    {
                        validPlacements++;
                    }
                }
            }
        }

        return validPlacements;
    }


    public void PopulateBoard(BattleshipList battleshiplist)
    {
        Random rnd = new Random();
        bool success = false;
        bool locationRandomizationSuccess = false;
        int shipLocationColumn;
        int shipLocationRow;

        if (Layout is null)
        {
            return;
        }

        while (!success)
        {
            for (int i = 0; i < battleshiplist.battleships.Count(); i++)
            {
                locationRandomizationSuccess = false;

                while (!locationRandomizationSuccess)
                {
                    List<List<string>> randomizerLayout = Layout
                        .Where(subList => subList.Any(item => item != string.Empty))
                        .ToList();

                    shipLocationRow = rnd.Next(0, randomizerLayout.Count);
                    shipLocationColumn = rnd.Next(0, randomizerLayout[shipLocationRow].Count);

                    bool fitsRight = false;
                    bool fitsLeft = false;
                    bool fitsTop = false;
                    bool fitsBottom = false;
                    List<int> orientation = new List<int>();
                    int randomOrientation;

                    if (!randomizerLayout[shipLocationRow][shipLocationColumn].Contains(string.Empty))
                    {
                        continue;
                    }

                    if ((randomizerLayout[shipLocationRow].Count() - shipLocationColumn) > battleshiplist.battleships[i].SizeOnGrid)
                    {
                        for (int y = 0; y < battleshiplist.battleships[i].SizeOnGrid; y++)
                        {
                            if (randomizerLayout[shipLocationRow][shipLocationColumn + y] == string.Empty)
                            {
                                fitsRight = false;

                                break;
                            }
                            else
                            {
                                fitsRight = true;
                            }
                        }
                    }
                    else
                    {
                        fitsRight = false;
                    }

                    if (shipLocationColumn >= battleshiplist.battleships[i].SizeOnGrid)
                    {
                        for (int y = 0; y < battleshiplist.battleships[i].SizeOnGrid; y++)
                        {
                            if (randomizerLayout[shipLocationRow][shipLocationColumn - y] == string.Empty)
                            {
                                fitsLeft = false;

                                break;
                            }
                            else
                            {
                                fitsLeft = true;
                            }
                        }
                    }
                    else
                    {
                        fitsLeft = false;
                    }

                    if (shipLocationRow >= battleshiplist.battleships[i].SizeOnGrid)
                    {
                        for (int y = 0; y < battleshiplist.battleships[i].SizeOnGrid; y++)
                        {
                            if (randomizerLayout[shipLocationRow - y][shipLocationColumn] == string.Empty)
                            {
                                fitsTop = false;

                                break;
                            }
                            else
                            {
                                fitsTop = true;
                            }
                        }
                    }
                    else
                    {
                        fitsTop = false;
                    }

                    if ((randomizerLayout[shipLocationRow].Count() - shipLocationRow) > battleshiplist.battleships[i].SizeOnGrid)
                    {
                        for (int y = 0; y < battleshiplist.battleships[i].SizeOnGrid; y++)
                        {
                            if (randomizerLayout[shipLocationRow + y][shipLocationColumn] == string.Empty)
                            {
                                fitsBottom = false;
                                break;
                            }
                            else
                            {
                                fitsBottom = true;
                            }
                        }
                    }
                    else
                    {
                        fitsBottom = false;
                    }


                    if (fitsRight)
                    {
                        orientation.Add(1);
                    }
                    if (fitsLeft)
                    {
                        orientation.Add(2);
                    }
                    if (fitsTop)
                    {
                        orientation.Add(3);
                    }
                    if (fitsBottom)
                    {
                        orientation.Add(4);
                    }

                    if (orientation.Count > 0)
                    {
                        var index = new Random().Next(0, orientation.Count());
                        randomOrientation = orientation[index];
                        switch (randomOrientation)
                        {
                            //right
                            case 1:
                                for (int y = 0; y < battleshiplist.battleships[i].SizeOnGrid; y++)
                                {
                                    this.Layout[shipLocationRow][shipLocationColumn + y] = string.Empty;
                                    battleshiplist.battleships[i].Coordinates.Add(shipLocationRow.ToString() + " " + (shipLocationColumn + y).ToString());
                                }
                                locationRandomizationSuccess = true;

                                break;
                            //left
                            case 2:
                                for (int y = 0; y < battleshiplist.battleships[i].SizeOnGrid; y++)
                                {
                                    this.Layout[shipLocationRow][shipLocationColumn - y] = string.Empty;
                                    battleshiplist.battleships[i].Coordinates.Add(shipLocationRow.ToString() + " " + (shipLocationColumn + y).ToString());
                                }
                                locationRandomizationSuccess = true;

                                break;
                            //top
                            case 3:
                                for (int y = 0; y < battleshiplist.battleships[i].SizeOnGrid; y++)
                                {
                                    this.Layout[shipLocationRow - y][shipLocationColumn] = string.Empty;
                                    battleshiplist.battleships[i].Coordinates.Add((shipLocationRow - y).ToString() + " " + shipLocationColumn.ToString());
                                }
                                locationRandomizationSuccess = true;

                                break;
                            //bottom
                            case 4:
                                for (int y = 0; y < battleshiplist.battleships[i].SizeOnGrid; y++)
                                {
                                    this.Layout[shipLocationRow + y][shipLocationColumn] = string.Empty;
                                    battleshiplist.battleships[i].Coordinates.Add((shipLocationRow + y).ToString() + " " + shipLocationColumn.ToString());
                                }
                                locationRandomizationSuccess = true;

                                break;
                        }
                    }
                    else
                    {
                        if (battleshiplist.battleships[i].SizeOnGrid > 1)
                        {
                            battleshiplist.battleships[i].SizeOnGrid--;
                            battleshiplist.battleships[i].UpdateHealthOnSizeChange();
                        }

                    }
                }
            }
            success = true;
        }
    }
}

public record class BattleshipList(List<Battleship> battleships)
{
    public BattleshipList() : this(new List<Battleship>()) { }

    public bool HitFunction(string coord)
    {
        var index = this.battleships.FindIndex(x => x.Coordinates.Contains(coord));
        if (index == -1)
        {
            Console.WriteLine("Miss!!");
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
    public void printRemainingBattleships()
    {
        int battleshipCount = battleships.Where(s => s.Hitpoints > 0).Count();

        for (int i = 0; i < battleshipCount; i++)
        {
            Console.WriteLine(battleships[i].SizeOnGrid.ToString());
        }
    }

}

public class Battleship(int sizeOnGrid)
{
    public int SizeOnGrid { get; set; } = sizeOnGrid;
    public int Hitpoints { get; set; } = sizeOnGrid;
    public List<string> Coordinates { get; set; } = new List<string>();

    public Battleship() : this(0) { }

    public void UpdateHealthOnSizeChange()
    {
        this.Hitpoints = SizeOnGrid;
    }

}