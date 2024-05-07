using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

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
    GameObject buttonsPrefab;
    [SerializeField]
    RectTransform buttonsPrefabLocation;

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
    private void Start()
    {
#if UNITY_ANDROID
        GameObject buttons = Instantiate(buttonsPrefab,buttonsPrefabLocation,buttonsPrefabLocation.position,buttonsPrefabLocation.rotation);
        Button leftButton = buttons.transform.Getchild(0).GetComponent<Button>();
        Button rightButton = buttons.transform.Getchild(1).GetComponent<Button>();
        Button upButton = buttons.transform.Getchild(2).GetComponent<Button>();
        Button downButton = buttons.transform.Getchild(3).GetComponent<Button>();
        Button jumpButton = buttons.transform.Getchild(4).GetComponent<Button>();
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
