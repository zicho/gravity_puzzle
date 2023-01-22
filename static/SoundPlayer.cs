namespace Core;

public static class SoundPlayer
{


    public static SoundLibrary SoundLibrary { get; set; }

    public static void PlaySound(string name)
    {
        SoundLibrary.PlaySound(name);
    }
}