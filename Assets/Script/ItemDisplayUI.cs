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

    [Header("�׽�Ʈ ������")]
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
            //Debug.LogError("������ ������ ������ ����");
        }
        if (itemsParentTransform == null)
        {
            //Debug.LogError("������ �θ� Ʈ������ ����");
        }

        GameObject iconGO = Instantiate(itemIconPrefab, itemsParentTransform);
        Image itemImage = iconGO.GetComponent<Image>();

        if(itemImage != null)
        {
            itemImage.sprite = newItem.itemIcon;
            itemImage.preserveAspect = true;
            displayedItemIcons.Add(itemImage);
            //Debug.Log($"�߰� ������ : {newItem.itemName}");
        }
        else
        {
            //Debug.LogWarning($"������ ������ ������ XXXX : {itemIconPrefab.name}");
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
            //Debug.Log("������ ������ ����");
        }
        else
        {
            //Debug.Log("������ �� ����");
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
            //Debug.LogWarning("�׽�Ʈ �������� ã�� ����");
        }
    }

    private void TestRemoveLastItem(InputAction.CallbackContext context)
    {
        RemoveLastItemFromDisplay();
    }


}
