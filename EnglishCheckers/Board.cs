using System.Collections.Generic;

namespace EnglishCheckers
{
    public class Board
    {
        private Square[,] m_GameBoard;

        public Board(int i_BoardSize)
        {
            m_GameBoard = new Square[i_BoardSize, i_BoardSize];
            setInitialBoard();
        }

        private void setInitialBoard()
        {
            /// for each square will set row+col 
            /// for each square will set coin to red/black/leave it null + put it in the players list
            /// 
        }
    }
}