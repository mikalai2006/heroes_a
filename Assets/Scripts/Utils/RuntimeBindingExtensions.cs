using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

public static class RuntimeBindingExtensions
{
    private static readonly Dictionary<VisualElement, List<(IProperty property, Action<object> binding)>> propertyBindings = new Dictionary<VisualElement, List<(IProperty property, Action<object> binding)>>();

    public static void BindProperty(this VisualElement element, IProperty property)
    {
        if (!propertyBindings.TryGetValue(element, out var bindingsList))
        {
            bindingsList = new List<(IProperty, Action<object>)>();
            propertyBindings.Add(element, bindingsList);
        }

        Action<object> onPropertyValueChanged = OnPropertyValueChanged;
        bindingsList.Add((property, onPropertyValueChanged));

        property.ValueChanged += onPropertyValueChanged;

        OnPropertyValueChanged(property.Value);

        void OnPropertyValueChanged(object newValue)
        {
            float val = (float)newValue;
            element.style.height = new StyleLength(new Length(val, LengthUnit.Percent)); // ?.ToString() ?? "";
        }
    }
    public static void BindPropertyText(this Label element, IProperty property)
    {
        if (!propertyBindings.TryGetValue(element, out var bindingsList))
        {
            bindingsList = new List<(IProperty, Action<object>)>();
            propertyBindings.Add(element, bindingsList);
        }

        Action<object> onPropertyValueChanged = OnPropertyValueChanged;
        bindingsList.Add((property, onPropertyValueChanged));

        property.ValueChanged += onPropertyValueChanged;

        OnPropertyValueChanged(property.Value);

        void OnPropertyValueChanged(object newValue)
        {
            //string val = (string)newValue;
            element.text = newValue.ToString();
        }
    }


    public static void BindPropertyInt(this IntegerField element, IProperty property)
    {
        if (!propertyBindings.TryGetValue(element, out var bindingsList))
        {
            bindingsList = new List<(IProperty, Action<object>)>();
            propertyBindings.Add(element, bindingsList);
        }

        Action<object> onPropertyValueChanged = OnPropertyValueChanged;
        bindingsList.Add((property, onPropertyValueChanged));

        property.ValueChanged += onPropertyValueChanged;

        OnPropertyValueChanged(property.Value);

        void OnPropertyValueChanged(object newValue)
        {
            int val = (int)newValue;
            element.value = val;
        }
    }


    public static void UnbindProperty(this VisualElement element, IProperty property)
    {
        if (!propertyBindings.TryGetValue(element, out var bindingsList))
        {
            return;
        }

        for (int i = bindingsList.Count - 1; i >= 0; i--)
        {
            var propertyBinding = bindingsList[i];
            if (propertyBinding.property == property)
            {
                propertyBinding.property.ValueChanged -= propertyBinding.binding;
                bindingsList.RemoveAt(i);
            }
        }
    }

    public static void UnbindAllProperties(this VisualElement element)
    {
        if (!propertyBindings.TryGetValue(element, out var bindingsList))
        {
            return;
        }

        foreach (var propertyBinding in bindingsList)
        {
            propertyBinding.property.ValueChanged -= propertyBinding.binding;
        }

        bindingsList.Clear();
    }
}