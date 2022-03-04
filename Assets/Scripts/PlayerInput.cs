using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public SpineAxieModel testModel;
    public SpineGauge testGaugeModel;
    public GameManager gameManager;

    void OnValidate()
    {
        //Debug.Log(testModel);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (testModel == null)
        {
            testModel = GetComponent<SpineAxieModel>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (testModel == null) return;
        if (testGaugeModel == null) return;

        if (Input.GetButtonDown("Fire1"))
        {
            //testModel.TryMove(new Vector3(2.5f, 1.67f, -5.83f));
            //testGaugeModel.SetGaugePercent(0.6f);
            //testModel.TryDie();

            if (gameManager != null)
            {
                gameManager.SimulateStep();
            }
        }

        if (Input.GetButtonDown("Fire2"))
        {
            //testModel.TryAttack(null);

            // var manager = FindObjectOfType<GameManager>();
            // if (manager)
            // {
            //     manager.SimulateAttack();
            // }
        }

        if (Input.GetButtonDown("Fire3"))
        {
            //testModel.TryDie();
            testGaugeModel.SetGaugePercent(0.4f);
        }
    }
}
