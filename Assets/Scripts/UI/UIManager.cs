using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    private BuildingPlacer _buildingPlacer;

    public Transform buildingMenu;
    public GameObject buildingButtonPrefab;

    public Transform resourcesUIParent;
    public GameObject gameResourcesDisplayPrefab;

    public Transform selectedUnitsListParent;
    public GameObject selectedUnitDisplayPrefab;
    public GameObject gameResourceCostPrefab;
    public GameObject selectedUnitMenu;
    private RectTransform _selectedUnitContentRectTransform;
    private RectTransform _selectedUnitButtonsRectTransform;
    private TextMeshProUGUI _selectedUnitTitleText;
    private TextMeshProUGUI _selectedUnitLevelText;
    private Transform _selectedUnitResourcesProductionParent;
    private Transform _selectedUnitActionButtonsParent;

    public Transform selectionGroupsParent;

    private Dictionary<string, TextMeshProUGUI> _resourceTexts;
    private Dictionary<string, Button> _buildingButtons;

    private void Awake()
    {
        _buildingPlacer = GetComponent<BuildingPlacer>();
        InitResources();
        InitBuildingButtons();
        ToggleAllSelectionGroupButtons();
        InitSelectedUnitMenu();

        _ShowSelectedUnitMenu(false);
    }

    private void ToggleAllSelectionGroupButtons()
    {
        for (int i = 1; i <= 9; i++)
            ToggleSelectionGroupButton(i, false);
    }

    public void ToggleSelectionGroupButton(int groupIndex, bool on)
    {
        selectionGroupsParent.Find(groupIndex.ToString()).gameObject.SetActive(on);
    }

    private void InitResources()
    {
        _resourceTexts = new Dictionary<string, TextMeshProUGUI>();

        foreach (KeyValuePair<string, GameResource> pair in Globals.GAME_RESOURCES)
        {
            GameObject display = Instantiate(gameResourcesDisplayPrefab, resourcesUIParent);
            display.name = pair.Key;
            _resourceTexts[pair.Key] = display.transform.Find("Text").GetComponent<TextMeshProUGUI>();
            SetResourceText(pair.Key, pair.Value.Amount);
            display.transform.Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>(
                    $"Textures/GameResources/{pair.Key}");
        }
    }

    private void InitBuildingButtons()
    {
        _buildingButtons = new Dictionary<string, Button>();

        for (int i = 0; i < Globals.BUILDING_DATA.Length; i++)
        {
            BuildingData data = Globals.BUILDING_DATA[i];
            GameObject button = Instantiate(buildingButtonPrefab, buildingMenu);
            Button b = button.GetComponent<Button>();

            button.name = data.unitName;
            button.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = data.unitName;
            _buildingButtons[data.code] = b;

            AddBuildingButtonListener(b, i);

            if (!Globals.BUILDING_DATA[i].CanBuy())
            {
                b.interactable = false;
            }

            button.GetComponent<BuildingButton>().Initialize(data);
        }
    }

    private void InitSelectedUnitMenu()
    {
        Transform selectedUnitMenuTransform = selectedUnitMenu.transform;
        _selectedUnitContentRectTransform = selectedUnitMenuTransform
            .Find("Content").GetComponent<RectTransform>();
        _selectedUnitButtonsRectTransform = selectedUnitMenuTransform
            .Find("Buttons").GetComponent<RectTransform>();
        _selectedUnitTitleText = selectedUnitMenuTransform
            .Find("Content/GeneralInfo/Title").GetComponent<TextMeshProUGUI>();
        _selectedUnitLevelText = selectedUnitMenuTransform
            .Find("Content/GeneralInfo/Level").GetComponent<TextMeshProUGUI>();
        _selectedUnitResourcesProductionParent = selectedUnitMenuTransform
            .Find("Content/ResourcesProduction");
        _selectedUnitActionButtonsParent = selectedUnitMenuTransform
            .Find("Buttons/SpecificActions");
    }

    private void OnEnable()
    {
        EventManager.AddListener("UpdateResourceTexts", OnUpdateResourceTexts);
        EventManager.AddListener("CheckBuildingButtons", OnCheckBuildingButtons);
        EventManager.AddTypedListener("SelectUnit", OnSelectUnit);
        EventManager.AddTypedListener("DeselectUnit", OnDeselectUnit);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener("UpdateResourceTexts", OnUpdateResourceTexts);
        EventManager.RemoveListener("CheckBuildingButtons", OnCheckBuildingButtons);
        EventManager.RemoveTypedListener("SelectUnit", OnSelectUnit);
        EventManager.RemoveTypedListener("DeselectUnit", OnDeselectUnit);
    }

    private void OnUpdateResourceTexts()
    {
        foreach (KeyValuePair<string, GameResource> pair in Globals.GAME_RESOURCES)
            SetResourceText(pair.Key, pair.Value.Amount);
    }

    private void SetResourceText(string resource, int value)
    {
        _resourceTexts[resource].text = value.ToString();
    }

    private void OnCheckBuildingButtons()
    {
        foreach (BuildingData data in Globals.BUILDING_DATA)
            _buildingButtons[data.code].interactable = data.CanBuy();
    }

    private void AddBuildingButtonListener(Button b, int i)
    {
        b.onClick.AddListener(() => _buildingPlacer.SelectPlacedBuilding(i));
    }

    private void OnSelectUnit(CustomEventData data)
    {
        AddSelectedUnitToUIList(data.unit);
        _SetSelectedUnitMenu(data.unit);
        _ShowSelectedUnitMenu(true);
    }

    private void OnDeselectUnit(CustomEventData data)
    {
        RemoveSelectedUnitFromUIList(data.unit.Code);
        if (Globals.SELECTED_UNITS.Count == 0)
            _ShowSelectedUnitMenu(false);
        else
            _SetSelectedUnitMenu(Globals.SELECTED_UNITS[^1].Unit);
    }

    private void _SetSelectedUnitMenu(Unit unit)
    {
        int contentHeight = 60 + unit.Production.Count * 16;
        _selectedUnitContentRectTransform.sizeDelta = new Vector2(64, contentHeight);
        _selectedUnitButtonsRectTransform.anchoredPosition = new Vector2(0, -contentHeight - 20);
        _selectedUnitButtonsRectTransform.sizeDelta = new Vector2(70, Screen.height - contentHeight - 20);
        _selectedUnitTitleText.text = unit.Data.unitName;
        _selectedUnitLevelText.text = $"Level {unit.Level}";
        foreach (Transform child in _selectedUnitResourcesProductionParent)
            Destroy(child.gameObject);
        if (unit.Production.Count > 0)
        {
            GameObject g; Transform t;
            foreach (ResourceValue resource in unit.Production)
            {
                g = GameObject.Instantiate(
                    gameResourceCostPrefab, _selectedUnitResourcesProductionParent);
                t = g.transform;
                t.Find("Text").GetComponent<TextMeshProUGUI>().text = $"+{resource.amount}";
                t.Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>($"Textures/GameResources/{resource.code}");
            }
        }
    }

    private void _ShowSelectedUnitMenu(bool show)
    {
        selectedUnitMenu.SetActive(show);
    }

    public void AddSelectedUnitToUIList(Unit unit)
    {
        Transform alreadyInstantiatedChild = selectedUnitsListParent.Find(unit.Code);
        if (alreadyInstantiatedChild != null)
        {
            TextMeshProUGUI t = alreadyInstantiatedChild.Find("Count").GetComponent<TextMeshProUGUI>();
            int count = int.Parse(t.text);
            t.text = (count + 1).ToString();
        }
        else
        {
            GameObject g = GameObject.Instantiate(
                selectedUnitDisplayPrefab, selectedUnitsListParent);
            g.name = unit.Code;
            Transform t = g.transform;
            t.Find("Count").GetComponent<TextMeshProUGUI>().text = "1";
            t.Find("Name").GetComponent<TextMeshProUGUI>().text = unit.Data.unitName;
        }
    }

    public void RemoveSelectedUnitFromUIList(string code)
    {
        Transform listItem = selectedUnitsListParent.Find(code);
        if (listItem == null) return;
        TextMeshProUGUI t = listItem.Find("Count").GetComponent<TextMeshProUGUI>();
        int count = int.Parse(t.text);
        count -= 1;
        if (count == 0)
            DestroyImmediate(listItem.gameObject);
        else
            t.text = count.ToString();
    }
}
