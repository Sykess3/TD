using System.Collections.Generic;

[System.Serializable]
public class EnemyCollection
{
    private List<Enemy> _enemies = new List<Enemy>();

    public void Add(Enemy enemy)
    {
        _enemies.Add(enemy);
        enemy.Dying += Remove;
    }

    public void RemoveAt(int index)
    {
        int lastIndex = _enemies.Count - 1;
        _enemies[index] = _enemies[lastIndex];
        _enemies.RemoveAt(lastIndex);
    }

    public void Remove(Enemy enemy)
    {
        _enemies.Remove(enemy);
    }

    public void MoveAll()
    {
        for (int i = 0; i < _enemies.Count; i++)
        {
            if (!_enemies[i].TryMove())
            {
                RemoveAt(i);
                i--;
            }
        }
    }
    
}