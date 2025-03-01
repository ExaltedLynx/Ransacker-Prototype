using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundAnimController : MonoBehaviour
{
    [SerializeField] Animator bgAnimator;
    private static Animator _bgAnimator;
    public static bool animInProgress = false;

    void Awake()
    {
        _bgAnimator = bgAnimator;
    }

    public static void StartBackgroundAnim()
    {
        animInProgress = true;
        _bgAnimator.SetTrigger("StartBackgroundAnim");
    }
}
