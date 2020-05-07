using UnityEngine;

public class MachineGunView : MonoBehaviour, IWeaponView
{
    private const int kFXPoolSize = 50;
    
    // (This should all be in a weapon class)
    public GameObject _bulletFXPrefab;
    
    public Transform _barrelHook;
    
    public MachineGunController controller { get; set; }
    
    public void OnTimeWarpEnter(float timeScale)
    {
        if (controller != null)
        {
            controller.OnTimeWarpEnter(timeScale);
        }
    }

    public void OnTimeWarpExit()
    {
        if (controller != null)
        {
            controller.OnTimeWarpExit();
        }
    }
    
    public Transform barrelHook
    {
        get { return _barrelHook; }
    }
    
    public void Fire(Vector3 target, float speedx)
    {
//        float speed = bulletSpeed;
//        Vector3 startPos = barrelHook.position;
//
//        TrailRenderer fx = getNextFX();
//        Tween t = _fxTweens[_fxIndex];
//
//        //fx.Clear();
//
//        fx.transform.position = startPos;
//        
//        t = fx.transform.DOMove(target, speed);
//        t.SetEase(Ease.Linear);
//        t.SetSpeedBased(true);
//
//        t.OnStart(() =>
//        {
//            // fx.time = time;
//            fx.transform.position = startPos;
//            fx.transform.LookAt(target, Vector3.up);
//            fx.gameObject.SetActive(true);
//            //fx.emitting = (true);
//            fx.Clear();
//        });
//
//        t.OnComplete(()=>
//        {
//            //fx.Clear();
//            //fx.emitting = (false);
//            fx.gameObject.SetActive(false);
//            t = null;
//        });
    }
}
