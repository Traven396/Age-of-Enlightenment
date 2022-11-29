using UnityEngine;
public interface ICastable
{
    Transform transform { get; }
    void Cast(Transform target);

}
