using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using _Project.MiniGame.Core;
using _Project.MiniGame.Domain;

namespace _Project.MiniGame.UI
{
    public sealed class TicTacToeGamePanel : MonoBehaviour
    {
        private const string PLAYER_TURN_TEXT = "Your move";
        private const string WIN_TEXT = "Win";
        private const string LOSE_TEXT = "Lose";
        private const string DRAW_TEXT = "Draw";
        private const string GOLD_TEXT = "Gold";
        
        private Action _closeRequestedAction;
        private Action _restartRequestedAction;
        
        [SerializeField] private Button[] _cellButtons;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _restartButton;
        [SerializeField] private TMP_Text[] _cellMarkTexts;
        [SerializeField] private TMP_Text _statusText;

        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        private TicTacToeMatchController _matchController;

        public void Initialize(
            TicTacToeMatchController matchController,
            Action closeRequestedAction,
            Action restartRequestedAction)
        {
            _matchController = matchController;
            _closeRequestedAction = closeRequestedAction;
            _restartRequestedAction = restartRequestedAction;

            _closeButton.gameObject.SetActive(false);
            _restartButton.gameObject.SetActive(false);
            
            _statusText.text = PLAYER_TURN_TEXT;

            BindCellButtons();
            BindCloseButton();
            BindRestartButton();
            BindMatchFinished();
            RefreshBoard();
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
        }
        
        private string GetCellMark(TicTacToeCellStateType cellState)
        {
            switch (cellState)
            {
                case TicTacToeCellStateType.Cross:
                    return "X";

                case TicTacToeCellStateType.Nought:
                    return "O";

                default:
                    return string.Empty;
            }
        }

        private string GetResultText(MiniGameResult result)
        {
            switch (result.Outcome)
            {
                case MiniGameOutcomeType.Win:
                    return $"{WIN_TEXT}. {GOLD_TEXT}: {result.Reward.GoldAmount}";

                case MiniGameOutcomeType.Draw:
                    return $"{DRAW_TEXT}. {GOLD_TEXT}: {result.Reward.GoldAmount}";

                default:
                    return LOSE_TEXT;
            }
        }
        
        private void BindCellButtons()
        {
            for (int cellIndex = 0; cellIndex < _cellButtons.Length; cellIndex++)
            {
                int capturedCellIndex = cellIndex;

                _cellButtons[cellIndex]
                    .OnClickAsObservable()
                    .Subscribe(_ => OnCellButtonClicked(capturedCellIndex))
                    .AddTo(_disposables);
            }
        }

        private void BindCloseButton()
        {
            _closeButton
                .OnClickAsObservable()
                .Subscribe(_ => _closeRequestedAction?.Invoke())
                .AddTo(_disposables);
        }

        private void BindRestartButton()
        {
            _restartButton
                .OnClickAsObservable()
                .Subscribe(_ => _restartRequestedAction?.Invoke())
                .AddTo(_disposables);
        }

        private void BindMatchFinished()
        {
            _matchController.OnMatchFinished.Subscribe(OnMatchFinished).AddTo(_disposables);
        }

        private void OnCellButtonClicked(int cellIndex)
        {
            bool isMoveSuccess = _matchController.TryPlayerMove(cellIndex);

            if (!isMoveSuccess)
                return;

            RefreshBoard();

            if (!_matchController.IsMatchFinished)
                _statusText.text = PLAYER_TURN_TEXT;
        }

        private void OnMatchFinished(MiniGameResult result)
        {
            RefreshBoard();
            
            _statusText.text = GetResultText(result);
            
            _closeButton.gameObject.SetActive(true);
            _restartButton.gameObject.SetActive(true);
        }

        private void RefreshBoard()
        {
            for (int cellIndex = 0; cellIndex < _cellButtons.Length; cellIndex++)
            {
                TicTacToeCellStateType cellState = _matchController.BoardState.GetCellState(cellIndex);

                _cellMarkTexts[cellIndex].text = GetCellMark(cellState);
                _cellButtons[cellIndex].interactable =
                    !_matchController.IsMatchFinished &&
                    cellState == TicTacToeCellStateType.Empty;
            }
        }
    }
}