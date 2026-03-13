using System;
using UniRx;
using _Project.MiniGame.Domain;
using _Project.MiniGame.Gameplay;

namespace _Project.MiniGame.Core
{
    public sealed class TicTacToeMatchController
    {
        private readonly AsyncSubject<MiniGameResult> _matchFinishedSubject;
        private readonly TicTacToeBoardState _boardState;
        private readonly TicTacToeBotPlayer _botPlayer;
        private readonly RewardService _rewardService;
        
        private bool _isMatchFinished;

        public TicTacToeMatchController(
            TicTacToeBoardState boardState,
            TicTacToeBotPlayer botPlayer,
            RewardService rewardService)
        {
            _boardState = boardState;
            _botPlayer = botPlayer;
            _rewardService = rewardService;
            
            _matchFinishedSubject = new AsyncSubject<MiniGameResult>();
        }
        
        public IObservable<MiniGameResult> OnMatchFinished => _matchFinishedSubject;
        public TicTacToeBoardState BoardState => _boardState;
        public bool IsMatchFinished => _isMatchFinished;

        public bool TryPlayerMove(int cellIndex)
        {
            if (_isMatchFinished)
                return false;

            bool isMoveSuccess = _boardState.TryMakeMove(cellIndex, TicTacToeCellStateType.Cross);

            if (!isMoveSuccess)
                return false;

            ProcessBoardAfterPlayerMove();

            return true;
        }

        private MiniGameOutcomeType ConvertOutcome(TicTacToeBoardEvaluationType boardEvaluation)
        {
            switch (boardEvaluation)
            {
                case TicTacToeBoardEvaluationType.CrossWon:
                    return MiniGameOutcomeType.Win;

                case TicTacToeBoardEvaluationType.NoughtWon:
                    return MiniGameOutcomeType.Lose;

                default:
                    return MiniGameOutcomeType.Draw;
            }
        }
        
        private void ProcessBoardAfterPlayerMove()
        {
            TicTacToeBoardEvaluationType boardEvaluation = _boardState.EvaluateBoard();

            if (boardEvaluation != TicTacToeBoardEvaluationType.InProgress)
            {
                FinishMatch(boardEvaluation);
                
                return;
            }

            _botPlayer.TryMakeMove(_boardState);

            boardEvaluation = _boardState.EvaluateBoard();

            if (boardEvaluation != TicTacToeBoardEvaluationType.InProgress)
                FinishMatch(boardEvaluation);
        }
        
        private void FinishMatch(TicTacToeBoardEvaluationType boardEvaluation)
        {
            if (_isMatchFinished)
                return;

            _isMatchFinished = true;

            MiniGameOutcomeType outcome = ConvertOutcome(boardEvaluation);
            RewardData reward = _rewardService.GenerateReward(outcome);
            MiniGameResult result = new MiniGameResult(outcome, reward);

            _matchFinishedSubject.OnNext(result);
            _matchFinishedSubject.OnCompleted();
        }
    }
}