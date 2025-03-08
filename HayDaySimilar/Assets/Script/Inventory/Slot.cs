using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class Slot : MonoBehaviour {

    public ItemObject ItemInfo;
    public TextMeshProUGUI ValueT;
    public Envanter envantersc;
    public Image Icon;
    public bool IsEmpty = true, CanItemPlaceable = true;
    public int PlaceableItemId = -1;
    public int Value;
    public int id;

    void Start () 
    {
        envantersc = Gamemanager.manger.Envantersc;
        TextUpdate();
    }

    public void TextUpdate()
    {
        ValueT.text = Value.ToString();
    }

    public void PointerExit()
    {
        envantersc.PointerCurrentSlot = null;
    }

    public void PointerEnter()
    {
        envantersc.PointerCurrentSlot = this;
    }

    public void PointerDown()
    {
        if (!CanItemPlaceable)
            return;

        Gamemanager.manger.CurrentDraggingItem = ItemInfo;
        envantersc.BaseItem.sprite = Icon.sprite;
        Icon.gameObject.SetActive(false);
        envantersc.PointerSelectSlot = this;
    }

    public void ItemAdd(int Tvalue, Sprite Ticon, ItemObject Info, bool merge = false)
    {
        if (Tvalue <= 0)
            return;

        print("Itemadded" + transform.name);
        IsEmpty = false;
        ItemInfo = Info;

        if (!merge)
            Value = Tvalue;
        else
            Value += Tvalue;

        Icon.sprite = Ticon;
        Icon.gameObject.SetActive(true);
        TextUpdate();
    }

    public void ItemRemove()
    {
        print("ItemRemoved" + transform.name);
        ItemInfo = null;
        Icon.sprite = null;
        Icon.gameObject.SetActive(false);
        Value = 0;
        IsEmpty = true;
        TextUpdate();
    }

    public void SaveSlot()
    {
        SavedSlots existingSlot = envantersc.SlotsSaved.Find(slot => slot.id == id);

        if (existingSlot != null)
        {
            // Eğer bu id'ye sahip bir obje zaten listede varsa, değişkenleri güncelle
            existingSlot.id = id;
            existingSlot.value = Value;
            existingSlot.Infoid = ItemInfo.id;

            Debug.Log("Slot güncellendi: " + id);
        }
        else
        {
            // Eğer listede yoksa, yeni olarak ekle
            SavedSlots newSlot = new SavedSlots
            {
                id = id,
                value = Value,
                Infoid = ItemInfo.id
            };

            envantersc.SlotsSaved.Add(newSlot);
            Debug.Log("Yeni slot eklendi: " + id);
        }
    }

    public void DeleteSlot()
    {
        SavedSlots existingSlot = envantersc.SlotsSaved.Find(slot => slot.id == id);

        envantersc.SlotsSaved.Remove(existingSlot);
    }

    //public void SaveSlot()
    //{
    //    SavedSlots SlotInfos = new SavedSlots();
    //    SlotInfos.id = id;
    //    SlotInfos.value = Value;
    //    SlotInfos.Infoid = ItemInfo.id;
    //    envantersc.SlotsSaved.Add(SlotInfos);
    //}
}
