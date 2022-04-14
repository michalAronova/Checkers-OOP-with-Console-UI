using System.Collections.Generic;

namespace EnglishCheckers
{
    public class Player
    {
        private bool m_IsHumanPlayer = true;
        private Dictionary<Coordinate, Coin> m_PlayersCoins;
        private int m_PlayerPoints = 0;
        private readonly eDirection r_Direction;
        public enum eDirection
        {
            Up,
            Down,
        }
        public Player(eDirection i_Direction)
        {
            r_Direction = i_Direction;
        }

        public bool IsHumanPlayer
        {
            get
            {
                return m_IsHumanPlayer;
            }
            set
            {
                m_IsHumanPlayer = value;
            }
        }
        public void Move(Square i_SourceSquare, Square i_DestinationSquare) //makes legal move
        {
            i_DestinationSquare.Coin = i_SourceSquare.Coin;
            i_SourceSquare.Coin = null;
            m_PlayersCoins.Remove(i_SourceSquare.Coordinate);
            m_PlayersCoins.Add(i_DestinationSquare.Coordinate, i_DestinationSquare.Coin);
        }
    }
}