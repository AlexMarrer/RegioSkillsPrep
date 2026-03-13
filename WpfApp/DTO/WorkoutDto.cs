using System;

namespace WpfApp.DTO;

public class WorkoutDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public string Duration => (EndDateTime - StartDateTime).ToString(@"hh\:mm\:ss");
}
