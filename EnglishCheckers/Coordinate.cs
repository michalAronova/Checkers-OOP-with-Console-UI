namespace EnglishCheckers
{
    public struct Coordinate
    {
        private int m_Row;
        private int m_Column;

        public Coordinate(int i_Row, int i_Column)
        {
            m_Row = i_Row;
            m_Column = i_Column;
        }

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