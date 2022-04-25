using System;
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
        private readonly bool r_twoPlayersMode;

        public GameManager(int i_BoardSize, bool i_IsHumanPlayer, string i_Player1Name, string i_Player2Name)
        {
            Dictionary<Coordinate, Coin> player1Coins = null;
            Dictionary<Coordinate, Coin> player2Coins = null;

            m_Board = new Board(i_BoardSize);
            m_Board.GetCoordinateToCoinDictionaries(out player1Coins, out player2Coins);
            m_ActivePlayer = new Player(eDirection.Up, eCoinType.Player1Coin, player1Coins, i_Player1Name);
            m_NextPlayer = new Player(eDirection.Down, eCoinType.Player2Coin, player2Coins, i_Player2Name);
            r_twoPlayersMode = i_IsHumanPlayer;
            m_NextPlayer.IsHumanPlayer = i_IsHumanPlayer;
        }

        public Board GameBoard
        {
            get
            {
                return m_Board;
            }
        }

        public Player ActivePlayer
        {
            get
            {
                return m_ActivePlayer;
            }
        }

        public Player NextPlayer
        {
            get
            {
                return m_NextPlayer;
            }
        }

        public bool TwoPlayersMode
        {
            get
            {
                return r_twoPlayersMode;
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
            m_ActivePlayer.UpdatePlayersCoins(i_InitiatedMove.Source, i_InitiatedMove.Destination, m_Board.Size);
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

            m_NextMoveIsDoubleJump = !true;
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

                countAndSetPoints(postMoveGameStatus);
                initiateGame();
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
                    postMoveGameStatus = eGameStatus.ActivePlayerHasAnotherMove;
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
            //testing testing//
            bool testingSmartMove = true;
            if(testingSmartMove)
            {
                randomMove = selectGoodMove();
            }
            //testing testing//
            else
            {
                computerPossibleMoves = calculateMovesForAllPlayersCoins(m_ActivePlayer.PlayersCoins);
                randomMoveNumber = random.Next(computerPossibleMoves.Count) - 1;
                randomMove = computerPossibleMoves[randomMoveNumber];
            }

            performMove(randomMove);
            postMoveGameStatus = checkForDoubleJumpAndHandleTurnTransfer(randomMove);
            o_SourceCoordinate = randomMove.Source;
            o_DestinationCoordinate = randomMove.Destination;
            System.Threading.Thread.Sleep(3000);

            return postMoveGameStatus;
        }

        private Move selectGoodMove()
        {
            List<Move> computerPossibleMoves;
            List<Move> goodMoves = new List<Move>();
            List<Move> badMoves = new List<Move>();
            Move chosenMove;
            Move makeKingMove = null;

            computerPossibleMoves = calculateMovesForAllPlayersCoins(m_ActivePlayer.PlayersCoins);
            if(existsTurnToKingMoveIn(computerPossibleMoves, out makeKingMove))
            {
                chosenMove = makeKingMove;
            }
            else if(computerPossibleMoves.Exists(move => move.IsJumpMove))
            {
                chosenMove = checkForPossibleDoubleJump(computerPossibleMoves);
                if(chosenMove == null)
                {
                    chosenMove = getRandomMove(computerPossibleMoves);
                }
            }
            else
            {
                splitMoveListByPossibleOutcomes(computerPossibleMoves, badMoves, goodMoves);
                if(goodMoves.Count == 0)
                {
                    chosenMove = getRandomMove(badMoves);
                }
                else
                {
                    chosenMove = choseMoveByPriority(goodMoves);
                }
            }

            return chosenMove;
        }

        private Move choseMoveByPriority(List<Move> i_MovesToPrioritize)
        {
            List<Move> kingMoves;
            Move chosenMove;
            Move randomGoodMove;
            Move randomKingMove;
            Random random = new Random();
            int choice = random.Next(1);
            int goodMoveUpRangeToChooseFrom;
            int goodMoveDownRangeToChooseFrom;

            prioritizeByDistance(i_MovesToPrioritize);
            kingMoves = takeOutKingMoves(i_MovesToPrioritize);
            goodMoveUpRangeToChooseFrom = i_MovesToPrioritize.Count > 3 ? i_MovesToPrioritize.Count / 2 : i_MovesToPrioritize.Count;
            goodMoveDownRangeToChooseFrom = i_MovesToPrioritize.Count > 3 ? i_MovesToPrioritize.Count / 4 : 0;
            randomGoodMove = getRandomMoveInRange(i_MovesToPrioritize, goodMoveDownRangeToChooseFrom, goodMoveUpRangeToChooseFrom);
            chosenMove = randomGoodMove;
            if(kingMoves.Count > 0)
            {
                randomKingMove = getRandomMove(kingMoves);
                if(choice == 1)
                {
                    chosenMove = randomKingMove;
                }
            }

            return chosenMove;
        }
        
        private bool existsTurnToKingMoveIn(List<Move> i_PossibleMoves, out Move o_MakeKingMove)
        {
            bool existsTurnToKingMove = false;
            List<Move> turnToKingMoves = new List<Move>();
            Coin currentMovingCoin;
            int currentDestinationRow;

            foreach(Move possibleMove in i_PossibleMoves)
            {
                currentMovingCoin = m_Board.GetSquare(possibleMove.Source).Coin;
                currentDestinationRow = possibleMove.Destination.Row;
                if(currentDestinationRow == m_Board.Size - 1 && !currentMovingCoin.IsKing)
                {
                    turnToKingMoves.Add(possibleMove);
                    existsTurnToKingMove = true;
                }
            }

            if(existsTurnToKingMove)
            {
                o_MakeKingMove = getRandomMove(turnToKingMoves);
            }
            else
            {
                o_MakeKingMove = null;
            }

            return existsTurnToKingMove;
        }

        private Move checkForPossibleDoubleJump(List<Move> i_PossibleMoves)
        {
            List<Move> possibleDoubleJumpMoves = new List<Move>();
            List<Move> possibleMovesFrom;
            Move chosenDoubleJump;

            foreach(Move possibleMove in i_PossibleMoves)
            {
                possibleMovesFrom = calculateMovesFrom(possibleMove.Destination, m_Board.GetSquare(possibleMove.Source).Coin);
                if(possibleMovesFrom.Exists(move => move.IsJumpMove))
                {
                    possibleDoubleJumpMoves.Add(possibleMove);
                }
            }

            if (possibleDoubleJumpMoves.Count>0)
            {
                chosenDoubleJump = getRandomMove(possibleDoubleJumpMoves);
            }
            else
            {
                chosenDoubleJump = null;
            }

            return chosenDoubleJump;
        }

        private void splitMoveListByPossibleOutcomes(List<Move> i_AllMoves, List<Move> o_BadMoves, List<Move> o_GoodMoves)
        {
            List<Move> opponentMovesAfterCurrentMove;

            foreach(Move possibleMove in i_AllMoves)
            {
                if (!possibleMove.IsJumpMove)
                {
                    performMove(possibleMove);
                    opponentMovesAfterCurrentMove = calculateMovesForAllPlayersCoins(m_NextPlayer.PlayersCoins);
                    if(isBadMove(possibleMove, opponentMovesAfterCurrentMove))
                    {
                        o_BadMoves.Add(possibleMove);
                    }
                    else
                    {
                        o_GoodMoves.Add(possibleMove);
                    }

                    undoMove(possibleMove);
                }
            }
        }

        private bool isBadMove(Move i_TestedMoved, List<Move> i_OpponentsMoves)
        {
            bool isBadMove = !true;

            foreach(Move opponentMove in i_OpponentsMoves)
            {
                if(opponentMove.IsJumpMove)
                {
                    if(opponentMove.CoordinateOfJumpedOverCoin.Equals(i_TestedMoved.Destination))
                    {
                        isBadMove = true;
                    }
                }
            }
            ///if(i_OpponentsMoves.Exists(i_Move => i_Move.IsJumpMove))
            ///{
            ///    if(i_OpponentsMoves.Exists(i_Move => i_Move.CoordinateOfJumpedOverCoin.Equals(i_TestedMoved.Destination)))
            ///    {
            ///        isBadMove = true;
            ///    }
            ///}
            return isBadMove;
        }
        private void prioritizeByDistance(List<Move> i_Moves)
        {
            i_Moves.Sort((i_MoveA, i_MoveB) => (i_MoveB.Destination.Row - i_MoveA.Destination.Row));
        }

        private List<Move> takeOutKingMoves(List<Move> i_Moves)
        {
            List<Move> kingMoves = new List<Move>();

            for(int i = 0; i < i_Moves.Count; i++) 
            {
                if(m_Board.GetSquare(i_Moves[i].Source).Coin.IsKing)
                {
                    kingMoves.Add(i_Moves[i]);
                    i_Moves.RemoveAt(i);
                }
            }

            return kingMoves;
        }

        private void undoMove(Move i_MoveToUndo)
        {
            m_Board.MoveCoin(i_MoveToUndo.Destination, i_MoveToUndo.Source);
            m_ActivePlayer.UpdatePlayersCoins(i_MoveToUndo.Destination, i_MoveToUndo.Source, m_Board.Size);
        }

        private int getRandomInRange(int i_Bottom, int i_Upper)
        {
            Random random = new Random();

            return random.Next((i_Upper - i_Bottom)) + i_Bottom;
        }

        private Move getRandomMove(List<Move> i_Moves)
        {
            return getRandomMoveInRange(i_Moves, 0, i_Moves.Count - 1);
        }

        private Move getRandomMoveInRange(List<Move> i_Moves, int i_Bottom, int i_Upper)
        {
            int randomIndex = getRandomInRange(i_Bottom, i_Upper);

            if(randomIndex == i_Moves.Count)
            {
                randomIndex -= 1;
            }

            return i_Moves[randomIndex];
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
                if(movesFromGivenCoin.Count != 0)
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
            eDirection coinsDirection = (i_CoinToMove.Type == m_ActivePlayer.CoinType)
                                                   ? m_ActivePlayer.Direction
                                                   : m_NextPlayer.Direction;
            if(i_CoinToMove.IsKing)
            {
                if(coinsDirection == eDirection.Down)
                {
                    kingMoves = calculateMovesByDirection(i_CoinToMove, i_SourceCoordinate, eDirection.Up);
                }
                else
                {
                    kingMoves = calculateMovesByDirection(i_CoinToMove, i_SourceCoordinate, eDirection.Down);
                }
            }

            moves = calculateMovesByDirection(i_CoinToMove, i_SourceCoordinate, coinsDirection);
            if(kingMoves != null)
            {
                kingMoves.ForEach(move => moves.Add(move));
            }

            return moves;
        }
        
        private List<Move> calculateMovesByDirection(Coin i_CoinToMove, Coordinate i_SourceCoordinate, eDirection i_Direction)
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

        public eGameStatus CurrentPlayerQuit()
        {
            eGameStatus gameStatus = eGameStatus.NextPlayerWins;

            countAndSetPoints(gameStatus);
            initiateGame();

            return gameStatus;
        }

        private void countAndSetPoints(eGameStatus i_EGameStatus)
        {
            if (i_EGameStatus == eGameStatus.ActivePlayerWins)
            {
                ActivePlayer.Points += ActivePlayer.GetCurrentGamePoints() - NextPlayer.GetCurrentGamePoints();
            }
            else if(i_EGameStatus == eGameStatus.NextPlayerWins)
            {
                NextPlayer.Points += NextPlayer.GetCurrentGamePoints() - ActivePlayer.GetCurrentGamePoints();
            }
        }

        private void initiateGame()
        {
            m_Board.SetInitialBoard();
            initiatePlayers();
        }

        private void initiatePlayers()
        {
            Dictionary<Coordinate, Coin> player1Coins = null;
            Dictionary<Coordinate, Coin> player2Coins = null;

            if (ActivePlayer.CoinType == eCoinType.Player2Coin)
            {
                swapPlayers(ref m_ActivePlayer, ref m_NextPlayer);
            }

            m_Board.GetCoordinateToCoinDictionaries(out player1Coins, out player2Coins);
            ActivePlayer.PlayersCoins = player1Coins;
            NextPlayer.PlayersCoins = player2Coins;
        }
    }
}