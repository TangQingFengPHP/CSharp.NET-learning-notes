namespace CompareList;

public class PersonComparer : IEqualityComparer<Person>
{
    public bool Equals(Person? x, Person? y)
    {
        // if (x == null && y == null)
        //     return true;
        // if (x == null || y == null)
        //     return false;
        // 使用判空的写法或者下面这种?写法
        return x?.Name == y?.Name && x?.Age == y?.Age;
    }

    public int GetHashCode(Person obj)
    {
        return HashCode.Combine(obj.Name, obj.Age);
        // 还有一种写法：
        // return obj.Name.GetHashCode() ^ obj.Age.GetHashCode();
    }
}