using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyFactory OriginFactory { get; set; }

    public void Recycle()
    {
        OriginFactory.Reclaim(this);
    }
}
