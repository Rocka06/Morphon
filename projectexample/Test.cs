using Godot;
using Morphon;

public partial class Test : Node
{
    [Export] public AnimalResource animal;

    private MorphonConfigFile file = new();

    public override void _Ready()
    {
        file.SetValue("Animal", "Animal", (IMorphonSerializable)animal);
        file.Save("user://Save.json");

        file.Clear();
        file.Load("user://Save.json");

        GD.Print("Loaded data:");
        GD.Print(file.GetValue<AnimalResource>("Animal", "Animal"));
    }
}
