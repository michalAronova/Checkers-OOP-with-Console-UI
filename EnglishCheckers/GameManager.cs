namespace EnglishCheckers
{
    public class GameManager
    {
        private Board m_Board = null;
        private Player m_Player1 = new Player();
        private Player m_Player2 = new Player();

        public GameManager(int i_BoardSize, bool i_IsHumanPlayer)
        {
            m_Board = new Board(i_BoardSize);
            m_Player2.IsHumanPlayer = i_IsHumanPlayer;
        }
    }
}