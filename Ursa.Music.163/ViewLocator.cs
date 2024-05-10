using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;

namespace Ursa.Music._163;

public class ViewLocator : IDataTemplate
{
    public Control? Build(object? data)
    {
        if (data is null)
            return null;

        if (data.GetType().Name is not "String")
            return new TextBlock { Text = "Not Found: " };
        var name = data.ToString();
        var type = Type.GetType("Ursa.Music._163.Page." + name + "Page");

        if (type != null)
        {
            var control = (Control)Activator.CreateInstance(type)!;
            return control;
        }

        return new TextBlock { Text = "Not Found: " + name };
    }

    public bool Match(object? data)
    {
        return true;
    }

    public object? GetPageType(object? data)
    {
        if (data is null)
            return null;

        if (data.GetType().Name is not "String")
            return null;
        var name = data.ToString();
        var type = Type.GetType("Ursa.Music._163.Page." + name + "Page");
        return type;
    }
}