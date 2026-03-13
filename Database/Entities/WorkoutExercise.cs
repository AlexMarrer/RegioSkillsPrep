using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Entities;

public class WorkoutExercise
{
  public Guid Id { get; set; }
  public Guid WorkoutId { get; set; }
  public Guid ExerciseId { get; set; }
  public int Order { get; set; }

  public Workout Workout { get; set; }
  public Exercise Exercise { get; set; }
  public ICollection<WorkoutSet> WorkoutSets { get; set; }
}
