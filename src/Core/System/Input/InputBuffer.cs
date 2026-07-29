using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Kaleidoscope.Core.System.Input;

public class InputBuffer
{
    private ulong _frame;
    private readonly List<BufferedInput> _bufferedInputs = [];

    public void Process()
    {
        _frame += 1;
        _bufferedInputs.RemoveAll(input => input.ValidUntil < _frame);
    }

    public void Add(BufferedInput input)
    {
        _bufferedInputs.Add(input);
    }
    
    public void Add(string name, string group, int priority, ulong validUntil)
    {
        Add(new BufferedInput(name, group, priority, validUntil));
    }

    public void Add(string name, string group, int priority, int frames)
    {
        if (frames < 0) return;
        Add(new BufferedInput(name, group, priority, _frame + (ulong)frames));
    }

    public List<BufferedInput> Take(string name)
    {
        var list = _bufferedInputs
            .Where(input => input.Name == name)
            .OrderByDescending(input => input.Priority)
            .ThenBy(input => input.Id)
            .ToList();
        list.ForEach(input => _bufferedInputs.Remove(input));
        return list;
    }

    public List<BufferedInput> TakeGroup(string group)
    {
        if (group == null) return [];
        var list = _bufferedInputs
            .Where(input => input.Group == group)
            .OrderByDescending(input => input.Priority)
            .ThenBy(input => input.Id)
            .ToList();
        list.ForEach(input => _bufferedInputs.Remove(input));
        return list;   
    }
    
}
