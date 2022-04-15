using System;
namespace EnglishCheckers
{
    public class UserInterface
    {
        private string m_Player1Name = null;
        private string m_Player2Name = null;
        public void InitializeGame()
        {
            int boardSize;
            bool twoPlayerMode = !true;
            bool exitGame = !true;
            GameManager gameManager = null;

            Console.WriteLine("Welcome to English Checkers!");
            m_Player1Name = getValidName();
            boardSize = getBoardSize();
            twoPlayerMode = getGameMode();
            m_Player2Name = twoPlayerMode ? getValidName() : "Computer";
            gameManager = new GameManager(boardSize, twoPlayerMode);
            while (!exitGame)
            {
                RunGame(gameManager);
                //ask if wants another round
                //initilize game
            }
        }

        private void RunGame(GameManager i_GameManager)
        {
            GameManager.eGameStatus eGameStatus = GameManager.eGameStatus.ContinueGame;

            while (eGameStatus == GameManager.eGameStatus.ContinueGame)
            {
                printBoard(i_GameManager.GameBoard);
                //print points/ whos turn
                eGameStatus = askForMove();
                while (eGameStatus == GameManager.eGameStatus.InvalidMove)
                {
                    eGameStatus = askForMove();
                }
                Console.ReadLine();
            }

            //while (the move returns continuegame) loop of asking for moves
            //if Q is entered instead of a move - the current player loses
            //otherwise print message
        }

        private void printBoard(Board i_GameBoard)
        {
            char row = 'a';

            printColumnLine(i_GameBoard.Size);
            printLowerBound(i_GameBoard.Size);
            for (int i = 0; i < i_GameBoard.Size; i++)
            {
                Console.WriteLine(string.Format("{0}|{1}", row, rowToString(i_GameBoard, i)));
                printLowerBound(i_GameBoard.Size);
                row++;
            }

            //clear screen
        }

        private string rowToString(Board i_GameBoard, int i_Row)
        {
            string rowToString = null;
            Coordinate coordinate = new Coordinate(i_Row, 0);
            Coin.eCoinType currentCoinType;
            bool isCurrentCoinKing = !true;

            for (int i = 0; i < i_GameBoard.Size; i++)
            {
                if (i_GameBoard.GetSquare(coordinate).Coin == null)
                {
                    rowToString += " ";
                }
                else
                {
                    currentCoinType = i_GameBoard.GetSquare(coordinate).Coin.Type;
                    isCurrentCoinKing = i_GameBoard.GetSquare(coordinate).Coin.IsKing;
                    if (currentCoinType == Coin.eCoinType.Player1Coin)
                    {
                        rowToString += isCurrentCoinKing ? "K" : "X";
                    }
                    else
                    {
                        rowToString += isCurrentCoinKing ? "U" : "O";
                    }
                }

                rowToString += "|";
                coordinate.Column++;
            }

            return rowToString;
        }

        private void printColumnLine(int i_BoardSize)
        {
            string firstLine = "  ";
            char column = 'A';

            for (int i = 0; i < i_BoardSize; i++)
            {
                firstLine += column;
                firstLine += " ";
                column++;
            }

            Console.WriteLine(firstLine);
        }

        private void printLowerBound(int i_BoardSize)
        {
            string lowerBound = "==";

            for (int i = 0; i < i_BoardSize * 2; i++)
            {
                lowerBound += "=";
            }

            Console.WriteLine(lowerBound);
        }

        private GameManager.eGameStatus askForMove()
        {
            //ask user
            //validate input of move
            // initiate move/ initiate computer move
            //return the game status to run
            return 0;
        }

        private string getValidName()
        {
            string name = null;
            bool isValidName = !true;

            Console.WriteLine("Please enter your name:");
            while (!isValidName)
            {
                name = Console.ReadLine();
                isValidName = (name.Length >= 1 && name.Length <= 20);
                for (int i = 0; i < name.Length && isValidName; i++)
                {
                    if (!char.IsLetter(name, i))
                    {
                        isValidName = !true;
                    }
                }

                invalidInputMessage(isValidName);
            }

            //clear screen

            return name;
        }

        private int getBoardSize()
        {
            int boardSize = 0;
            bool validInput = !true;
            System.Console.WriteLine(string.Format(
@"Please choose board size:
(6) 6X6     (8) 8X8     (10) 10X10"));
            while (!validInput)
            {
                validInput = int.TryParse(Console.ReadLine(), out boardSize) && (boardSize == 6 || boardSize == 8 || boardSize == 10);
                invalidInputMessage(validInput);
            }

            //clear screen

            return boardSize;
        }

        private bool getGameMode()
        {
            bool isTwoPlayerMode = !true;
            bool validInput = !true;
            int gameMode = 1;

            System.Console.WriteLine(string.Format(
@"Please choose game mode:
(1) 1 player     (2) 2 players"));
            while (!validInput)
            {
                validInput = int.TryParse(Console.ReadLine(), out gameMode) && (gameMode == 1 || gameMode == 2);
                invalidInputMessage(validInput);
            }

            isTwoPlayerMode = (gameMode == 2);
            //clear screen

            return isTwoPlayerMode;
        }

        private void invalidInputMessage(bool i_IsValidInput)
        {
            if (!i_IsValidInput)
            {
                Console.WriteLine("Invalid input! Please try again");
            }
        }
    }
}