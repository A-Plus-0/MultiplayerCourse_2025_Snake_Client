using System;
using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    [SerializeField] private LayerMask _collisionLayer;
    [SerializeField] private float _overlapRadius = .5f;
    [SerializeField] private float _rotateSpeed = 90f;
    private Transform _snakeHead;
    private Vector3 _targetDirection = Vector3.zero;
    private float _speed;

    public void Init(Transform snakeHead, float speed)
    {
        _snakeHead = snakeHead;
        _speed = speed;
    }
    void Update()
    {
        Rotate();
        Move();
        CheckExit();
    }

    private void FixedUpdate()
    {
        CheckCollision();
    }

    private void CheckCollision()
    {
        Collider[] colliders = Physics.OverlapSphere(_snakeHead.position, _overlapRadius, _collisionLayer);

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].TryGetComponent(out Apple apple))
            {
                apple.Collect();
            }
            else
            {
                if (colliders[i].GetComponentInParent<Snake>())
                {
                    Transform transformEnemy = colliders[i].transform;
                    float playerAngle = Vector3.Angle(transformEnemy.position - _snakeHead.position, transform.forward);
                    float enemyAngle = Vector3.Angle(_snakeHead.position - transformEnemy.position, transformEnemy.forward);
                    if (playerAngle < enemyAngle + 5)
                    {
                        GameOver();
                    }
                }
                else
                {
                    GameOver();
                }
            }
        }
    }

    private void GameOver()
    {
        FindObjectOfType<Controller>().Destroy();
        Destroy(gameObject);
    }

    private void Rotate()
    {
        Quaternion targetRotation = Quaternion.LookRotation(_targetDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * _rotateSpeed);
    }

    private void Move()
    {
        transform.position += transform.forward * _speed * Time.deltaTime;
    }

    private void CheckExit()
    {
        if (Math.Abs(_snakeHead.position.x) > 128 || Math.Abs(_snakeHead.position.y) > 128) GameOver();
    }

    public void SetTargetDirection(Vector3 pointToLook)
    {
        _targetDirection = pointToLook - transform.position;
    }

    public void GetMoveInfo(out Vector3 position)
    {
        position = transform.position;
    }
}
