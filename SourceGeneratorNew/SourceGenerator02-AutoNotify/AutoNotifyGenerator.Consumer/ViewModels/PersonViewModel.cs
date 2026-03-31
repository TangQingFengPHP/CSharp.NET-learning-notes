using AutoNotify;

namespace AutoNotifyGenerator.Consumer.ViewModels;

public partial class PersonViewModel
{
    [AutoNotify]
    private string _name = string.Empty;

    [AutoNotify]
    private int _age;
}
