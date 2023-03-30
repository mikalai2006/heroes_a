using UnityEngine;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.UIElements;


public class UITown : MonoBehaviour
{
    [SerializeField] public UnityAction OnHideSetting;
    [SerializeField] public UnityAction OnSave;
    [SerializeField] private UIDocument _uiDoc;
    private VisualElement _box;
    private const string _nameBox = "Town";
    private const string _nameButtonClose = "ButtonClose";
    private SceneInstance _townScene;

    [SerializeField] private Canvas _bgCanvas;
    private BaseTown _activeTown;
    private Player _activePlayer;

    public async void Init(SceneInstance townScene)
    {
        await ResourceSystem.Instance.LoadCollectionsAsset<ScriptableBuildBase>(Constants.Towns.TOWN_CASTLE);

        foreach (var t in ResourceSystem.Instance.GetCastleTown())
        {
            foreach (var b in t.Builds)
            {
                Debug.Log($"RequireBuilds= {b.RequiredBuilds}");
                Debug.Log($"RequireBuilds= {System.Convert.ToString((byte)b.RequiredBuilds, 2)}");
                Debug.Log($"TypeBuild= {b.TypeBuild.ToString()}");
                Debug.Log($"TypeBuild= {System.Convert.ToString((byte)b.TypeBuild, 2)}");
            }
        }

        Player activePlayer = LevelManager.Instance.ActivePlayer;
        _activePlayer = activePlayer;

        _townScene = townScene;

        _box = _uiDoc.rootVisualElement.Q<VisualElement>(_nameBox);

        var btnClose = _box.Q<Button>(_nameButtonClose);
        btnClose.clickable.clicked += OnClickClose;

        _bgCanvas.worldCamera = Camera.main;
        Camera.main.transform.position = new Vector3(-20, 0, -10);
    }

    private async void OnClickClose()
    {
        await GameManager.Instance.AssetProvider.UnloadAdditiveScene(_townScene);
        Camera.main.transform.position = _activePlayer.ActiveTown.gameObject.transform.position - new Vector3(0, 0, 10);

    }
}

