namespace EnglishCheckers
{
    public class Square
    {
        private int m_Row;
        private int m_Column;
        private Coin m_Coin = null;
        public int Row
        {
            get
            {
                return m_Row;
            }
            set
            {
                m_Row = value;
            }
        }

        public int Column
        {
            get
            {
                return m_Column;
            }
            set
            {
                m_Column = value;
            }
        }
    }
}