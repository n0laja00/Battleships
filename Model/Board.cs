using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Battleships.Model;

/// <summary>
/// Class used for boards.
/// </summary>
public class Board()
{
    int Columns { get; set; }
    int Rows { get; set; }
    string[,]? Layout { get; set; }
    string[,]? LayoutFogOfWar { get; set; }
    List<List<int>>? LayoutFogOfWarShipLocations { get; set; }

    public Board(int sizeOfBoard) : this()
    {
        this.Columns = sizeOfBoard;
        this.Rows = sizeOfBoard;
        this.Layout = DrawLayoutFunction();
        this.LayoutFogOfWar = DrawLayoutFogOfWarFunction();
        this.LayoutFogOfWarShipLocations = new List<List<int>>();
    }
    /// <summary>
    /// Draws board for fog of war
    /// </summary>
    /// <returns>list of list of strings, grid/returns>
    private string[,] DrawLayoutFogOfWarFunction()
    {
        string[,] board = new string[Rows, Columns];
        for (int i = 0; i < Rows; i++)
        {
            List<string> row = new List<string>();
            for (int y = 0; y < Columns; y++)
            {
                board[i, y] = ".";
            }

        }
        return board;
    }

    public bool validateHit(int row, int column)
    {
        if (LayoutFogOfWar?[row, column] == "X" || LayoutFogOfWar?[row, column] == "O")
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Draws layout for board population
    /// </summary>
    /// <returns>list of list of strings, grid</returns>
    private string[,] DrawLayoutFunction()
    {
        string[,] board = new string[Rows, Columns];
        for (int i = 0; i < Rows; i++)
        {
            for (int y = 0; y < Columns; y++)
            {
                board[i, y] = $"{i} {y}";
            }

        }
        return board;
    }

    /// <summary>
    /// prints layout used for randomization. used for debugging.
    /// </summary>
    public void PrintBoard()
    {
        if (Layout is null)
        {
            return;
        }

        for (int i = 0; i < Rows; i++)
        {
            Console.Write($"{i + 1}: ");
            for (int y = 0; y < Layout.GetLength(1); y++)
            {
                Console.Write(Layout[i, y] + "\t");
            }
            Console.WriteLine();
        }

    }

    /// <summary>
    /// Prints current fog of war visible to player
    /// </summary>
    public void PrintFogOfWarBoard()
    {
        if (LayoutFogOfWar is null)
        {
            return;
        }

        Console.WriteLine("This is what the board looks like...");

        for (int i = 0; i < Rows; i++)
        {
            Console.Write($"|{i + 1} : ");
            for (int y = 0; y < LayoutFogOfWar.GetLength(1); y++)
            {
                Console.Write("|" + LayoutFogOfWar[i, y]);
            }
            Console.WriteLine();
        }

    }

    /// <summary>
    /// Clears fog of war in certain coordinates
    /// </summary>
    /// <param name="row"></param>
    /// <param name="column"></param>
    /// <param name="hit"></param>
    public void ClearFogOfWarFromCoord(int row, int column, bool hit)
    {
        if (LayoutFogOfWar is null)
        {
            return;
        }

        if (LayoutFogOfWarShipLocations is null)
        {
            return;
        }


        if (hit)
        {
            LayoutFogOfWar[row, column] = "O";


            LayoutFogOfWarShipLocations.Add(new List<int> { row, column });

        }
        else
        {
            LayoutFogOfWar[row, column] = "X";
        }
    }

    /// <summary>
    /// Calculates valid placements on the board
    /// </summary>
    /// <param name="battleshipList">list of battleships that are to be placed on the board</param>
    /// <returns>int</returns>
    public int CalculateValidPlacements(BattleshipRecord battleshipList)
    {
        string[,]? boardForValidPlacements = DrawLayoutFunction();

        int validPlacements = 0;
        int HitsInFogOfWar = 0;

        if (LayoutFogOfWar is null)
        {
            return -1;
        }

        int battleshipCount = battleshipList.battleships.Where(s => s.Hitpoints > 0).Count();
        List<Battleship> battleshipsRemaining = battleshipList.battleships.FindAll(s => s.Hitpoints > 0);

        if (battleshipCount == 0)
        {
            return 0;
        }

        for (int i = 0; i < LayoutFogOfWar.GetLength(0); i++)
        {
            for (int y = 0; y < LayoutFogOfWar.GetLength(1); y++)
            {
                if (LayoutFogOfWar[i, y] == "O")
                {
                    HitsInFogOfWar++;
                    boardForValidPlacements[i, y] = string.Empty;
                }
                if (LayoutFogOfWar[i, y] == "X")
                {
                    boardForValidPlacements[i, y] = string.Empty;
                }
            }
        }


        for (int battleship = 0; battleship < battleshipCount; battleship++)
        {
            List<int> verticalCells = new List<int>();
            List<int> horizontalCells = new List<int>();
            Battleship currentBattleship = battleshipsRemaining[battleship];
            int[] coordinatesIntArr;
            bool isRevealed = false;
            bool isDamaged = false;
            bool isVertical;
            bool isHorizontal;

            if (currentBattleship.Hitpoints < (currentBattleship.SizeOnGrid - 1))
            {
                isRevealed = true;
            }

            if (currentBattleship.Hitpoints < currentBattleship.SizeOnGrid)
            {
                isDamaged = true;
            }

            if (!isRevealed && isDamaged && currentBattleship.SizeOnGrid > 1)
            {
                for (int i = 0; i < currentBattleship.Coordinates.Count(); i++)
                {
                    coordinatesIntArr = currentBattleship.Coordinates[i].ToArray();
                    if (LayoutFogOfWar[coordinatesIntArr[0], coordinatesIntArr[1]] == ("O"))
                    {
                        List<int> orientations = ViableOrientationsChecker(boardForValidPlacements, currentBattleship, coordinatesIntArr[0], coordinatesIntArr[1]);
                        for (int y = 0; y < orientations.Count(); y++)
                        {
                            validPlacements++;
                        }
                    }
                }
            }

            if (isRevealed && currentBattleship.SizeOnGrid > 1)
            {
                coordinatesIntArr = currentBattleship.Coordinates[0].ToArray();
                isVertical = currentBattleship.Coordinates.All(s => s[1].Equals(coordinatesIntArr[1]));
                isHorizontal = currentBattleship.Coordinates.All(s => s[0].Equals(coordinatesIntArr[0]));

                if (isVertical)
                {
                    Console.WriteLine("One ship is vertical!");
                }

                if (isHorizontal)
                {
                    Console.WriteLine("One ship is Horizontal!");
                }

                for (int y = 0; y < currentBattleship.Coordinates.Count(); y++)
                {
                    for (int x = 0; x < currentBattleship.Coordinates[y].Count(); x++)
                    {
                        int row = currentBattleship.Coordinates[y][0];
                        int column = currentBattleship.Coordinates[y][1];
                        boardForValidPlacements[row, column] = string.Empty;
                    }
                }
            }


            if (!isRevealed && !isDamaged)
            {
                for (int i = 0; i < boardForValidPlacements.GetLength(0); i++)
                {
                    int continousVertical = 0;
                    for (int y = 0; y < boardForValidPlacements.GetLength(1); y++)
                    {
                        if (boardForValidPlacements[y, i] == string.Empty)
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

                for (int i = 0; i < boardForValidPlacements.GetLength(0); i++)
                {
                    int continousHorizontal = 0;
                    for (int y = 0; y < boardForValidPlacements.GetLength(1); y++)
                    {
                        if (boardForValidPlacements[i, y] == string.Empty)
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
            }

            foreach (var number in verticalCells)
            {
                if (number >= currentBattleship.SizeOnGrid)
                {
                    for (int i = 0; i < Math.Floor((decimal)(number / currentBattleship.SizeOnGrid)); i++)
                    {
                        validPlacements++;
                    }
                }
            }

            foreach (var number in horizontalCells)
            {
                if (number > 1 && number >= currentBattleship.SizeOnGrid)
                {
                    for (int i = 0; i < Math.Floor((decimal)(number / currentBattleship.SizeOnGrid)); i++)
                    {
                        validPlacements++;
                    }
                }
            }

            if (currentBattleship.SizeOnGrid == 1)
            {
                for (int y = 0; y < boardForValidPlacements.GetLength(0); y++)
                {
                    for (int x = 0; x < boardForValidPlacements.GetLength(1); x++)
                    {
                        validPlacements++;
                    }
                }
            }
        }

        return validPlacements;
    }

    /// <summary>
    /// Used to populate the board randomly.
    /// </summary>
    /// <param name="battleshipList">List of battleships to be placed on the board</param>
    public void PopulateBoard(BattleshipRecord battleshipList)
    {
        Random rnd = new Random();
        bool success = false;
        bool locationRandomizationSuccess = false;

        if (Layout is null)
        {
            return;
        }

        while (!success)
        {
            for (int i = 0; i < battleshipList.battleships.Count(); i++)
            {
                locationRandomizationSuccess = false;

                while (!locationRandomizationSuccess)
                {
                    List<List<string>> randomizerList = CreateRandomizerListFromLayoutArray(Layout);
                    List<int> orientations = new List<int>();

                    int randomizedLocationRow = rnd.Next(0, randomizerList.Count());
                    int randomizedLocationColumn = rnd.Next(0, randomizerList[randomizedLocationRow].Count());

                    string randomizedCoords = randomizerList[randomizedLocationRow][randomizedLocationColumn];

                    int[] coordinatesInt = randomizedCoords.Split(' ').Select(n => Convert.ToInt32(n)).ToArray();
                    int shipLocationRow = coordinatesInt[0];
                    int shipLocationColumn = coordinatesInt[1];


                    if (battleshipList.battleships[i].SizeOnGrid > 1)
                    {
                        orientations = ViableOrientationsChecker(Layout, battleshipList.battleships[i], shipLocationRow, shipLocationColumn);
                    }
                    else
                    {
                        orientations = [1, 2, 3, 4];
                    }

                    if (orientations.Count() > 0)
                    {
                        var index = new Random().Next(0, orientations.Count());
                        int randomOrientation = orientations[index];
                        switch (randomOrientation)
                        {
                            //right
                            case 1:
                                for (int y = 0; y < battleshipList.battleships[i].SizeOnGrid; y++)
                                {
                                    Layout[shipLocationRow, shipLocationColumn + y] = string.Empty;
                                    List<int> shipCoordinates = [shipLocationRow, shipLocationColumn + y];
                                    battleshipList.battleships[i].Coordinates.Add(shipCoordinates);
                                }
                                locationRandomizationSuccess = true;

                                break;
                            //left
                            case 2:
                                for (int y = 0; y < battleshipList.battleships[i].SizeOnGrid; y++)
                                {
                                    this.Layout[shipLocationRow, shipLocationColumn - y] = string.Empty;
                                    List<int> shipCoordinates = [shipLocationRow, shipLocationColumn - y];
                                    battleshipList.battleships[i].Coordinates.Add(shipCoordinates);
                                }
                                locationRandomizationSuccess = true;

                                break;
                            //top
                            case 3:
                                for (int y = 0; y < battleshipList.battleships[i].SizeOnGrid; y++)
                                {
                                    this.Layout[shipLocationRow - y, shipLocationColumn] = string.Empty;
                                    List<int> shipCoordinates = [shipLocationRow - y, shipLocationColumn];
                                    battleshipList.battleships[i].Coordinates.Add(shipCoordinates);
                                }
                                locationRandomizationSuccess = true;

                                break;
                            //bottom
                            case 4:
                                for (int y = 0; y < battleshipList.battleships[i].SizeOnGrid; y++)
                                {
                                    this.Layout[shipLocationRow + y, shipLocationColumn] = string.Empty;
                                    List<int> shipCoordinates = [shipLocationRow + y, shipLocationColumn];
                                    battleshipList.battleships[i].Coordinates.Add(shipCoordinates);
                                }
                                locationRandomizationSuccess = true;

                                break;
                        }
                    }
                    else
                    {
                        battleshipList.battleships[i].DecreaseHealthAndOnSizeChange();
                    }
                }
            }
            success = true;
        }
    }

    private List<int> ViableOrientationsChecker(string[,] viableOrientationLayoutArray, Battleship battleship, int shipLocationRow, int shipLocationColumn)
    {
        List<List<string>> viableOrientationLayout = CreateListFromArray(viableOrientationLayoutArray);
        bool fitsRight = false;
        bool fitsLeft = false;
        bool fitsTop = false;
        bool fitsBottom = false;

        List<int> orientations = new List<int>();

        int rowItemsCount = viableOrientationLayout[shipLocationRow].Count();
        int rowCount = viableOrientationLayout.Count();


        if (rowItemsCount >= battleship.SizeOnGrid && (shipLocationColumn + battleship.SizeOnGrid) <= rowItemsCount)
        {
            for (int y = 0; y < battleship.SizeOnGrid; y++)
            {
                if (viableOrientationLayout[shipLocationRow][shipLocationColumn + y] == string.Empty || (shipLocationColumn + y) >= rowItemsCount)
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

        if (shipLocationColumn >= battleship.SizeOnGrid)
        {
            for (int y = 0; y < battleship.SizeOnGrid; y++)
            {
                if (viableOrientationLayout[shipLocationRow][shipLocationColumn - y] == string.Empty)
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

        if (shipLocationRow >= battleship.SizeOnGrid)
        {
            for (int y = 0; y < battleship.SizeOnGrid; y++)
            {
                if (viableOrientationLayout[shipLocationRow - y][shipLocationColumn] == string.Empty)
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

        if (rowCount >= battleship.SizeOnGrid && (shipLocationRow + battleship.SizeOnGrid) <= rowCount)
        {
            for (int y = 0; y < battleship.SizeOnGrid; y++)
            {
                if (viableOrientationLayout[shipLocationRow + y][shipLocationColumn] == string.Empty)
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
            orientations.Add(1);
        }
        if (fitsLeft)
        {
            orientations.Add(2);
        }
        if (fitsTop)
        {
            orientations.Add(3);
        }
        if (fitsBottom)
        {
            orientations.Add(4);
        }

        return orientations;
    }

    private List<List<string>> CreateListFromArray(string[,] array)
    {
        List<List<string>> list = new List<List<string>>();

        for (int i = 0; i < array.GetLength(0); i++)
        {
            List<string> rowList = new List<string>();
            for (int y = 0; y < array.GetLength(1); y++)
            {
                rowList.Add(array[i, y]);
            }
            list.Add(rowList);
        }
        return list;
    }

    private List<List<string>> CreateRandomizerListFromLayoutArray(string[,] array)
    {
        List<List<string>> list = new List<List<string>>();

        for (int i = 0; i < array.GetLength(0); i++)
        {
            List<string> rowList = new List<string>();
            for (int y = 0; y < array.GetLength(1); y++)
            {
                if (array[i, y] != string.Empty)
                {
                    rowList.Add(array[i, y]);
                }
            }
            if (rowList.Count() > 0)
            {
                list.Add(rowList);
            }
        }
        return list;
    }


}

