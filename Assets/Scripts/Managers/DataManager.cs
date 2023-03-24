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

    private List<IDataPlay> _playDataObject;
    private List<IDataGame> _gameDataObject;

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

        _playDataObject = FindAllPlayDataObjects();
        _gameDataObject = FindAllGameDataObjects();

        foreach (IDataPlay obj in _playDataObject)
        {
            obj.LoadDataPlay(_dataPlay);
        }

        foreach (IDataGame obj in _gameDataObject)
        {
            obj.LoadDataGame(_dataGame);
        }
    }

    public void Save()
    {
        New();

        _playDataObject = FindAllPlayDataObjects();
        _gameDataObject = FindAllGameDataObjects();

        SaveDataGame();

        SaveDataPlay();

    }

    public void SaveDataGame()
    {
        foreach (IDataGame obj in _gameDataObject)
        {
            obj.SaveDataGame(ref _dataGame);
        }

        _fileDataHandler.SaveDataGame(_dataGame);
    }

    public void SaveDataPlay()
    {

        foreach (IDataPlay obj in _playDataObject)
        {
            obj.SaveDataPlay(ref _dataPlay);
        }

        _fileDataHandler.SaveDataPlay(_dataPlay);
    }

    private List<IDataGame> FindAllGameDataObjects()
    {
        IEnumerable<IDataGame> unitsDataObject = FindObjectsOfType<MonoBehaviour>().OfType<IDataGame>();

        return new List<IDataGame>(unitsDataObject);
    }

    private List<IDataPlay> FindAllPlayDataObjects()
    {
        IEnumerable<IDataPlay> unitsDataObject = FindObjectsOfType<MonoBehaviour>().OfType<IDataPlay>();

        return new List<IDataPlay>(unitsDataObject);
    }

    protected override void OnApplicationQuit()
    {
        // Save();
        base.OnApplicationQuit();
    }

}
