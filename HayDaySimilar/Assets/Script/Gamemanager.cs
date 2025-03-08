using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Gamemanager : MonoBehaviour
{
    public static Gamemanager manger;

    public ItemObject CurrentDraggingItem;
    public bool HoeToolB, SickleToolB, BuildB, EnvanterActive;

    public Camera mainkam;
    public Envanter Envantersc;
    public KameraSc Kamsc;
    public TileController TileContsc;
    public ItemObject[] ItemPool;
    public TextMeshProUGUI GoldText;

    [SerializeField] TextMeshProUGUI NameText, ExpText;
    [SerializeField] GameObject EnvanterObj;
    [SerializeField] GameObject NicknamePan;

    Vector2 ObjFirstPos;
    GameObject followobj;
    string strname;

    private void Awake()
    {
        manger = this;
    }

    public void SubmitText(TMP_InputField field)
    {
        strname = field.text;
    }

    public void SubmitButton()
    {
        NameText.text = strname;
        PlayerPrefs.SetString("Nick", strname);
        NicknamePan.gameObject.SetActive(false);
    }

    private void Start()
    {
        if (!PlayerPrefs.HasKey("Nick"))
        {
            Envantersc.DeleteJsonFile();
            Envantersc.Itemadd(ItemPool[1], 3);
            Envantersc.Itemadd(ItemPool[3], 3);
            NicknamePan.gameObject.SetActive(true);
        }
        else
        {
            NicknamePan.gameObject.SetActive(false);
            strname = PlayerPrefs.GetString("Nick");
            NameText.text = strname;
        }

        EnvanterObj.gameObject.SetActive(false);
        EnvanterActive = false;
    }

    public void EnvanterCloseOrOpen(bool Tbool)
    {
        EnvanterObj.gameObject.SetActive(Tbool);
        EnvanterActive = Tbool;
    }

    private void Update()
    {
        ExpText.text = PlayerPrefs.GetFloat("Exp").ToString();
        Kamsc.enabled = !EnvanterObj.activeSelf;

        if(followobj != null)
        {
            followobj.transform.position = Input.mousePosition;
            print("Hareket");

            if(Input.GetButtonUp("Fire1"))
            {
                print("NoHareket");
                SickleToolB = false;
                followobj.transform.position = ObjFirstPos;
                followobj = null;
                manger.Kamsc.CantMoveableB = false;
            }
        }
    }

    public void HoetoolV(Button but)
    {
        HoeToolB = !HoeToolB;
        but.interactable = !HoeToolB;
    }

    public void SickletoolV(GameObject obj)
    {
        manger.Kamsc.CantMoveableB = true;
        ObjFirstPos = obj.transform.position;
        followobj = obj;
        SickleToolB = !SickleToolB;
    }
}
