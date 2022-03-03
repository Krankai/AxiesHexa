using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public AxiesSpineModel testModel;

    // Start is called before the first frame update
    void Start()
    {
        if (testModel == null)
        {
            testModel = GetComponent<AxiesSpineModel>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (testModel == null) return;

        if (Input.GetButtonDown("Fire1"))
        {
            testModel.TryMove(new Vector3(2.5f, 1.67f, -5.83f));
        }

        if (Input.GetButtonDown("Fire2"))
        {
            testModel.TryAttack();
        }

        if (Input.GetButtonDown("Fire3"))
        {
            testModel.TryDie();
        }
    }
}
