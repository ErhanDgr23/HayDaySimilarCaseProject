using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;
using Unity.VisualScripting;

[Serializable]
public class SavedSlots
{
    public int id;
    public int value;
    public int Infoid;
}

[Serializable]
public class SavedSlotsWrapper
{
    public List<SavedSlots> slots;
}

public class Envanter : MonoBehaviour {

    public Sprite[] Icons;
    public Image BaseItem;

    [Header("OtoFull")]
    public List<SavedSlots> SlotsSaved = new List<SavedSlots>();
    public Slot PointerCurrentSlot;
    public Slot PointerSelectSlot;

    [SerializeField] Transform SlotsParent;

    Gamemanager manger;
    List<Slot> slots = new List<Slot>();
    string PathFile;

    private void Start()
    {
        PathFile = Path.Combine(Application.persistentDataPath, "EnvanterData.json");
        manger = Gamemanager.manger;
        manger.GoldText.text = PlayerPrefs.GetInt("Gold").ToString();

        for (int i = 0; i < SlotsParent.childCount; i++){
            slots.Add(SlotsParent.GetChild(i).GetComponent<Slot>());
            slots[i].id = i;
        }

        Invoke("StartInvoke", 0.5f);
    }

    public void StartInvoke()
    {
        if (!File.Exists(PathFile))
        {
            Debug.LogWarning("JSON dosyası bulunamadı! Boş liste oluşturuluyor.");
            SlotsSaved = new List<SavedSlots>();
        }
        else
        {
            string json = File.ReadAllText(PathFile);
            SlotsSaved = JsonUtility.FromJson<SavedSlotsWrapper>(json).slots;
            Debug.Log("JSON Yüklendi: " + json);
        }

        if (SlotsSaved.Count > 0)
            for (int i = 0; i < SlotsSaved.Count; i++)
            {
                slots[SlotsSaved[i].id].id = SlotsSaved[i].id;
                slots[SlotsSaved[i].id].IsEmpty = false;
                slots[SlotsSaved[i].id].Value = SlotsSaved[i].value;
                slots[SlotsSaved[i].id].ItemInfo = manger.ItemPool[SlotsSaved[i].Infoid];
                slots[SlotsSaved[i].id].Icon.sprite = manger.ItemPool[SlotsSaved[i].Infoid].icon;
                slots[SlotsSaved[i].id].Icon.gameObject.SetActive(true);
                slots[SlotsSaved[i].id].TextUpdate();
            }
    }

    public void Itemadd(ItemObject info, int value)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (!slots[i].IsEmpty && slots[i].ItemInfo.id == info.id && (slots[i].Value + value) < 17)
            {
                slots[i].ItemAdd(value, info.icon, info, true);
                return;
            }
            else if(slots[i].IsEmpty)
            {
                slots[i].ItemAdd(value, info.icon, info, true);
                return;
            }
        }

        //foreach (var item in slots)
        //{
        //    if (!item.IsEmpty && item.ItemInfo != null && item.ItemInfo.id == info.id && (item.Value + value) < 17)
        //    {
        //        item.ItemAdd(value, info.icon, true);
        //        return;
        //    }
        //}

        //foreach (var item in slots)
        //{
        //    if (item.IsEmpty || item.ItemInfo != null && item.ItemInfo.id == info.id && (item.Value + value) < 17)
        //    {
        //        item.ItemAdd(value, info.icon, true);
        //    }
        //}
    }

    public void SellOrDelete(bool Money)
    {
        SendItemBack();

        if (Money)
            PlayerPrefs.SetInt("Gold", PlayerPrefs.GetInt("Gold") + (PointerSelectSlot.Value * PointerSelectSlot.ItemInfo.SellValue));

        manger.GoldText.text = PlayerPrefs.GetInt("Gold").ToString();
        PointerSelectSlot.ItemRemove();
    }

    public void Update()
    {
        if (PointerSelectSlot != null && PointerSelectSlot.Value > 0)
        {
            BaseItem.transform.position = Input.mousePosition;
            BaseItem.gameObject.SetActive(true);
        }
        else
            BaseItem.gameObject.SetActive(false);

        if (Input.GetButtonUp("Fire1"))
        {
            if (PointerCurrentSlot != null && PointerCurrentSlot != PointerSelectSlot)
            {
                if (PointerCurrentSlot.IsEmpty && PointerCurrentSlot.CanItemPlaceable && PointerSelectSlot.Value > 0)
                {
                    if(PointerCurrentSlot.PlaceableItemId == -1 || PointerCurrentSlot.PlaceableItemId != -1 && PointerSelectSlot.ItemInfo.type == ItemType.Plant)
                    {
                        print("added");
                        PointerCurrentSlot.ItemAdd(PointerSelectSlot.Value, PointerSelectSlot.Icon.sprite, PointerSelectSlot.ItemInfo);
                        PointerSelectSlot.ItemRemove();
                    }
                    else
                        SendItemBack();
                }
                else if (!PointerCurrentSlot.IsEmpty && PointerCurrentSlot.ItemInfo.id == PointerSelectSlot.ItemInfo.id && (PointerCurrentSlot.Value + PointerSelectSlot.Value) < 17 && PointerCurrentSlot.CanItemPlaceable && PointerSelectSlot.Value > 0)
                {
                    if(PointerCurrentSlot.PlaceableItemId == -1 || PointerCurrentSlot.PlaceableItemId != -1 && PointerSelectSlot.ItemInfo.type == ItemType.Plant)
                    {
                        print("addedwithvalue");
                        PointerCurrentSlot.ItemAdd(PointerSelectSlot.Value, PointerSelectSlot.Icon.sprite, PointerSelectSlot.ItemInfo, true);
                        PointerSelectSlot.ItemRemove();
                    }
                    else
                        SendItemBack();
                }
                else if (PointerSelectSlot != null)
                    SendItemBack();
            }
            else if(PointerSelectSlot != null)
            {
                if(PointerSelectSlot.Value > 0)
                    PointerSelectSlot.Icon.gameObject.SetActive(true);
                else
                    PointerSelectSlot.Icon.gameObject.SetActive(false);
                BaseItem.transform.position = Vector3.zero;
                BaseItem.gameObject.SetActive(false);
            }

            PointerSelectSlot = null;
            manger.CurrentDraggingItem = null;
        }
    }

    public void SendItemBack()
    {
        print("cantadded");
        PointerSelectSlot.Icon.gameObject.SetActive(true);
        BaseItem.transform.position = Vector3.zero;
        BaseItem.gameObject.SetActive(false);
    }

    public void SaveEnvanter()
    {
        foreach (var item in slots)
            item.DeleteSlot();

        foreach (var item in slots)
        {
            if(item.ItemInfo != null)
                item.SaveSlot();
        }

        SavedSlotsWrapper wrapper = new SavedSlotsWrapper { slots = SlotsSaved };
        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(PathFile, json);
        Debug.Log("JSON Kaydedildi: " + json);
    }

    public void DeleteJsonFile()
    {
        if (File.Exists(PathFile))
        {
            File.Delete(PathFile);
            print("Kayıt Silindi");
        }
    }
}
