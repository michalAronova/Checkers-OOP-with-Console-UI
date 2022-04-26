using System;
using System.Text;

namespace EnglishCheckers
{
    public class UserInterface
    {   
        private const char r_Player1Coin = 'X';
        private const char r_Player2Coin = 'O';
        private const char r_Player1King = 'K';
        private const char r_Player2King = 'U';

        public void StartGame()
        {
            int boardSize;
            bool exitGame = !true;
            bool twoPlayerMode = !true;
            GameManager gameManager = null;
            string player1Name = null;
            string player2Name = null;

            Console.WriteLine("Welcome to English Checkers!");
            player1Name = getValidName();  
            boardSize = getBoardSize();
            twoPlayerMode = getGameMode();
            player2Name = twoPlayerMode ? getValidName() : "Computer";
            gameManager = new GameManager(boardSize, twoPlayerMode, player1Name, player2Name);
            while (!exitGame)
            {
                runGame(gameManager);
                gameManager.InitiateGame();
                exitGame = shouldExitGame();
            }

            Console.WriteLine("Goodbye and Thank you for playing :)");
        }

        private void runGame(GameManager io_GameManager)
        {
            eGameStatus eGameStatus = eGameStatus.ContinueGame;
            StringBuilder previousMove = new StringBuilder();
            const bool v_ValidMove = true;

            while (eGameStatus == eGameStatus.ContinueGame || eGameStatus == eGameStatus.ActivePlayerHasAnotherMove)
            {
                printBoard(io_GameManager.GameBoard);
                printGameState(io_GameManager.ActivePlayer, io_GameManager.NextPlayer, previousMove, eGameStatus);
                previousMove.Clear();
                eGameStatus = getAndInitiateMove(io_GameManager, previousMove);
                while (eGameStatus == eGameStatus.InvalidMove)
                {
                    invalidInputMessage(!v_ValidMove);
                    eGameStatus = getAndInitiateMove(io_GameManager, previousMove);
                }
            }

            printBoard(io_GameManager.GameBoard);
            printGameResult(eGameStatus, io_GameManager.ActivePlayer.Name, io_GameManager.NextPlayer.Name);
            printPoints(io_GameManager.ActivePlayer, io_GameManager.NextPlayer);
        }

        private void printBoard(Board i_GameBoard)
        {
            char row = 'a';
            string correspondingStringToRow;

            Ex02.ConsoleUtils.Screen.Clear();
            printColumnLine(i_GameBoard.Size);
            printLowerBound(i_GameBoard.Size);
            for (int i = 0; i < i_GameBoard.Size; i++)
            {
                correspondingStringToRow = createRowToString(i_GameBoard, i);
                Console.WriteLine(" {0} |{1}", row, correspondingStringToRow);
                printLowerBound(i_GameBoard.Size);
                row++;
            }
        }

        private void printGameState(Player i_ActivePlayer, Player i_NextPlayer, StringBuilder i_PreviousMove, eGameStatus i_EGameStatus)
        {
            string currentPlayerName, previousPlayerName;
            char currentPlayerCoin, previousPlayerCoin;

            if(i_EGameStatus == eGameStatus.ActivePlayerHasAnotherMove)
            {
                previousPlayerName = currentPlayerName = i_ActivePlayer.Name;
                previousPlayerCoin = currentPlayerCoin = (i_ActivePlayer.CoinType == eCoinType.Player1Coin) ? r_Player1Coin : r_Player2Coin;
            }
            else
            {
                previousPlayerName = i_NextPlayer.Name;
                currentPlayerName = i_ActivePlayer.Name;
                previousPlayerCoin = (i_ActivePlayer.CoinType == eCoinType.Player1Coin) ? r_Player2Coin : r_Player1Coin;
                currentPlayerCoin = (i_ActivePlayer.CoinType == eCoinType.Player1Coin) ? r_Player1Coin : r_Player2Coin;
            }

            if (i_PreviousMove.Length != 0)
            {
                Console.WriteLine("{0}'s move was ({1}): {2}", previousPlayerName, previousPlayerCoin, i_PreviousMove);
            }

            Console.WriteLine("{0}'s turn ({1}):", currentPlayerName, currentPlayerCoin);
        }

