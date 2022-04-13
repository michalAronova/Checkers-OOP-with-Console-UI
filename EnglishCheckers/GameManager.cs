namespace EnglishCheckers
{
    public class GameManager
    {
        private Board m_Board = null;
        private Player m_Player1 = new Player();
        private Player m_Player2 = new Player();
        private bool m_Player1sTurn = true;
        public enum eGameStatus
        {
            ContinueGame,
            Player1Wins,
            Player2Wins,
            InvalidMove,
            Tie,
        }

        public GameManager(int i_BoardSize, bool i_IsHumanPlayer)
        {
            m_Board = new Board(i_BoardSize);
            m_Player2.IsHumanPlayer = i_IsHumanPlayer;
        }

        public Board GameBoard
        {
            get
            {
                return m_Board;
            }
        }

        public eGameStatus InitiateMove(Coordinate i_SourceCoordinate, Coordinate i_DestinationCoordinate)
        {
            bool isValidMove = ValidateMove(i_SourceCoordinate, i_DestinationCoordinate);
            eGameStatus postMoveGameStatus;

            if(isValidMove)
            {

            }
            else
            {
                postMoveGameStatus = eGameStatus.InvalidMove;
            }

            return postMoveGameStatus;
        }

        private bool ValidateMove(Coordinate i_SourceCoordinate, Coordinate i_DestinationCoordinate)
        {

        }

        public eGameStatus InitiateComputerMove()
        {

        }
    }
}