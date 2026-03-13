using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WpfApp.DTO;

public class ExerciseDto
{
  public Guid Id { get; set; }
  public required string Name { get; set; }
  public string Description { get; set; } = string.Empty;
  public required string Type { get; set; }
}
