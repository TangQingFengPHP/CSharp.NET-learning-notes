// See https://aka.ms/new-console-template for more information

using Sealed;

Console.WriteLine("Hello, World!");

var myClass = new MyClass();
myClass.Show();

#if STAGING
Console.WriteLine("This is a staging environment.");
#endif