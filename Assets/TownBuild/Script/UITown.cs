using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.UIElements;


public class UITown : MonoBehaviour
{
    [SerializeField] public UnityAction OnHideSetting;
    [SerializeField] public UnityAction OnSave;
    [SerializeField] private UIDocument _uiDoc;
    [SerializeField] private UITownInfo _uiTownInfo;

    // private const string _nameBox = "TownPanel";
    private VisualElement _box;
    private const string _nameButtonClose = "ButtonClose";
    private SceneInstance _townScene;

    [SerializeField] private Canvas _bgCanvas;
    private BaseTown _activeTown;
    private Player _activePlayer;
    private string NameOverlay = "Overlay";

    private Camera _cameraMain;

    public void Init(SceneInstance townScene)
    {
        _cameraMain = Camera.main;
        _cameraMain.gameObject.SetActive(false);

        _box = _uiDoc.rootVisualElement; // .Q<VisualElement>(_nameBox);

        _uiTownInfo.Init(_box);

        Player activePlayer = LevelManager.Instance.ActivePlayer;
        _activePlayer = activePlayer;
        _activeTown = _activePlayer.ActiveTown;

        // await ResourceSystem.Instance.LoadCollectionsAsset<ScriptableBuildBase>(Constants.Towns.TOWN_CASTLE);
        var activeBuildTown = ResourceSystem.Instance.GetBuildTowns().Where(t => t.TypeFaction == TypeFaction.Castle).First();
        foreach (var t in activeBuildTown.Builds)
        {
            foreach (var b in t.BuildLevels)
            {
                Debug.Log($"RequireBuilds= {b.RequiredBuilds}");
                Debug.Log($"RequireBuilds= {System.Convert.ToString((byte)b.RequiredBuilds, 2)}");
                Debug.Log($"TypeBuild= {b.TypeBuild.ToString()}");
                Debug.Log($"TypeBuild= {System.Convert.ToString((byte)b.TypeBuild, 2)}");
                Debug.Log($"May be build= {b.RequiredBuilds & _activeTown.Data.ProgressBuilds}");
            }
        }
        _townScene = townScene;

        var btnClose = _box.Q<Button>(_nameButtonClose);
        btnClose.clickable.clicked += OnClickClose;

        // _bgCanvas.worldCamera = Camera.main;
        // Camera.main.transform.position = new Vector3(-20, 0, -10);
        // Camera.main.orthographicSize = 5.2f;


        // Fill overlay color player.
        Color color = _activePlayer.DataPlayer.color;
        color.a = .6f;
        UQueryBuilder<VisualElement> builder = new UQueryBuilder<VisualElement>(_box);
        List<VisualElement> list = builder.Name(NameOverlay).ToList();
        foreach (var overlay in list)
        {
            overlay.style.backgroundColor = color;
        }
    }

    private async void OnClickClose()
    {
        _cameraMain.gameObject.SetActive(true);
        await GameManager.Instance.AssetProvider.UnloadAdditiveScene(_townScene);
        // Camera.main.transform.position = _activePlayer.ActiveTown.gameObject.transform.position - new Vector3(0, 0, 10);
    }
}

