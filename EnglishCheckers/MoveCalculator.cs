using System.Collections.Generic;

namespace EnglishCheckers
{
    internal class MoveCalculator
    {
        private readonly Board r_Board;
        private readonly Player r_ActivePlayer;
        private readonly Player r_NextPlayer;

        public MoveCalculator(Board i_Board, Player i_ActivePlayer, Player i_NextPlayer)
        {
            r_Board = i_Board;
            r_ActivePlayer = i_ActivePlayer;
            r_NextPlayer = i_NextPlayer;
        }

        internal List<Move> calculateMovesForAllPlayersCoins(Dictionary<Coordinate, Coin> i_PlayersCoins)
        {
            List<Move> allPossibleMoves = new List<Move>();
            List<Move> movesFromGivenCoin;
            foreach (KeyValuePair<Coordinate, Coin> coinCoordinate in i_PlayersCoins)
            {
                movesFromGivenCoin = calculateMovesFrom(coinCoordinate.Key, coinCoordinate.Value);
                removeNoJumps(allPossibleMoves);
                if (movesFromGivenCoin.Count != 0)
                {
                    movesFromGivenCoin.ForEach(move => allPossibleMoves.Add(move));
                }
            }

            return allPossibleMoves;
        }

        internal List<Move> calculateMovesFrom(Coordinate i_SourceCoordinate, Coin i_CoinToMove)
        {
            List<Move> moves = new List<Move>();
            List<Move> kingMoves = null;
            Player.eDirection coinsDirection = (i_CoinToMove.Type == r_ActivePlayer.CoinType)
                                                   ? r_ActivePlayer.Direction
                                                   : r_NextPlayer.Direction;
            if (i_CoinToMove.IsKing)
            {
                if (coinsDirection == Player.eDirection.Down)
                {
                    kingMoves = calculateMovesByDirection(i_CoinToMove, i_SourceCoordinate, Player.eDirection.Up);
                }
                else
                {
                    kingMoves = calculateMovesByDirection(i_CoinToMove, i_SourceCoordinate, Player.eDirection.Down);
                }
            }

            moves = calculateMovesByDirection(i_CoinToMove, i_SourceCoordinate, coinsDirection);

            if (kingMoves != null)
            {
                kingMoves.ForEach(move => moves.Add(move));
            }
            return moves;
        }

        internal List<Move> calculateMovesByDirection(Coin i_CoinToMove, Coordinate i_SourceCoordinate, Player.eDirection i_Direction)
        {
            List<Coordinate> possibleDiagonalCoordinates = r_Board.GetDiagonalInDirection(i_SourceCoordinate, i_Direction);
            bool isAJumpMove = true;
            bool isLeftMove;
            List<Move> possibleMoves = new List<Move>();
            List<Coordinate> possibleJumps = null;
            foreach (Coordinate diagonalCoordinate in possibleDiagonalCoordinates)
            {
                if (r_Board.GetSquare(diagonalCoordinate).Coin == null)
                {
                    possibleMoves.Add(new Move(i_SourceCoordinate, diagonalCoordinate, i_Direction, !isAJumpMove, null));
                }
                else if (i_CoinToMove.Type != r_Board.GetSquare(diagonalCoordinate).Coin.Type)
                {
                    isLeftMove = r_Board.checkIfLeftMove(i_SourceCoordinate, diagonalCoordinate);
                    possibleJumps = r_Board.GetDiagonalInDirection(diagonalCoordinate, i_Direction);
                    foreach (Coordinate jumpCoordinate in possibleJumps)
                    {
                        if (r_Board.GetSquare(jumpCoordinate).Coin == null && isLeftMove == r_Board.checkIfLeftMove(diagonalCoordinate, jumpCoordinate))
                        {
                            possibleMoves.Add(new Move(i_SourceCoordinate, jumpCoordinate,
                                                            i_Direction, isAJumpMove, diagonalCoordinate));
                        }
                    }
                }
            }
            return possibleMoves;
        }
        internal void removeNoJumps(List<Move> i_Moves)
        {
            List<Move> movesToRemove = new List<Move>();

            if (i_Moves.Exists(move => move.IsJumpMove))
            {
                foreach (Move move in i_Moves)
                {
                    if (!move.IsJumpMove)
                    {
                        movesToRemove.Add(move);
                    }
                }
            }

            foreach (Move move in movesToRemove)
            {
                i_Moves.Remove(move);
            }
        }
        internal List<Move> calculateJumpsOnlyFrom(Coordinate i_GivenDestination)
        {
            List<Move> JumpsFromCoordinate = calculateMovesFrom(i_GivenDestination, r_Board.GetSquare(i_GivenDestination).Coin);
            removeNoJumps(JumpsFromCoordinate);
            return JumpsFromCoordinate;
        }
    }
}