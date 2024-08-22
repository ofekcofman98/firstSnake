using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Diagnostics;

public class PlayerController : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] private GameObject _snakePartsPrefab;
    // [SerializeField] private GameManager _gameManager;
    [SerializeField] private float _stepTimeout = 0.5f;
    private float _stepTimer = 0;
    private Vector3 _recentPlayerMovement;
    private List<GameObject> _snakeParts = new();

    public static PlayerController Instance {get; private set;}

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        var initialSnakePart = Instantiate(_snakePartsPrefab, Vector3.zero, Quaternion.identity);
        _snakeParts.Add(initialSnakePart);
    }
    void Update()
    {
        if (!GameManager.Instance.IsGameRunning) return;

        var playerInput = GetInputOnUpdate();
        if(playerInput != Vector3.zero)
        {
            _recentPlayerMovement = playerInput;
        }

        _stepTimer += Time.deltaTime;
        if (_stepTimer >= _stepTimeout)
        {
            if (_recentPlayerMovement == Vector3.zero)
            {
                return;
            }

            var newSnakeHeadPosition = _snakeParts.Last().transform.position + _recentPlayerMovement;
            var newSnakeHead = Instantiate(_snakePartsPrefab, newSnakeHeadPosition, Quaternion.identity);
            _snakeParts.Add(newSnakeHead);

            var contactedWall = GameManager.Instance.IsPositionOnWall(newSnakeHeadPosition);
            if (contactedWall)
            {
                GameManager.Instance.GameOver();
                return;
            }


            var foundFood = GameManager.Instance.IsThereFoodInPosition(newSnakeHead.transform.position);
            if(foundFood)
            {
                GameManager.Instance.EatFood();
            }

            if(!foundFood)
            {
                var snakeTail = _snakeParts.First();
                _snakeParts.Remove(snakeTail);
                Destroy(snakeTail);
            }

            _stepTimer = 0;
        }
    }

    private Vector3 GetInputOnUpdate()
    {
        var movementDirection = new Vector3();
        if(Input.GetKeyDown(KeyCode.RightArrow) && _recentPlayerMovement != Vector3.left)
        {
            movementDirection = Vector3.right;
        }
        if(Input.GetKeyDown(KeyCode.LeftArrow) && _recentPlayerMovement != Vector3.right)
        {
            movementDirection = Vector3.left;
        }
        if(Input.GetKeyDown(KeyCode.DownArrow) && _recentPlayerMovement != Vector3.up)
        {
            movementDirection = Vector3.down;
        }
        if(Input.GetKeyDown(KeyCode.UpArrow) && _recentPlayerMovement != Vector3.down)
        {
            movementDirection = Vector3.up;
        }
    
        return movementDirection;
    }

    public bool IsPositionOnSnake(Vector3 position)
    {
        return _snakeParts.Any(snakePart =>
            Mathf.Approximately(snakePart.transform.position.x, position.x) &&
            Mathf.Approximately(snakePart.transform.position.y, position.y));
    }


}
