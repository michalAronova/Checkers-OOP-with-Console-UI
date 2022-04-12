using System.Security.Cryptography;

namespace EnglishCheckers
{
    public class Coin
    {
        public enum eCoinType
        {
            Red,
            Black,
        }
        private eCoinType m_CoinType; //default?
        private bool m_IsKing = false;
        
        public eCoinType Type
        {
            get
            {
                return m_CoinType;
            }
            set
            {
                m_CoinType = value;
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