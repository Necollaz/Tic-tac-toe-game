using System;

namespace _Project.MiniGame.Domain
{
    public sealed class TicTacToeBoardState
    {
        private const int CELLS_COUNT = 9;
        private const int MIN_CELL_INDEX = 0;
        private const int MAX_CELL_INDEX = CELLS_COUNT - 1;

        private readonly TicTacToeCellStateType[] _cellStates;
        private readonly int[,] _winCombinations =
        {
            { 0, 1, 2 },
            { 3, 4, 5 },
            { 6, 7, 8 },

            { 0, 3, 6 },
            { 1, 4, 7 },
            { 2, 5, 8 },

            { 0, 4, 8 },
            { 2, 4, 6 }
        };

        public TicTacToeBoardState()
        {
            _cellStates = new TicTacToeCellStateType[CELLS_COUNT];

            Reset();
        }
        
        public int CellsCount => CELLS_COUNT;

        public TicTacToeCellStateType GetCellState(int cellIndex)
        {
            ValidateCellIndex(cellIndex);
            
            return _cellStates[cellIndex];
        }

        public TicTacToeBoardEvaluationType EvaluateBoard()
        {
            if (HasWinningCombination(TicTacToeCellStateType.Cross))
                return TicTacToeBoardEvaluationType.CrossWon;
            
            if (HasWinningCombination(TicTacToeCellStateType.Nought))
                return TicTacToeBoardEvaluationType.NoughtWon;

            if (HasAnyEmptyCell())
                return TicTacToeBoardEvaluationType.InProgress;

            return TicTacToeBoardEvaluationType.Draw;
        }
        
        public bool CanMakeMove(int cellIndex)
        {
            if (IsCellIndexOutOfRange(cellIndex))
                return false;
            
            return _cellStates[cellIndex] == TicTacToeCellStateType.Empty;
        }

        public bool TryMakeMove(int cellIndex, TicTacToeCellStateType cellState)
        {
            if (cellState == TicTacToeCellStateType.Empty)
                return false;
            
            if (!CanMakeMove(cellIndex))
                return false;
            
            _cellStates[cellIndex] = cellState;
            
            return true;
        }

        private bool IsCellIndexOutOfRange(int cellIndex) => cellIndex < MIN_CELL_INDEX || cellIndex > MAX_CELL_INDEX;
        
        private bool HasAnyEmptyCell()
        {
            for (int cellIndex = 0; cellIndex < CELLS_COUNT; cellIndex++)
            {
                if (_cellStates[cellIndex] == TicTacToeCellStateType.Empty)
                    return true;
            }
            
            return false;
        }

        private bool HasWinningCombination(TicTacToeCellStateType cellState)
        {
            for (int combinationIndex = 0; combinationIndex < _winCombinations.GetLength(0); combinationIndex++)
            {
                int firstCellIndex = _winCombinations[combinationIndex, 0];
                int secondCellIndex = _winCombinations[combinationIndex, 1];
                int thirdCellIndex = _winCombinations[combinationIndex, 2];
                
                bool isWinningLine = 
                    _cellStates[firstCellIndex] == cellState && 
                    _cellStates[secondCellIndex] == cellState && 
                    _cellStates[thirdCellIndex] == cellState;
                
                if (isWinningLine)
                    return true;
            }
            
            return false;
        }

        private void ValidateCellIndex(int cellIndex)
        {
            if (IsCellIndexOutOfRange(cellIndex))
                throw new ArgumentOutOfRangeException(nameof(cellIndex), cellIndex, "Cell index is out of range.");
        }

        private void Reset()
        {
            for (int cellIndex = 0; cellIndex < CELLS_COUNT; cellIndex++)
                _cellStates[cellIndex] = TicTacToeCellStateType.Empty;
        }
    }
}