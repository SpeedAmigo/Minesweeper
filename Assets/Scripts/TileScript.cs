using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    [SerializeField] private  Sprite unclickedTile;
    [SerializeField] private Sprite flaggedTile;
    [SerializeField] private Sprite mineTile;
    [SerializeField] private Sprite mineWrongTile;
    [SerializeField] private Sprite mineHitTile;
    public List<Sprite> clickedTiles;
    
    public SpriteRenderer _spriteRenderer;
    
    private bool _clicked;
    
    public bool flagged = false;
    public bool active = true;
    public bool isMine = false;
    public int mineCount = 0;

    public void MouseLeftClick()
    {
        if (active)
        {
            if (MinesweeperManager.Instance.isStarted == false)
            {
                MinesweeperManager.Instance.GameStarter(new Vector2Int((int)transform.position.x, (int)transform.position.y));
            }

            if (MinesweeperManager.Instance.isStarted)
            {
                ClickedTile();
            }
        }
    }

    public void MouseRightClick()
    {
        if (active)
        {
            if (!flagged && MinesweeperManager.Instance.MinesCount > 0)
            {
                flagged = true;
                _spriteRenderer.sprite = flaggedTile;
                MinesweeperManager.Instance.MinesCount--;
                MinesweeperManager.Instance.AddFlaggedTileToList(new Vector2Int((int)transform.position.x, (int)transform.position.y));
            }
            else if (flagged)
            {
                flagged = false;
                _spriteRenderer.sprite = unclickedTile;
                MinesweeperManager.Instance.MinesCount++;
                MinesweeperManager.Instance.RemoveFlaggedTileFromList(new Vector2Int((int)transform.position.x, (int)transform.position.y));
            }
        }
    }
    
    private void ClickedTile()
    {
        if (active && !flagged)
        {
            active = false;
            if (isMine)
            {
                MinesweeperManager.Instance.GameOver();
                _spriteRenderer.sprite = mineHitTile;
            }
            else
            {
                if (mineCount == 0)
                {
                    Vector2Int gridPosition = new Vector2Int((int)transform.position.x, (int)transform.position.y);
                    
                    MinesweeperManager.Instance.DeactivateEmpty(gridPosition);
                }
                _spriteRenderer.sprite = clickedTiles[mineCount];
                MinesweeperManager.Instance.UpdateTileState();
            }
        }
    }

    private void UncoverTile(bool gameWon)
    {
        if (!active) return;

        if (gameWon)
        {
            _spriteRenderer.sprite = isMine ? flaggedTile : clickedTiles[mineCount];
        }
        else
        {
            _spriteRenderer.sprite = isMine ? mineTile : clickedTiles[mineCount];
        }
        
        active = false;
    }

    private void ResetTile()
    {
        active = true;
        isMine = false;
        mineCount = 0;
        flagged = false;
        _spriteRenderer.sprite = unclickedTile;
    }
    
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = unclickedTile;
    }

    private void OnEnable()
    {
        MinesweeperManager.OnRestartEvent += ResetTile;
        MinesweeperManager.OnTileUncoverEvent += UncoverTile;
    }

    private void OnDisable()
    {
        MinesweeperManager.OnRestartEvent -= ResetTile;
        MinesweeperManager.OnTileUncoverEvent -= UncoverTile;
    }
}
