using Avalonia.Collections;

namespace Sach.Models;

public class Advantage
{
    public AvaloniaList<With> With { get; set; }
    public AvaloniaList<Vs> Vs { get; set; }
}