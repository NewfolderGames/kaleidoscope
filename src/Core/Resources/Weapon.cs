using System.Collections.Generic;
using Godot;

namespace Kaleidoscope.Core.Resources;

[GlobalClass]
public partial class Weapon : Resource
{
    [Export] public string[] Attacks { get; private set; }
    [Export] public SpriteFrames Sprite { get; private set; }
    [Export] public SpriteFrames BorderSprite { get; private set; }
}