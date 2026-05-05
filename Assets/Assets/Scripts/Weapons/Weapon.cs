using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public virtual void StartUse() { }
    public virtual void ReleaseUse() { }
    public virtual void Tick() { }   
}