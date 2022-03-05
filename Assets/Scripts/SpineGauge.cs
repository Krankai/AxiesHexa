using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpineGauge : MonoBehaviour
{
    [Range(0, 1)]
    public float fillPercent;
    public AnimationReferenceAsset fillAnimation;

    SkeletonRenderer skeletonRenderer;
    float fillSpeed = 1f;
    float currentPercent;

    void Awake()
    {
        skeletonRenderer = GetComponent<SkeletonRenderer>();
        currentPercent = 1f;
    }

    public void SetAlphaGaugeView(float alpha)
    {
        if (skeletonRenderer == null | skeletonRenderer.skeleton == null) return;
        skeletonRenderer.skeleton.a = alpha;
    }

    public void SetGaugePercent(float percent)
    {
        StartCoroutine(GaugeRountine(percent));
        currentPercent = percent;
    }

    IEnumerator GaugeRountine(float percent)
    {
        if (skeletonRenderer == null) yield break;

        var skeleton = skeletonRenderer.skeleton;
        if (skeleton == null) yield break;

        if (fillAnimation == null) yield break;
        float start = currentPercent * fillAnimation.Animation.duration;
        float target = percent * fillAnimation.Animation.duration;

        float step = fillSpeed * Time.deltaTime;
        for (float t = start; t > target; t -= step)
        {
            fillAnimation.Animation.Apply(skeleton, 0, t, false, null, 1f, Spine.MixPose.Setup, Spine.MixDirection.In);
            fillPercent = t / fillAnimation.Animation.duration;

            skeleton.Update(step);
            skeleton.UpdateWorldTransform();

            yield return null;
        }

        fillAnimation.Animation.Apply(skeleton, 0, target, false, null, 1f, Spine.MixPose.Setup, Spine.MixDirection.In);
        fillPercent = percent;

        skeleton.Update(step);
        skeleton.UpdateWorldTransform();
    }
}
