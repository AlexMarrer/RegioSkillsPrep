using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Database.Entities;

public class TemplateExercise
{
  [Key]
  public Guid Id { get; set; }
  public Guid TemplateId { get; set; }
  public Guid ExerciseId { get; set; }
  public int Order { get; set; }

  public Template Template { get; set; }
  public Exercise Exercise { get; set; }
  public ICollection<TemplateSet> TemplateSets { get; set; }
}
