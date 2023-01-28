using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GoofyFPS : MonoBehaviour
{
    public TextMeshProUGUI text;
    float[] frames = new float[50];
    int lastFrameIndex;

    private void Update() {
        frames[lastFrameIndex] = Time.unscaledDeltaTime;
        lastFrameIndex = (lastFrameIndex + 1) % frames.Length;

        text.text = Mathf.RoundToInt(calculateFPS()).ToString();
    }

    float calculateFPS() {
        float total = 0f;
        foreach (float delta in frames) {
            total += delta;
        }
        return frames.Length / total;
    }
}
