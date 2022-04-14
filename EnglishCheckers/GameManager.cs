using System;

namespace EnglishCheckers
{
    public class GameManager
    {
        private Board m_Board = null;
        private Player m_Player1 = new Player(Player.eDirection.Down);
        private Player m_Player2 = new Player(Player.eDirection.Up);
        private bool m_Player1sTurn = true;
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
            m_Player2.IsHumanPlayer = i_IsHumanPlayer;
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
            eGameStatus postMoveGameStatus;
            Player currentPlayer; //?


            if (m_Player1sTurn)
            {
                currentPlayer = m_Player1;
            }
            else
            {
                currentPlayer = m_Player2;
            }

            if (isValidMove)
            {
                currentPlayer.Move(m_Board.GetSquare(i_SourceCoordinate), m_Board.GetSquare(i_DestinationCoordinate));
                //check if win/tie

                //else
                m_Player1sTurn = !m_Player1sTurn;
                postMoveGameStatus = eGameStatus.ContinueGame;
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
            //check if valid physically
            //
            //check if valid logicaly, maybe player must move somewhere else
            //return result
            return isValidMove;
        }

        public eGameStatus InitiateComputerMove()
        {
            
        }
    }
}