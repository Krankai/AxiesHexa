using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxiesSpineView : MonoBehaviour
{
    [Header("Components")]
    public AxiesSpineModel model;
    public SkeletonAnimation skeletonAnimation;
    public AnimationReferenceAsset idle, move, attack, die;

    AxiesSpineModelState previousViewState;

    // Start is called before the first frame update
    void Start()
    {
        if (skeletonAnimation == null) return;

        model.AttackEvent += PlayAttack;
        model.DieEvent += PlayDie;
    }

    // Update is called once per frame
    void Update()
    {
        if (skeletonAnimation == null) return;
        if (model == null) return;

        // Detect changes in model.state
        AxiesSpineModelState currentModelState = model.state;

        if (previousViewState != currentModelState)
        {
            PlayNewStableAnimation();
        }

        previousViewState = currentModelState;
    }

    void PlayNewStableAnimation()
    {
        AxiesSpineModelState newModelState = model.state;
        Spine.Animation nextAnimation;

        if (newModelState == AxiesSpineModelState.Moving)
        {
            nextAnimation = move;
        }
        else
        {
            nextAnimation = idle;
        }

        skeletonAnimation.AnimationState.SetAnimation(0, nextAnimation, true);
    }

    public void PlayAttack()
    {
        var attackTrack = skeletonAnimation.AnimationState.SetAnimation(1, attack, false);
        attackTrack.AttachmentThreshold = 1f;
        attackTrack.MixDuration = 0f;

        var empty = skeletonAnimation.state.AddEmptyAnimation(1, 0.5f, 0f);
        empty.AttachmentThreshold = 1f;
    }

    public void PlayDie()
    {
        var dieTrack = skeletonAnimation.AnimationState.SetAnimation(2, die, false);
        dieTrack.AttachmentThreshold = 1f;
        dieTrack.MixDuration = 0f;

        var empty = skeletonAnimation.state.AddEmptyAnimation(2, 0.5f, 0f);
        empty.AttachmentThreshold = 1f;

        StartCoroutine(DieRoutine());
    }

    IEnumerator DieRoutine()
    {
        if (model.fadeOutDuration <= 0) yield break;

        // Fade out "skeleton" to simulate dying animation
        {
            float startAlpha = skeletonAnimation.skeleton.a;
            float endAlpha = 0f;

            for (float t = 0f; t < model.fadeOutDuration; t += Time.deltaTime)
            {
                float normalizedTime = t / model.fadeOutDuration;
                skeletonAnimation.skeleton.a = Mathf.Lerp(startAlpha, endAlpha, normalizedTime);
                yield return null;
            }
            skeletonAnimation.skeleton.a = 0f;
        }

        model.state = AxiesSpineModelState.Die;

        yield return new WaitForSeconds(0.1f);
        Debug.Log("Died!!!");
    }
}
