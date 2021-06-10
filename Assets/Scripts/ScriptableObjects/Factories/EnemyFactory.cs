using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

[CreateAssetMenu(menuName = "Factories/EnemyFactory")]
public class EnemyFactory : GameObjectFactory
{
    [SerializeField] private Enemy prefab;

    public Enemy Get()
    {
        Enemy instance = CreateGameObjectInstance(prefab);
        instance.OriginFactory = this;
        return instance;
    }

    public void Reclaim(Enemy enemy)
    {
        Destroy(enemy.gameObject);
    }
}
