using UnityEngine;

public class Snake : MonoBehaviour
{
    public float Speed { get { return _speed; } }

    [SerializeField] private int _playerLayer = 6;
    [SerializeField] private Tail _tailPrefab;
    [field: SerializeField] public Transform _head { get; private set; }
    [SerializeField] private float _speed = 2;
    private Tail _tail;

    public void Init(int detailCount, bool isPlayer = false)
    {

        if (isPlayer)
        {
            gameObject.layer = _playerLayer;
            var children = GetComponentsInChildren<Transform>();
            for (int i = 0; i < children.Length; i++)
            {
                children[i].gameObject.layer = _playerLayer;
            }
        }
        _tail = Instantiate(_tailPrefab, transform.position, Quaternion.identity);
        _tail.Init(_head, _speed, detailCount, _playerLayer, isPlayer);

    }
    public void SetDetailCount(int detailCount)
    {
        _tail.SetDetailCount(detailCount);
    }

    public void Destroy(string clientID)
    {
        var detailPositions = _tail.GetDetailPosition();
        detailPositions.id = clientID;
        string json = JsonUtility.ToJson(detailPositions);
        MultiplayerManager.Instance.SendMessage("gameOver", json); 
        _tail.Destroy();
        Destroy(gameObject);
    }

    void Update()
    {
        Move();
    }

    private void Move()
    {
        transform.position += _head.forward * Time.deltaTime * _speed;
    }
    public void SetRotation(Vector3 pointToLook)
    {
        _head.LookAt(pointToLook);
    }

}
