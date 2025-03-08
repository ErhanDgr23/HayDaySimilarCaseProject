using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine;
using System;

public class Farmtile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public Transform ColliderParent;
    public int id;

    [SerializeField] Sprite normal, Plant, GrowPlant;
    [SerializeField] float cooldown = 50;

    private const string TimerKey = "StartTime";

    SpriteRenderer MySprite;
    Gamemanager manger;
    bool CanHarvest, Planted;

    float zmn;

    void Start()
    {
        MySprite = GetComponent<SpriteRenderer>();
        manger = Gamemanager.manger;

        if (!PlayerPrefs.HasKey("EkildiB" + id))
            PlayerPrefs.SetInt("EkildiB" + id, 0);
        else if (PlayerPrefs.GetInt("EkildiB" + id) == 1)
        {
            cooldown = manger.ItemPool[PlayerPrefs.GetInt("EkilenItemId" + id)].GrowTime;
            Planted = true;
            MySprite.sprite = Plant;
            CheckTimer(cooldown);
        }
    }

    void Update()
    {
        if (zmn >= 10 && Planted)
        {
            CheckTimer(cooldown);
            zmn = 0f;
        }
        else if(Planted)
            zmn += Time.deltaTime;
    }

    public void SetTimer()
    {
        DateTime now = DateTime.Now;
        PlayerPrefs.SetString(TimerKey + +id, now.ToString());
        PlayerPrefs.Save();
        Debug.Log($"Zaman başladı: {now}");
    }

    public bool CheckTimer(float durationInSeconds)
    {
        if (!PlayerPrefs.HasKey(TimerKey + id))
        {
            Debug.Log("Zaman ayarlanmadı!");
            return false;
        }

        DateTime startTime = DateTime.Parse(PlayerPrefs.GetString(TimerKey + id));
        TimeSpan elapsedTime = DateTime.Now - startTime;

        if (elapsedTime.TotalSeconds >= durationInSeconds)
        {
            MySprite.sprite = GrowPlant;
            CanHarvest = true;

            Debug.Log($"Süre doldu! Geçen zaman: {elapsedTime.TotalSeconds} saniye");
            return true;
        }
        else
        {
            Debug.Log($"Süre henüz dolmadı. Kalan: {durationInSeconds - elapsedTime.TotalSeconds} saniye");
            return false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (manger.SickleToolB && CanHarvest)
        {
            PlayerPrefs.SetFloat("Exp", PlayerPrefs.GetFloat("Exp") + 10f);
            PlayerPrefs.DeleteKey(TimerKey + id);
            print(PlayerPrefs.GetInt("EkilenItemId" + id));
            manger.Envantersc.Itemadd(manger.ItemPool[PlayerPrefs.GetInt("EkilenItemId" + id)], 1);
            print(manger.ItemPool[PlayerPrefs.GetInt("EkilenItemId" + id)]);
            PlayerPrefs.SetInt("EkildiB" + id, 0);
            MySprite.sprite = normal;
            CanHarvest = false;
            Planted = false;
        }
        else if(!Planted && manger.CurrentDraggingItem != null && manger.CurrentDraggingItem.type == ItemType.Seed && manger.Envantersc.PointerSelectSlot.Value > 0)
        {
            PlayerPrefs.SetInt("EkilenItemId" + id, manger.CurrentDraggingItem.GrowItemId);
            PlayerPrefs.SetInt("EkildiB" + id, 1);
            manger.Envantersc.PointerSelectSlot.Value--;
            manger.Envantersc.PointerSelectSlot.TextUpdate();
            MySprite.sprite = Plant;
            Planted = true;
            cooldown = manger.CurrentDraggingItem.GrowTime;
            SetTimer();

            if(manger.Envantersc.PointerSelectSlot.Value <= 0f)
            {
                manger.Envantersc.PointerSelectSlot.ItemRemove();
                manger.Envantersc.PointerSelectSlot = null;
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Mouse Ayrıldı!");
    }
}
