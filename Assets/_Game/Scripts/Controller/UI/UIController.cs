
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct UIItem
{
    public UIType type;
    public GameObject prefab;
}

public class UIController : EventTarget
{
    [SerializeField] private List<UIItem> uiItems = new(); 
    private Dictionary<UIType, GameObject> _uiItemsDict = new();

    void Awake()
    {
        foreach (var item in uiItems)
        {
            var instance = Instantiate(item.prefab, transform);
            instance.SetActive(false);
            _uiItemsDict[item.type] = instance;
        }
    }

    public void Show(UIType type)
    {
        if (_uiItemsDict.TryGetValue(type, out GameObject uiObject))
        {
            uiObject.SetActive(true);
        }
        else
        {
            var item = uiItems.Find(i => i.type == type);
            if (item.prefab != null)
            {
                var instance = Instantiate(item.prefab, transform);
                _uiItemsDict[type] = instance;
            }
            else
            {
                Debug.LogWarning($"⚠️ UIController: UIType '{type}' not found in uiItems.");
            } 
        }
    }

    public void Hide(UIType type)
    {
        if (_uiItemsDict.TryGetValue(type, out GameObject uiObject))
        {
            uiObject.SetActive(false);
        }
    }
}