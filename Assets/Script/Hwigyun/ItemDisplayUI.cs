using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;

[System.Serializable]
public class ItemData
{
    public string itemName;
    public Sprite itemIcon;

    public ItemData(string name, Sprite icon)
    {
        itemName = name;
        itemIcon = icon;
    }
}

public class ItemDisplayUI : MonoBehaviour
{
    public GameObject itemIconPrefab;
    public Transform itemsParentTransform;

    [Header("테스트 아이템")]
    public List<ItemData> testItems;

    private List<Image> displayedItemIcons = new List<Image>();

    void OnEnable()
    {
        if(InputManager.Instance != null &&  InputManager.Instance.PlayerInputActions != null)
        {
            InputManager.Instance.PlayerInputActions.UI.EquippedItem.performed += TestEquippedItem;
            InputManager.Instance.PlayerInputActions.UI.RemoveLastItem.performed += TestRemoveLastItem;
        }
    }

    void OnDisable()
    {
        if (InputManager.Instance != null && InputManager.Instance.PlayerInputActions != null)
        {
            InputManager.Instance.PlayerInputActions.UI.EquippedItem.performed -= TestEquippedItem;
            InputManager.Instance.PlayerInputActions.UI.RemoveLastItem.performed -= TestRemoveLastItem;
        }
    }

    void Start()
    {
        ClearDisplayedItems();
    }

    public void AddItemToDisplay(ItemData newItem)
    {
        if(itemIconPrefab == null)
        {
            //Debug.LogError("아이템 아이콘 프리팹 없음");
        }
        if (itemsParentTransform == null)
        {
            //Debug.LogError("아이템 부모 트랜스폼 없음");
        }

        GameObject iconGO = Instantiate(itemIconPrefab, itemsParentTransform);
        Image itemImage = iconGO.GetComponent<Image>();

        if(itemImage != null)
        {
            itemImage.sprite = newItem.itemIcon;
            itemImage.preserveAspect = true;
            displayedItemIcons.Add(itemImage);
            //Debug.Log($"추가 아이템 : {newItem.itemName}");
        }
        else
        {
            //Debug.LogWarning($"아이템 아이콘 프리팹 XXXX : {itemIconPrefab.name}");
            Destroy(iconGO);
        }
    }

    public void RemoveLastItemFromDisplay()
    {
        if(displayedItemIcons.Count > 0)
        {
            Image lastIcon = displayedItemIcons[displayedItemIcons.Count - 1];
            displayedItemIcons.RemoveAt(displayedItemIcons.Count - 1);
            Destroy(lastIcon.gameObject);
            //Debug.Log("마지막 아이템 지움");
        }
        else
        {
            //Debug.Log("아이템 못 지움");
        }
    }

    public void ClearDisplayedItems()
    {
        foreach(Image icon in displayedItemIcons)
        {
            Destroy(icon.gameObject);
        }
        displayedItemIcons.Clear();
    }

    private int testItemIndex = 0;
    private void TestEquippedItem(InputAction.CallbackContext context)
    {
        if(testItems.Count > 0)
        {
            ItemData itemToAdd = testItems[testItemIndex];
            AddItemToDisplay(itemToAdd);
            testItemIndex = (testItemIndex + 1) % testItems.Count;
        }
        else
        {
            //Debug.LogWarning("테스트 아이템을 찾지 못함");
        }
    }

    private void TestRemoveLastItem(InputAction.CallbackContext context)
    {
        RemoveLastItemFromDisplay();
    }


}
