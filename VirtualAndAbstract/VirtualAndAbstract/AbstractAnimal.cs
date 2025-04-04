namespace VirtualAndAbstract;

public abstract class AbstractAnimal
{
    public abstract string Name { get; set; }
    public abstract void MakeSound();
    public virtual void Sleep()
    {
        Console.WriteLine("The animal is sleeping.");
    }

}