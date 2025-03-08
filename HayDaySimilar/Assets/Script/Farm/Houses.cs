using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.EventSystems;

public class Houses : MonoBehaviour, IPointerDownHandler{

    public Transform ColliderParent;
    public int Houseid;

    [SerializeField] GameObject HouseCraft, HouseJob;
    [SerializeField] TextMeshProUGUI JobCountValueT;
    [SerializeField] Image JobTimerImage;
    [SerializeField] Slot IngredientSlot;
    [SerializeField] Slot CraftResSlot;
    [SerializeField] float JobTimer;

    ItemObject ItemInfo, seedInfo;
    Gamemanager manger;
    bool CanTakeItem;
    [SerializeField] bool ThereIsJob;
    [SerializeField] int jobvalue;
    float zmn;

    private void Start()
    {
        manger = Gamemanager.manger;

        if (PlayerPrefs.HasKey("JobVal" + Houseid))
        {
            jobvalue = PlayerPrefs.GetInt("JobVal" + Houseid);
            ItemInfo = manger.ItemPool[PlayerPrefs.GetInt("CurrentItemId" + Houseid)];
            seedInfo = manger.ItemPool[ItemInfo.SeedItemId];
            //CraftResSlot.ItemAdd(PlayerPrefs.GetInt("ItemCountHolder" + Houseid), seedInfo.icon, seedInfo, true);

            if (PlayerPrefs.GetInt("JobVal" + Houseid) != 0)
            {
                ThereIsJob = true;
                IngredientSlot.CanItemPlaceable = false;
                IngredientSlot.transform.GetChild(2).gameObject.SetActive(true);
            }

            JobCountValueT.text = jobvalue.ToString();
            CheckAndRunTasks(false);
        }
    }

    public void ItemAddEnvanter()
    {
        if (!CanTakeItem)
        {
            PlayerPrefs.SetInt("CurrentItemId" + Houseid, ItemInfo.id);
            PlayerPrefs.SetInt("JobVal" + Houseid, jobvalue);
            PlayerPrefs.SetString("lastExitTime" + Houseid, DateTime.Now.ToString());
            PlayerPrefs.SetInt("ItemCountHolder" + Houseid, 0);

            manger.Envantersc.Itemadd(seedInfo, CraftResSlot.Value);
            CraftResSlot.ItemRemove();
        }
    }

    private void Update()
    {
        if (zmn >= JobTimer && ThereIsJob)
        {
            //CraftResSlot.ItemAdd(5, seedInfo.icon, seedInfo, true);
            //jobvalue--;
            //PlayerPrefs.SetInt("JobVal" + Houseid, jobvalue);
            //PlayerPrefs.SetInt("ItemCountHolder" + Houseid, CraftResSlot.Value);
            //JobCountValueT.text = jobvalue.ToString();
            CheckAndRunTasks(true);
            if (jobvalue <= 0)
                ThereIsJob = false;

            zmn = 0f;
        }
        else if (ThereIsJob)
        {
            zmn += Time.deltaTime;
            JobTimerImage.fillAmount = zmn / JobTimer;
        }

        if (!manger.EnvanterActive)
        {
            HouseCraft.gameObject.SetActive(false);

            if(!ThereIsJob)
                HouseJob.gameObject.SetActive(false);
            else
                HouseJob.gameObject.SetActive(true);
        }
    }

    private void CheckAndRunTasks(bool SaveTime)
    {
        if (!PlayerPrefs.HasKey("lastExitTime" + Houseid)) return;

        string lastExitTimeStr = PlayerPrefs.GetString("lastExitTime" + Houseid);
        DateTime lastExitTime;

        if (!DateTime.TryParse(lastExitTimeStr, out lastExitTime))
            return;

        TimeSpan timeElapsed = DateTime.Now - lastExitTime;
        int cyclesPassed = Mathf.FloorToInt((float)timeElapsed.TotalSeconds / JobTimer);
        cyclesPassed = Mathf.Clamp(cyclesPassed, 0, jobvalue);
        CraftResSlot.ItemAdd(5 * cyclesPassed, seedInfo.icon, seedInfo, true);
        jobvalue -= cyclesPassed;

        if(SaveTime)
            PlayerPrefs.SetString("lastExitTime" + Houseid, DateTime.Now.ToString());

        JobCountValueT.text = jobvalue.ToString();

        if (jobvalue <= 0)
        {
            ThereIsJob = false;
            IngredientSlot.CanItemPlaceable = true;
            IngredientSlot.transform.GetChild(2).gameObject.SetActive(false);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        manger.EnvanterCloseOrOpen(true);

        if (manger.EnvanterActive)
        {
            HouseCraft.gameObject.SetActive(true);
            HouseJob.gameObject.SetActive(false);
        }
        else if (ThereIsJob)
        {
            HouseCraft.gameObject.SetActive(false);
            HouseJob.gameObject.SetActive(true);
        }
    }

    public void CraftItems()
    {
        CancelInvoke("CraftItemsInvoke");
        Invoke("CraftItemsInvoke", 0.1f);
    }

    public void CraftItemsInvoke()
    {
        if (ThereIsJob)
            return;

        if (IngredientSlot.Value > 0)
        {
            ItemObject SeedObj = manger.ItemPool[IngredientSlot.ItemInfo.SeedItemId];
            CraftResSlot.ItemAdd(IngredientSlot.Value * 5, SeedObj.icon, SeedObj);
            CanTakeItem = true;
        }
        else
        {
            CraftResSlot.ItemRemove();
        }
    }

    public void Craft()
    {
        CraftResSlot.ItemRemove();
        CanTakeItem = false;

        if (IngredientSlot.Value > 0)
        {
            jobvalue = IngredientSlot.Value;
            ItemInfo = IngredientSlot.ItemInfo;
            seedInfo = manger.ItemPool[IngredientSlot.ItemInfo.SeedItemId];
            IngredientSlot.ItemRemove();
            JobCountValueT.text = jobvalue.ToString();
            ThereIsJob = true;
            IngredientSlot.CanItemPlaceable = false;
            IngredientSlot.transform.GetChild(2).gameObject.SetActive(true);

            PlayerPrefs.SetInt("CurrentItemId" + Houseid, ItemInfo.id);
            PlayerPrefs.SetInt("JobVal" + Houseid, jobvalue);
            PlayerPrefs.SetString("lastExitTime" + Houseid, DateTime.Now.ToString());
        }
    }
}
