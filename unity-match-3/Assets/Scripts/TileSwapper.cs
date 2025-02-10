using UnityEngine;


[System.Serializable]
public class TileSwapper
{
	[SerializeField, Range(0.1f, 10f)]
	float _duration = 0.25f;

	[SerializeField, Range(0.1f, 10f)]
	float _maxDepthOffset = 0.5f;

	private Tile _tileA;
	private Tile _tileB;

	private Vector3 _positionA;
	private Vector3 _positionB;

	private float _progress = -1.0f;
	private bool _pingPong;


	public float Swap(Tile a, Tile b, bool pingPong)
	{
		_tileA = a;
		_tileB = b;
		_positionA = a.transform.localPosition;
		_positionB = b.transform.localPosition;
		_pingPong = pingPong;
		_progress = 0.0f;

		return pingPong ? 2.0f * _duration : _duration;
	}

	public void Update()
	{
		if (_progress < 0f)
			return;

		_progress += Time.deltaTime;
		if (_progress >= _duration)
		{
			if (_pingPong)
			{
				_progress -= _duration;
				_pingPong = false;
				(_tileA, _tileB) = (_tileB, _tileA);
			}else
			{
				_progress -= 1f;
				_tileA.transform.localPosition = _positionB;
				_tileB.transform.localPosition = _positionA;
				return;
			}
		}

		float t = _progress / _duration;
		float z = Mathf.Sin(Mathf.PI * t) * _maxDepthOffset;
		Vector3 p = Vector3.Lerp(_positionA, _positionB, t);
		p.z = z;
		_tileA.transform.localPosition = p;
		p = Vector3.Lerp(_positionA, _positionB, 1f - t);
		p.z = z;
		_tileB.transform.localPosition = p;
	}
}
