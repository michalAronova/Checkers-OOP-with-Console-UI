﻿using System;
using System.Collections.Generic;

namespace EnglishCheckers
{
    public class GameManager
    {
        private Board m_Board = null;
        private Player m_ActivePlayer;
        private Player m_NextPlayer;
        private Move m_LastMove;
        private bool m_NextMoveIsDoubleJump;

        public GameManager(int i_BoardSize, bool i_IsHumanPlayer)
        {
            Dictionary<Coordinate, Coin> player1Coins;
            Dictionary<Coordinate, Coin> player2Coins;
            m_Board = new Board(i_BoardSize);
            m_Board.GetCoordinateToCoinDictionaries(out player1Coins, out player2Coins);
            m_ActivePlayer = new Player(Player.eDirection.Up, eCoinType.Player1Coin, player1Coins);
            m_NextPlayer = new Player(Player.eDirection.Down, eCoinType.Player2Coin, player2Coins);
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
            List<Move> activePlayersValidMoves;
            Move initiatedMove;
            bool isValidMove;
            eGameStatus postMoveGameStatus;

            if (m_NextMoveIsDoubleJump)
            {
                activePlayersValidMoves = calculateJumpsOnlyFrom(m_LastMove.Destination);
            }
            else
            {
                activePlayersValidMoves = calculateMovesForAllPlayersCoins(m_ActivePlayer.PlayersCoins);
            }
            
            initiatedMove = activePlayersValidMoves.Find(move => move.Source.Equals(i_SourceCoordinate) && move.Destination.Equals(i_DestinationCoordinate));
            isValidMove = initiatedMove != null;

            if(isValidMove) 
            {
                performMove(initiatedMove);
                //somewhere here (maybe inside perform move? maybe inside the methods inside it - in player maybe?)
                //need to check if reached end - if yes make it a king!
                postMoveGameStatus = checkForDoubleJumpAndHandleTurnTransfer(initiatedMove);
            }
            else
            {
                postMoveGameStatus = eGameStatus.InvalidMove;
            }

            return postMoveGameStatus;
        }

        private void performMove(Move i_InitiatedMove)
        {
            m_Board.MoveCoin(i_InitiatedMove.Source, i_InitiatedMove.Destination);
            m_ActivePlayer.UpdatePlayersCoins(i_InitiatedMove.Source, i_InitiatedMove.Destination);
            if(i_InitiatedMove.IsJumpMove)
            {
                m_Board.RemoveCoin(i_InitiatedMove.CoordinateOfJumpedOverCoin);
                m_NextPlayer.RemovePlayersAteCoin(i_InitiatedMove.CoordinateOfJumpedOverCoin);
            }
        }

        private eGameStatus handleTurnTransfer()
        {
            eGameStatus postMoveGameStatus;
            List<Move> activePlayersValidMoves;
            List<Move> nextPlayersValidMoves;
            m_NextMoveIsDoubleJump = false;

            swapPlayers(ref m_ActivePlayer, ref m_NextPlayer);
            nextPlayersValidMoves = calculateMovesForAllPlayersCoins(m_NextPlayer.PlayersCoins);

            if (nextPlayersValidMoves.Count == 0)
            {
                activePlayersValidMoves = calculateMovesForAllPlayersCoins(m_ActivePlayer.PlayersCoins);
                if (activePlayersValidMoves.Count == 0)
                {
                    postMoveGameStatus = eGameStatus.Tie;
                }
                else
                {
                    postMoveGameStatus = eGameStatus.ActivePlayerWins;
                }
            }
            else
            {
                postMoveGameStatus = eGameStatus.ContinueGame;
            }

            return postMoveGameStatus;
        }
        private eGameStatus checkForDoubleJumpAndHandleTurnTransfer(Move i_InitiatedMove)
        {
            List<Move> activePlayersValidMoves;
            eGameStatus postMoveGameStatus;
            if (i_InitiatedMove.IsJumpMove)
            {
                activePlayersValidMoves = calculateMovesFrom(i_InitiatedMove.Destination, m_Board.GetSquare(i_InitiatedMove.Destination).Coin);
                if (activePlayersValidMoves.Exists(move => move.IsJumpMove))
                {
                    m_NextMoveIsDoubleJump = true;
                    m_LastMove = i_InitiatedMove;
                    postMoveGameStatus = eGameStatus.ContinueGame;
                }
                else
                {
                    postMoveGameStatus = handleTurnTransfer();
                }
            }
            else
            {
                postMoveGameStatus = handleTurnTransfer();
            }
            return postMoveGameStatus;
        }
        private void swapPlayers(ref Player i_Player1, ref Player i_Player2)
        {
            Player tempPlayer = i_Player1;
            i_Player1 = i_Player2;
            i_Player2 = tempPlayer;
        }

