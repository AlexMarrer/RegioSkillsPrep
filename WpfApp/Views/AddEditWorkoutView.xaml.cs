using Database;
using Database.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using WpfApp.DTO;

namespace WpfApp.Views;

/// <summary>
/// Interaktionslogik für AddEditWorkoutView.xaml
/// </summary>
public partial class AddEditWorkoutView : UserControl
{
  private Guid? _workoutId;
  private ObservableCollection<WorkoutExerciseDto> _exercises = new();
  private bool _isEditing = false;

  public AddEditWorkoutView()
  {
    InitializeComponent();
    LoadExerciseComboBox();
    ExercisesList.ItemsSource = _exercises;
    PageTitle.Text = "Add workout";

    // Template-Auswahl anbieten
    OfferTemplateSelection();
  }

  public AddEditWorkoutView(Guid workoutId)
  {
    InitializeComponent();
    _workoutId = workoutId;
    LoadExerciseComboBox();
    ExercisesList.ItemsSource = _exercises;
    PageTitle.Text = "Edit workout";
    LoadWorkout(workoutId);
  }

  private void LoadExerciseComboBox()
  {
    using var ctx = new ApplicationDbContext();
    ExerciseComboBox.ItemsSource = ctx.Exercise.ToList();
  }

  private void LoadWorkout(Guid workoutId)
  {
    using var ctx = new ApplicationDbContext();
    var workout = ctx.Workout
        .Include(w => w.WorkoutExercises)
            .ThenInclude(we => we.Exercise)
        .Include(w => w.WorkoutExercises)
            .ThenInclude(we => we.WorkoutSets)
        .FirstOrDefault(w => w.Id == workoutId);

    if (workout == null) { return; }

    WorkoutNameBox.Text = workout.Name;
    StartDatePicker.SelectedDate = workout.StartDateTime.Date;
    StartTimeBox.Text = workout.StartDateTime.ToString("HH:mm");
    EndDatePicker.SelectedDate = workout.EndDateTime.Date;
    EndTimeBox.Text = workout.EndDateTime.ToString("HH:mm");

    foreach (var workoutExercise in workout.WorkoutExercises.OrderBy(x => x.Order))
    {
      var sets = new ObservableCollection<WorkoutSetDto>(
          workoutExercise.WorkoutSets.OrderBy(set => set.SetNumber).Select(set => new WorkoutSetDto
          {
            SetNumber = set.SetNumber,
            Weight = set.Weight,
            Reps = set.Repetitions,
          }));

      sets.CollectionChanged += (s, args) => RenumberSets(sets);

      _exercises.Add(new WorkoutExerciseDto
      {
        ExerciseId = workoutExercise.ExerciseId,
        ExerciseName = workoutExercise.Exercise.Name,
        Sets = sets
      });
    }
  }

