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
        private void Move(Square i_SourceSquare, Square i_DestinationSquare)
        {

        }
    }
}