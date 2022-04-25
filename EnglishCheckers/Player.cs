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
            set
            {
                m_PlayersCoins = value;
            }
        }

        public void UpdatePlayersCoins(Coordinate i_SourceCoordinate, Coordinate i_DestinationCoordinate, int i_BoardSize)
        {
            Coin movedCoin = m_PlayersCoins[i_SourceCoordinate];

            m_PlayersCoins.Remove(i_SourceCoordinate);
            if (!movedCoin.IsKing)
            {
                if ((movedCoin.Type == eCoinType.Player1Coin && i_DestinationCoordinate.Row == 0) ||
                    (movedCoin.Type == eCoinType.Player2Coin && i_DestinationCoordinate.Row == i_BoardSize - 1))
                {
                    movedCoin.IsKing = true;
                }
            }

            m_PlayersCoins.Add(i_DestinationCoordinate, movedCoin);
        }

        public void RemovePlayersAteCoin(Coordinate i_CoinCoordinate)
        {
            m_PlayersCoins.Remove(i_CoinCoordinate);
        }

        public int GetCurrentGamePoints()
        {
            int pointsCount = 0, pointsToAdd = 0;

            foreach (KeyValuePair<Coordinate, Coin> coinCoordinate in m_PlayersCoins)
            {
                pointsToAdd = (coinCoordinate.Value.IsKing) ? 4 : 1;
                pointsCount += pointsToAdd;
            }

            return pointsCount;
        }
    }
}