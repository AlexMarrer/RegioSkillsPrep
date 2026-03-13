using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace WpfApp.DTO;

public class WorkoutExerciseDto
{
  public Guid ExerciseId { get; set; }
  public string ExerciseName { get; set; }
  public ObservableCollection<WorkoutSetDto> Sets { get; set; }
}
