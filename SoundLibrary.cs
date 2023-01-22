using Godot;
using System;

namespace Core;

public partial class SoundLibrary : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SoundPlayer.SoundLibrary = this;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void PlaySound(string name) {
		var sfx = GetNode<AudioStreamPlayer>(name);
		sfx.Play();
	}
}
