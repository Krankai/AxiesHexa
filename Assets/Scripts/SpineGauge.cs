using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpineGauge : MonoBehaviour
{
    [Range(0, 1)]
    public float fillPercent = 0f;
    public AnimationReferenceAsset fillAnimation;

    SkeletonRenderer skeletonRenderer;


    void Awake()
    {
        skeletonRenderer = GetComponent<SkeletonRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetGaugePercent(float percent)
    {
        // if (fillAnimation == null) return;
        // if (skeletonRenderer == null) return;

        // var skeleton = skeletonRenderer.skeleton;
        // if (skeleton == null) return;

        // fillAnimation.Animation.Apply(skeleton, 0, percent, false, null, 1f, Spine.MixPose.Setup, Spine.MixDirection.In);
        // skeleton.Update(Time.deltaTime);
        // skeleton.UpdateWorldTransform();

        StartCoroutine(GaugeRountine(percent));
    }

    IEnumerator GaugeRountine(float percent)
    {
        if (fillAnimation == null) yield break;
        if (skeletonRenderer == null) yield break;

        var skeleton = skeletonRenderer.skeleton;
        if (skeleton == null) yield break;

        for (float t = 1f; t > percent; t -= Time.deltaTime)
        {
            fillAnimation.Animation.Apply(skeleton, 0, t, false, null, 1f, Spine.MixPose.Setup, Spine.MixDirection.In);
            skeleton.Update(Time.deltaTime);
            skeleton.UpdateWorldTransform();

            yield return null;
        }

        fillAnimation.Animation.Apply(skeleton, 0, percent, false, null, 1f, Spine.MixPose.Setup, Spine.MixDirection.In);
        skeleton.Update(Time.deltaTime);
        skeleton.UpdateWorldTransform();
    }
}
