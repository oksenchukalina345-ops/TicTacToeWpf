using System;
using System.Collections.Generic;
namespace TicTacToeWpf.Models
{
    public enum CellValue { Empty, X, O }
    public enum GameDifficulty { Easy = 1, Medium = 2, Hard = 3 }
    public enum GameStatus { InProgress, XWins, OWins, Draw }
    public class TicTacToeGame
    {
        public const int BoardSize = 3;
        private readonly CellValue[,] _board = new CellValue[BoardSize, BoardSize];

        public CellValue CurrentPlayer { get; private set; } = CellValue.X;
        public GameStatus Status { get; private set; } = GameStatus.InProgress;
        public GameDifficulty Difficulty { get; set; } = GameDifficulty.Easy;

        public TicTacToeGame()
        {
            ResetGame();
        }

        public void ResetGame()
        {
            for (int r = 0; r < BoardSize; r++)
            {
                for (int c = 0; c < BoardSize; c++)
                {
                    _board[r, c] = CellValue.Empty;
                }
            }
            CurrentPlayer = CellValue.X;
            Status = GameStatus.InProgress;
        }

        public CellValue GetCell(int row, int col) => _board[row, col];

        public bool MakeMove(int row, int col)
        {
            if (Status != GameStatus.InProgress || _board[row, col] != CellValue.Empty)
                return false;

            _board[row, col] = CurrentPlayer;
            UpdateGameStatus();

            if (Status == GameStatus.InProgress)
            {
                CurrentPlayer = (CurrentPlayer == CellValue.X) ? CellValue.O : CellValue.X;
            }

            return true;
        }

        public List<(int Row, int Col)> GetAvailableMoves()
        {
            var moves = new List<(int Row, int Col)>();
            for (int r = 0; r < BoardSize; r++)
            {
                for (int c = 0; c < BoardSize; c++)
                {
                    if (_board[r, c] == CellValue.Empty)
                        moves.Add((r, c));
                }
            }
            return moves;
        }

        private void UpdateGameStatus()
        {
            CellValue winner = CheckWinner();
            if (winner == CellValue.X)
            {
                Status = GameStatus.XWins;
            }
            else if (winner == CellValue.O)
            {
                Status = GameStatus.OWins;
            }
            else if (IsDraw())
            {
                Status = GameStatus.Draw;
            }
        }

        public CellValue CheckWinner()
        {
            for (int i = 0; i < BoardSize; i++)
            {
                if (_board[i, 0] != CellValue.Empty && IsLineSame(_board[i, 0], _board[i, 1], _board[i, 2]))
                    return _board[i, 0];

                if (_board[0, i] != CellValue.Empty && IsLineSame(_board[0, i], _board[1, i], _board[2, i]))
                    return _board[0, i];
            }

            if (_board[0, 0] != CellValue.Empty && IsLineSame(_board[0, 0], _board[1, 1], _board[2, 2]))
                return _board[0, 0];

            if (_board[0, 2] != CellValue.Empty && IsLineSame(_board[0, 2], _board[1, 1], _board[2, 0]))
                return _board[0, 2];

            return CellValue.Empty;
        }

        private bool IsLineSame(CellValue a, CellValue b, CellValue c) => a == b && b == c;

        public bool IsDraw() => CheckWinner() == CellValue.Empty && GetAvailableMoves().Count == 0;

        public (int Row, int Col)? ChooseComputerMove()
        {
            var available = GetAvailableMoves();
            if (available.Count == 0 || Status != GameStatus.InProgress)
                return null;

            var rand = new Random();

            if (Difficulty == GameDifficulty.Easy)
            {
                return available[rand.Next(available.Count)];
            }

            if (Difficulty == GameDifficulty.Medium)
            {
                var blockMove = FindWinningMove(CellValue.X);
                return blockMove ?? available[rand.Next(available.Count)];
            }

            if (Difficulty == GameDifficulty.Hard)
            {
                var winMove = FindWinningMove(CellValue.O);
                if (winMove.HasValue) return winMove;

                var blockMove = FindWinningMove(CellValue.X);
                if (blockMove.HasValue) return blockMove;

                return available[rand.Next(available.Count)];
            }

            return available[rand.Next(available.Count)];
        }
        private (int Row, int Col)? FindWinningMove(CellValue player)
        {
            foreach (var (r, c) in GetAvailableMoves())
            {
                _board[r, c] = player;
                bool isWin = CheckWinner() == player;
                _board[r, c] = CellValue.Empty;

                if (isWin) return (r, c);
            }
            return null;
        }
    }
}