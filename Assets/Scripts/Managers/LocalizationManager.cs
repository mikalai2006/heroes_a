// using System.Collections;
// using System.Collections.Generic;

// using UnityEngine;
// using UnityEngine.Localization;
// using UnityEngine.Localization.SmartFormat.PersistentVariables;
// using UnityEngine.Localization.Tables;
// using UnityEngine.ResourceManagement.AsyncOperations;

// public class LocalizationManager : Singleton<LocalizationManager>
// {
//     [SerializeField] private LocalizedStringTable _AdventureTable;
//     public StringTable AdventureLocales;

//     protected override void Awake()
//     {
//         base.Awake();
//         OnLocalization();
//     }

//     private void OnLocalization()
//     {
//         var op = _AdventureTable.GetTableAsync();
//         if (op.IsDone)
//         {
//             OnTableLoaded(op);
//         }
//         else
//         {
//             op.Completed -= OnTableLoaded;
//             op.Completed += OnTableLoaded;
//         }
//     }
//     private void OnTableLoaded(AsyncOperationHandle<StringTable> op)
//     {
//         AdventureLocales = op.Result;
//     }
// }
