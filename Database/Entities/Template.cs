using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Database.Entities;

[Index(nameof(Name), IsUnique = true)]
public class Template
{
  [Key]
  public Guid Id { get; set; }
  public required string Name { get; set; }

  public ICollection<TemplateExercise> TemplateExercises { get; set; }
}
