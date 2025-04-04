namespace VirtualAndAbstract;

public class Dog2 : AbstractAnimal, IAnimal
{
    public override string Name { get; set; } = "Dog";
    public override void MakeSound()
    {
        Console.WriteLine("汪汪汪！");
    }

    public override void Sleep()
    {
        Console.WriteLine("狗狗正在睡觉！");
    }
}