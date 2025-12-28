using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;

public class MinesweeperManager : MonoBehaviour
{ 
    public static MinesweeperManager Instance;
    
    public static event Action OnRestartEvent;
    public static event Action<int> OnBombCountEvent;

    public static event Action OnTileUncoverEvent;
    
    [Header("Dependencies")]
    [SerializeField] private Transform tilePrefab;
    [SerializeField] private Transform gridField;
    
    [Header("Settings")]
    [SerializeField] private MinesweeperSize minesweeperSize;
    [SerializeField] private Vector2Int safeRegionSize;
    
    public bool isStarted = false;
    
    private Timer timer;
    
    private List<TileScript> _tilesList = new();
    private readonly List<TileScript> _minesList = new();
    private readonly HashSet<TileScript> _visitedTiles = new();
    private Dictionary<Vector2Int, TileScript> _tileDictionary = new();
    private BoardSize _boardSize;
    
    #region Helpers
    private BoardSize GetBoardSize(MinesweeperSize size)
    {
        switch (size)
        {
            case MinesweeperSize.Small:
                return new BoardSize(9, 9, 12);
            case MinesweeperSize.Medium:
                return new BoardSize(16, 16, 40);
            case MinesweeperSize.Large:
                return new BoardSize(24, 24, 99);
            default:
                throw new ArgumentException("Invalid Minesweeper size");
        }
    }
    
    private readonly List<Vector2Int> _directions = new()
    {
        new Vector2Int(0, 1), // up
        new Vector2Int(0, -1), // down
        new Vector2Int(-1, 0), // left
        new Vector2Int(1, 0), // right
        new Vector2Int(-1, 1), // up-left
        new Vector2Int(1, 1), // up-right
        new Vector2Int(-1, -1), // down-left
        new Vector2Int(1, -1), // down-right
    };
    
    private HashSet<Vector2Int> GenerateSafeArea(Vector2Int start, int minSize, int maxSize)
    {
        HashSet<Vector2Int> safeArea = new();
        Queue<Vector2Int> queue = new();

        int targetSize = Random.Range(minSize, maxSize + 1);

        queue.Enqueue(start);
        safeArea.Add(start);

        while (queue.Count > 0 && safeArea.Count < targetSize)
        {
            Vector2Int current = queue.Dequeue();
            
            var shuffledDirs = _directions.OrderBy(_ => Random.value);

            foreach (var dir in shuffledDirs)
            {
                Vector2Int next = current + dir;

                if (!_tileDictionary.ContainsKey(next)) continue;
                if (safeArea.Contains(next)) continue;

                safeArea.Add(next);
                queue.Enqueue(next);

                if (safeArea.Count >= targetSize)
                    break;
            }
        }

        return safeArea;
    }
    
    private List<TileScript> ListUpdate()
    {
        List<TileScript> updatedTilesList = _tilesList.Except(_minesList).ToList();
        
        return updatedTilesList;
    }
    
    #endregion

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        timer = GetComponent<Timer>();
        _boardSize = GetBoardSize(minesweeperSize);
        CreateBoard(_boardSize.width, _boardSize.height);
    }
    
    public void GameStarter(Vector2Int gridPosition)
    {
        var safeArea = GenerateSafeArea(
            gridPosition,
            minSize: safeRegionSize.x,
            maxSize: safeRegionSize.y
        );

        PlaceMines(_boardSize.mines, safeArea);
        
        foreach (var pos in safeArea)
        {
            if (_tileDictionary[pos].mineCount == 0)
                DeactivateEmpty(pos);
        }
        
        timer.StartTimer();
    }
    
    private void CreateBoard(int width, int height)
    {
        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                Transform tileTransform = Instantiate(tilePrefab, gridField, true);
                
                float xIndex = col - ((width - 1) / 2f);
                float yIndex = row - ((height - 1) / 2f);
                tileTransform.localPosition = new Vector2(xIndex, yIndex);
                Vector2Int gridPosition = new Vector2Int((int)xIndex, (int)yIndex);
                
                TileScript tileScript = tileTransform.GetComponent<TileScript>();
                _tileDictionary[gridPosition] = tileScript;
                _tilesList.Add(tileScript);
            }
        }
        
        OnBombCountEvent?.Invoke(_boardSize.mines);
    }
    
    public void GameRestart()
    {
        OnRestartEvent?.Invoke();
        
        isStarted = false;
        _visitedTiles.Clear();
        timer.ResetTimer();
        timer.StopTimer();
    }

    public void GameOver()
    {
        timer.StopTimer();
        OnTileUncoverEvent?.Invoke();   
    }
    
    private void PlaceMines(int mineCount, HashSet<Vector2Int> safeArea)
    {
        List<Vector2Int> availablePositions = new List<Vector2Int>(_tileDictionary.Keys);
        
        foreach (var pos in safeArea)
        {
            availablePositions.Remove(pos);

            foreach (var dir in _directions)
                availablePositions.Remove(pos + dir);
        }
        
        for (int i = 0; i < mineCount; i++)
        {
            int randomIndex = Random.Range(0, availablePositions.Count);
            Vector2Int randomPosition = availablePositions[randomIndex];
            
            TileScript mineTile = _tileDictionary[randomPosition];
            mineTile.isMine = true;
            _minesList.Add(mineTile);
            
            availablePositions.RemoveAt(randomIndex);
            
            GetNeighbours(randomPosition);
            ListUpdate();
        }
    }
    
    private void GetNeighbours(Vector2Int minePosition)
    {
        foreach (Vector2Int dir in _directions)
        {
            Vector2Int neighborPos = minePosition + dir;

            if (_tileDictionary.TryGetValue(neighborPos, out TileScript neighborScript))
            {
                if (!neighborScript.isMine)
                {
                    neighborScript.mineCount++;
                }
            }
        }
        isStarted = true;
    }
    
    public void DeactivateEmpty(Vector2Int gridPosition)
    {
        if (_visitedTiles.Contains(_tileDictionary[gridPosition])) return;
        
        TileScript currentTile = _tileDictionary[gridPosition];
        _visitedTiles.Add(currentTile);
        
        currentTile._spriteRenderer.sprite = currentTile.clickedTiles[currentTile.mineCount];
        currentTile.active = false;
        
        foreach (Vector2Int dir in _directions)
        {
            Vector2Int neighborPos = gridPosition + dir;

            if (_tileDictionary.TryGetValue(neighborPos, out TileScript neighborScript))
            {
                if (neighborScript.mineCount == 0 && !_visitedTiles.Contains(neighborScript))
                {
                    DeactivateEmpty(neighborPos);
                }
                else if (!neighborScript.isMine)
                {
                    neighborScript._spriteRenderer.sprite = neighborScript.clickedTiles[neighborScript.mineCount];
                    neighborScript.active = false;
                }
            }
        }
    }
    
    public void UpdateTileState()
    {
        bool stateCheck = ListUpdate().All(tileScript => tileScript.active == false);

        if (stateCheck)
        {
            Debug.Log("Game Won!");
            timer.StopTimer();
        }
    }
}

public enum MinesweeperSize
{
    Small,
    Medium,
    Large
}