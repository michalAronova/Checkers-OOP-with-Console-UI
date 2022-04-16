﻿using System.Collections.Generic;

namespace EnglishCheckers
{
    public class GameManager
    {
        private Board m_Board = null;
        private Player m_ActivePlayer = new Player(Player.eDirection.Down, Coin.eCoinType.Player1Coin);
        private Player m_NextPlayer = new Player(Player.eDirection.Up, Coin.eCoinType.Player2Coin);
        public enum eGameStatus
        {
            ContinueGame,
            ActivePlayerWins,
            NextPlayerWins,
            InvalidMove,
            Tie,
        }

        public GameManager(int i_BoardSize, bool i_IsHumanPlayer)
        {
            m_Board = new Board(i_BoardSize);
            m_NextPlayer.IsHumanPlayer = i_IsHumanPlayer;
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
            List<Move> activePlayersValidMoves = calculateMovesForAllPlayersCoins(m_ActivePlayer.PlayersCoins);
            List<Move> nextPlayersValidMoves;
            ///bool isValidMove = ValidateMove(i_SourceCoordinate, i_DestinationCoordinate);
            
            bool isValidMove = activePlayersValidMoves.Exists(
                move => move.Source.Equals(i_SourceCoordinate) && move.Destination.Equals(i_DestinationCoordinate));

            eGameStatus postMoveGameStatus;

            if(isValidMove)
            {
                m_ActivePlayer.Move(m_Board.GetSquare(i_SourceCoordinate), m_Board.GetSquare(i_DestinationCoordinate));
                //Board needs to change here too
                nextPlayersValidMoves = calculateMovesForAllPlayersCoins(m_NextPlayer.PlayersCoins);
                if(nextPlayersValidMoves.Count == 0)
                {
                    postMoveGameStatus = eGameStatus.ActivePlayerWins;
                }
                else
                {
                    activePlayersValidMoves = calculateMovesFrom(
                        i_DestinationCoordinate,
                        m_Board.GetSquare(i_DestinationCoordinate).Coin);
                    if(activePlayersValidMoves.Exists(move => move.IsJumpMove))
                    {
                        ///need to calcuate moves from destination -> if theres another jump - no swap! this is the only valid move!
                        /// this might mean we need to save this list in player :(
                        /// and then here we will calculate it and remove all others
                        /// but gamemanager has to be the one to calculate it
                        postMoveGameStatus = eGameStatus.ContinueGame; //no swapping, stays in activePlayers
                    }
                    else
                    {
                        Player.SwapPlayers(ref m_ActivePlayer, ref m_NextPlayer);
                        if(m_ActivePlayer.IsHumanPlayer)
                        {
                            postMoveGameStatus = eGameStatus.ContinueGame;
                        }
                        else
                        {
                            postMoveGameStatus = InitiateComputerMove();
                        }
                    }
                }
            }
            else
            {
                postMoveGameStatus = eGameStatus.InvalidMove;
            }

            return postMoveGameStatus;
        }
        
        private void removeNoJumps(List<Move> i_Moves)
        {
            if(i_Moves.Exists(move => move.IsJumpMove))
            {
                foreach(Move move in i_Moves)
                {
                    if(!move.IsJumpMove)
                    {
                        i_Moves.Remove(move);
                    }
                }
            }
        }

        private List<Move> calculateMovesForAllPlayersCoins(Dictionary<Coordinate, Coin> i_PlayersCoins)
        {
            List<Move> allPossibleMoves = new List<Move>();
            List<Move> movesFromGivenCoin;
            foreach (KeyValuePair<Coordinate, Coin> coinCoordinate in i_PlayersCoins)
            {
                movesFromGivenCoin = calculateMovesFrom(coinCoordinate.Key, coinCoordinate.Value);
                if(movesFromGivenCoin != null)
                {
                    movesFromGivenCoin.ForEach(move => allPossibleMoves.Add(move));
                }
            }
            removeNoJumps(allPossibleMoves);
            return allPossibleMoves;
        }

