using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using TicTacToeWpf.Models;
namespace TicTacToeWpf.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly TicTacToeGame _game = new();
        public ObservableCollection<CellViewModel> Cells { get; } = new();

        public GameDifficulty SelectedDifficulty
        {
            get => _game.Difficulty;
            set
            {
                _game.Difficulty = value;
                OnPropertyChanged();
            }
        }

        private string _statusMessage = "Ваш хід (X)";
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        public ICommand MakeMoveCommand { get; }
        public ICommand NewGameCommand { get; }

        public MainViewModel()
        {
            MakeMoveCommand = new RelayCommand(ExecuteMakeMove, CanMakeMove);
            NewGameCommand = new RelayCommand(_ => ResetGame());

            InitializeBoard();
        }
        private void InitializeBoard()
        {
            Cells.Clear();
            for (int r = 0; r < TicTacToeGame.BoardSize; r++)
            {
                for (int c = 0; c < TicTacToeGame.BoardSize; c++)
                {
                    Cells.Add(new CellViewModel(r, c));
                }
            }
        }
        private bool CanMakeMove(object? parameter)
        {
            if (parameter is CellViewModel cell)
            {
                return _game.Status == GameStatus.InProgress && cell.IsEmpty;
            }
            return false;
        }
        private void ExecuteMakeMove(object? parameter)
        {
            if (parameter is not CellViewModel cell) return;

            if (_game.MakeMove(cell.Row, cell.Col))
            {
                cell.Value = _game.GetCell(cell.Row, cell.Col);
                CheckGameEnd();

                if (_game.Status == GameStatus.InProgress)
                {
                    var compMove = _game.ChooseComputerMove();
                    if (compMove.HasValue)
                    {
                        _game.MakeMove(compMove.Value.Row, compMove.Value.Col);
                        var compCell = GetCellVm(compMove.Value.Row, compMove.Value.Col);
                        if (compCell != null)
                            compCell.Value = _game.GetCell(compMove.Value.Row, compMove.Value.Col);

                        CheckGameEnd();
                    }
                }
            }
        }
        private CellViewModel? GetCellVm(int r, int c)
        {
            int index = r * TicTacToeGame.BoardSize + c;
            return index < Cells.Count ? Cells[index] : null;
        }
        private void CheckGameEnd()
        {
            switch (_game.Status)
            {
                case GameStatus.XWins:
                    StatusMessage = "Переміг гравець (X)!";
                    MessageBox.Show(StatusMessage, "Кінець гри");
                    break;
                case GameStatus.OWins:
                    StatusMessage = "Переміг комп'ютер (O)!";
                    MessageBox.Show(StatusMessage, "Кінець гри");
                    break;
                case GameStatus.Draw:
                    StatusMessage = "Нічия!";
                    MessageBox.Show(StatusMessage, "Кінець гри");
                    break;
                default:
                    StatusMessage = "Ваш хід (X)";
                    break;
            }
        }
        private void ResetGame()
        {
            _game.ResetGame();
            foreach (var cell in Cells)
            {
                cell.Value = CellValue.Empty;
            }
            StatusMessage = "Нова гра почалась. Ваш хід (X)";
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}