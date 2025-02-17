using System;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;


public class FloatingScore : MonoBehaviour
{
	[SerializeField] private TextMeshPro displayText;

	[SerializeField, Range(0.1f, 1f)]
	private float displayDuration = 0.5f;

	[SerializeField, Range(0f, 4f)]
	private float riseSpeed = 2f;


	private float _age;
	private IObjectPool<FloatingScore> _pool;


	public void Show(Vector3 pos, int value)
	{
		_pool ??= new ObjectPool<FloatingScore>(() => Instantiate(this),
			fs => fs.gameObject.SetActive(true),
			fs => fs.gameObject.SetActive(false));

		FloatingScore instance = _pool.Get();
		instance._pool = _pool;
		instance.displayText.SetText("{0}", value);
		instance.transform.localPosition = pos + new Vector3(0.5f, 0.5f);
		instance._age = 0f;
	}

	public void Update()
	{
		_age += Time.deltaTime;
		if (_age >= displayDuration)
		{
			_pool.Release(this);
		}else
		{
			Vector3 p = transform.localPosition;
			p.y += riseSpeed * Time.deltaTime;
			transform.localPosition = p;
		}
	}
}