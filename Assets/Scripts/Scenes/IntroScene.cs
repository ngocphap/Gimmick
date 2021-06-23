using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IntroScene : MonoBehaviour
{
    // create an animated storyline
    float startTime;
    float runTime;

    // flag that we called for the next scene
    bool calledNextScene;

    // key press detection flag
    bool inputDetected = false;

    // show runtime text
    [SerializeField] bool showRunTime;

    // progress tracking and fading delay
    float progress;
    float fadeTimer;
    [SerializeField] float fadeDelay = 5f;

    // game objects we use in this scene
    [SerializeField] GameObject outsideLab;
    [SerializeField] GameObject insideLab;
    [SerializeField] GameObject player;

    // music clip for this scene
    public AudioClip musicClip;

    // canvas texts
    Text runTimeText;
    TextMeshProUGUI tmpDialogueText;

    // points our player runs to
    float[] playerRunPoints = {
        0.38f,
        2.45f
    };

    // current music volume
    float musicVolume;

    private enum IntroStates { OutsideLab, ScreenFade1, InsideLab, ScreenFade2, NextScene };
    IntroStates introState = IntroStates.OutsideLab;

    string[] dialogueStrings = {
        "CHÚC MỪNG SINH NHẬT CON GÁI ....!!! ",
        "WOWWWW THẬT TUYỆT   !!!!Gimmick ,Búp bê mới !!! ",
        " OLD DOLL  :\n\n\tChúng ta bị vứt bỏ trong 1 chiếu hộp :( :(",
        " CHÚNG TA SẼ BẮT CÓC CÔ ẤY !!!",
        "Mr.Gimmick : MÌNH SẼ CỨU CHỦ NHÂN !!!",
    };

    void Awake()
    {
        // get text objects
        runTimeText = GameObject.Find("RunTime").GetComponent<Text>();
        tmpDialogueText = GameObject.Find("DialogueText").GetComponent<TextMeshProUGUI>();
        // make sure there is no dialogue at start
        tmpDialogueText.text = "";
        // no user control allowed during this scene
        player.GetComponent<PlayerController>().FreezeInput(true);
        // all children in the InsideLab start transparent
        foreach (Transform child in insideLab.transform)
        {
            child.gameObject.GetComponent<SpriteRenderer>().color = Color.clear;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // track when we started
        startTime = Time.time;
        // set up the scene music - 3/4 volume, no loop, and play
        SoundManager.Instance.MusicSource.clip = musicClip;
        SoundManager.Instance.MusicSource.volume = 0.75f;
        SoundManager.Instance.MusicSource.loop = false;
        SoundManager.Instance.MusicSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        // how long has the scene been running for
        runTime = Time.time - startTime;

        // this is for seeing the run time of the scene and is helpful for making 
        // the storytelling triggers - use checkbox in inspector to turn it off
        runTimeText.text = showRunTime ? String.Format("RunTime: {0:0.00}", runTime) : "";

        // check for any key input to exit the scene
        //   in case the user wants to skip it :(
        if (Input.anyKey && !inputDetected && introState != IntroStates.ScreenFade2)
        {
            // allow this only once
            inputDetected = true;
            // call init scene exit function (it'll jump to the end state)
            InitSceneExit();
        }

        switch (introState)
        {
            case IntroStates.OutsideLab:
                // sometime in the future...
                if (UtilityFunctions.InTime(runTime, 2.0f))
                {
                    tmpDialogueText.text = dialogueStrings[0];
                }
                // dr. light's lab
                if (UtilityFunctions.InTime(runTime, 5.0f))
                {
                    tmpDialogueText.text = dialogueStrings[1];
                }
                if (UtilityFunctions.InTime(runTime, 10.0f))
                {
                    tmpDialogueText.text = dialogueStrings[2];
                }
                if (UtilityFunctions.InTime(runTime, 20.0f))
                {
                    tmpDialogueText.text = dialogueStrings[3];
                }
                // switch to screen fade / transition state
                if (UtilityFunctions.OverTime(runTime, 50.0f))
                {
                    introState = IntroStates.ScreenFade1;
                }
                break;
            case IntroStates.ScreenFade1:
                // progress of timer with a range of 0 to 1 like LERPing
                progress = Mathf.Clamp(fadeTimer, 0, fadeDelay) / fadeDelay;
                fadeTimer += Time.deltaTime;
                // change color alpha on all children of InsideLab game object
                // to fade in from 0 to 1
                foreach (Transform child in insideLab.transform)
                {
                    child.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, progress);
                }
                // change color alpha of dialogue text but 1.5 times faster 
                // and opposite of fading out from 1 to 0
                tmpDialogueText.color = new Color(1, 1, 1, 1f - (progress * 1.5f));
                // when progress completes
                if (progress >= 1f)
                {
                    // reset the dialogue text object and set new alignment
                    tmpDialogueText.text = "";
                    tmpDialogueText.color = Color.white;
                    tmpDialogueText.alignment = TextAlignmentOptions.TopLeft;
                    // switch to the InsideLab state
                    introState = IntroStates.InsideLab;
                }
                break;
            case IntroStates.InsideLab:
                // dr. light asks megaman to come over
                if (UtilityFunctions.InTime(runTime, 54.0f))
                {
                    tmpDialogueText.text = dialogueStrings[4];
                }
                // remove the dialogue
                if (UtilityFunctions.InTime(runTime, 57.0f))
                {
                    tmpDialogueText.text = "";
                }
                // megaman runs into the scene to x coordinate and stops
                if (UtilityFunctions.InTime(runTime, 57.0f, 58.0f))
                {
                   
                        player.GetComponent<PlayerController>().SimulateJump();
                   
                }
               
                // switch to next screen fade state (fade out the scene)
                if (UtilityFunctions.InTime(runTime, 59.0f))
                {
                    // call scene exit function (it'll move to the next state)
                    InitSceneExit();
                }
                break;
            case IntroStates.ScreenFade2:
                // progress of timer with a range of 0 to 1 like LERPing
                progress = Mathf.Clamp(fadeTimer, 0, fadeDelay) / fadeDelay;
                fadeTimer += Time.deltaTime;
                // change color alpha on all children of InsideLab game object
                // to fade out from 1 to 0
                foreach (Transform child in insideLab.transform)
                {
                    child.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1f - progress);
                }
                // fade out music by lowering the volume
                SoundManager.Instance.MusicSource.volume = musicVolume * (1f - progress);
                // when progress completes
                if (progress >= 1f)
                {
                    // make sure music volume is at zero
                    SoundManager.Instance.MusicSource.volume = 0;
                    // switch to the next scene state
                    introState = IntroStates.NextScene;
                }
                break;
            case IntroStates.NextScene:
                // tell GameManager to trigger the next scene
                if (!calledNextScene)
                {
                    GameManager.Instance.StartNextScene();
                    calledNextScene = true;
                }
                break;
        }
    }

    private void InitSceneExit()
    {
        // reset the fade timer
        fadeTimer = 0;
        // clear out the dialogue text
        tmpDialogueText.text = "";
        // hide the outside lab object (and its children) and the player
        outsideLab.SetActive(false);
        player.SetActive(false);
        // get music volume and save it
        musicVolume = SoundManager.Instance.MusicSource.volume;
        // switch to next state
        introState = IntroStates.ScreenFade2;
    }
}