        private string createRowToString(Board i_GameBoard, int i_Row)
        {
            string rowToString = null;
            Coordinate coordinate = new Coordinate(i_Row, 0);
            eCoinType currentCoinType;
            bool isCurrentCoinKing = !true;

            for (int i = 0; i < i_GameBoard.Size; i++)
            {
                if (i_GameBoard.GetSquare(coordinate).Coin == null)
                {
                    rowToString += "   ";
                }
                else
                {
                    currentCoinType = i_GameBoard.GetSquare(coordinate).Coin.Type;
                    isCurrentCoinKing = i_GameBoard.GetSquare(coordinate).Coin.IsKing;
                    rowToString += " ";
                    if (currentCoinType == eCoinType.Player1Coin)
                    {
                        rowToString += isCurrentCoinKing ? r_Player1King : r_Player1Coin;
                    }
                    else
                    {
                        rowToString += isCurrentCoinKing ? r_Player2King : r_Player2Coin;
                    }

                    rowToString += " ";
                }

                rowToString += "|";
                coordinate.Column++;
            }

            return rowToString;
        }

        private void printColumnLine(int i_BoardSize)
        {
            string firstLine = "     ";
            char column = 'A';

            for (int i = 0; i < i_BoardSize; i++)
            {
                firstLine += column;
                firstLine += "   ";
                column++;
            }

            Console.WriteLine(firstLine);
        }

        private void printLowerBound(int i_BoardSize)
        {
            string lowerBound = "====";

            for (int i = 0; i < i_BoardSize; i++)
            {
                lowerBound += "====";
            }

            Console.WriteLine(lowerBound);
        }

        private void printGameResult(eGameStatus i_EGameStatus, string i_ActivePlayerName, string i_NextPlayerName)
        {
            string winnersName = null;

            if (i_EGameStatus == eGameStatus.Tie)
            {
                Console.WriteLine("It's a tie!");
            }
            else
            {
                winnersName = (i_EGameStatus == eGameStatus.ActivePlayerWins) ? i_ActivePlayerName : i_NextPlayerName;
                Console.WriteLine("{0} won!!", winnersName);
            }
        }

