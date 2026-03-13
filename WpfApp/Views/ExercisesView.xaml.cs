using Database;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using WpfApp.DTO;

namespace WpfApp.Views;

/// <summary>
/// Interaktionslogik für ExercisesView.xaml
/// </summary>
public partial class ExercisesView : UserControl
{
  public ExercisesView()
  {
    InitializeComponent();
    Loaded += ExercisesView_Loaded;
  }

  private void ExercisesView_Loaded(object sender, RoutedEventArgs e)
  {
    LoadExercises();
  }

  private void LoadExercises()
  {
    using var db = new ApplicationDbContext();
    var Exercises = db.Exercise
        .OrderByDescending(w => w.Name)
        .Select(w => new ExerciseDto
        {
          Id = w.Id,
          Name = w.Name,
          Description = w.Description,
          Type = w.Type
        })
        .ToList();

    ExerciseGrid.ItemsSource = Exercises;
  }

  private void EditExercise_Click(object sender, RoutedEventArgs e)
  {
    var selected = ExerciseGrid.SelectedItem as ExerciseDto;
    if (selected == null)
    {
      MessageBox.Show("Please select a Exercise first.");
      return;
    }

    MessageBox.Show("Work in progress...");
    return;
    // Todo : Open Edit/Add Exercise view with selected Exercise details
  }

  private void ViewExercise_Click(object sender, RoutedEventArgs e)
  {
    var selected = ExerciseGrid.SelectedItem as ExerciseDto;
    if (selected == null)
    {
      MessageBox.Show("Please select a Exercise first.");
      return;
    }

    var mainWindow = Window.GetWindow(this) as MainWindow;
    mainWindow.NavigateTo(new ExerciseProgressView(selected.Id, selected.Name));
  }

  private void AddExercise_Click(object sender, RoutedEventArgs e)
  {
    MessageBox.Show("Work in progress...");
    return;
    // Todo : Open Edit/Add Exercise view without
  }

  private void DeleteExercise_Click(object sender, RoutedEventArgs e)
  {
    var selected = ExerciseGrid.SelectedItem as ExerciseDto;

    if (selected == null)
    {
      MessageBox.Show("Please select a Exercise first.");
      return;
    }

    var result = MessageBox.Show($"Are you sure you want to delete Exercise '{selected.Name}'?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

    if (result == MessageBoxResult.Yes)
    {
      using var db = new ApplicationDbContext();
      var Exercise = db.Exercise.Find(selected.Id);

      if (Exercise != null)
      {
        db.Exercise.Remove(Exercise);
        db.SaveChanges();
      }

      LoadExercises();
    }
  }

  private void ExercisesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
  {

  }
}
