using UnityEngine;

//[CreateAssetMenu(fileName = "BulletEffect", menuName = "Scriptable Objects/BulletEffect")]
public abstract class BulletEffect : ScriptableObject
{
    public string effectName;
}

public interface IOnHitEffect
{
    void OnHit(Bullet bullet, RaycastHit hit);
}

public interface IOnFireEffect
{
    void OnFire(Bullet bullet);
}

public interface IOnExpireEffect
{
    public void OnExpire(Bullet bullet);
}

public interface IOnUpdateEffect
{
    void OnUpdate(Bullet bullet, float time);
}

