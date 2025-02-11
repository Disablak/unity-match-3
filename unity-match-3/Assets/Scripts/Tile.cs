using System;
using UnityEngine;


public class Tile : MonoBehaviour
{
	[SerializeField, Range(0f, 1f)]
	private float disappearDuration = 0.25f;

	private PrefabInstancePool<Tile> _pool;
	private float disappearProgress;

	public Tile Spawn(Vector3 pos)
	{
		Tile instance = _pool.GetInstance(this);
		instance._pool = _pool;
		instance.transform.localPosition = pos;
		instance.transform.localScale = Vector3.one;
		disappearProgress = -1f;
		instance.enabled = false;
		return instance;
	}

	public float Disappear()
	{
		disappearProgress = 0f;
		enabled = true;
		return disappearDuration;
	}

	public void Despawn()
	{
		_pool.Recycle(this);
	}

	private void Update()
	{
		if (disappearProgress >= 0f)
		{
			disappearProgress += Time.deltaTime;
			if (disappearProgress >= disappearDuration)
			{
				Despawn();
				return;
			}
			transform.localScale = Vector3.one * (1f - disappearProgress / disappearDuration);
		}
	}
}