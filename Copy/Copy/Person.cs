using System.Reflection;
using System.Text.Json;

namespace Copy;

public class Person
{
    public string Name { get; set; }
    public Address Address { get; set; }
    
    /// <summary>
    /// Shallow copy of the object
    /// </summary>
    /// <returns></returns>
    public Person ShallowCopy()
    {
        return (Person)MemberwiseClone();
    }

    /// <summary>
    /// Manual deep copy of the object
    /// </summary>
    /// <returns></returns>
    public Person ManualDeepCopy()
    {
        return new Person
        {
            Name = Name,
            Address = new Address
            {
                City = Address.City,
            }
        };
    }

    /// <summary>
    /// Deep copy of the object using JSON serialization
    /// </summary>
    /// <param name="obj"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T? DeepCopy<T>(T obj)
    {
        var json = JsonSerializer.Serialize(obj);
        return JsonSerializer.Deserialize<T>(json);
    }

    /// <summary>
    /// Deep copy of the object using reflection
    /// </summary>
    /// <param name="original"></param>
    /// <returns></returns>
    public static object? DeepCopyReflection(object? original)
    {
        if (original == null) return null;
        var type = original.GetType();
        var copy = Activator.CreateInstance(type);
        foreach (var field in
                 type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        {
            var value = field.GetValue(original);
            if (value is ICloneable cloneable)
            {
                field.SetValue(copy, cloneable.Clone());
            }
            else
            {
                field.SetValue(copy, DeepCopyReflection(value));
            }
        }
        return copy;
    }
}

public class Address
{
    public string City { get; set; }
}