        private List<Move> calculateMovesFrom(Coordinate i_SourceCoordinate, Coin i_CoinToMove)
        {
            List<Move> moves = new List<Move>();
            List<Move> kingMoves = null;
            Player.eDirection coinsDirection = (i_CoinToMove.Type == m_ActivePlayer.CoinType)
                                                   ? m_ActivePlayer.Direction
                                                   : m_NextPlayer.Direction;
            if(i_CoinToMove.IsKing)
            {
                if(coinsDirection == Player.eDirection.Down)
                {
                    kingMoves = calculateMovesByDirection(i_CoinToMove, i_SourceCoordinate, Player.eDirection.Up);
                }
                else
                {
                    kingMoves = calculateMovesByDirection(i_CoinToMove, i_SourceCoordinate, Player.eDirection.Down);
                }
            }

            moves = calculateMovesByDirection(i_CoinToMove, i_SourceCoordinate, coinsDirection);

            if(kingMoves != null)
            {
                kingMoves.ForEach(move => moves.Add(move));
            }
            return moves;
        }

        private List<Move> calculateMovesByDirection(Coin i_CoinToMove, Coordinate i_SourceCoordinate, Player.eDirection i_Direction)
        {
            List<Coordinate> possibleDiagonalCoordinates = m_Board.GetDiagonalInDirection(i_SourceCoordinate, i_Direction);
            bool isAJumpMove = true;
            List<Move> possibleMoves = new List<Move>();
            List<Coordinate> possibleJumps = null;
            foreach (Coordinate diagonalCoordinate in possibleDiagonalCoordinates)
            {
                if(m_Board.GetSquare(diagonalCoordinate).Coin == null)
                {
                    possibleMoves.Add(new Move(i_SourceCoordinate, diagonalCoordinate, i_Direction, !isAJumpMove));
                }
                else if(i_CoinToMove.Type != m_Board.GetSquare(diagonalCoordinate).Coin.Type)
                {
                    possibleJumps = m_Board.GetDiagonalInDirection(diagonalCoordinate, i_Direction);
                    foreach(Coordinate jumpCoordinate in possibleJumps)
                    {
                        if(m_Board.GetSquare(jumpCoordinate).Coin == null)
                        {
                            possibleMoves.Add(new Move(i_SourceCoordinate, jumpCoordinate, i_Direction, isAJumpMove));
                        }
                    }
                }
            }
            return possibleMoves;
        }
        ///private bool ValidateMove(Coordinate i_SourceCoordinate, Coordinate i_DestinationCoordinate)
        ///{
        ///    bool isValidMove = true;
        ///    if(checkIfInBorders(i_SourceCoordinate) && checkIfInBorders(i_DestinationCoordinate))
        ///    {
        ///
        ///    }
        ///    else
        ///    {
        ///        
        ///    }
        ///    //check if valid logicaly, maybe player must move somewhere else
        ///    //return result
        ///    return isValidMove;
        ///}

        ///private bool checkIfInBorders(Coordinate i_Coordinate)
        ///{
        ///    return (i_Coordinate.Column >= 0 && i_Coordinate.Column < m_Board.Size)
        ///           && (i_Coordinate.Row >= 0 && i_Coordinate.Row < m_Board.Size);
        ///}

