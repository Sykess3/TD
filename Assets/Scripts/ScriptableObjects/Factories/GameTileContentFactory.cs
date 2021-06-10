using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Factories/GameTileContentFactory")]
public class GameTileContentFactory : GameObjectFactory
{
    [SerializeField] private GameTileContent _emptyPrefab;
    [SerializeField] private GameTileContent _destinationPrefab;
    [SerializeField] private GameTileContent _wallPrefab;
    [SerializeField] private GameTileContent _spawnPointPrefab;

    private Scene _contentScene;

    public void Reclaim(GameTileContent content)
    {
        Destroy(content.gameObject);
    }

    public GameTileContent Get(GameTileContentType type)
    {
        switch (type)
        {
            case GameTileContentType.Destination:
                return InternalGet(_destinationPrefab);
            case GameTileContentType.Empty:
                return InternalGet(_emptyPrefab);
            case GameTileContentType.Wall:
                return InternalGet(_wallPrefab);
            case GameTileContentType.SpawnPoint:
                return InternalGet(_spawnPointPrefab);
        }
        return null;
    }

    private GameTileContent InternalGet(GameTileContent prefab)
    {
        var instance = CreateGameObjectInstance(prefab);
        instance.OriginFactory = this;
        return instance;
    }
    
}
