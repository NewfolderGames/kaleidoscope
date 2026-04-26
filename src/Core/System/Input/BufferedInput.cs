namespace Kaleidoscope.Core.System.Input;

public class BufferedInput
{
    public readonly int Id;
    public readonly string Name;
    public readonly string Group;
    public readonly int Priority;
    public readonly ulong ValidUntil;
    
    private static int _nextId = 0;
    
    public BufferedInput(string name, string group, int priority, ulong validUntil)
    {
        Name = name;
        Group = group;
        ValidUntil = validUntil;
        Priority = priority;
        Id = ++_nextId;
    }
}