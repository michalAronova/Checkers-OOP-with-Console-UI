using System;
using System.Diagnostics.Eventing.Reader;

namespace EnglishCheckers
{
    public class GameManager
    {
        private Board m_Board = null;
        private Player m_ActivePlayer = new Player(Player.eDirection.Down, Coin.eCoinType.Player1Coin);
        private Player m_NextPlayer = new Player(Player.eDirection.Up, Coin.eCoinType.Player2Coin);
        public enum eGameStatus
        {
            ContinueGame,
            Player1Wins,
            Player2Wins,
            InvalidMove,
            Tie,
        }

        public GameManager(int i_BoardSize, bool i_IsHumanPlayer)
        {
            m_Board = new Board(i_BoardSize);
            m_NextPlayer.IsHumanPlayer = i_IsHumanPlayer;
        }

        public Board GameBoard
        {
            get
            {
                return m_Board;
            }
        }

        public eGameStatus InitiateMove(Coordinate i_SourceCoordinate, Coordinate i_DestinationCoordinate)
        {
            bool isValidMove = ValidateMove(i_SourceCoordinate, i_DestinationCoordinate);
            eGameStatus postMoveGameStatus; //some assignment here?

            if (isValidMove)
            {
                m_ActivePlayer.Move(m_Board.GetSquare(i_SourceCoordinate), m_Board.GetSquare(i_DestinationCoordinate));
                
                ///thought- maybe add next possible moves for each square? does it belong in square?
                /// if does not belong in square, we need to go through all the players coins
                /// and check for next possible moves, each time. why not save it somehow?
                /// then computer move can use that too.
                
                /// check for next player's possible moves
                /// if no possible moves
                ///     check for current players possible moves
                ///     if no possible moves for current - tie
                ///     else current wins
                /// else swap players and continue game
                else
                {
                    Player.SwapPlayers(ref m_ActivePlayer, ref m_NextPlayer);
                    if(m_ActivePlayer.IsHumanPlayer)
                    {
                        postMoveGameStatus = eGameStatus.ContinueGame;
                    }
                    else
                    {
                        postMoveGameStatus = InitiateComputerMove();
                    }
                }

                return postMoveGameStatus;
            }
            else
            {
                postMoveGameStatus = eGameStatus.InvalidMove;
            }

            return postMoveGameStatus;
        }

        private bool ValidateMove(Coordinate i_SourceCoordinate, Coordinate i_DestinationCoordinate)
        {
            bool isValidMove = true;
            if(checkIfInBorders(i_SourceCoordinate) && checkIfInBorders(i_DestinationCoordinate))
            {

            }
            else
            {
                
            }
            //check if valid logicaly, maybe player must move somewhere else
            //return result
            return isValidMove;
        }

        private bool checkIfInBorders(Coordinate i_Coordinate)
        {
            return (i_Coordinate.Column >= 0 && i_Coordinate.Column < m_Board.Size)
                   && (i_Coordinate.Row >= 0 && i_Coordinate.Row < m_Board.Size);
        }

        private bool checkIfLogicallyValid(Coordinate i_SourceCoordinate, Coordinate i_DestinationCoordinate, out bool o_AteARivalCoin)
        {
            bool isLogicalMove = isValidSourceCoordination(i_SourceCoordinate);
            if(isLogicalMove && m_Board.GetSquare(i_DestinationCoordinate).Coin == null)
            {
                isLogicalMove = checkIfDiagonalMove(i_SourceCoordinate, i_DestinationCoordinate, out o_AteARivalCoin);
            }
            return isLogicalMove;
        }

        private bool isValidSourceCoordination(Coordinate i_SourceCoordinate)
        {
            bool isValidSource = (m_Board.GetSquare(i_SourceCoordinate).Coin == null);
            if(isValidSource)
            {
                isValidSource = (m_Board.GetSquare(i_SourceCoordinate).Coin.Type == m_ActivePlayer.CoinType);
            }
            return isValidSource;
        }

        private bool checkIfDiagonalMove(Coordinate i_SourceCoordinate, Coordinate i_DestinationCoordinate, out bool o_AteARivalCoin)
        {
            Coin movingCoin = m_Board.GetSquare(i_SourceCoordinate).Coin;
            bool isValidDiagonal = false;
            bool validDownward = checkIfDownwardDiagonal(i_SourceCoordinate,i_DestinationCoordinate, out o_AteARivalCoin);
            bool validUpward = checkIfUpwardDiagonal(i_SourceCoordinate, i_DestinationCoordinate, out o_AteARivalCoin);

            if(movingCoin.IsKing)
            {
                isValidDiagonal = validUpward || validDownward;
            }
            else
            {
                if(m_ActivePlayer.Direction == Player.eDirection.Down)
                {
                    isValidDiagonal = validDownward;
                }
                else
                {
                    isValidDiagonal = validUpward;
                }
            }

            return isValidDiagonal;
        }

        private bool checkIfUpwardDiagonal(Coordinate i_SourceCoordinate, Coordinate i_DestinationCoordinate, out bool o_AteARivalCoin)
        {
            o_AteARivalCoin = false;
            bool isUpwardsDiagonal = (i_SourceCoordinate.Row - 1 == i_DestinationCoordinate.Row) && 
                                     (checkIfDiagonalColumn(i_SourceCoordinate.Column, i_DestinationCoordinate.Column, false));
            if(!isUpwardsDiagonal)
            {
                isUpwardsDiagonal = (i_SourceCoordinate.Row - 2 == i_DestinationCoordinate.Row) &&
                                    (checkIfDiagonalColumn(i_SourceCoordinate.Column, i_DestinationCoordinate.Column, true));
                if(isUpwardsDiagonal)
                {
                    o_AteARivalCoin = isThereARivalCoinBetween(i_SourceCoordinate, i_DestinationCoordinate);
                    isUpwardsDiagonal = o_AteARivalCoin;
                }
            }
            return isUpwardsDiagonal;
        }
        private bool checkIfDownwardDiagonal(Coordinate i_SourceCoordinate, Coordinate i_DestinationCoordinate, out bool o_AteARivalCoin)
        {
            o_AteARivalCoin = false;
            bool isDownwardsDiagonal = (i_SourceCoordinate.Row + 1 == i_DestinationCoordinate.Row) &&
                                     (checkIfDiagonalColumn(i_SourceCoordinate.Column, i_DestinationCoordinate.Column, false));
            if (!isDownwardsDiagonal)
            {
                isDownwardsDiagonal = (i_SourceCoordinate.Row + 2 == i_DestinationCoordinate.Row) &&
                                      (checkIfDiagonalColumn(i_SourceCoordinate.Column, i_DestinationCoordinate.Column, true));
                if (isDownwardsDiagonal)
                {
                    o_AteARivalCoin = isThereARivalCoinBetween(i_SourceCoordinate, i_DestinationCoordinate);
                    isDownwardsDiagonal = o_AteARivalCoin;
                }
            }
            return isDownwardsDiagonal;
        }

        private bool isThereARivalCoinBetween(Coordinate i_SourceCoordinate, Coordinate i_DestinationCoordinate)
        {
            
        }

        private bool checkIfDiagonalColumn(int i_SourceColumn, int i_DestinationColumn, bool i_IsDoubleMove)
        {
            int distanceToCheck = i_IsDoubleMove ? 2 : 1;
            return ((i_SourceColumn - distanceToCheck == i_DestinationColumn) || (i_SourceColumn + distanceToCheck == i_DestinationColumn));
        }
        private eGameStatus InitiateComputerMove()
        {
            
        }
    }
}