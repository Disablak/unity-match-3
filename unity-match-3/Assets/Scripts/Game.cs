using System;
using System.Linq;
using UnityEngine;


public class Game : MonoBehaviour
{
	[SerializeField]
	private Match3Skin _match3;

	[SerializeField]
	private bool _autoGame;

	private Vector2 _dragStart;
	private bool _isDragging;


	private void Awake()
	{
		_match3.StartNewGame();
	}

	private void Update()
	{
		if (_match3.IsPlaying)
		{
			if (_match3.IsBusy == false)
			{
				HandleInput();
			}

			_match3.DoWork();
		}
		else if (Input.GetKeyDown(KeyCode.Space))
		{
			_match3.StartNewGame();
		}
	}

	private void HandleInput()
	{
		if (_autoGame)
		{
			_match3.DoAutoMove();
		}
		else if (_isDragging == false && Input.GetMouseButtonDown(0))
		{
			_dragStart = Input.mousePosition;
			_isDragging = true;
		}
		else if (_isDragging && Input.GetMouseButton(0))
		{
			_isDragging = _match3.EvaluateDrag(_dragStart, Input.mousePosition);
		}
		else
		{
			_isDragging = false;
		}
	}
}

public enum MoveDirection
{
	None, Up, Right, Down, Left
}
