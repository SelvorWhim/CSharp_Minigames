﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FourInARow
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Beginning 2-player game of 4-in-a-row"); // todo: indicate number of rows and columns? or just print the board...
            Game game = new Game();
            while ((int)game.State <= 1)
            {
                Console.WriteLine("input column: ");
                int inputColumnNumber = Convert.ToInt32(Console.ReadLine()); // 1-indexed // todo: what if the user inputs garbage
                game.MakeMove(inputColumnNumber - 1);
            }
        }
    }

    public class Game
    {
        // options: make these variables?
        private const int NUM_ROWS = 6;
        private const int NUM_COLS = 7;

        public GameState State { get; private set; } = GameState.TurnP1;
        private Board gameBoard;

        public Game()
        {
            gameBoard = new Board(NUM_ROWS, NUM_COLS);
            gameBoard.PrintBoard(); // prints board at start of game
        }

        // todo-later - some interface other than just printing results
        public void MakeMove(int columnIndex)
        {
            if (State >= GameState.WinP1) // game over man, game over!
            {
                Console.WriteLine("The game is over! You can't make more moves."); // if the loop ends once the game is won this shouldn't happen. option: throw something?
                return;
            }
            ChipColor chipColor = (ChipColor)State;
            MoveResult res = gameBoard.MakeMove(columnIndex, chipColor);
            switch (res)
            {
                case MoveResult.MoveFailed:
                    Console.WriteLine("You can't put that there!");
                    break;
                case MoveResult.MoveSuccessful:
                    gameBoard.PrintBoard();
                    State = (GameState)(((int)State + 1) % 2);
                    Console.WriteLine($"Move successful! It's player {State + 1}'s turn"); // after state update
                    break;
                case MoveResult.GameEnded:
                    Console.WriteLine($"Game over! Player {State - 1} won!");
                    gameBoard.PrintBoard();
                    State += 2;
                    break;
            }
        }

        private class Board
        {
            private List<Column> cols;
            private int numCols;
            private int numRows;
            private const int IN_A_ROW = 4;

            public Board(int numRows, int numCols)
            {
                cols = new List<Column>(numCols);
                this.numRows = numRows;
                this.numCols = numCols;
                for (int i = 0; i < numCols; i++)
                {
                    cols.Add(new Column(numRows));
                }
            }

            public MoveResult MakeMove(int columnIndex, ChipColor chipColor)
            {
                if (columnIndex < 0 || columnIndex >= numCols)
                {
                    return MoveResult.MoveFailed;
                }
                else
                {
                    if (cols[columnIndex].AddChip(chipColor))
                    {
                        if (CheckForWin(chipColor, cols[columnIndex].NumChips - 1, columnIndex))
                        {
                            return MoveResult.GameEnded;
                        }
                        else
                        {
                            return MoveResult.MoveSuccessful;
                        }
                    }
                    else
                    {
                        return MoveResult.MoveFailed;
                    }
                }
            }

            // center row/col - indexes of chip around which to check
            private bool CheckForWin(ChipColor chipColor, int centerRow, int centerCol)
            {
                return CheckForWinInWindow(chipColor, Math.Max(centerRow - (IN_A_ROW - 1), 0), Math.Min(centerRow + (IN_A_ROW - 1), numRows), Math.Max(centerCol - (IN_A_ROW - 1), 0), Math.Min(centerCol - (IN_A_ROW - 1), numCols));
            }

            private bool CheckForWinInWindow(ChipColor chipColor, int rowFrom, int rowTo, int colFrom, int colTo)
            {
                if (rowTo - rowFrom + 1 < IN_A_ROW || colFrom - colFrom + 1 < IN_A_ROW) // window's too small...this shouldn't really happen unless the board is smaller than the number in a row (e.g. 4x4)
                {
                    return false;
                }
                return true; // TODO
            }

            public void PrintBoard()
            {
                // TODO
            }
        }

        private class Column
        {
            private List<ChipColor> chips;
            public int NumChips { get; private set; } = 0; // equal to chips.Count
            private int maxChips;

            public Column(int size /*number of rows*/)
            {
                chips = new List<ChipColor>(size);
                maxChips = size;
            }

            // true if successful, false if not
            public bool AddChip(ChipColor chipColor)
            {
                if (NumChips >= maxChips)
                {
                    return false;
                }
                else
                {
                    chips.Add(chipColor);
                    NumChips++;
                    return true;
                }
            }
        }

        private enum ChipColor { P1, P2 /*, Empty */ }; // equal to game state if game isn't over // red and white? Whatever, this can be generalized to more players, if 3-player 4-in-a-row is a thing
        public enum GameState { TurnP1 = 0, TurnP2 = 1, WinP1 = 2, WinP2 = 3 } // win is turn + 2, next turn is (turn+1)%2
        public enum MoveResult { MoveFailed, MoveSuccessful, GameEnded } // win is turn + 2, next turn is (turn+1)%2
    }
}