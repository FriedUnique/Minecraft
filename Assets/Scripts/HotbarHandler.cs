using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class HotbarHandler : MonoBehaviour
{
    public Image cursor;
    public GameObject slotItem;
    public Sprite slotPlaceholder;

    private List<InventoryObject> inventory;
    private GameObject[] itemSlots = new GameObject[9];

    public int currentSlot = 0;
    private const int maxSize = 9;

    private void Start() {
        // setup the icons

        inventory = new List<InventoryObject>(9) { null, null, null, null, null, null, null, null, null };

        for (int i = 0; i < maxSize; i++) {
            itemSlots[i] = Instantiate(slotItem, new Vector3(0, 0, 0), Quaternion.identity);
            itemSlots[i].transform.SetParent(transform);
            itemSlots[i].transform.localPosition = new Vector3(88 * i - 88 * 4, 0f, 0f);
        }

        RenderInventory();
    }

    private void Update() {
        inputHandeling();

        cursor.transform.localPosition = new Vector3(88 * currentSlot - 88*4, 0, 0);
        RenderInventory();
    }

    private void RenderInventory() {
        for (int i = 0; i < inventory.Count; i++) {
            if (inventory[i] == null || inventory[i].amount == 0) {
                itemSlots[i].GetComponentInChildren<Image>().sprite = slotPlaceholder;
                itemSlots[i].GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "";
                continue;
            }

            itemSlots[i].GetComponentInChildren<Image>().sprite = inventory[i].itemSprite;
            itemSlots[i].GetComponentInChildren<TMPro.TextMeshProUGUI>().text = inventory[i].amount.ToString();
        }
    }

    private void inputHandeling() {
        if (Input.GetAxis("Mouse ScrollWheel") < 0) {
            if (currentSlot >= maxSize - 1) {
                currentSlot = 0;
            } else {
                currentSlot++;
            }
        } else if (Input.GetAxis("Mouse ScrollWheel") > 0) {
            if (currentSlot <= 0) {
                currentSlot = maxSize - 1;
            } else {
                currentSlot--;
            }
        }

        #region neger code
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            currentSlot = 0;
        } else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            currentSlot = 1;
        } else if (Input.GetKeyDown(KeyCode.Alpha3)) {
            currentSlot = 2;
        } else if (Input.GetKeyDown(KeyCode.Alpha4)) {
            currentSlot = 3;
        } else if (Input.GetKeyDown(KeyCode.Alpha5)) {
            currentSlot = 4;
        } else if (Input.GetKeyDown(KeyCode.Alpha6)) {
            currentSlot = 5;
        } else if (Input.GetKeyDown(KeyCode.Alpha7)) {
            currentSlot = 6;
        } else if (Input.GetKeyDown(KeyCode.Alpha8)) {
            currentSlot = 7;
        } else if (Input.GetKeyDown(KeyCode.Alpha9)) {
            currentSlot = 8;
        }
        #endregion
    }

    public void AddBlockToInventory(TextureAtlas.BlockType block, int amount = 1) {
        for(int j=0; j< inventory.Count; j++) {
            if(inventory[j] == null) { continue; }

            if(inventory[j].blockType == block && inventory[j].amount < 64) {
                inventory[j].amount += amount; return;
            }
        }

        int i = inventory.FindIndex(obj => obj == null);
        if(i == -1) { Debug.Log("Inventory full"); return; }

        inventory[i] = new InventoryObject(block, amount);
    }

    public TextureAtlas.BlockType UseCurrentBlock() {
        if (inventory[currentSlot] == null) { return TextureAtlas.BlockType.Air; }

        if (inventory[currentSlot].amount == 0) { 
            inventory[currentSlot] = null;
            return TextureAtlas.BlockType.Air;
        }

        inventory[currentSlot].amount--;
        return inventory[currentSlot].blockType;
    }

}

public class InventoryObject {
    public TextureAtlas.BlockType blockType;
    public int amount;
    public Sprite itemSprite;

    public InventoryObject(TextureAtlas.BlockType blockType, int amount) {
        this.blockType = blockType;
        this.amount = amount;
        this.itemSprite = TerrainGenerator.instance.atlas.GetSpriteFromBlock(TextureAtlas.BlockType.Grass);
    }
}
