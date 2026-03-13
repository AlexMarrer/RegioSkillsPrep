using Database;
using System.Windows;
using System.Windows.Controls;
using WpfApp.DTO;

namespace WpfApp.Views;

/// <summary>
/// Interaktionslogik für WorkoutsView.xaml
/// </summary>
public partial class WorkoutsView : UserControl
{
  public WorkoutsView()
  {
    InitializeComponent();
    Loaded += WorkoutsView_Loaded;
  }

  private void WorkoutsView_Loaded(object sender, RoutedEventArgs e)
  {
    LoadWorkouts();
  }

  private void LoadWorkouts()
  {
    using var db = new ApplicationDbContext();
    var workouts = db.Workout
        .OrderByDescending(w => w.StartDateTime)
        .Select(w => new WorkoutDto
        {
          Id = w.Id,
          Name = w.Name,
          StartDateTime = w.StartDateTime,
          EndDateTime = w.EndDateTime
        })
        .ToList();

    WorkoutsGrid.ItemsSource = workouts;
  }

  private void EditWorkout_Click(object sender, RoutedEventArgs e)
  {
    var selected = WorkoutsGrid.SelectedItem as WorkoutDto;
    if (selected == null)
    {
      MessageBox.Show("Please select a workout first.");
      return;
    }

    var mainWindow = Window.GetWindow(this) as MainWindow;
    mainWindow?.NavigateTo(new AddEditWorkoutView(selected.Id));
  }

  private void ViewWorkout_Click(object sender, RoutedEventArgs e)
  {
    var selected = WorkoutsGrid.SelectedItem as WorkoutDto;
    if (selected == null)
    {
      MessageBox.Show("Please select a workout first.");
      return;
    }
    var ctx = new ApplicationDbContext();
    var exercise = ctx.WorkoutExercise
        .Where(we => we.WorkoutId == selected.Id)
        .Select(we => we.Exercise)
        .FirstOrDefault();

    var mainWindow = Window.GetWindow(this) as MainWindow;
    mainWindow?.NavigateTo(new ExerciseProgressView(exercise.Id, exercise.Name));
  }

  private void AddWorkout_Click(object sender, RoutedEventArgs e)
  {
    var mainWindow = Window.GetWindow(this) as MainWindow;
    mainWindow?.NavigateTo(new AddEditWorkoutView());
  }

  private void DeleteWorkout_Click(object sender, RoutedEventArgs e)
  {
    var button = sender as Button;
    var workoutDto = button.DataContext as WorkoutDto;

    var result = MessageBox.Show($"Are you sure you want to delete workout '{workoutDto.Name}'?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

    if (result == MessageBoxResult.Yes)
    {
      using var db = new ApplicationDbContext();
      var workout = db.Workout.Find(workoutDto.Id);

      if (workout != null)
      {
        db.Workout.Remove(workout);
        db.SaveChanges();
      }

      LoadWorkouts();
    }
  }

  private void WorkoutsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
  {

  }
}
