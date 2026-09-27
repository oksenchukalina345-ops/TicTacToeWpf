using System.ComponentModel;
using System.Runtime.CompilerServices;
using TicTacToeWpf.Models;
namespace TicTacToeWpf.ViewModels
{
    public class CellViewModel : INotifyPropertyChanged
    {
        public int Row { get; }
        public int Col { get; }
        private CellValue _value;
        public CellValue Value
        {
            get => _value;
            set
            {
                _value = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayValue));
                OnPropertyChanged(nameof(IsEmpty));
            }
        }
        public string DisplayValue => Value switch
        {
            CellValue.X => "X",
            CellValue.O => "O",
            _ => ""
        };

        public bool IsEmpty => Value == CellValue.Empty;

        public CellViewModel(int row, int col)
        {
            Row = row;
            Col = col;
            Value = CellValue.Empty;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}