using UnityEngine;

[SelectionBase]
public class GameTileContent : MonoBehaviour
{
    [SerializeField] private GameTileContentType _type;

    private GameTileContentFactory _originFactory;

    public GameTileContentType Type => _type;
    public bool IsBlockingPath => Type == GameTileContentType.Tower || Type == GameTileContentType.Wall;

    public void Init(GameTileContentFactory originFactory)
    {
        _originFactory = originFactory;
    }

    public void Recycle()
    {
        _originFactory.Reclaim(this);
    }

}

public enum GameTileContentType
{
    Empty,
    Destination,
    Wall,
    SpawnPoint,
    Tower
}
