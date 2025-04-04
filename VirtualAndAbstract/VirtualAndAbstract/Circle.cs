namespace VirtualAndAbstract;

public class Circle : Shape
{
    public override void Draw()
    {
        Console.WriteLine("画一个圆形");
    }

    public override void Move()
    {
        Console.WriteLine("图形在移动");
    }
}