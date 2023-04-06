using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class LevelManager : Singleton<LevelManager>, IDataPlay, IDataGame
{
    [SerializeField] private Transform _camera;
    public DataLevel Level;

    [Header("General")]
    [Space(10)]
    public bool isWater = false;
    [SerializeField] private int _countArea;
    public int CountArea => _countArea;

    public DataGameMode GameModeData;

    [Header("Setting level")]
    [Space(10)]
    public int countPlayer;
    public int countBot;
    public int maxPlayer;
    //public List<Player> listPlayer;

    public Color[] colors = new Color[4] {
        Color.red,
        Color.green,
        Color.yellow,
        Color.cyan
    };

    public Player ActivePlayer
    {
        get { return Level.listPlayer[Level.activePlayer]; }
        set
        {
            Level.listPlayer[Level.activePlayer] = value;
        }
    }

    public LevelManager()
    {
        GameModeData = new DataGameMode();
    }

    //protected override void Awake()
    //{
    //    base.Awake();
    //    GameManager.OnBeforeStateChanged += OnBeforeStateChanged;
    //    GameManager.OnAfterStateChanged += OnAfterStateChanged;
    //}

    //private void OnDestroy()
    //{
    //    GameManager.OnBeforeStateChanged -= OnBeforeStateChanged;
    //    GameManager.OnAfterStateChanged -= OnAfterStateChanged;
    //}

    //public void OnBeforeStateChanged(GameState newState)
    //{
    //    switch (newState)
    //    {
    //        case GameState.SaveGame:
    //            SaveSystem.SaveLevel(level);
    //            //UnitManager.Instance.SaveUnits();
    //            break;
    //    }
    //}
    //public void OnAfterStateChanged(GameState newState)
    //{
    //    switch (newState)
    //    {
    //        case GameState.SaveGame:

    //            break;
    //    }
    //}

    public void NewLevel()
    {

        Level = new DataLevel();
        Level.countPlayer = countPlayer;
        _countArea = Mathf.CeilToInt((GameModeData.width * GameModeData.height) / (((GameModeData.width * GameModeData.height) / countPlayer) * GameModeData.koofSizeArea));
        Level.activePlayer = -1;

        for (int i = 0; i < countPlayer; i++)
        {
            var dataPlayer = new PlayerData();
            dataPlayer.id = i;
            dataPlayer.color = colors[i];
            dataPlayer.playerType = PlayerType.User;

            var player = new Player(dataPlayer);
            Level.listPlayer.Add(player);
        }

        for (int i = countPlayer; i < (countPlayer + countBot); i++)
        {
            var dataPlayer = new PlayerData();
            dataPlayer.id = i;
            dataPlayer.color = colors[i];
            dataPlayer.playerType = PlayerType.Bot;
            var player = new Player(dataPlayer);

            Level.listPlayer.Add(player);
        }

    }

    public void StepNextPlayer()
    {
        if (Level.activePlayer < countPlayer - 1)
        {
            Level.activePlayer++;
        }
        else
        {
            Level.activePlayer = 0;
        }

        //level.activePlayer = level.activePlayer < (countPlayer + countEnemies + 1) ? level.activePlayer++ : 0;

        if (ActivePlayer.ActiveHero == null)
        {
            //GetActivePlayer().SetActiveHero(GetActivePlayer().GetActiveHero());
            ActivePlayer.ActiveHero = ActivePlayer.DataPlayer.PlayerDataReferences.ListHero[0];
        }

        SetPositionCamera(
            new Vector3(ActivePlayer.ActiveHero.Position.x, ActivePlayer.ActiveHero.Position.y, -10f)
            );

        GameManager.Instance.MapManager.ResetSky(ActivePlayer.DataPlayer.nosky);

        if (ActivePlayer.DataPlayer.nosky.Count == 0)
        {
            List<GridTileNode> listNoskyNode = GameManager.Instance
                .MapManager.DrawSky(ActivePlayer.ActiveHero.OccupiedNode, 5);
            ActivePlayer.SetNosky(listNoskyNode);
        }

        //Debug.Log($" Active Hero {level.activePlayer}");
    }

    public void AddArea(int id, TileLandscape landscape)
    {
        //Debug.Log($"Add area {id}");
        Area area = new Area();
        area.id = id;
        area.typeGround = landscape.typeGround;
        //TileLandscape landscape = ResourceSystem.Instance.GetLandscape(typeGround);
        area.isFraction = landscape.isFraction;
        Level.listArea.Add(area);
    }
    public void RemoveArea(Area area)
    {
        Level.listArea.Remove(area);
    }
    public Area GetArea(int id)
    {
        //Debug.Log($"Get area {id}");
        List<Area> listArea = Level.listArea.Where(t => t.id == id).ToList();
        return listArea.Count > 0 ? listArea[0] : null;
    }

    //public void AddPlayer(Player player)
    //{
    //    listPlayer.Add(player.data.id, player);
    //}

    public Player GetPlayer(int id)
    {
        //Debug.Log($"GetPlayer {id}");
        return id >= Level.listPlayer.Count ? null : Level.listPlayer[id];
    }

    public override string ToString()
    {
        string text = string.Format("Level::: \r\n {0}", Level.ToString());
        //foreach (Player player in listPlayer.Values)
        //{
        //    text += string.Format("\r\nPlayer::: id:[{0}] color:[{1}] type:[{2}]",
        //        player.data.id,
        //        player.data.color,
        //        player.data.playerType
        //        );
        //}
        foreach (Area area in Level.listArea)
        {
            text += string.Format("\r\nArea::: id:[{0}] countNode:[{1}] startPosition:[{2}] \n {3}",
                area.id,
                area.countNode,
                area.startPosition,
                area.Stat.ToString()
                );
        }
        return text;
    }

    public void SetPositionCamera(Vector3 pos)
    {
        _camera.transform.position = pos;
    }

    public void LoadDataPlay(DataPlay data)
    {
        Level = data.Level;
        //foreach (Player player in level.listPlayer)
        //{
        //    player.DataPlayer = new PlayerData()
        //    {
        //        ListTown = new List<UnitBase>(),
        //        ListHero = new List<Hero>(),
        //        ListMines = new List<UnitBase>(),
        //    };
        //}
        //level.listArea = new List<Area>();
    }

    public void SaveDataPlay(ref DataPlay data)
    {
        data.Level = Level;
    }

    public void LoadDataGame(DataGame data)
    {
        GameModeData = data.dataMap.GameModeData;
    }

    public void SaveDataGame(ref DataGame data)
    {
        data.dataMap.GameModeData = GameModeData;
    }
}
