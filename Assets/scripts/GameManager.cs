using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int _width = 20;
    [SerializeField] private int _height = 12;
    [SerializeField] private GameObject _wallPrefab;
    [SerializeField] private GameObject _foodPrefab;
    
    public static GameManager Instance { get; private set; }

    private bool _isGameRunning = true;
    public bool IsGameRunning => _isGameRunning;

    private GameObject _food;

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
        createWalls();
        SpawnFood();
    }

    private void Update()
    {
        if(!_isGameRunning)
        {
            if(Input.anyKeyDown)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
            }
        }
    }

    private int RightBorder => (_width / 2) + 1;
    private int LeftBorder => -1 * (_width / 2 + 1);
    private int TopBorder => (_height / 2) + 1;
    private int BottomBorder => -1 * (_height / 2 + 1);


    private void createWalls()
    {
        CreateWall("RightWall",RightBorder, 0, 1, _height+1);
        CreateWall("LeftWall", LeftBorder, 0, 1, _height+1);
        CreateWall("TopWall", 0, TopBorder, _width+3, 1);
        CreateWall("BottomWall", 0, BottomBorder, _width+3, 1);

    }
    
    private void CreateWall(string name, int x, int y, int height, int width)
    {
        var rightWall = Instantiate(_wallPrefab);
        rightWall.name = name;
        rightWall.transform.position = new Vector3(x,y,0);
        rightWall.transform.localScale = new Vector3(height,width,0);

    }

    private void SpawnFood()
    {
        var nextFoodPosition = GetNextFoodPosition();
        _food = Instantiate(_foodPrefab, nextFoodPosition, Quaternion.identity);
    }
    
    public bool IsThereFoodInPosition(Vector3 snakePosition)
    {
        if (_food == null)
            return false;
        
        return _food.transform.position == snakePosition;
    }

    public void EatFood()
    {
        Destroy(_food);
        // _currentScore++;
        // UpdateScoreText();
        SpawnFood();
    }

        private Vector3 GetNextFoodPosition()
    {
        var newPosition = new Vector3(0,0,0);
        var positionValid = false;
        while (!positionValid)
        {
            var x = Random.Range(LeftBorder, RightBorder);
            var y = Random.Range(BottomBorder, TopBorder);
            newPosition = new Vector3(x, y, 0);
            positionValid = !IsPositionOnWall(newPosition) && 
                            !PlayerController.Instance.IsPositionOnSnake(newPosition);
        }
        return newPosition;
    }


    public bool IsPositionOnWall(Vector3 position)
    {
        return (position.x <= LeftBorder || 
                position.x >= RightBorder || 
                position.y >= TopBorder ||
                position.y <= BottomBorder);
    }

    internal void GameOver()
    {
        _isGameRunning = false;
    }
}
