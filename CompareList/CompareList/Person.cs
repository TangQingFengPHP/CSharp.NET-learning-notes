namespace CompareList;

public class Person
{
    public string Name { get; set; }
    public int Age { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj is Person person)
        {
            return Name == person.Name && Age == person.Age;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Age);
    }
}