using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;


public class FloatingScore : MonoBehaviour
{
	[SerializeField] private TextMeshPro displayText;
	[SerializeField] private float flyDistance = 3f;
	[SerializeField] private float flyHeight = 2f;
	[SerializeField] private AnimationCurve curveFly;
	[SerializeField] private Vector2Int minMaxAngle;

	[SerializeField]
	private float displayDuration = 0.5f;

	private float _age;
	private IObjectPool<FloatingScore> _pool;
	private Vector2 _startPos;
	private Vector2 _dirFly;


	public void Show(Vector3 pos, int value)
	{
		_pool ??= new ObjectPool<FloatingScore>(() => Instantiate(this),
			fs => fs.gameObject.SetActive(true),
			fs => fs.gameObject.SetActive(false));

		FloatingScore instance = _pool.Get();
		instance._pool = _pool;
		instance.displayText.SetText("{0}", value);
		instance.transform.localPosition = pos + new Vector3(0.5f, 0.5f);
		instance.transform.DOMove(new Vector3(instance.transform.localPosition.x, instance.transform.localPosition.y + flyDistance, -0.1f), displayDuration).SetEase(Ease.OutCubic).OnComplete(() => _pool.Release(instance));
	}

	private Vector2 GetRandomDirection()
	{
		int randomAngle = Random.Range(minMaxAngle.x, minMaxAngle.y);
		return new Vector2(Mathf.Cos(randomAngle * Mathf.Deg2Rad), Mathf.Sin(randomAngle * Mathf.Deg2Rad));
	}

	public static Vector2 Parabola(Vector2 start, Vector2 end, float height, float t)
	{
		Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

		var mid = Vector2.Lerp(start, end, t);

		return new Vector2(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t));
	}
}