  private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs arg)
  {
    _isEditing = true;
  }

  public void OfferTemplateSelection()
  {
    using var ctx = new ApplicationDbContext();
    var templates = ctx.Template.ToList();

    if (!templates.Any())
    {
      MessageBox.Show("No templates available. You can add exercises manually.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
      return;
    }

    var result = MessageBox.Show("Do you want to start from a template?", "Template Selection", MessageBoxButton.YesNo, MessageBoxImage.Question);

    if (result == MessageBoxResult.Yes)
    {
      // TODO: Template-Auswahl Dialog öffnen
      // Für jetzt simpel mit erstem Template
    }
  }

  private void Cancel_Click(object sender, RoutedEventArgs e)
  {
    if (_isEditing)
    {
      var result = MessageBox.Show(
          "You have unsaved changes. Are you sure you want to cancel?",
          "Unsaved Changes", MessageBoxButton.YesNo, MessageBoxImage.Warning);

      if (result == MessageBoxResult.No) return;
    }

    var mainWindow = (MainWindow)Application.Current.MainWindow;
    mainWindow.NavigateTo(new WorkoutsView());
  }

  private void RenumberSets(ObservableCollection<WorkoutSetDto> sets)
  {
    for (int i = 0; i < sets.Count; i++)
    {
      sets[i].SetNumber = i + 1;
    }
  }

  private bool ValidateForm(DateTime start, DateTime end)
  {
    if (string.IsNullOrWhiteSpace(WorkoutNameBox.Text))
    {
      MessageBox.Show("Name is required.");
      return false;
    }

    if (StartDatePicker.SelectedDate == null || EndDatePicker.SelectedDate == null)
    {
      MessageBox.Show("Start and End dates are required.");
      return false;
    }

    if (!TimeSpan.TryParse(StartTimeBox.Text, out _) ||
        !TimeSpan.TryParse(EndTimeBox.Text, out _))
    {
      MessageBox.Show("Invalid time format. Use HH:mm.");
      return false;
    }

    if (end <= start)
    {
      MessageBox.Show("End must be after start.");
      return false;
    }

    if (!_exercises.Any())
    {
      MessageBox.Show("A workout must contain at least one exercise.");
      return false;
    }

    return true;
  }

  private bool ValidateAgainstDatabase(ApplicationDbContext ctx, DateTime start, DateTime end)
  {
    var overlapping = ctx.Workout
        .Where(w => w.Id != _workoutId)
        .Any(w => start < w.EndDateTime && end > w.StartDateTime);

    if (overlapping)
    {
      MessageBox.Show("This workout overlaps with an existing workout.");
      return false;
    }

    var nameExists = ctx.Workout
        .Any(w => w.Name == WorkoutNameBox.Text && w.Id != _workoutId);

    if (nameExists)
    {
      MessageBox.Show("A workout with this name already exists.");
      return false;
    }

    return true;
  }

  private void Save_Click(object sender, RoutedEventArgs e)
  {
    using var ctx = new ApplicationDbContext();
    _ = TimeSpan.TryParse(StartTimeBox.Text, out var startTime);
    _ = TimeSpan.TryParse(EndTimeBox.Text, out var endTime);
    var start = StartDatePicker.SelectedDate!.Value.Date + startTime;
    var end = EndDatePicker.SelectedDate!.Value.Date + endTime;

    if (!ValidateForm(start, end)) return;
    if (!ValidateAgainstDatabase(ctx, start, end)) return;

    // --- Save ---
    Workout workout;
    if (_workoutId.HasValue)
    {
      workout = ctx.Workout
          .Include(w => w.WorkoutExercises)
              .ThenInclude(we => we.WorkoutSets)
          .First(w => w.Id == _workoutId.Value);

      // Alte Exercises/Sets entfernen
      ctx.WorkoutSet.RemoveRange(workout.WorkoutExercises.SelectMany(we => we.WorkoutSets));
      ctx.WorkoutExercise.RemoveRange(workout.WorkoutExercises);
    }
    else
    {
      workout = new Workout() { Name = WorkoutNameBox.Text };
      ctx.Workout.Add(workout);
    }

    workout.Name = WorkoutNameBox.Text;
    workout.StartDateTime = start;
    workout.EndDateTime = end;

    for (int i = 0; i < _exercises.Count; i++)
    {
      var exerciseDto = _exercises[i];
      var workoutExercise = new WorkoutExercise
      {
        WorkoutId = workout.Id,
        ExerciseId = exerciseDto.ExerciseId,
        Order = i + 1
      };
      ctx.WorkoutExercise.Add(workoutExercise);

      foreach (var setDto in exerciseDto.Sets)
      {
        ctx.WorkoutSet.Add(new WorkoutSet
        {
          WorkoutExerciseId = workoutExercise.Id,
          SetNumber = setDto.SetNumber,
          Weight = setDto.Weight,
          Repetitions = setDto.Reps
        });
      }
    }

    ctx.SaveChanges();

    // Zurück zur Workout-Liste
    var mainWindow = (MainWindow)Application.Current.MainWindow;
    mainWindow.NavigateTo(new WorkoutsView());
  }

  private void AddExercise_Click(object sender, RoutedEventArgs e)
  {
    var selectedExercise = ExerciseComboBox.SelectedItem as Exercise;
    if (selectedExercise is null)
    {
      MessageBox.Show("Please select an exercise first.");
      return;
    }

    if (_exercises.Any(ex => ex.ExerciseId == selectedExercise.Id))
    {
      MessageBox.Show("This exercise is already added.");
      return;
    }

    var exercise = new WorkoutExerciseDto
    {
      ExerciseId = selectedExercise.Id,
      ExerciseName = selectedExercise.Name,
      Sets = new ObservableCollection<WorkoutSetDto>
      {
        new WorkoutSetDto { SetNumber = 1, Weight = 0, Reps = 0 }
      }
    };

    exercise.Sets.CollectionChanged += (s, args) => RenumberSets(exercise.Sets);

    _exercises.Add(exercise);

    _isEditing = true;
  }

  private void RemoveExercise_Click(object sender, RoutedEventArgs e)
  {
    var button = sender as Button;
    var exerciseId = button.Tag as Guid?;
    var exercise = _exercises.FirstOrDefault(ex => ex.ExerciseId == exerciseId);

    if (exerciseId is null) { return; }

    _exercises.Remove(exercise);
    _isEditing = true;
  }
}
