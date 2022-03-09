using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScreenController : MonoBehaviour
{
    public GameManager gameManager;
    public RawImage background;
    public Text defenderWinText;
    public Text attackerWinText;
    public Text drawText;

    public Button returnButton;
    public Button tryAgainButton;
    public Button bigTryAgainButton;


    public void OnDefendersWin()
    {
        gameObject.SetActive(true);

        background.gameObject.SetActive(true);
        defenderWinText.gameObject.SetActive(true);
        attackerWinText.gameObject.SetActive(false);
        drawText.gameObject.SetActive(false);

        returnButton.gameObject.SetActive(true); returnButton.interactable = true;
        tryAgainButton.gameObject.SetActive(true); tryAgainButton.interactable = true;
        bigTryAgainButton.gameObject.SetActive(false);
    }

    public void OnAttackersWin()
    {
        gameObject.SetActive(true);

        background.gameObject.SetActive(true);
        attackerWinText.gameObject.SetActive(true);
        defenderWinText.gameObject.SetActive(false);
        drawText.gameObject.SetActive(false);

        returnButton.gameObject.SetActive(true); returnButton.interactable = true;
        tryAgainButton.gameObject.SetActive(true); tryAgainButton.interactable = true;
        bigTryAgainButton.gameObject.SetActive(false);
    }

    public void OnDraw()
    {
        gameObject.SetActive(true);

        background.gameObject.SetActive(true);
        drawText.gameObject.SetActive(true);
        attackerWinText.gameObject.SetActive(false);
        defenderWinText.gameObject.SetActive(false);

        returnButton.gameObject.SetActive(true); returnButton.interactable = true;
        tryAgainButton.gameObject.SetActive(true); tryAgainButton.interactable = true;
        bigTryAgainButton.gameObject.SetActive(false);
    }

    public void OnReturn()
    {
        if (background != null) background.gameObject.SetActive(false);
        if (defenderWinText != null) defenderWinText.gameObject.SetActive(false);
        if (attackerWinText != null) attackerWinText.gameObject.SetActive(false);

        if (tryAgainButton != null)
        {
            tryAgainButton.interactable = false;
            tryAgainButton.gameObject.SetActive(false);
        }
        if (returnButton != null)
        {
            returnButton.interactable = false;
            returnButton.gameObject.SetActive(false);
        }

        if (bigTryAgainButton != null)
        {
            bigTryAgainButton.gameObject.SetActive(true);
            bigTryAgainButton.interactable = true;
        }
    }

    public void OnTryAgain()
    {
        if (gameManager == null) return;

        gameManager.ReSetup();
    }
}
