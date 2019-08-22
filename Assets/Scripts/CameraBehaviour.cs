using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    Camera cam;

	[Header("Shake Values")]
	// Default values
	public float defaultShakeDuration;
	public float defaultShakeMagnitude;
	public float defaultDampingSpeed;

	private float currShakeDuration;
	private float currShakeMagnitude;
    private float currDampingSpeed;
    private Vector3 initPos;

    void Start() {
        initPos = transform.position;
    }

    IEnumerator Shake()
        {
            while (true)
            {
                if (currShakeDuration > 0) {
                    transform.localPosition = new Vector3(Random.Range(-1f, 1f) * currShakeMagnitude, Random.Range(-1f, 1f) * currShakeMagnitude, initPos.z);
                    currShakeDuration -= currDampingSpeed * Time.deltaTime;
                } else {
                    transform.localPosition = initPos;
                    break;
                }
                yield return null;
            }
        }

        public void TriggerShake()
        {
            currShakeDuration = defaultShakeDuration;
            currShakeMagnitude = defaultShakeMagnitude;
            currDampingSpeed = defaultDampingSpeed;
            StartCoroutine(Shake());
        }

        public void TriggerMildShake() {
            currShakeDuration = defaultShakeDuration / 1.5f;
            currShakeMagnitude = defaultShakeMagnitude / 1.5f;
            currDampingSpeed = defaultDampingSpeed;
            StartCoroutine(Shake());
        }

        public void TriggerWeakShake() {
            currShakeDuration = defaultShakeDuration / 2;
            currShakeMagnitude = defaultShakeMagnitude / 2;
            currDampingSpeed = defaultDampingSpeed * 2;
            StartCoroutine(Shake());
        }

        public void TriggerShake(float duration=0, float magnitude=0, float dampingSpeed=0)
        {
            if (duration == 0) {
                currShakeDuration = defaultShakeDuration;
            } else {
                currShakeDuration = duration;
            }
            if (magnitude == 0) {
                currShakeMagnitude = defaultShakeMagnitude;
            } else {
                currShakeMagnitude = magnitude;
            }
            if (dampingSpeed == 0) {
                currDampingSpeed = defaultDampingSpeed;
            } else {
                currDampingSpeed = dampingSpeed;
            }
            StartCoroutine(Shake());
    }
}
