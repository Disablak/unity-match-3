using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Pool;


public class Tile : MonoBehaviour
{
	[SerializeField, Range(0f, 1f)]
	private float disappearDuration = 0.25f;

	[SerializeField] private Sprite[] sprTiles;
	[SerializeField] private SpriteRenderer sprRenderer;

	private IObjectPool<Tile> _pool;


	public Tile Spawn(Vector3 pos, TileState state)
	{
		_pool ??= new ObjectPool<Tile>(() => Instantiate(this),
			tile => tile.gameObject.SetActive(true),
			tile => tile.gameObject.SetActive(false));

		Tile instance = _pool.Get();
		instance.sprRenderer.sprite = sprTiles[(int)state - 1];
		instance._pool = _pool;
		instance.transform.localPosition = pos;
		instance.enabled = false;
		return instance;
	}

	public float Disappear()
	{
		enabled = true;
		Vector3 startScale = sprRenderer.transform.localScale;
		sprRenderer.transform.DOScale(Vector3.zero, disappearDuration).OnComplete(() =>
		{
			sprRenderer.transform.localScale = startScale;
			Despawn();
		});
		return disappearDuration;
	}

	public void Despawn()
	{
		_pool.Release(this);
	}

	public float Fall(float toY, float speed)
	{
		enabled = true;

		float duration = (transform.localPosition.y - toY) * speed;

		transform.DOMoveY(toY, duration).SetEase(Ease.OutBounce);

		return duration;
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