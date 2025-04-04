namespace VirtualAndAbstract;

public class Cat : AbstractAnimal
{
    public override string Name { get; set; } = "Cat";
    public override void MakeSound()
    {
        Console.WriteLine("喵喵喵！"); 
    }
}