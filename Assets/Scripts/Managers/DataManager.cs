using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    [Header("File Storage Config")]
    [SerializeField] private string fileNameGame;
    [SerializeField] private string fileNameMap;
    [SerializeField] private bool useEncryption;

    private FileDataHandler _fileDataHandler;

    private DataPlay _dataPlay;
    private DataGame _dataGame;
    public DataPlay DataPlay { get { return _dataPlay; } }

    private List<BaseMapEntity> _playCustomDataObject;
    private List<ISaveDataGame> _gameDataObject;
    private List<ISaveDataPlay> _playDataObject;
    private List<ILoadGame> _loadPlayDataObject;

    private void Start()
    {
        _fileDataHandler = new FileDataHandler(Application.persistentDataPath, fileNameGame, fileNameMap, useEncryption);

        // Load();
    }

    public void New()
    {
        _dataGame = new DataGame();
        _dataPlay = new DataPlay();

    }

    public void Load()
    {

        _dataGame = _fileDataHandler.LoadDataGame();
        _dataPlay = _fileDataHandler.LoadDataPlay();


        if (this._dataGame == null || this._dataPlay == null)
        {
            Debug.LogWarning("No data Map or Game was found. Init data to defaults.");
            New();
        }

        // _playDataObject = FindAllPlayDataObjects();
        _gameDataObject = FindAllGameDataObjects();
        _loadPlayDataObject = FindAllILoadGameObjects();

        foreach (ILoadGame obj in _loadPlayDataObject)
        {
            obj.LoadGameData(_dataPlay, _dataGame);
        }
    }

    public void Save()
    {
        New();

        _playDataObject = FindAllPlayDataObjects();
        _gameDataObject = FindAllGameDataObjects();
        _playCustomDataObject = FindAllPlayCustomDataObjects();

        SaveDataGame();

        SaveDataPlay();

    }

    public void SaveDataGame()
    {
        foreach (ISaveDataGame obj in _gameDataObject)
        {
            obj.SaveDataGame(ref _dataGame);
        }

        _fileDataHandler.SaveDataGame(_dataGame);
    }

    public void SaveDataPlay()
    {

        foreach (BaseMapEntity obj in _playCustomDataObject)
        {
            var intObj = (ISaveDataPlay)obj.GetMapObjectClass;
            intObj.SaveDataPlay(ref _dataPlay);
        }
        foreach (ISaveDataPlay obj in _playDataObject)
        {
            obj.SaveDataPlay(ref _dataPlay);
        }
        _fileDataHandler.SaveDataPlay(_dataPlay);
    }

    private List<ISaveDataGame> FindAllGameDataObjects()
    {
        IEnumerable<ISaveDataGame> unitsDataObject
            = FindObjectsOfType<MonoBehaviour>().OfType<ISaveDataGame>();

        return new List<ISaveDataGame>(unitsDataObject);
    }

    private List<BaseMapEntity> FindAllPlayCustomDataObjects()
    {
        IEnumerable<BaseMapEntity> unitsDataObject
            = FindObjectsOfType<MonoBehaviour>().OfType<BaseMapEntity>();

        return new List<BaseMapEntity>(unitsDataObject);
    }
    private List<ILoadGame> FindAllILoadGameObjects()
    {
        IEnumerable<ILoadGame> unitsDataObject
            = FindObjectsOfType<MonoBehaviour>().OfType<ILoadGame>();

        return new List<ILoadGame>(unitsDataObject);
    }
    private List<ISaveDataPlay> FindAllPlayDataObjects()
    {
        IEnumerable<ISaveDataPlay> unitsDataObject = FindObjectsOfType<MonoBehaviour>().OfType<ISaveDataPlay>();

        return new List<ISaveDataPlay>(unitsDataObject);
    }

    protected override void OnApplicationQuit()
    {
        // Save();
        base.OnApplicationQuit();
    }

}
