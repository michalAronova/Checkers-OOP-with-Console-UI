using System;
using System.Collections.Generic;

namespace EnglishCheckers
{
    public class Player
    {
        private string m_PlayersName;
        private bool m_IsHumanPlayer = true;
        private readonly eCoinType r_PlayersCoinType;
        private Dictionary<Coordinate, Coin> m_PlayersCoins;
        private int m_PlayerPoints = 0;
        private readonly eDirection r_Direction;

        public Player(eDirection i_Direction, eCoinType i_CoinType, Dictionary<Coordinate,Coin> i_PlayersCoins, string i_PlayerName)
        {
            r_Direction = i_Direction;
            r_PlayersCoinType = i_CoinType;
            m_PlayersCoins = i_PlayersCoins;
            m_PlayersName = i_PlayerName;
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

        public eCoinType CoinType
        {
            get
            {
                return r_PlayersCoinType;
            }
        }

        public int Points
        {
            get
            {
                return m_PlayerPoints;
            }
            set
            {
                m_PlayerPoints = value;
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

        public void UpdatePlayersCoins(Coordinate i_SourceSquare, Coordinate i_DestinationSquare)
        {
            Coin movedCoin = m_PlayersCoins[i_SourceSquare];
            m_PlayersCoins.Remove(i_SourceSquare);
            m_PlayersCoins.Add(i_DestinationSquare, movedCoin);
        }

        public void RemovePlayersAteCoin(Coordinate i_CoinCoordinate)
        {
            m_PlayersCoins.Remove(i_CoinCoordinate);
        }
    }
}