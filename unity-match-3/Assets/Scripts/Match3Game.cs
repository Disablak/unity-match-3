using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;


public class Match3Game : MonoBehaviour
{
	[SerializeField] private int2 _size = 8;

	private List<Match> _matches;
	private Grid2D<TileState> _grid;

	public TileState this[int x, int y] => _grid[x, y];
	public TileState this[int2 c] => _grid[c];
	public int2 Size => _size;
	public bool HasMatches => _matches.Count > 0;

	public void StartNewGame()
	{
		if (_grid.IsUndefined)
		{
			_grid = new Grid2D<TileState>(_size);
			_matches = new List<Match>();
		}

		FillGrid();
	}

	public bool TryMove(Move move)
	{
		_grid.Swap(move.From, move.To);
		if (FindMatches())
			return true;

		_grid.Swap(move.From, move.To);
		return false;
	}

	private bool FindMatches()
	{
		FindHorizontalMatches();
		FindVerticalMatches();

		return HasMatches;
	}

	private void FindHorizontalMatches()
	{
		for (int y = 0; y < _size.y; y++)
		{
			TileState start = _grid[0, y];
			int length = 1;
			for (int x = 1; x < _size.x; x++)
			{
				TileState t = _grid[x, y];
				if (start == t)
				{
					length++;
				}else
				{
					if (length >= 3)
					{
						_matches.Add(new Match(x - length, y, length, true));
					}
					start = t;
					length = 1;
				}
			}
			if (length >= 3)
			{
				_matches.Add(new Match(_size.x - length, y, length, true));
			}
		}
	}

	private void FindVerticalMatches()
	{
		for (int x = 0; x < _size.x; x++)
		{
			TileState start = _grid[x, 0];
			int length = 1;
			for (int y = 1; y < _size.y; y++)
			{
				TileState t = _grid[x, y];
				if (start == t)
				{
					length++;
				}else
				{
					if (length >= 3)
					{
						_matches.Add(new Match(x, y - length, length, false));
					}
					start = t;
					length = 1;
				}
			}
			if (length >= 3)
			{
				_matches.Add(new Match(x, _size.y - length, length, false));
			}
		}
	}

	private void FillGrid()
	{
		for (int y = 0; y < _size.y; y++)
		{
			for (int x = 0; x < _size.x; x++)
			{
				GenerateCell(x, y);
			}
		}
	}

	private void GenerateCell(int x, int y)
	{
		TileState a = TileState.None;
		TileState b = TileState.None;
		int potentialMatchCount = 0;
		if (x > 1)
		{
			a = _grid[x - 1, y];
			if (a == _grid[x - 2, y])
				potentialMatchCount++;
		}

		if (y > 1)
		{
			b = _grid[x, y - 1];
			if (b == _grid[x, y - 2])
			{
				potentialMatchCount++;
				if (potentialMatchCount == 1)
					a = b;
				else if (b < a)
					(a, b) = (b, a);
			}
		}

		TileState tile = (TileState)Random.Range(1, 8 - potentialMatchCount);
		if (potentialMatchCount > 0 && tile >= a)
		{
			tile++;
		}

		if (potentialMatchCount == 2 && tile >= b)
		{
			tile++;
		}

		_grid[x, y] = tile;
	}
}