        ///private bool checkIfLogicallyValid(Coordinate i_SourceCoordinate, Coordinate i_DestinationCoordinate, out bool o_AteARivalCoin)
        ///{
        ///    bool isLogicalMove = isValidSourceCoordination(i_SourceCoordinate);
        ///    if(isLogicalMove && m_Board.GetSquare(i_DestinationCoordinate).Coin == null)
        ///    {
        ///        isLogicalMove = checkIfDiagonalMove(i_SourceCoordinate, i_DestinationCoordinate, out o_AteARivalCoin);
        ///    }
        ///    return isLogicalMove;
        ///}
        ///
        ///private bool isValidSourceCoordination(Coordinate i_SourceCoordinate)
        ///{
        ///    bool isValidSource = (m_Board.GetSquare(i_SourceCoordinate).Coin == null);
        ///    if(isValidSource)
        ///    {
        ///        isValidSource = (m_Board.GetSquare(i_SourceCoordinate).Coin.Type == m_ActivePlayer.CoinType);
        ///    }
        ///    return isValidSource;
        ///}
        ///
        ///private bool checkIfDiagonalMove(Coordinate i_SourceCoordinate, Coordinate i_DestinationCoordinate, out bool o_AteARivalCoin)
        ///{
        ///    Coin movingCoin = m_Board.GetSquare(i_SourceCoordinate).Coin;
        ///    bool isValidDiagonal = false;
        ///    bool validDownward = checkIfDownwardDiagonal(i_SourceCoordinate,i_DestinationCoordinate, out o_AteARivalCoin);
        ///    bool validUpward = checkIfUpwardDiagonal(i_SourceCoordinate, i_DestinationCoordinate, out o_AteARivalCoin);
        ///
        ///    if(movingCoin.IsKing)
        ///    {
        ///        isValidDiagonal = validUpward || validDownward;
        ///    }
        ///    else
        ///    {
        ///        if(m_ActivePlayer.Direction == Player.eDirection.Down)
        ///        {
        ///            isValidDiagonal = validDownward;
        ///        }
        ///        else
        ///        {
        ///            isValidDiagonal = validUpward;
        ///        }
        ///    }
        ///
        ///    return isValidDiagonal;
        ///}
        ///
        ///private bool checkIfUpwardDiagonal(Coordinate i_SourceCoordinate, Coordinate i_DestinationCoordinate, out bool o_AteARivalCoin)
        ///{
        ///    o_AteARivalCoin = false;
        ///    bool isUpwardsDiagonal = (i_SourceCoordinate.Row - 1 == i_DestinationCoordinate.Row) && 
        ///                             (checkIfDiagonalColumn(i_SourceCoordinate.Column, i_DestinationCoordinate.Column, false));
        ///    if(!isUpwardsDiagonal)
        ///    {
        ///        isUpwardsDiagonal = (i_SourceCoordinate.Row - 2 == i_DestinationCoordinate.Row) &&
        ///                            (checkIfDiagonalColumn(i_SourceCoordinate.Column, i_DestinationCoordinate.Column, true));
        ///        if(isUpwardsDiagonal)
        ///        {
        ///            o_AteARivalCoin = isThereARivalCoinBetween(i_SourceCoordinate, i_DestinationCoordinate);
        ///            isUpwardsDiagonal = o_AteARivalCoin;
        ///        }
        ///    }
        ///    return isUpwardsDiagonal;
        ///}
        ///private bool checkIfDownwardDiagonal(Coordinate i_SourceCoordinate, Coordinate i_DestinationCoordinate, out bool o_AteARivalCoin)
        ///{
        ///    o_AteARivalCoin = false;
        ///    bool isDownwardsDiagonal = (i_SourceCoordinate.Row + 1 == i_DestinationCoordinate.Row) &&
        ///                             (checkIfDiagonalColumn(i_SourceCoordinate.Column, i_DestinationCoordinate.Column, false));
        ///    if (!isDownwardsDiagonal)
        ///    {
        ///        isDownwardsDiagonal = (i_SourceCoordinate.Row + 2 == i_DestinationCoordinate.Row) &&
        ///                              (checkIfDiagonalColumn(i_SourceCoordinate.Column, i_DestinationCoordinate.Column, true));
        ///        if (isDownwardsDiagonal)
        ///        {
        ///            o_AteARivalCoin = isThereARivalCoinBetween(i_SourceCoordinate, i_DestinationCoordinate);
        ///            isDownwardsDiagonal = o_AteARivalCoin;
        ///        }
        ///    }
        ///    return isDownwardsDiagonal;
        ///}
        ///
        ///private bool isThereARivalCoinBetween(Coordinate i_SourceCoordinate, Coordinate i_DestinationCoordinate)
        ///{
        ///    
        ///}
        ///
        ///private bool checkIfDiagonalColumn(int i_SourceColumn, int i_DestinationColumn, bool i_IsDoubleMove)
        ///{
        ///    int distanceToCheck = i_IsDoubleMove ? 2 : 1;
        ///    return ((i_SourceColumn - distanceToCheck == i_DestinationColumn) || (i_SourceColumn + distanceToCheck == i_DestinationColumn));
        ///}
        private eGameStatus InitiateComputerMove()
        {
            
        }
    }
}