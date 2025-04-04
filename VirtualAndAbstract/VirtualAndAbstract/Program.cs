// See https://aka.ms/new-console-template for more information

using VirtualAndAbstract;

// 基类的ShowMessage方法
var baseClass = new BaseClass();
baseClass.ShowMessage();

// 派生类的ShowMessage方法
var derivedClass = new DerivedClass();
derivedClass.ShowMessage();

// virtual 属性覆写
Animal animal = new Dog();
Console.WriteLine(animal.Name);

// abstract 方法调用和属性覆写
AbstractAnimal dog2 = new Dog2();
dog2.MakeSound();
dog2.Sleep();
Console.WriteLine(dog2.Name);

AbstractAnimal cat = new Cat();
cat.MakeSound();
Console.WriteLine(cat.Name);

// 抽象方法和虚方法同时使用
Shape shape = new Circle();
shape.Draw();
shape.Move();
