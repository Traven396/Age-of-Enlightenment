using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEntity
{
    void ApplyMotion(Vector3 force, ForceMode forceMode);
}
