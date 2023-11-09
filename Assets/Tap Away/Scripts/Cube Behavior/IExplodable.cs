using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IExplodable
{
    void Explode(Vector3 hitPoint, float explodeForce, float explodeRadius);
}
