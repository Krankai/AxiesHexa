using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpineAxieView : MonoBehaviour
{
    [Header("Components")]
    public SpineAxieModel model;
    public SkeletonAnimation skeletonAnimation;
    public AnimationReferenceAsset idle, move, attack, die;

    SpineAxieModelState previousViewState;

    int deathSetCounter = 0;        // max: 2 - include: 'die' animation and 'fade-out' animation
    const int compareDeathCounter = 1;      // note: do not take into account 'die' animation anymore (since too long)

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

        // Update facing direction
        if (IsUnmatchDirection())
        {
            Turn(model.facingLeft);
        }

        // Detect changes in model.state
        SpineAxieModelState currentModelState = model.state;

        if (previousViewState != currentModelState)
        {
            PlayNewStableAnimation();
        }

        previousViewState = currentModelState;
    }

    void PlayNewStableAnimation()
    {
        SpineAxieModelState newModelState = model.state;
        Spine.Animation nextAnimation;

        if (newModelState == SpineAxieModelState.Moving)
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
        empty.End += delegate
        {
            if (model.currentHealth <= 0)
            {
                //model.TryDie();
                PlayDie();
            }
            else if (model.gameManager)
            {
                Debug.Log("[" + model.axieType + model.gameManager.i + "] Attack at position: " + model.gameObject.transform.position);
                model.gameManager.OnFinishAxieAnimation();
            }
        };
    }

    public void PlayDie()
    {
        var dieTrack = skeletonAnimation.AnimationState.SetAnimation(2, die, false);
        dieTrack.AttachmentThreshold = 1f;
        dieTrack.MixDuration = 0.5f;
        dieTrack.TrackEnd = float.PositiveInfinity;

        // var empty = skeletonAnimation.state.AddEmptyAnimation(2, 0.2f, 0f);
        // empty.AttachmentThreshold = 1f;

        //empty.End += delegate
        dieTrack.Complete += delegate
        {
            // NOTE: "death" animation (stun) takes too long to complete/end -> remove this from counter to avoid delay
            // Now only rely on fade-out duraction to decide whether "die" is done or not

            // if (++deathSetCounter >= compareDeathCounter && model.gameManager)
            // {
            //     model.gameManager.FinishAxieAnimation();
            //     Destroy(model.gameObject, 0.1f);
            // }
        };

        StartCoroutine(DieRoutine());
    }

    IEnumerator DieRoutine()
    {
        if (model.fadeOutDuration <= 0) yield break;

        yield return new WaitForSeconds(0.1f);

        // Fade out "skeleton" to simulate dying animation
        {
            float startAlpha = skeletonAnimation.skeleton.a;
            float endAlpha = 0f;

            for (float t = 0f; t < model.fadeOutDuration; t += Time.deltaTime)
            {
                float normalizedTime = t / model.fadeOutDuration;
                float stepAlpha = Mathf.Lerp(startAlpha, endAlpha, normalizedTime);

                skeletonAnimation.skeleton.a = stepAlpha;               // axie view
                model.healthGauge.SetAlphaGaugeView(stepAlpha);         // health view

                yield return null;
            }
            skeletonAnimation.skeleton.a = 0f;
            model.healthGauge.SetAlphaGaugeView(0f);
            model.healthGauge.gameObject.SetActive(false);
        }

        //model.state = SpineAxieModelState.Die;
        if (++this.deathSetCounter >= compareDeathCounter && model.gameManager)
        {
            Debug.Log("[" + model.axieType + model.gameManager.i + "] Die at position: " + model.gameObject.transform.position);
            model.gameManager.OnFinishAxieAnimation();

            if (model.axieType == AxieType.Defense)
            {
                model.gameManager.OnDefenderDeath();
            }
            else if (model.axieType == AxieType.Attack)
            {
                model.gameManager.OnAttackerDeath();
            }

            Destroy(model.gameObject, 0.1f);
        }
    }

    public void Turn(bool facingLeft)
    {
        if (skeletonAnimation == null) return;
        
        var scaleVector = skeletonAnimation.transform.localScale;
        skeletonAnimation.transform.localScale = new Vector3(Mathf.Abs(scaleVector.x) * (facingLeft ? 1f : -1f), scaleVector.y, scaleVector.z);
    }

    bool IsUnmatchDirection()
    {
        if (skeletonAnimation == null) return false;

        return (model.facingLeft != skeletonAnimation.transform.localScale.x > 0);
    }
}
