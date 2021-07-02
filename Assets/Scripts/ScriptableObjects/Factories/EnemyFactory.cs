using UnityEngine;

[CreateAssetMenu(menuName = "Factories/EnemyFactory")]
public class EnemyFactory : GameObjectFactory
{
    [SerializeField] private Enemy prefab;

    [FloatRangeSlider(0.5f, 2f)] 
    [SerializeField] private FloatRange _scale = new FloatRange(1f);
    [FloatRangeSlider(-0.4f, 0.4f)]
    [SerializeField] private FloatRange _offset;
    [FloatRangeSlider(1f, 10f)]
    [SerializeField] private FloatRange _speed;

    public Enemy Get()
    {
        Enemy instance = CreateGameObjectInstance(prefab);
        instance.Init(_scale.RandomValueInRange, _speed.RandomValueInRange, this, _offset.RandomValueInRange);
        return instance;
    }

    public void Reclaim(Enemy enemy)
    {
        Destroy(enemy.gameObject);
    }
}
