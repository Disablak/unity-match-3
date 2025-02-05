using UnityEngine;


public class Match3Skin : MonoBehaviour
{
	public bool IsPlaying => true;
	public bool IsBusy => false;
	public void StartNewGame() {}
	public void DoWork() {}

	public bool EvalueteDrag(Vector3 start, Vector3 end)
	{
		return false;
	}
}