using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Database.Entities;

public class TemplateSet
{
  [Key]
  public Guid Id { get; set; }
  public Guid TemplateExerciseId { get; set; }
  public int SetNumber { get; set; }
  public int Repetitions { get; set; }

  public TemplateExercise TemplateExercise { get; set; }
}
