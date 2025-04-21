using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DifficultyMultiplier : MonoBehaviour
{
    public static float difficultyScale { get; private set; }
    [SerializeField] private float startMultiplier = 1f;
    [SerializeField] private float endMultiplier = 2.3f;
    [SerializeField] private float scaleTimeSeconds = 1800;
    [SerializeField] private float diffScale = 1f;
    private float multiplierDiff;
    private float currTime = 0;

    private void Awake()
    {
        difficultyScale = startMultiplier;
        multiplierDiff = endMultiplier - startMultiplier;
    }

    private void FixedUpdate()
    {
        if (currTime < scaleTimeSeconds)
        {
            CalcDifficultyMultiplier();
            currTime += Time.fixedDeltaTime;
        }
    }

    public void ToggleDiffScaling(bool toggle)
    {
        enabled = toggle;
    }

    private void CalcDifficultyMultiplier()
    {
        difficultyScale = CubicEaseOut(currTime, startMultiplier, multiplierDiff, scaleTimeSeconds);
        diffScale = difficultyScale;
    }

    private float CubicEaseOut(float currTime, float initialValue, float totalChange, float duration)
    {
        return totalChange * ((currTime = currTime / duration - 1) * currTime * currTime + 1) + initialValue;
    }
}
