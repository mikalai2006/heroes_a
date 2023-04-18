using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UIElements;


public abstract class UILocaleBase : MonoBehaviour
{
    [SerializeField] public LocalizedStringTable _localization;
    private VisualElement _box;

    private List<LocalizeObj> _elementList;

    public void Localize(VisualElement root = null)
    {
        _box = root;
        OnLocalization();
    }

    private void OnLocalization()
    {
        var op = _localization.GetTableAsync();
        _elementList = HelperUI.FindAllTextElements(_box);
        if (op.IsDone)
        {
            OnTableLoaded(op);
        }
        else
        {
            op.Completed -= OnTableLoaded;
            op.Completed += OnTableLoaded;
        }
    }
    private void OnTableLoaded(AsyncOperationHandle<StringTable> op)
    {
        StringTable table = op.Result;

        foreach (var item in _elementList)
        {
            var entry = op.Result[item.Key];
            if (entry != null)
                item.Element.text = entry.LocalizedValue;
            else
                Debug.LogWarning($"No {op.Result.LocaleIdentifier.Code} translation for key: '{item.Key}'");
        }
    }
}

