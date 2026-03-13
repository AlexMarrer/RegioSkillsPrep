using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Database.Entities;

[Index(nameof(Name), IsUnique = true)]
public class Exercise
{
  [Key]
  public Guid Id { get; set; }
  [MaxLength(100)]
  public required string Name { get; set; }
  [MaxLength(255)]
  public string Description { get; set; } = string.Empty;
  [MaxLength(100)]
  public required string Type { get; set; }
}
