namespace EnglishCheckers
{
    public class Square
    {
        private Coordinate m_Coordinate;
        private Coin m_Coin = null;

        public Coordinate Coordinate
        {
            get
            {
                return m_Coordinate;
            }
            set
            {
                m_Coordinate = value;
            }
        }

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