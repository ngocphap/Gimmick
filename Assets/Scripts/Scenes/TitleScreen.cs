using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TitleScreen : MonoBehaviour
{
    // access to our TextMeshPro object
    TextMeshProUGUI tmpTitleText;

    // flag that we called for the next scene
    bool calledNextScene;

    // key press detection flag
    bool inputDetected = false;

    // alpha color for keypress text
    int alphaKeyPressText = 255;

    // sound clip for key press
    public AudioClip keyPressClip;

    private enum TitleScreenStates { WaitForInput, NextScene };
    TitleScreenStates titleScreenState = TitleScreenStates.WaitForInput;

    // platform dependent key press (or tap) string
#if UNITY_STANDALONE
    string insertKeyPressText = "START";
#endif



    // title text tmp rich text string
    //string titleText;
    string titleText =
@" ><size=50 ><color=#FFFFFF{0:X2}>{1}</color></size></font>";

    void Awake()
    {
        // get title text tmp object
        tmpTitleText = GameObject.Find("TitleText").GetComponent<TextMeshProUGUI>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // title text settings
        tmpTitleText.alignment = TextAlignmentOptions.Center;
        tmpTitleText.alignment = TextAlignmentOptions.Midline;
        tmpTitleText.fontStyle = FontStyles.UpperCase;
        // set initial scene state
        titleScreenState = TitleScreenStates.WaitForInput;
    }

    // Update is called once per frame
    void Update()
    {
        switch (titleScreenState)
        {
            case TitleScreenStates.WaitForInput:
                tmpTitleText.text = String.Format(titleText, alphaKeyPressText, insertKeyPressText);
                // check for any key/tap input to continue
                if (Input.anyKey && !inputDetected)
                {
                    // do this only once
                    inputDetected = true;
                    // coroutine to flash the title text
                    StartCoroutine(FlashTitleText());
                    // play key press sound
                    SoundManager.Instance.Play(keyPressClip);
                }
                break;
            case TitleScreenStates.NextScene:
                // tell GameManager to trigger the next scene
                if (!calledNextScene)
                {
                    GameManager.Instance.StartNextScene();
                    calledNextScene = true;
                }
                break;
        }
    }

    private IEnumerator FlashTitleText()
    {
        // flash the key press text
        for (int i = 0; i < 5; i++)
        {
            alphaKeyPressText = 0;
            yield return new WaitForSeconds(0.1f);
            alphaKeyPressText = 255;
            yield return new WaitForSeconds(0.1f);
        }
        // finally hide it
        alphaKeyPressText = 0;
        yield return new WaitForSeconds(0.1f);
        // move to the next scene state
        titleScreenState = TitleScreenStates.NextScene;
    }
}
