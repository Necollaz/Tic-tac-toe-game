using _Project.MiniGame.Domain;

namespace _Project.MiniGame.Gameplay
{
    public sealed class TicTacToeBotPlayer
    {
        private readonly TicTacToeBotMoveSelector _moveSelector;

        public TicTacToeBotPlayer(TicTacToeBotMoveSelector moveSelector)
        {
            _moveSelector = moveSelector;
        }

        public bool TryMakeMove(TicTacToeBoardState boardState)
        {
            int selectedCellIndex = _moveSelector.SelectMove(boardState);
            
            if (selectedCellIndex < 0)
                return false;

            return boardState.TryMakeMove(selectedCellIndex, TicTacToeCellStateType.Nought);
        }
    }
}