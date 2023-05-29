using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.ResourceManagement.ResourceProviders;

public class LevelManager : Singleton<LevelManager>, ISaveDataPlay, ISaveDataGame
{
    [SerializeField] private Transform _camera;
    public DataLevel Level;
    private DataLevel DefaultSettings;
    public SceneInstance activeScene;
    public GameDate GameDate { get; private set; }
    public SOGameSetting ConfigGameSettings;
    public SOStrenghtMonsters CurrentStrenghtMonsters
        => ConfigGameSettings.StrenghtMonsters.Find(t => t.strenghtMonster == Level.Settings.strenghtMonster);
    //protection_index = monsters_strength_zone + monsters_strength_map
    public SOProtectionIndex CurrentProtectionIndex
        => ConfigGameSettings.ProtectionIndices
        .Find(t => t.protectionIndex == CurrentStrenghtMonsters.monstersStrengthZone + CurrentStrenghtMonsters.monstersStrengthMap);
    public List<CurrentPlayerType> TypePlayers = new List<CurrentPlayerType>();

    public Player ActivePlayer
    {
        get { return Level.listPlayer.ElementAtOrDefault(Level.activePlayer); }
        set
        {
            Level.listPlayer[Level.activePlayer] = value;
        }
    }
    private void Start()
    {
        DefaultSettings = Level;
    }

    protected override void Awake()
    {
        base.Awake();
        GameManager.OnAfterStateChanged += OnAfterStateChanged;
    }

    private void OnDestroy()
    {
        GameManager.OnAfterStateChanged -= OnAfterStateChanged;
    }

    public void OnAfterStateChanged(GameState newState)
    {
        switch (newState)
        {
            case GameState.NextPlayer:
                StepNextPlayer();
                break;
        }
    }

    public void Init()
    {
        TypePlayers.Clear();
        Level = new DataLevel();
        Level = DefaultSettings;
        ConfigGameSettings = ResourceSystem.Instance
            .GetAllAssetsByLabel<SOGameSetting>(Constants.Labels.LABEL_GAMESETTING)
            .First();
        CreateListTypePlayers();
        GameDate = new GameDate();
        GameDate.Init(0);
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
            Player player = new Player();
            PlayerData dataPlayer = new PlayerData()
            {
                id = i,
                color = ConfigGameSettings.colors[i],
                playerType = PlayerType.User,
                command = 0
            };
            StartSetting startSetting = new StartSetting();

            if (Level.Settings.TypeGame == TypeGame.MultipleOneDevice)
            {
                startSetting.TypePlayerItem = TypePlayers[i];
            }
            else
            {
                startSetting.TypePlayerItem = i == 0 ? noBotType.First() : botType;
            }
            dataPlayer.playerType = startSetting.TypePlayerItem.TypePlayer;

            // Area area = new Area();
            // area.idPlayer = player.DataPlayer.id;
            // area.typeGround =
            // Level.listArea.Add(area);
            player.New(dataPlayer);
            Level.listPlayer.Add(player);
        }
    }

    public async void StepNextPlayer()
    {
        if (Level.activePlayer < (Level.Settings.countPlayer + Level.Settings.countBot - 1))
        {
            Level.activePlayer++;
        }
        else
        {
            GameDate.AddDay();
            Level.activePlayer = 0;
        }

        GameManager.Instance.MapManager.ResetSky(Level.nosky);
        GameManager.Instance.ChangeState(GameState.NextDay);

        if (ActivePlayer.ActiveHero == null)
        {
            if (ActivePlayer.DataPlayer.PlayerDataReferences.ListHero.Count > 0)
                ActivePlayer.DataPlayer.PlayerDataReferences.ListHero[0].SetHeroAsActive();
        }
        else
        {
            ActivePlayer.ActiveHero.SetHeroAsActive();
        }

        if (GameDate.week == 0)
        {
            GameManager.Instance.ChangeState(GameState.NextWeek);
        }

        if (ActivePlayer.DataPlayer.playerType == PlayerType.Bot)
        {
            await ActivePlayer.RunBot();
        }
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

        GameDate = new GameDate();
        GameDate.Init(dataPlay.GameDate.countDay);

        Level = new DataLevel();
        Level = dataPlay.Level;
        Level.Settings.countPlayer = dataPlay.Level.Settings.countPlayer;
        for (int i = 0; i < dataPlay.Level.Settings.countPlayer; i++)
        {
            var data = dataPlay.Level.listPlayer[i];
            var player = new Player();
            player.Load(dataPlay.Level.listPlayer[i].DataPlayer);
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
        data.GameDate = GameDate;
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
