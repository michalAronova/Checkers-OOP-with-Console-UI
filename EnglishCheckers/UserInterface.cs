namespace EnglishCheckers
{
    public class UserInterface
    {
        public void InitializeGame()
        {
            // 1. ask for name - check validity
            // 2. ask for board size - check validity and save in parameter
            // 3. ask if 1 or 2 players. if 2 -> 1.
            // create GameManager according to above
            // call RunGame
        }
        private void RunGame(GameManager i_GameManager)
        {
            //print board
            //while (the move returns continuegame) loop of asking for moves
            //if Q is entered instead of a move - the current player loses
            //otherwise print message
        }

        private void printBoard(Board i_GameBoard)
        {
            
        }

        private GameManager.eGameStatus askForMove()
        {
            //ask user
            //validate input of move
            // initiate move/initiate computer move
            //return the game status to run
        }
    }
}