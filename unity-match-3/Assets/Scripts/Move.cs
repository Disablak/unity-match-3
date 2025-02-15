using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

[System.Serializable]
public struct Move
{
	public MoveDirection MoveDirection {get; private set;}

	public int2 From {get; private set;}
	public int2 To {get; private set;}

	public bool IsValid => MoveDirection != MoveDirection.None;

	public Move(int2 coord, MoveDirection dir)
	{
		MoveDirection = dir;
		From = coord;
		To = coord + dir switch
		{
			MoveDirection.Up => int2(0, 1),
			MoveDirection.Right => int2(1, 0),
			MoveDirection.Down => int2(0, -1),
			_ => int2(-1, 0)
		};
	}

	public static Move FindMove(Match3Game game)
	{
		Vector2 size = game.Size;
		for (int2 c = 0; c.y < size.y; c.y++)
		{
			for (c.x = 0; c.x < size.x; c.x++)
			{
				TileState t = game[c];

				if (c.x >= 3 && game[c.x - 2, c.y] == t && game[c.x - 3, c.y] == t)
					return new Move(c, MoveDirection.Left);

				if (c.x + 3 < size.x && game[c.x + 2, c.y] == t && game[c.x + 3, c.y] == t)
					return new Move(c, MoveDirection.Right);

				if (c.y >= 3 && game[c.x, c.y - 2] == t && game[c.x, c.y - 3] == t)
					return new Move(c, MoveDirection.Down);

				if (c.y + 3 < size.y && game[c.x, c.y + 2] == t && game[c.x, c.y + 3] == t)
					return new Move(c, MoveDirection.Up);

				if (c.y > 1)
				{
					if (c.x > 1 && game[c.x - 1, c.y - 1] == t)
					{
						if ((c.x >= 2 && game[c.x - 2, c.y - 1] == t) || (c.x + 1 < size.x && game[c.x + 1, c.y - 1] == t))
							return new Move(c, MoveDirection.Down);
						if ((c.y >= 2 && game[c.x - 1, c.y - 2] == t) || (c.y + 1 < size.y && game[c.x - 1, c.y + 1] == t))
							return new Move(c, MoveDirection.Left);
					}

					if (c.x + 1 < size.x && game[c.x + 1, c.y - 1] == t)
					{
						if (c.x + 2 < size.x && game[c.x + 2, c.y - 1] == t)
							return new Move(c, MoveDirection.Down);
						if ((c.y >= 2 && game[c.x + 1, c.y - 2] == t) || (c.y + 1 < size.y && game[c.x + 1, c.y + 1] == t))
							return new Move(c, MoveDirection.Right);
					}
				}


				if (c.y + 1 < size.y)
				{
					if (c.x > 1 && game[c.x - 1, c.y + 1] == t)
					{
						if ((c.x >= 2 && game[c.x - 2, c.y + 1] == t) || (c.x + 1 < size.x && game[c.x + 1, c.y + 1] == t))
							return new Move(c, MoveDirection.Up);
						if (c.y + 2 < size.y && game[c.x - 1, c.y + 2] == t)
							return new Move(c, MoveDirection.Left);
					}

					if (c.x + 1 < size.x && game[c.x + 1, c.y + 1] == t)
					{
						if (c.x + 2 < size.x && game[c.x + 2, c.y + 1] == t)
							return new Move(c, MoveDirection.Up);
						if (c.y + 2 < size.y && game[c.x + 1, c.y + 2] == t)
							return new Move(c, MoveDirection.Right);
					}
				}
			}
		}

		return default;
	}
}