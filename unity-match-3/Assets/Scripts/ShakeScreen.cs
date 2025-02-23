using DG.Tweening;
using UnityEngine;


public class ShakeScreen : MonoBehaviour
{
	[SerializeField] private Camera _mainCamera;
	[SerializeField] private float duration = 0.5f; // How long the shake lasts
	[SerializeField] private float strength = 0.5f; // How much the screen shakes
	[SerializeField] private int vibrato = 10; // How many shakes happen in the duration
	[SerializeField] private float randomness = 90f; // Randomness of shake direction

	public void Shake(float strength)
	{
		_mainCamera.transform.DOShakePosition(duration, strength, vibrato, randomness);
	}
}