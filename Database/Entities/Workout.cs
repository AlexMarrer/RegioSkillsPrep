using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Database.Entities;

[Index(nameof(Name), IsUnique = true)]
public class Workout
{
  public Guid Id { get; set; }
  public required string Name { get; set; }
  public DateTime StartDateTime { get; set; }
  public DateTime EndDateTime { get; set; }

  public ICollection<WorkoutExercise> WorkoutExercises { get; set; }
}
