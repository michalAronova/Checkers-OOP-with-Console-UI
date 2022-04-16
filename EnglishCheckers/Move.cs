using System.Collections.Generic;

namespace EnglishCheckers
{
    public class Move
    {
        private Coordinate m_SourceCoordinate;
        private Coordinate m_DestinationCoordinate;
        private Player.eDirection m_Direction;
        private bool m_IsJumpMove;

        public Move(Coordinate i_SourceCoordinate, Coordinate i_DestinationCoordinate, Player.eDirection i_Direction, bool i_IsJumpMove)
        {
            m_SourceCoordinate = i_SourceCoordinate;
            m_DestinationCoordinate = i_DestinationCoordinate;
            m_Direction = i_Direction;
            m_IsJumpMove = i_IsJumpMove;
        }
        public Coordinate Source
        {
            get
            {
                return m_SourceCoordinate;
            }
            set
            {
                m_SourceCoordinate = value;
            }
        }
        public Coordinate Destination
        {
            get
            {
                return m_DestinationCoordinate;
            }
            set
            {
                m_DestinationCoordinate = value;
            }
        }
        public Player.eDirection Direction
        {
            get
            {
                return m_Direction;
            }
            set
            {
                m_Direction = value;
            }
        }
        public bool IsJumpMove
        {
            get
            {
                return m_IsJumpMove;
            }
            set
            {
                m_IsJumpMove = value;
            }
        }
        
    }
}