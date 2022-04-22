using System.Collections.Generic;

namespace EnglishCheckers
{
    public class Move
    {
        private Coordinate m_SourceCoordinate;
        private Coordinate m_DestinationCoordinate;
        private eDirection m_Direction;
        private bool m_IsJumpMove;
        private Coordinate? m_CoordinateOfJumpedOverCoin;

        public Move(Coordinate i_SourceCoordinate, Coordinate i_DestinationCoordinate, eDirection i_Direction, bool i_IsJumpMove, Coordinate? i_JumpedOverCoordinate)
        {
            m_SourceCoordinate = i_SourceCoordinate;
            m_DestinationCoordinate = i_DestinationCoordinate;
            m_Direction = i_Direction;
            m_IsJumpMove = i_IsJumpMove;
            if(i_JumpedOverCoordinate != null)
            {
                m_CoordinateOfJumpedOverCoin = i_JumpedOverCoordinate;
            }
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

        public eDirection Direction
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

        public Coordinate CoordinateOfJumpedOverCoin
        {
            get
            {
                return m_CoordinateOfJumpedOverCoin.Value;
            }
            set
            {
                m_CoordinateOfJumpedOverCoin = value;
            }
        }
    }
}