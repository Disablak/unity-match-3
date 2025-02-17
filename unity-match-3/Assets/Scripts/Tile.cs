using System;
using UnityEngine;
using UnityEngine.Pool;


public class Tile : MonoBehaviour
{
	[SerializeField, Range(0f, 1f)]
	private float disappearDuration = 0.25f;

	[SerializeField] private Sprite[] sprTiles;
	[SerializeField] private SpriteRenderer sprRenderer;

	private IObjectPool<Tile> _pool;
	private float _disappearProgress;
	private FallingState _fallingState;

	public Tile Spawn(Vector3 pos, TileState state)
	{
		_pool ??= new ObjectPool<Tile>(() => Instantiate(this),
			tile => tile.gameObject.SetActive(true),
			tile => tile.gameObject.SetActive(false));

		Tile instance = _pool.Get();
		instance.sprRenderer.sprite = sprTiles[(int)state - 1];
		instance._pool = _pool;
		instance.transform.localPosition = pos;
		instance.transform.localScale = Vector3.one;
		instance._disappearProgress = -1f;
		instance._fallingState.progress = -1f;
		instance.enabled = false;
		return instance;
	}

	public float Disappear()
	{
		_disappearProgress = 0f;
		enabled = true;
		return disappearDuration;
	}

	public void Despawn()
	{
		_pool.Release(this);
	}

	public float Fall(float toY, float speed)
	{
		_fallingState.fromY = transform.localPosition.y;
		_fallingState.toY = toY;
		_fallingState.duration = (_fallingState.fromY - toY) / speed;
		_fallingState.progress = 0f;
		enabled = true;
		return _fallingState.duration;
	}

	private void Update()
	{
		if (_disappearProgress >= 0f)
		{
			_disappearProgress += Time.deltaTime;
			if (_disappearProgress >= disappearDuration)
			{
				Despawn();
				return;
			}
			transform.localScale = Vector3.one * (1f - _disappearProgress / disappearDuration);
		}

		if (_fallingState.progress >= 0f)
		{
			Vector3 position = transform.localPosition;
			_fallingState.progress += Time.deltaTime;
			if (_fallingState.progress >= _fallingState.duration)
			{
				_fallingState.progress = -1f;
				position.y = _fallingState.toY;
				enabled = _disappearProgress >= 0f;
			}else
			{
				position.y = Mathf.Lerp(_fallingState.fromY, _fallingState.toY, _fallingState.progress / _fallingState.duration);
			}

			transform.localPosition = position;
		}
	}


	[System.Serializable]
	private struct FallingState
	{
		public float fromY;
		public float toY;
		public float duration;
		public float progress;
	}
}