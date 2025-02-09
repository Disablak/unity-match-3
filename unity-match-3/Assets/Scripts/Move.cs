using Unity.Mathematics;
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
}