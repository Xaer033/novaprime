﻿using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MachineGunView : MonoBehaviour
{
    private const int kFXPoolSize = 50;
    
    public LayerMask targetLayerMask;
    
    public float bulletSpeed;
    public float fireCooldown;


    // (This should all be in a weapon class)
    public GameObject _bulletFXPrefab;
    private TrailRenderer[] _fxPool;
    private Tween[] _fxTweens;
    private int _fxIndex;
    
    public Transform barrelHook;
    
    void Awake ()
    {
        setupFXPool();
    }
    
    public void Fire(Vector3 target, float speedx)
    {
        float speed = bulletSpeed;
        Vector3 startPos = barrelHook.position;

        TrailRenderer fx = getNextFX();
        Tween t = _fxTweens[_fxIndex];

        //fx.Clear();

        fx.transform.position = startPos;
        
        t = fx.transform.DOMove(target, speed);
        t.SetEase(Ease.Linear);
        t.SetSpeedBased(true);

        t.OnStart(() =>
        {
            // fx.time = time;
            fx.transform.position = startPos;
            fx.gameObject.SetActive(true);
            //fx.emitting = (true);
            fx.Clear();
        });

        t.OnComplete(()=>
        {
            //fx.Clear();
            //fx.emitting = (false);
            fx.gameObject.SetActive(false);
            t = null;
        });
    }

    private TrailRenderer getNextFX()
    {
        TrailRenderer fx = _fxPool[_fxIndex];
        _fxIndex = (_fxIndex + 1) % _fxPool.Length;
        return fx;
    }

    private void setupFXPool()
    {
        _fxIndex = 0;

        _fxPool = new TrailRenderer[kFXPoolSize];
        _fxTweens = new Tween[kFXPoolSize];

        for(int i = 0; i < kFXPoolSize; ++i)
        {
            GameObject fxObj = GameObject.Instantiate<GameObject>(_bulletFXPrefab, transform.position, transform.rotation);
            _fxPool[i] = fxObj.GetComponent<TrailRenderer>();
            _fxPool[i].gameObject.SetActive(false);
            //_fxPool[i].emitting = (false);
        }
    }
}