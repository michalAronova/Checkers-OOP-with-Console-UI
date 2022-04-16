using System;
using System.Collections.Generic;

namespace EnglishCheckers
{
    public class Player
    {
        private string m_PlayersName;
        private bool m_IsHumanPlayer = true;
        private readonly Coin.eCoinType r_PlayersCoinType;
        private Dictionary<Coordinate, Coin> m_PlayersCoins;
        private int m_PlayerPoints = 0;
        private readonly eDirection r_Direction;
        public enum eDirection
        {
            Up,
            Down,
        }
        public Player(eDirection i_Direction, Coin.eCoinType i_CoinType)
        {
            r_Direction = i_Direction;
            r_PlayersCoinType = i_CoinType;
        }

        public string Name
        {
            get
            {
                return m_PlayersName;
            }
            set
            {
                m_PlayersName = value;
            }
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
        public Coin.eCoinType CoinType
        {
            get
            {
                return r_PlayersCoinType;
            }
        }
        public eDirection Direction
        {
            get
            {
                return r_Direction;
            }
        }
        public Dictionary<Coordinate, Coin> PlayersCoins
        {
            get
            {
                return m_PlayersCoins;
            }
        }
        public void Move(Square i_SourceSquare, Square i_DestinationSquare) //makes legal move
        {
            i_DestinationSquare.Coin = i_SourceSquare.Coin;
            i_SourceSquare.Coin = null;
            m_PlayersCoins.Remove(i_SourceSquare.Coordinate);
            m_PlayersCoins.Add(i_DestinationSquare.Coordinate, i_DestinationSquare.Coin);
        }
        

        public static void SwapPlayers(ref Player i_Player1, ref Player i_Player2)
        {
            Player tempPlayer = i_Player1;
            i_Player1 = i_Player2;
            i_Player2 = tempPlayer;
        }

    }
}