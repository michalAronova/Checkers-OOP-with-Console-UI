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
            /// for each square will set coin to red/black/leave it null + put it in the players
            /// 
        }

        public int Size
        {
            get
            {
                return m_GameBoard.GetLength(0);
            }
        }

        public Square GetSquare(int i_Row, int i_Col)
        {
            return m_GameBoard[i_Row, i_Col];
        }

        public void SetSquare(int i_Row, int i_Col, Coin i_Coin)
        {
            m_GameBoard[i_Row, i_Col].Coin = i_Coin;
        }
    }
}