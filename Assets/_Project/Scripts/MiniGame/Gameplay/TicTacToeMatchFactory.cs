using _Project.MiniGame.Core;
using _Project.MiniGame.Domain;

namespace _Project.MiniGame.Gameplay
{
    public sealed class TicTacToeMatchFactory
    {
        private readonly RewardService _rewardService;
        private readonly TicTacToeBotMoveSelector _botMoveSelector;

        public TicTacToeMatchFactory(RewardService rewardService, TicTacToeBotMoveSelector botMoveSelector)
        {
            _rewardService = rewardService;
            _botMoveSelector = botMoveSelector;
        }

        public TicTacToeMatchController Create()
        {
            TicTacToeBoardState boardState = new TicTacToeBoardState();
            TicTacToeBotPlayer botPlayer = new TicTacToeBotPlayer(_botMoveSelector);

            return new TicTacToeMatchController(boardState, botPlayer, _rewardService);
        }
    }
}