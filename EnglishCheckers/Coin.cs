using System.Security.Cryptography;
using System.Security.Policy;

namespace EnglishCheckers
{
    public class Coin
    {
        private eCoinType m_CoinType; //default?
        private bool m_IsKing = false;

        public Coin(eCoinType i_CoinType)
        {
            m_CoinType = i_CoinType;
        }
        public eCoinType Type
        {
            get
            {
                return m_CoinType;
            }
        }
        public bool IsKing
        {
            get
            {
                return m_IsKing;
            }
            set
            {
                m_IsKing = value;
            }
        }
    }
}