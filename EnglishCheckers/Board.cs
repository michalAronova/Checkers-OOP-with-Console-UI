﻿using System.Collections.Generic;

namespace EnglishCheckers
{
    public class Board
    {
        private Square[,] m_GameBoard;
        private int m_BoardSize;

        public Board(int i_BoardSize)
        {
            m_BoardSize = i_BoardSize;
            m_GameBoard = new Square[m_BoardSize, m_BoardSize];
            setInitialBoard();
        }

        private void setInitialBoard()
        {
            int numberOfInitialPlayerRows = (m_BoardSize / 2 - 1);

            for(int i = 0; i < m_BoardSize; i++)
            {
                if(i < numberOfInitialPlayerRows)
                {
                    setupRow(i, Coin.eCoinType.Player2Coin);
                }
                else if(i >= m_BoardSize - numberOfInitialPlayerRows)
                {
                    setupRow(i, Coin.eCoinType.Player1Coin);
                }
                else
                {
                    setRowCoordinates(i);
                }
            }
        }

        private void setupRow(int i_Row, Coin.eCoinType i_CoinType)
        {
            for(int i = 0; i < m_BoardSize; i++)
            {
                setRowCoordinates(i_Row);
                if(isACoinInitialSpot(i_Row, i))
                {
                    m_GameBoard[i_Row, i].Coin = new Coin(i_CoinType);
                }
            }
        }

        private void setRowCoordinates(int i_Row)
        {
            Coordinate currentCoordinate;
            for(int i = 0; i < m_BoardSize; i++)
            {
                currentCoordinate = new Coordinate(i_Row, i);
                m_GameBoard[i_Row, i].Coordinate = currentCoordinate;
            }
        }

        private bool isACoinInitialSpot(int i_Row, int i_Column)
        {
            return (i_Row + i_Column) % 2 != 0;
        }

        public int Size
        {
            get
            {
                return m_GameBoard.GetLength(0);
            }
        }

        public Square GetSquare(Coordinate i_Coordinate)
        {
            return m_GameBoard[i_Coordinate.Row, i_Coordinate.Column];
        }

        public void SetSquare(Coordinate i_Coordinate, Coin i_Coin)
        {
            m_GameBoard[i_Coordinate.Row, i_Coordinate.Column].Coin = i_Coin;
        }
    }
}