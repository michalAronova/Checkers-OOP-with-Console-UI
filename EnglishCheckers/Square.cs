namespace EnglishCheckers
{
    public class Square
    {
        private Coordinate m_Coordinate;
        private Coin m_Coin = null;

        public Coin Coin
        {
            get
            {
                return m_Coin;
            }
            set
            {
                m_Coin = value;
            }
        }
    }
}