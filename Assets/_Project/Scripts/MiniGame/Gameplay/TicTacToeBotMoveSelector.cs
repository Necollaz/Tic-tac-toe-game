using System.Collections.Generic;
using UnityEngine;
using _Project.MiniGame.Domain;

namespace _Project.MiniGame.Gameplay
{
    public sealed class TicTacToeBotMoveSelector
    {
        private const int CENTER_CELL_INDEX = 4;

        public int SelectMove(TicTacToeBoardState boardState)
        {
            if (boardState.CanMakeMove(CENTER_CELL_INDEX))
                return CENTER_CELL_INDEX;
            
            List<int> availableCells = GetAvailableCells(boardState);

            if (availableCells.Count == 0)
                return -1;
            
            int randomIndex = Random.Range(0, availableCells.Count);
            
            return availableCells[randomIndex];
        }

        private List<int> GetAvailableCells(TicTacToeBoardState boardState)
        {
            List<int> availableCells = new List<int>();

            for (int cellIndex = 0; cellIndex < boardState.CellsCount; cellIndex++)
            {
                if (boardState.CanMakeMove(cellIndex))
                    availableCells.Add(cellIndex);
            }
            
            return availableCells;
        }
    }
}