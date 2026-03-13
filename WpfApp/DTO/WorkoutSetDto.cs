using System.ComponentModel;

namespace WpfApp.DTO;

public class WorkoutSetDto
{

  private int _setNumber;
  private decimal _weight;
  private int _reps;

  public int SetNumber
  {
    get => _setNumber;
    set { _setNumber = value; OnPropertyChanged(nameof(SetNumber)); }
  }

  public decimal Weight
  {
    get => _weight;
    set { _weight = value; OnPropertyChanged(nameof(Weight)); }
  }

  public int Reps
  {
    get => _reps;
    set { _reps = value; OnPropertyChanged(nameof(Reps)); }
  }

  public event PropertyChangedEventHandler PropertyChanged;
  protected void OnPropertyChanged(string name) =>
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
