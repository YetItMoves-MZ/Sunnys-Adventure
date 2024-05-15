using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.EventSystems;

public class InGameUIEngine : MonoBehaviour
{
    [SerializeField]
    List<AudioClip> audioClipsSF;
    static List<AudioClip> audioClips;
    [SerializeField]
    AudioSource AudioSourceSF;
    static AudioSource audioSource;
    [SerializeField]
    TMP_Text cherryTextSF;
    static TMP_Text cherryText;
    [SerializeField]
    TMP_Text gemTextSF;
    static TMP_Text gemText;
    [SerializeField]
    GameObject signUISF;
    static GameObject signUI;
    [SerializeField]
    Color cherryTextColorNotDoneSF;
    static Color cherryTextColorNotDone;
    [SerializeField]
    Color cherryTextColorDoneSF;
    static Color cherryTextColorDone;

    [SerializeField]
    GameObject buttonsLeftPrefab;
    [SerializeField]
    GameObject buttonsRightPrefab;
    [SerializeField]
    RectTransform buttonsLeftPrefabLocation;
    [SerializeField]
    RectTransform buttonsRightPrefabLocation;

    static int _cherryAmount;
    public static int cherryAmount
    {
        get
        {
            return _cherryAmount;
        }
        set
        {
            if (_cherryAmount - 1 == value)
            {
                audioSource.clip = audioClips[0];
                audioSource.Play();
                InGameScoreManagment.cherriesCollected++;
            }
            _cherryAmount = value;
            if (value > 0)
            {
                cherryText.color = cherryTextColorNotDone;
                cherryText.text = value.ToString();
            }
            else
            {
                cherryText.color = cherryTextColorDone;
                cherryText.text = "Done!";
                canGameEnd = true;
            }

        }
    }

    static int _gemAmount;
    public static int gemAmount
    {
        get
        {
            return _gemAmount;
        }
        set
        {
            bool isBigGem = value >= _gemAmount + 10;
            if (value > _gemAmount)
            {
                InGameScoreManagment.gemsCollected += value - _gemAmount;
            }
            _gemAmount = value;
            if (isBigGem)
            {
                audioSource.clip = audioClips[2];
                audioSource.Play();
            }
            else if (_gemAmount > 0)
            {
                audioSource.clip = audioClips[1];
                audioSource.Play();
            }
            gemText.text = value.ToString();
        }
    }

    public static bool canGameEnd;
    public static bool needToCheckHighScore;
    bool pointerJumpUpSaved = false;
    private void Start()
    {
#if UNITY_ANDROID
        SetAndroidButtons();
#endif


        needToCheckHighScore = false;
        canGameEnd = false;
        cherryText = cherryTextSF;
        gemText = gemTextSF;
        signUI = signUISF;
        cherryTextColorNotDone = cherryTextColorNotDoneSF;
        cherryTextColorDone = cherryTextColorDoneSF;
        audioSource = AudioSourceSF;
        audioClips = audioClipsSF;
        AtRoundStart();
    }

    void SetAndroidButtons()
    {
        GameObject leftButtons = Instantiate(buttonsLeftPrefab, buttonsLeftPrefabLocation.position, buttonsLeftPrefabLocation.rotation, buttonsLeftPrefabLocation);
        EventTrigger leftButton = leftButtons.transform.GetChild(0).GetComponent<EventTrigger>();
        EventTrigger rightButton = leftButtons.transform.GetChild(1).GetComponent<EventTrigger>();
        EventTrigger upButton = leftButtons.transform.GetChild(2).GetComponent<EventTrigger>();
        EventTrigger downButton = leftButtons.transform.GetChild(3).GetComponent<EventTrigger>();

        GameObject rightButtons = Instantiate(buttonsRightPrefab, buttonsRightPrefabLocation.position, buttonsRightPrefabLocation.rotation, buttonsRightPrefabLocation);
        EventTrigger jumpButton = rightButtons.transform.GetChild(0).GetComponent<EventTrigger>();


        SetEventTriggerByAxis(leftButton, EventTriggerType.PointerDown, "Horizontal", -1);
        SetEventTriggerByAxis(leftButton, EventTriggerType.PointerUp, "Horizontal", 0);

        SetEventTriggerByAxis(rightButton, EventTriggerType.PointerDown, "Horizontal", 1);
        SetEventTriggerByAxis(rightButton, EventTriggerType.PointerUp, "Horizontal", 0);

        SetEventTriggerByAxis(upButton, EventTriggerType.PointerDown, "Vertical", 1);
        SetEventTriggerByAxis(upButton, EventTriggerType.PointerUp, "Vertical", 0);

        SetEventTriggerByAxis(downButton, EventTriggerType.PointerDown, "Vertical", -1);
        SetEventTriggerByAxis(downButton, EventTriggerType.PointerUp, "Vertical", 0);

        SetEventTriggerByAxis(jumpButton, EventTriggerType.PointerClick, "Jump", 1);
        SetEventTriggerByAxis(jumpButton, EventTriggerType.PointerUp, "Jump", -1);
        SetEventTriggerByAxis(jumpButton, EventTriggerType.PointerDown, "Jump", 0.5f);
    }

    void SetEventTriggerByAxis(EventTrigger eventTrigger, EventTriggerType type, string key, float value)
    {
        EventTrigger.Entry trigger = new EventTrigger.Entry();
        trigger.eventID = type;
        trigger.callback.AddListener((data) => { OnSetAxis(key, value); });
        eventTrigger.triggers.Add(trigger);
    }

    void OnSetAxis(string key, float value)
    {
        CustomInput.SetAxis(key, value);
    }

    void AtRoundStart()
    {
        cherryAmount = GameObject.FindGameObjectsWithTag("Cherry").Length;
        gemAmount = 0;
    }

    public static void ReadSign(SignSO sign)
    {
        if (signUI.transform.GetChild(0).TryGetComponent(out TMP_Text signText))
        {
            signText.color = sign.color;
            signText.text = "";
            foreach (string line in sign.Content)
            {
                signText.text += line;
                signText.text += "\n";
            }
        }
        signUI.SetActive(true);
    }

    public static void StopReadSign()
    {
        signUI.SetActive(false);
    }


}
