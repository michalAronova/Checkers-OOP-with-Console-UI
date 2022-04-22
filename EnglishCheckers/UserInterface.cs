using System;
using System.Text;

namespace EnglishCheckers
{
    public class UserInterface
    {
        private string m_Player1Name = null;                    //GETTER FROM GAMEMANAGER.PLAYER
        private string m_Player2Name = null;                    //GETTER FROM GAMEMANAGER.PLAYER     
        private bool m_twoPlayerMode = !true;                   //GETTER FROM GAMEMANAGER
        private const char r_Player1Coin = 'X';
        private const char r_Player2Coin = 'O';
        private const char r_Player1King = 'K';
        private const char r_Player2King = 'U';

        public void InitializeGame()
        {
            int boardSize;
            bool exitGame = !true;
            GameManager gameManager = null;

            Console.WriteLine("Welcome to English Checkers!");
            m_Player1Name = getValidName();                                          //SETTER TO GAMEMANAGER.PLAYER    
            boardSize = getBoardSize();
            m_twoPlayerMode = getGameMode();
            m_Player2Name = m_twoPlayerMode ? getValidName() : "Computer";           //SETTER TO GAMEMANAGER.PLAYER
            gameManager = new GameManager(boardSize, m_twoPlayerMode);
            while (!exitGame)
            {
                runGame(gameManager);
                //initilize game from gamemanager
                exitGame = shouldExitGame();
            }

            Console.WriteLine("Goodbye and Thank you for playing :)");
        }

        private void runGame(GameManager io_GameManager)
        {
            GameManager.eGameStatus eGameStatus = GameManager.eGameStatus.ContinueGame;
            StringBuilder previousMove = new StringBuilder();
            bool isPlayer1sTurn = !true;        //NO NEED, GETTER FROM GM
            const bool v_ValidMove = true;

            while (eGameStatus == GameManager.eGameStatus.ContinueGame)
            {
                isPlayer1sTurn = !isPlayer1sTurn; //NO NEED, GETTER FROM GM
                printBoard(io_GameManager.GameBoard);
                printGameState(isPlayer1sTurn, previousMove, eGameStatus);
                previousMove.Clear();
                eGameStatus = getAndInitiateMove(io_GameManager, previousMove, isPlayer1sTurn);
                while (eGameStatus == GameManager.eGameStatus.InvalidMove)
                {
                    invalidInputMessage(!v_ValidMove);
                    eGameStatus = getAndInitiateMove(io_GameManager, previousMove, isPlayer1sTurn);
                }

                //sleep 3 secs?? maybe in the start of the block
                //clear screen
            }

            printGameResult(eGameStatus);
            //get and print points from game manager..
        }

        private void printBoard(Board i_GameBoard)
        {
            char row = 'a';
            string correspondingStringToRow;

            printColumnLine(i_GameBoard.Size);
            printLowerBound(i_GameBoard.Size);
            for (int i = 0; i < i_GameBoard.Size; i++)
            {
                correspondingStringToRow = createRowToString(i_GameBoard, i);
                Console.WriteLine(" {0} |{1}", row, correspondingStringToRow);
                printLowerBound(i_GameBoard.Size);
                row++;
            }

            ///////////////////clear screen
        }

        private void printGameState(bool i_IsPlayer1sTurn, StringBuilder i_PreviousMove, GameManager.eGameStatus i_EGameStatus)
        {
            string currentPlayerName = i_IsPlayer1sTurn ? m_Player1Name : m_Player2Name;
            string previousPlayerName = i_IsPlayer1sTurn ? m_Player2Name : m_Player1Name;
            char currentPlayerCoin = i_IsPlayer1sTurn ? r_Player1Coin : r_Player2Coin;
            char previousPlayerCoin = i_IsPlayer1sTurn ? r_Player2Coin : r_Player1Coin;

            if (i_PreviousMove.Length != 0)
            {
                Console.WriteLine(string.Format("{0}'s move was ({1}): {2}", previousPlayerName, previousPlayerCoin, i_PreviousMove));
            }

            Console.WriteLine(string.Format("{0}'s turn ({1}):", currentPlayerName, currentPlayerCoin));
        }

        private string createRowToString(Board i_GameBoard, int i_Row)
        {
            string rowToString = null;
            Coordinate coordinate = new Coordinate(i_Row, 0);
            Coin.eCoinType currentCoinType;
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
                    if (currentCoinType == Coin.eCoinType.Player1Coin)
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

        private void printGameResult(GameManager.eGameStatus i_EGameStatus)
        {
            switch (i_EGameStatus)
            {
                case GameManager.eGameStatus.ActivePlayerWins:
                    Console.WriteLine("{0} won!!", m_Player1Name);
                    break;
                case GameManager.eGameStatus.NextPlayerWins:
                    if (m_twoPlayerMode)
                    {
                        Console.WriteLine("{0} won!!", m_Player2Name);
                    }
                    else
                    {
                        Console.WriteLine("The computer won!!");
                    }
                    break;
                case GameManager.eGameStatus.Tie:
                    Console.WriteLine("It's a tie!");
                    break;
            }
        }

        private GameManager.eGameStatus getAndInitiateMove(GameManager io_GameManager, StringBuilder o_MoveString, bool i_IsPlayer1sTurn)
        {
            GameManager.eGameStatus gameStatus = 0;
            Coordinate sourceCoordinate = new Coordinate();
            Coordinate destinationCoordinate = new Coordinate();
            bool didCurrentPlayerQuit = !true;

            if (!m_twoPlayerMode && !i_IsPlayer1sTurn)
            {
                // gameStatus = io_GameManager.InitiateComputerMove(out sourceCoordinate, out destinationCoordinate);
                convertCoordinatesToMoveString(o_MoveString, sourceCoordinate, destinationCoordinate);
            }
            else
            {
                didCurrentPlayerQuit = getValidMoveAndReturIfQuit(o_MoveString, io_GameManager.GameBoard.Size);
                if (didCurrentPlayerQuit)
                {
                    //gameStatus =  gameManager.player quit..
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

            ///////////////////////////////clear screen

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

            ///////////////////////////////clear screen
            return didPlayerQuit;
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