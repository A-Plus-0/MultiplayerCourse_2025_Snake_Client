using System.Collections.Generic;
using UnityEngine;
using Colyseus;
using Unity.VisualScripting;

public class MultiplayerManager : ColyseusManager<MultiplayerManager>
{
    #region Server

    private const string GameRoomName = "state_handler";

    private ColyseusRoom<State> _room;
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        InitializeClient();

        Connection();
    }

    private async void Connection()
    {
        Dictionary<string, object> data = new Dictionary<string, object>()
        {
            {"skins", _skins.length }
        };
        _room = await client.JoinOrCreate<State>(GameRoomName, data);
        _room.OnStateChange += OnChange;
    }

    private void OnChange(State state, bool isFirstState)
    {
        if (isFirstState == false) return;
        _room.OnStateChange -= OnChange;

        state.players.ForEach((key, player) =>
        {
            if (key == _room.SessionId) CreatePlayer(player);
            else CreateEnemy(key, player);
        });

        _room.State.players.OnAdd += CreateEnemy;
        _room.State.players.OnRemove += RemoveEnemy;
    }

    protected override void OnApplicationQuit()
    {
        base.OnApplicationQuit();
        LeaveRoom();
    }

    public void LeaveRoom()
    {
        _room?.Leave();
    }

    public void SendMessage(string key, Dictionary<string, object> data)
    {
        _room.Send(key, data);
    }

    #endregion

    [SerializeField] Skins _skins;

    #region Player
    [SerializeField] private PlayerAim _playerAim;
    [SerializeField] private Controller _controllerPrefab;
    [SerializeField] private Snake _snakePrefab;
    private void CreatePlayer(Player player)
    {
        Vector3 position = new Vector3(player.x, 0, player.z);
        Quaternion quaternion = Quaternion.identity;


        Snake snake = Instantiate(_snakePrefab, position, quaternion);
        snake.Init(player.d, _skins.GetMaterial(player.skin));

        PlayerAim aim = Instantiate(_playerAim, position, quaternion);
        aim.Init(snake.Speed);

        Controller controller = Instantiate(_controllerPrefab);
        controller.Init(aim, player, snake);
    }

    #endregion

    #region Enemy
    Dictionary<string, EnemyController> _enemys = new Dictionary<string, EnemyController>();

    private void CreateEnemy(string key, Player player)
    {
        Vector3 position = new Vector3(player.x, 0, player.z);

        Snake snake = Instantiate(_snakePrefab, position, Quaternion.identity);
        snake.Init(player.d, _skins.GetMaterial(player.skin));

        EnemyController enemyController = snake.AddComponent<EnemyController>();
        enemyController.Init(player, snake);

        _enemys.Add(key, enemyController);
    }
    private void RemoveEnemy(string key, Player value)
    {
        if (_enemys.ContainsKey(key) == false)
        {
            Debug.LogError("Попытка уничтожить врага, которого не было в словаре");
            return;
        }
        EnemyController enemy = _enemys[key];
        _enemys.Remove(key);
        enemy.Destroy();
    }



    #endregion

}
