using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Database.Entities;

public class WorkoutSet
{
  [Key]
  public Guid Id { get; set; }
  public Guid WorkoutExerciseId { get; set; }
  public int SetNumber { get; set; }
  public decimal Weight { get; set; }
  public int Repetitions { get; set; }

  public WorkoutExercise WorkoutExercise { get; set; }
}
