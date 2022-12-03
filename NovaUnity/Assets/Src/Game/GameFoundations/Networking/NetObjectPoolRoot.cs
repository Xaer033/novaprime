using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class NetObjectPoolRoot : INetworkObjectPool
{
	private Dictionary<object, NetObjectPool> _poolsByPrefab = new Dictionary<object, NetObjectPool>();
	private Dictionary<NetworkObject, NetObjectPool> _poolsByInstance = new Dictionary<NetworkObject, NetObjectPool>();

	private readonly Transform _poolParent;
	
	public NetObjectPoolRoot(Transform poolParent)
	{
		_poolParent = poolParent;
	}
	
	public NetObjectPool GetPool<T>(T prefab) where T : NetworkObject
	{
		NetObjectPool pool;
		if (!_poolsByPrefab.TryGetValue(prefab, out pool))
		{
			pool = new NetObjectPool();
			_poolsByPrefab[prefab] = pool;
		}

		return pool;
	}

	public NetworkObject AcquireInstance(NetworkRunner runner, NetworkPrefabInfo info)
	{
		NetworkObject prefab;
		foreach (var pair in NetworkProjectConfig.Global.PrefabTable.GetEntries())
		{
			Debug.Log($"{pair.Item1}|{pair.Item2}");
		}
		
		if (NetworkProjectConfig.Global.PrefabTable.TryGetPrefab(info.Prefab, out prefab))
		{
			NetObjectPool pool = GetPool(prefab);
			NetworkObject newt = pool.GetFromPool(Vector3.zero, Quaternion.identity);

			if (newt == null)
			{
				newt = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity);
				_poolsByInstance[newt] = pool;
			}

			newt.gameObject.SetActive(true);
			return newt;
		}

		Debug.LogError("No prefab for " + info.Prefab);
		return null;
	}

	public void ReleaseInstance(NetworkRunner runner, NetworkObject no, bool isSceneObject)
	{
		Debug.Log($"Releasing {no} instance, isSceneObject={isSceneObject}");
		if (no != null)
		{
			NetObjectPool pool;
			if (_poolsByInstance.TryGetValue(no, out pool))
			{
				pool.ReturnToPool(no);
				no.gameObject.SetActive(false); // Should always disable before re-parenting, or we will dirty it twice
				no.transform.SetParent(_poolParent, false);
			}
			else
			{
				no.gameObject.SetActive(false); // Should always disable before re-parenting, or we will dirty it twice
				no.transform.SetParent(null, false);
				GameObject.Destroy(no.gameObject);
			}
		}
	}

	public void ClearPools()
	{
		foreach (NetObjectPool pool in _poolsByPrefab.Values)
		{
			pool.Clear();
		}

		foreach (NetObjectPool pool in _poolsByInstance.Values)
		{
			pool.Clear();
		}

		_poolsByPrefab = new Dictionary<object, NetObjectPool>();
	}
}
