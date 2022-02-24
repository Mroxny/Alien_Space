using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public void Shake(float duration, float magnitude) {
        StartCoroutine(StartShake(duration, magnitude));
    }

    private IEnumerator StartShake(float duration, float magnitude) {
        Vector3 orginalPos = transform.localPosition;
        float elapsed = 0;

        while (elapsed < duration) {
            float x = Random.Range(-1,1) * magnitude;
            float y = Random.Range(-1, 1) * magnitude;

            transform.localPosition = new Vector3(x, y, orginalPos.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = orginalPos;  
    }
}
