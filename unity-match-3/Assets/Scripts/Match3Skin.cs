using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;


public class Match3Skin : MonoBehaviour
{
	[SerializeField] private Tile[] _tilePrefabs;
	[SerializeField] private Match3Game _game;
	[SerializeField] private float _dragThreshold = 0.5f;
	[SerializeField] private TileSwapper _tileSwapper;

	private Grid2D<Tile> _tiles;
	private float2 _tileOffset;
	private float _busyDuration;

	public bool IsPlaying => true;
	public bool IsBusy => _busyDuration > 0.0f;


	public void DoWork()
	{
		if (IsBusy)
		{
			_tileSwapper.Update();
			_busyDuration -= Time.deltaTime;
			if (IsBusy)
				return;
		}


		if (_game.HasMatches)
		{
			ProcessMatches();
		}
		else
		if (_game.NeedsFilling)
		{
			DropTiles();
		}
	}

	public void StartNewGame()
	{
		_busyDuration = 0.0f;

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
		bool success = _game.TryMove(move);
		Tile a = _tiles[move.From];
		Tile b = _tiles[move.To];
		_busyDuration = _tileSwapper.Swap(a, b, !success);
		if (success)
		{
			_tiles[move.From] = b;
			_tiles[move.To] = a;
		}
	}

	private void DropTiles()
	{
		_game.DropTiles();

		for (int i = 0; i < _game.DroppedTiles.Count; i++)
		{
			TileDrop drop = _game.DroppedTiles[i];
			Tile tile;
			if (drop.fromY < _tiles.SizeY)
			{
				tile = _tiles[drop.coordinates.x, drop.fromY];
				tile.transform.localPosition = new Vector3(drop.coordinates.x + _tileOffset.x, drop.coordinates.y + _tileOffset.y);
			}else
			{
				tile = SpawnTile(_game[drop.coordinates], drop.coordinates.x, drop.coordinates.y);
			}

			_tiles[drop.coordinates] = tile;
		}
	}

	private void ProcessMatches()
	{
		_game.ProcessMatches();

		for (int i = 0; i < _game.ClearedTileCoordinates.Count; i++)
		{
			int2 c = _game.ClearedTileCoordinates[i];
			_busyDuration = Mathf.Max(_tiles[c].Disappear(), _busyDuration);
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