        private void printPoints(Player i_ActivePlayer, Player i_NextPlayer)
        {
            Console.WriteLine(@"
Score:
{0}: {2}        {1}: {3}", i_ActivePlayer.Name, i_NextPlayer.Name, i_ActivePlayer.Points, i_NextPlayer.Points);
        }

        private eGameStatus getAndInitiateMove(GameManager io_GameManager, StringBuilder o_MoveString)
        {
            eGameStatus gameStatus = 0;
            Coordinate sourceCoordinate = new Coordinate();
            Coordinate destinationCoordinate = new Coordinate();
            bool didCurrentPlayerQuit = !true;

            if (!io_GameManager.TwoPlayersMode && (io_GameManager.ActivePlayer.CoinType == eCoinType.Player2Coin))
            {
                gameStatus = io_GameManager.InitiateComputerMove(out sourceCoordinate, out destinationCoordinate);
                convertCoordinatesToMoveString(o_MoveString, sourceCoordinate, destinationCoordinate);
            }
            else
            {
                didCurrentPlayerQuit = getValidMoveAndReturIfQuit(o_MoveString, io_GameManager.GameBoard.Size);
                if (didCurrentPlayerQuit)
                {
                    gameStatus = io_GameManager.CurrentPlayerQuit();
                }
                else
                {
                    convertStringToCoordinates(o_MoveString, out sourceCoordinate, out destinationCoordinate);
                    gameStatus = io_GameManager.InitiateMove(sourceCoordinate, destinationCoordinate);
                }
            }

            return gameStatus;
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

            Ex02.ConsoleUtils.Screen.Clear();

            return name;
        }

        private bool getValidMoveAndReturIfQuit(StringBuilder o_MoveString, int i_BoardSize)
        {
            bool isValidMove = !true;
            bool didPlayerQuit = !true;

            while (!isValidMove)
            {
                o_MoveString.Clear();
                o_MoveString.Insert(0, Console.ReadLine());
                if (o_MoveString.Length == 1 && o_MoveString[0] == 'Q')
                {
                    isValidMove = true;
                    didPlayerQuit = true;
                }
                else
                {
                    isValidMove = (o_MoveString.Length == 5);
                    for (int i = 0; i < o_MoveString.Length && isValidMove; i++)
                    {
                        switch (i)
                        {
                            case 0:
                            case 3:
                                if (o_MoveString[i] < 'A' || o_MoveString[i] >= ('A' + i_BoardSize))
                                {
                                    isValidMove = !true;
                                }
                                break;
                            case 1:
                            case 4:
                                if (o_MoveString[i] < 'a' || o_MoveString[i] >= ('a' + i_BoardSize))
                                {
                                    isValidMove = !true;
                                }
                                break;
                            case 2:
                                if (o_MoveString[i] != '>')
                                {
                                    isValidMove = !true;
                                }
                                break;
                        }
                    }
                }
                invalidInputMessage(isValidMove);
            }

            return didPlayerQuit;
        }

        private int getBoardSize()
        {
            int boardSize = 0;
            bool validInput = !true;

            System.Console.WriteLine(
@"Please choose board size:
(6) 6X6     (8) 8X8     (10) 10X10");
            while (!validInput)
            {
                validInput = int.TryParse(Console.ReadLine(), out boardSize) && (boardSize == 6 || boardSize == 8 || boardSize == 10);
                invalidInputMessage(validInput);
            }

            Ex02.ConsoleUtils.Screen.Clear();

            return boardSize;
        }

        private bool getGameMode()
        {
            bool isTwoPlayerMode = !true;
            bool validInput = !true;
            int gameMode = 1;

            System.Console.WriteLine(
@"Please choose game mode:
(1) 1 player     (2) 2 players");
            while (!validInput)
            {
                validInput = int.TryParse(Console.ReadLine(), out gameMode) && (gameMode == 1 || gameMode == 2);
                invalidInputMessage(validInput);
            }

            isTwoPlayerMode = (gameMode == 2);
            Ex02.ConsoleUtils.Screen.Clear();

            return isTwoPlayerMode;
        }

        private void invalidInputMessage(bool i_IsValidInput)
        {
            if (!i_IsValidInput)
            {
                Console.WriteLine("Invalid input! Please try again");
            }
        }

        private void convertStringToCoordinates(StringBuilder i_MoveString, out Coordinate o_sourceCoordinate, out Coordinate o_destinationCoordinate)
        {
            o_sourceCoordinate = convertCharsToCoordinate(i_MoveString[0], i_MoveString[1]);
            o_destinationCoordinate = convertCharsToCoordinate(i_MoveString[3], i_MoveString[4]); ;
        }

        private Coordinate convertCharsToCoordinate(char i_Column, char i_Row)
        {
            Coordinate coordinate = new Coordinate((i_Row - 'a'),(i_Column - 'A'));

            return coordinate;
        }

        private void convertCoordinatesToMoveString(StringBuilder o_MoveString, Coordinate i_SourceCoordinate, Coordinate i_DestinationCoordinate)
        {
            insertCoordinateToString(o_MoveString, 0, i_SourceCoordinate);
            o_MoveString.Insert(2, '>');
            insertCoordinateToString(o_MoveString, 3, i_DestinationCoordinate);
        }

        private void insertCoordinateToString(StringBuilder o_MoveString, int i_StartIndex, Coordinate i_Coordinate)
        {
            o_MoveString.Insert(i_StartIndex, getCharOfColumn(i_Coordinate.Column));
            i_StartIndex++;
            o_MoveString.Insert(i_StartIndex, getCharOfRow(i_Coordinate.Row));
        }

        private char getCharOfColumn(int i_Column)
        {
            return (char)('A' + i_Column);
        }

        private char getCharOfRow(int i_Row)
        {
            return (char)('a' + i_Row);
        }

        private bool shouldExitGame()
        {
            string input;
            bool exitGame = !true;

            System.Console.WriteLine("Insert any key to continue, or Q to Exit game");
            input = Console.ReadLine();
            if (input.Equals("Q"))
            {
                exitGame = true;
            }

            return exitGame;
        }
    }
}