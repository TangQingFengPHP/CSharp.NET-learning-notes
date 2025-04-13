// 值拷贝

using System.Dynamic;
using AutoMapper;
using Copy;
using Force.DeepCloner;
using Mapper = ExpressMapper.Mapper;

int a = 10;
int b = a;
b = 20;
Console.WriteLine("a = {0}, b = {1}", a, b); // a = 10, b = 20

// 引用类型是引用拷贝
Person p1 = new Person { Name = "Alice" };
Person p2 = p1;
p2.Name = "Bob";
Console.WriteLine("p1.Name = {0}, p2.Name = {1}", p1.Name, p2.Name); // p1.Name = Bob, p2.Name = Bob

// 自定义结构体也是值类型（值拷贝）
Point point1 = new Point { X = 1, Y = 2 };
Point point2 = point1; // 值拷贝
point2.X = 100;
Console.WriteLine("point1.X = {0}, point2.X = {1}", point1.X, point2.X); // point1.X = 1, point2.X = 100 

// 使用浅拷贝
var original = new Person { Name = "Alice", Address = new Address { City = "Beijing" } };
var copy = original.ShallowCopy();

copy.Name = "Bob";
copy.Address.City = "Shanghai";

Console.WriteLine("original.Name = {0}, copy.Name = {1}", original.Name, copy.Name); // original.Name = Alice, copy.Name = Bob
Console.WriteLine("original.Address.City = {0}, copy.Address.City = {1}", original.Address.City, copy.Address.City); // original.Address.City = Shanghai, copy.Address.City = Shanghai

// 使用手动深拷贝
var copy2 = original.ManualDeepCopy();
copy2.Name = "Charlie";
copy2.Address.City = "London";
Console.WriteLine("original.Name = {0}, copy2.Name = {1}", original.Name, copy2.Name); // original.Name = Alice, copy2.Name = Charlie
Console.WriteLine("original.Address.City = {0}, copy2.Address.City = {1}", original.Address.City, copy2.Address.City); // original.Address.City = Shanghai, copy2.Address.City = London

// 使用json序列化深拷贝
var copy3 = Person.DeepCopy(original);
if (copy3 != null)
{
    copy3.Name = "John";
    copy3.Address.City = "Paris";
    Console.WriteLine("original.Name = {0}, copy3.Name = {1}", original.Name, copy3.Name); // original.Name = Alice, copy3.Name = John
    Console.WriteLine("original.Address.City = {0}, copy3.Address.City = {1}", original.Address.City, copy3.Address.City); // original.Address.City = Shanghai, copy3.Address.City = Paris
}

// 使用反射深拷贝
var copy4 = Person.DeepCopyReflection(original);
if (copy4 != null)
{
    var innerPerson = (Person)copy4;
    innerPerson.Name = "David";
    innerPerson.Address.City = "Hong Kong";
    Console.WriteLine("original.Name = {0}, innerPerson.Name = {1}", original.Name, innerPerson.Name); // original.Name = Alice, innerPerson.Name = David
    Console.WriteLine("original.Address.City = {0}, innerPerson.Address.City = {1}", original.Address.City, innerPerson.Address.City); // original.Address.City = Shanghai, innerPerson.Address.City = Hong Kong
}

// 使用第三方库DeepCloner的基础用法
var copy5 = original.DeepClone();
copy5.Name = "Jack";
copy5.Address.City = "Singapore";
Console.WriteLine("original.Name = {0}, copy5.Name = {1}", original.Name, copy5.Name); // original.Name = Alice, copy5.Name = Jack
Console.WriteLine("original.Address.City = {0}, copy5.Address.City = {1}", original.Address.City, copy5.Address.City); // original.Address.City = Shanghai, copy5.Address.City = Singapore

// 使用第三方库DeepCloner的深拷贝，处理循环引用
var company = new Company();
var employee = new Employee { Name = "Tom", Company = company };
company.Employees = new List<Employee> { employee };

var companyCopy = company.DeepClone();
companyCopy.Employees[0].Name = "Jerry";
Console.WriteLine("company.Employees[0].Name = {0}", company.Employees[0].Name);
Console.WriteLine("companyCopy.Employees[0].Name = {0}", companyCopy.Employees[0].Name);

// 使用AutoMapper的深拷贝
var config = new MapperConfiguration(cfg => cfg.CreateMap<Source, Destination>());
var mapper = config.CreateMapper();

var source = new Source { Id = 1, Name = "Alice" };

var destination = mapper.Map<Destination>(source);
destination.Name = "Bob";

Console.WriteLine("source.Name = {0}, destination.Name = {1}", source.Name, destination.Name); // source.Name = Alice, destination.Name = Bob

// 使用ExpressMapper.Core 的深拷贝
Mapper.Register<Source, Destination>();

var source2 = new Source { Id = 2, Name = "Alice" };
var destination2 = Mapper.Map<Source, Destination>(source2);
destination2.Name = "Bob";

Console.WriteLine("source2.Name = {0}, destination2.Name = {1}", source2.Name, destination2.Name); // source2.Name = Alice, destination2.Name = Bob
