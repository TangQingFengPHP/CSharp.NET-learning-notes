namespace VirtualAndAbstract;

public abstract class Shape
{
    // 抽象类中可以有抽象方法和虚方法
    public abstract void Draw();

    public virtual void Move()
    {
        Console.WriteLine("Shape is moving");
    }
}