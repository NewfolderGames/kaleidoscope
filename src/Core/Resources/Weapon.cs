using Godot;
using Godot.Collections;
using CollectionExtensions = System.Collections.Generic.CollectionExtensions;

namespace Kaleidoscope.Core.Resources;

[GlobalClass]
public partial class Weapon : Resource
{
	[ExportCategory("Rendering")]
	[Export] public SpriteFrames Sprite { get; private set; }
	[Export] public SpriteFrames BorderSprite { get; private set; }

}
