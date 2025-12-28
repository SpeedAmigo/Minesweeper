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
            flagged = !flagged;

            if (flagged)
            {
                _spriteRenderer.sprite = flaggedTile;
            }
            else
            {
                _spriteRenderer.sprite = unclickedTile;
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

    private void UncoverTile()
    {
        if (!active) return;
        
        active = false;
        _spriteRenderer.sprite = isMine ? mineTile : clickedTiles[mineCount];
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
