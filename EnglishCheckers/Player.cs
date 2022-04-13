using System.Collections.Generic;

namespace EnglishCheckers
{
    public class Player
    {
        private bool m_IsHumanPlayer = true;
        private LinkedList<Coin> m_PlayersCoins;

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
        }
    }
}