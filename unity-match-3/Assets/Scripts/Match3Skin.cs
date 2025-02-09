using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;


public class Match3Skin : MonoBehaviour
{
	[SerializeField] private Tile[] _tilePrefabs;
	[SerializeField] private Match3Game _game;
	[SerializeField] private float _dragThreshold = 0.5f;

	private Grid2D<Tile> _tiles;
	private float2 _tileOffset;

	public bool IsPlaying => true;
	public bool IsBusy => false;
	public void DoWork() {}

	public void StartNewGame()
	{
		_game.StartNewGame();
		_tileOffset = -0.5f * (float2)_game.Size;

		if (_tiles.IsUndefined)
		{
			_tiles = new Grid2D<Tile>(_game.Size);
		}
		else
		{
			for (int y = 0; y < _tiles.SizeY; y++)
			{
				for (int x = 0; x < _tiles.SizeX; x++)
				{
					_tiles[x, y].Despawn();
					_tiles[x, y] = null;
				}
			}
		}


		for (int y = 0; y < _tiles.SizeY; y++)
		{
			for (int x = 0; x < _tiles.SizeX; x++)
			{
				_tiles[x, y] = SpawnTile(_game[x, y], x, y);
			}
		}
	}

	private Tile SpawnTile(TileState tileState, int x, int y)
	{
		return _tilePrefabs[(int)tileState - 1].Spawn(new Vector3(x + _tileOffset.x, y + _tileOffset.y));
	}

	public bool EvalueteDrag(Vector3 start, Vector3 end)
	{
		float2 a = ScreenToTileSpace(start);
		float2 b = ScreenToTileSpace(end);

		var move = new Move(
			(int2)floor(a), (b - a) switch
			{
			var d when d.x > _dragThreshold => MoveDirection.Right,
			var d when d.x < -_dragThreshold => MoveDirection.Left,
			var d when d.y > _dragThreshold => MoveDirection.Up,
			var d when d.y < -_dragThreshold => MoveDirection.Down,
			_ => MoveDirection.None
			}
		);

		if (move.IsValid && _tiles.AreValidCoordinates(move.From) && _tiles.AreValidCoordinates(move.To))
		{
			DoMove(move);
			return false;
		}

		return true;
	}

	private void DoMove(Move move)
	{
		if (_game.TryMove(move))
		{
			(_tiles[move.From].transform.localPosition, _tiles[move.To].transform.localPosition) =
				(_tiles[move.To].transform.localPosition, _tiles[move.From].transform.localPosition);

			_tiles.Swap(move.From, move.To);
		}

		if (_game.HasMatches)
		{
			ProcessMatches();
		}
	}

	private void ProcessMatches()
	{
		_game.ProcessMatches();

		for (int i = 0; i < _game.ClearedTileCoordinates.Count; i++)
		{
			int2 c = _game.ClearedTileCoordinates[i];
			_tiles[c].Despawn();
			_tiles[c] = null;
		}
	}

	private float2 ScreenToTileSpace(Vector3 screenPosition)
	{
		Ray ray = Camera.main.ScreenPointToRay(screenPosition);
		Vector3 p = ray.origin - ray.direction * (ray.origin.z / ray.direction.z);
		return float2(p.x - _tileOffset.x + 0.5f, p.y - _tileOffset.y + 0.5f);
	}
}