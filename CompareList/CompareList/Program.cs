using CompareList;

// 基本类型比较（元素顺序必须一致）
var list1 = new List<int> { 1, 2, 3 };
var list2 = new List<int> { 1, 2, 3 };
Console.WriteLine($"list1.SequenceEqual(list2): {list1.SequenceEqual(list2)}"); // 输出: True

// 忽略顺序比较
var list3 = new List<int> { 1, 2, 3 };
var list4 = new List<int> { 3, 2, 1 };

Console.WriteLine(list3.OrderBy(x => x).SequenceEqual(list4.OrderBy(x => x))); // 输出: True

// 先排序再比较
list3.Sort();
list4.Sort();
Console.WriteLine(list3.SequenceEqual(list4)); // 输出: True

// 复杂类型（自定义对象列表）
var person1 = new List<Person>
{
    new Person { Name = "John", Age = 18 },
    new Person { Name = "Jane", Age = 19 },
};

var person2 = new List<Person>
{
    new Person { Name = "John", Age = 18 },
    new Person { Name = "Jane", Age = 19 },
};
Console.WriteLine($"person1.SequenceEqual(person2, new PersonComparer()): {person1.SequenceEqual(person2, new PersonComparer())}"); // 输出: True

// Person对象已经实现了Equals和GetHashCode方法，所以可以直接使用SequenceEqual方法进行比较。
Console.WriteLine($"person1.SequenceEqual(person2): {person1.SequenceEqual(person2)}"); // 输出: True

// 判断是否完全包含对方（不关心顺序）
Console.WriteLine(list3.Count == list4.Count && !list3.Except(list4).Any() && !list4.Except(list3).Any()); // 输出: True

// 使用 SetEquals（无序、无重复判断）
Console.WriteLine(new HashSet<int>(list3).SetEquals(list4)); // 输出: True

// 或者
// HashSet<int> set1 = new HashSet<int>(list3);
// HashSet<int> set2 = new HashSet<int>(list4);
//
// bool isEqual = set1.SetEquals(set2);
// Console.WriteLine(isEqual); // 输出: True

// 比较两个 null 列表
List<int>? list5 = null;
List<int>? list6 = null;
Console.WriteLine(list5 == list6); // 输出: True

// 比较两个带null元素的列表
List<string?> list7 = new List<string?> { "a", null };
List<string?> list8 = new List<string?> { "a", null };
Console.WriteLine(list7.SequenceEqual(list8)); // 输出: True