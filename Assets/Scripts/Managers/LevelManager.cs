using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class LevelManager : Singleton<LevelManager>, ISaveDataPlay, ISaveDataGame
{
    [SerializeField] private Transform _camera;
    public DataLevel Level;
    private DataLevel DefaultSettings;
    public ScriptableGameSetting ConfigGameSettings;
    public List<CurrentPlayerType> TypePlayers = new List<CurrentPlayerType>();
    // [Header("General")]
    // [Space(10)]

    // public DataGameMode GameModeData;
    // public DataGameSetting DataGameSetting;

    // [Header("Setting level")]
    // [Space(10)]
    public Player ActivePlayer
    {
        get { return Level.listPlayer[Level.activePlayer]; }
        set
        {
            Level.listPlayer[Level.activePlayer] = value;
        }
    }
    private void Start()
    {
        DefaultSettings = Level;
    }

    // public LevelManager()
    // {
    //     GameModeData = new DataGameMode();
    // }

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

    public void Init()
    {
        TypePlayers.Clear();
        Level = new DataLevel();
        Level = DefaultSettings;
        ConfigGameSettings = ResourceSystem.Instance
            .GetAllAssetsByLabel<ScriptableGameSetting>(Constants.Labels.LABEL_GAMESETTING)
            .First();
        CreateListTypePlayers();
    }

    public void CreateListTypePlayers()
    {

        foreach (var type in ConfigGameSettings.TypesPlayer)
        {
            TypePlayers.Add(new CurrentPlayerType()
            {
                title = type.title.GetLocalizedString(),
                TypePlayer = type.TypePlayer
            });
        };
    }

    public void CreateListPlayer()
    {
        Level = DefaultSettings;
        Level.activePlayer = -1;
        Level.listPlayer = new List<Player>();
        Level.listArea = new List<Area>();

        var noBotType = TypePlayers.Where(t => t.TypePlayer != PlayerType.Bot).ToList();
        var botType = TypePlayers.Find(t => t.TypePlayer == PlayerType.Bot);

        for (int i = 0; i < Level.Settings.countPlayer + Level.Settings.countBot; i++)
        {
            var dataPlayer = new PlayerData()
            {
                id = i,
                color = ConfigGameSettings.colors[i],
                playerType = PlayerType.User
            };

            Player player = new Player(dataPlayer);
            if (Level.Settings.TypeGame == TypeGame.MultipleOneDevice)
            {
                player.StartSetting.TypePlayerItem = TypePlayers[i];
            }
            else
            {
                player.StartSetting.TypePlayerItem = i == 0 ? noBotType.First() : botType;
            }
            player.DataPlayer.playerType = player.StartSetting.TypePlayerItem.TypePlayer;
            Level.listPlayer.Add(player);

            // Area area = new Area();
            // area.idPlayer = player.DataPlayer.id;
            // area.typeGround =
            // Level.listArea.Add(area);
        }

        // for (int i = Level.Settings.countPlayer; i < (Level.Settings.countPlayer + Level.Settings.countBot); i++)
        // {
        //     var dataPlayer = new PlayerData()
        //     {
        //         id = i,
        //         color = ConfigGameSettings.colors[i],
        //         // playerType = PlayerType.Bot
        //     };

        //     var player = new Player(dataPlayer);
        //     player.StartSetting.TypePlayerItem = botType;
        //     Level.listPlayer.Add(player);
        // }

        // Level.countPlayer = countPlayer;
        // Level.countBot = countBot;
        // Level.activePlayer = -1;

        // for (int i = 0; i < countPlayer; i++)
        // {
        //     var dataPlayer = new PlayerData();
        //     dataPlayer.id = i;
        //     dataPlayer.color = DataGameSetting.colors[i];
        //     dataPlayer.playerType = PlayerType.User;

        //     var player = new Player(dataPlayer);
        //     Level.listPlayer.Add(player);
        // }

        // for (int i = countPlayer; i < (countPlayer + countBot); i++)
        // {
        //     var dataPlayer = new PlayerData();
        //     dataPlayer.id = i;
        //     dataPlayer.color = colors[i];
        //     dataPlayer.playerType = PlayerType.Bot;
        //     var player = new Player(dataPlayer);

        //     Level.listPlayer.Add(player);
        // }
    }
    public void NewLevel()
    {
        // Level.countPlayer = countPlayer;
        // Level.countBot = countBot;
        // Level.activePlayer = -1;

        // for (int i = 0; i < countPlayer; i++)
        // {
        //     var dataPlayer = new PlayerData();
        //     dataPlayer.id = i;
        //     dataPlayer.color = DataGameSetting.colors[i];
        //     dataPlayer.playerType = PlayerType.User;

        //     var player = new Player(dataPlayer);
        //     Level.listPlayer.Add(player);
        // }

        // for (int i = countPlayer; i < (countPlayer + countBot); i++)
        // {
        //     var dataPlayer = new PlayerData();
        //     dataPlayer.id = i;
        //     dataPlayer.color = colors[i];
        //     dataPlayer.playerType = PlayerType.Bot;
        //     var player = new Player(dataPlayer);

        //     Level.listPlayer.Add(player);
        // }

    }

    public async void StepNextPlayer()
    {
        if (Level.activePlayer < (Level.Settings.countPlayer + Level.Settings.countBot - 1))
        {
            Level.activePlayer++;
        }
        else
        {
            Level.activePlayer = 0;
        }

        //level.activePlayer = level.activePlayer < (countPlayer + countEnemies + 1) ? level.activePlayer++ : 0;
        // GameManager.Instance.MapManager.ResetSky(ActivePlayer.DataPlayer.nosky);

        if (ActivePlayer.ActiveHero == null)
        {
            //GetActivePlayer().SetActiveHero(GetActivePlayer().GetActiveHero());
            // ActivePlayer.ActiveHero = ActivePlayer.DataPlayer.PlayerDataReferences.ListHero[0];
            if (ActivePlayer.DataPlayer.PlayerDataReferences.ListHero.Count > 0)
                ActivePlayer.DataPlayer.PlayerDataReferences.ListHero[0].SetHeroAsActive();
        }
        else
        {
            ActivePlayer.ActiveHero.SetHeroAsActive();
        }

        if (ActivePlayer.DataPlayer.playerType == PlayerType.Bot)
        {
            await ActivePlayer.RunBot();
        }

        // if (ActivePlayer.DataPlayer.nosky.Count == 0)
        // {
        //     List<GridTileNode> listNoskyNode = GameManager.Instance
        //         .MapManager.DrawSky(ActivePlayer.ActiveHero.OccupiedNode, 5);
        //     ActivePlayer.SetNosky(listNoskyNode);
        // }

        // //Debug.Log($" Active Hero {level.activePlayer}");
    }

    public void AddArea(int id, TileLandscape landscape, int idPlayer = -1)
    {
        Area area = new Area();
        area.id = id;
        area.idPlayer = idPlayer;
        area.typeGround = landscape.typeGround;
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
        return id >= Level.listPlayer.Count ? null : Level.listPlayer.Find(t => t.DataPlayer.id == id);
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

    public void LoadLevel(DataPlay dataPlay, DataGame dataGame)
    {
        Level.GameModeData = dataGame.dataMap.GameModeData;

        Level = new DataLevel();
        Level = dataPlay.Level;
        Level.Settings.countPlayer = dataPlay.Level.Settings.countPlayer;
        for (int i = 0; i < dataPlay.Level.Settings.countPlayer; i++)
        {
            var data = dataPlay.Level.listPlayer[i];
            var player = new Player(dataPlay.Level.listPlayer[i].DataPlayer);
            Level.listPlayer.Add(player);
        }

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

    // public void LoadDataGame(DataGame data)
    // {
    //     GameModeData = data.dataMap.GameModeData;
    // }

    public void SaveDataGame(ref DataGame data)
    {
        data.dataMap.GameModeData = Level.GameModeData;
    }
}