        public eGameStatus InitiateComputerMove(out Coordinate o_SourceCoordinate, out Coordinate o_DestinationCoordinate)
        {
            Random random = new Random();
            int randomMoveNumber;
            Move randomMove;
            eGameStatus postMoveGameStatus;
            List<Move> computerPossibleMoves;

            computerPossibleMoves = calculateMovesForAllPlayersCoins(m_ActivePlayer.PlayersCoins);
            randomMoveNumber = random.Next(computerPossibleMoves.Count) - 1;
            randomMove = computerPossibleMoves[randomMoveNumber];
            performMove(randomMove);
            postMoveGameStatus = checkForDoubleJumpAndHandleTurnTransfer(randomMove);
            o_SourceCoordinate = randomMove.Source;
            o_DestinationCoordinate = randomMove.Destination;

            return postMoveGameStatus;
        }

        private void removeNoJumps(List<Move> i_Moves)
        {
            List<Move> movesToRemove = new List<Move>();
        
            if(i_Moves.Exists(move => move.IsJumpMove))
            {
                foreach(Move move in i_Moves)
                {
                    if(!move.IsJumpMove)
                    {
                        movesToRemove.Add(move);
                    }
                }
            }
        
            foreach(Move move in movesToRemove)
            {
                i_Moves.Remove(move);
            }
        }
        
        private List<Move> calculateJumpsOnlyFrom(Coordinate i_GivenDestination)
        {
            List<Move> JumpsFromCoordinate = calculateMovesFrom(i_GivenDestination, m_Board.GetSquare(i_GivenDestination).Coin);
            removeNoJumps(JumpsFromCoordinate);
            return JumpsFromCoordinate;
        }
        
        private List<Move> calculateMovesForAllPlayersCoins(Dictionary<Coordinate, Coin> i_PlayersCoins)
        {
            List<Move> allPossibleMoves = new List<Move>();
            List<Move> movesFromGivenCoin;
            foreach (KeyValuePair<Coordinate, Coin> coinCoordinate in i_PlayersCoins)
            {
                movesFromGivenCoin = calculateMovesFrom(coinCoordinate.Key, coinCoordinate.Value);
                removeNoJumps(allPossibleMoves);    
                if(movesFromGivenCoin.Count != 0)
                {
                    movesFromGivenCoin.ForEach(move => allPossibleMoves.Add(move));
                }
            }
        
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
            bool isLeftMove;
            List<Move> possibleMoves = new List<Move>();
            List<Coordinate> possibleJumps = null;
            foreach (Coordinate diagonalCoordinate in possibleDiagonalCoordinates)
            {
                if(m_Board.GetSquare(diagonalCoordinate).Coin == null)
                {
                    possibleMoves.Add(new Move(i_SourceCoordinate, diagonalCoordinate, i_Direction, !isAJumpMove, null));
                }
                else if(i_CoinToMove.Type != m_Board.GetSquare(diagonalCoordinate).Coin.Type)
                {
                    isLeftMove = m_Board.checkIfLeftMove(i_SourceCoordinate, diagonalCoordinate);
                    possibleJumps = m_Board.GetDiagonalInDirection(diagonalCoordinate, i_Direction);
                    foreach(Coordinate jumpCoordinate in possibleJumps)
                    {
                        if(m_Board.GetSquare(jumpCoordinate).Coin == null && isLeftMove == m_Board.checkIfLeftMove(diagonalCoordinate, jumpCoordinate))
                        {
                            possibleMoves.Add(new Move(i_SourceCoordinate, jumpCoordinate, 
                                                            i_Direction, isAJumpMove, diagonalCoordinate));
                        }
                    }
                }
            }
            return possibleMoves;
        }
    }
}