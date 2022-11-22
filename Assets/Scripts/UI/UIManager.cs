using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    private BuildingPlacer buildingPlacer;

    public Transform buildingMenu;
    public GameObject buildingButtonPrefab;

    public Transform resourcesUIParent;
    public GameObject gameResourcesDisplayPrefab;

    public Transform selectedUnitsListParent;
    public GameObject selectedUnitDisplayPrefab;
    public GameObject gameResourceCostPrefab;
    public GameObject selectedUnitMenu;
    private RectTransform selectedUnitContentRectTransform;
    private RectTransform selectedUnitButtonsRectTransform;
    private TextMeshProUGUI selectedUnitTitleText;
    private TextMeshProUGUI selectedUnitLevelText;
    private Transform selectedUnitResourcesProductionParent;
    private Transform selectedUnitActionButtonsParent;

    private Entity selectedEntity;
    public GameObject unitSkillButtonPrefab;

    public Transform selectionGroupsParent;

    private Dictionary<string, TextMeshProUGUI> resourceTexts;
    private Dictionary<string, Button> buildingButtons;

    private void Awake()
    {
        buildingPlacer = GetComponent<BuildingPlacer>();
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
        resourceTexts = new Dictionary<string, TextMeshProUGUI>();

        foreach (KeyValuePair<string, GameResource> pair in Globals.GAME_RESOURCES)
        {
            GameObject display = Instantiate(gameResourcesDisplayPrefab, resourcesUIParent);
            display.name = pair.Key;
            resourceTexts[pair.Key] = display.transform.Find("Text").GetComponent<TextMeshProUGUI>();
            SetResourceText(pair.Key, pair.Value.Amount);
            display.transform.Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>(
                    $"Textures/GameResources/{pair.Key}");
        }
    }

    private void InitBuildingButtons()
    {
        buildingButtons = new Dictionary<string, Button>();

        for (int i = 0; i < Globals.BUILDING_DATA.Length; i++)
        {
            BuildingData data = Globals.BUILDING_DATA[i];
            GameObject button = Instantiate(buildingButtonPrefab, buildingMenu);
            Button b = button.GetComponent<Button>();

            button.name = data.unitName;
            button.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = data.unitName;
            buildingButtons[data.code] = b;

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
        selectedUnitContentRectTransform = selectedUnitMenuTransform
            .Find("Content").GetComponent<RectTransform>();
        selectedUnitButtonsRectTransform = selectedUnitMenuTransform
            .Find("Buttons").GetComponent<RectTransform>();
        selectedUnitTitleText = selectedUnitMenuTransform
            .Find("Content/GeneralInfo/Title").GetComponent<TextMeshProUGUI>();
        selectedUnitLevelText = selectedUnitMenuTransform
            .Find("Content/GeneralInfo/Level").GetComponent<TextMeshProUGUI>();
        selectedUnitResourcesProductionParent = selectedUnitMenuTransform
            .Find("Content/ResourcesProduction");
        selectedUnitActionButtonsParent = selectedUnitMenuTransform
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
        resourceTexts[resource].text = value.ToString();
    }

    private void OnCheckBuildingButtons()
    {
        foreach (BuildingData data in Globals.BUILDING_DATA)
            buildingButtons[data.code].interactable = data.CanBuy();
    }

    private void AddBuildingButtonListener(Button b, int i)
    {
        b.onClick.AddListener(() => buildingPlacer.SelectPlacedBuilding(i));
    }

    private void OnSelectUnit(CustomEventData data)
    {
        AddSelectedUnitToUIList(data.Entity);
        _SetSelectedUnitMenu(data.Entity);
        _ShowSelectedUnitMenu(true);
    }

    private void OnDeselectUnit(CustomEventData data)
    {
        RemoveSelectedUnitFromUIList(data.Entity.Code);
        if (Globals.SELECTED_UNITS.Count == 0)
            _ShowSelectedUnitMenu(false);
        else
            _SetSelectedUnitMenu(Globals.SELECTED_UNITS[^1].Entity);
    }

    private void _SetSelectedUnitMenu(Entity entity)
    {
        selectedEntity = entity;
        
        SetSelectedUnitMenuUI(selectedEntity);
        SetUnitProduction(selectedEntity);
        SetSkillButtons(selectedEntity);
    }
    
    private void SetSelectedUnitMenuUI(Entity entity)
    {
        int contentHeight = 120 + entity.Production.Count * 16;

        selectedUnitContentRectTransform.sizeDelta = new Vector2(64, contentHeight);

        selectedUnitButtonsRectTransform.anchoredPosition = new Vector2(0, 0);
        selectedUnitButtonsRectTransform.sizeDelta = new Vector2(0, Screen.height - contentHeight);

        selectedUnitTitleText.text = entity.Data.unitName;
        selectedUnitLevelText.text = $"Level {entity.Level}";
    }

    public void SetUnitProduction(Entity entity)
    {
        foreach (Transform child in selectedUnitResourcesProductionParent)
            Destroy(child.gameObject);
        
        if (entity.Production.Count <= 0) return;
        foreach (ResourceValue resource in entity.Production)
        {
            GameObject g = GameObject.Instantiate(
                gameResourceCostPrefab, selectedUnitResourcesProductionParent);
            Transform t = g.transform;
            t.Find("Text").GetComponent<TextMeshProUGUI>().text = $"+{resource.amount}";
            t.Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>($"Textures/GameResources/{resource.code}");
        }
    }

    private void SetSkillButtons(Entity entity)
    {
        foreach (Transform child in selectedUnitActionButtonsParent)
            Destroy(child.gameObject);
        
        if (entity.SkillManagers.Count <= 0) return;
        for (int i = 0; i < entity.SkillManagers.Count; i++)
        {
            GameObject g = GameObject.Instantiate(
                unitSkillButtonPrefab, selectedUnitActionButtonsParent);
            Transform t = g.transform;
            Button b = g.GetComponent<Button>();
            entity.SkillManagers[i].SetButton(b);
            t.Find("Text").GetComponent<TextMeshProUGUI>().text =
                entity.SkillManagers[i].skill.skillName;
            _AddUnitSkillButtonListener(b, i);
        }
    }
    
    private void _AddUnitSkillButtonListener(Button b, int i)
    {
        b.onClick.AddListener(() => selectedEntity.TriggerSkill(i));
    }

    private void _ShowSelectedUnitMenu(bool show)
    {
        selectedUnitMenu.SetActive(show);
    }

    public void AddSelectedUnitToUIList(Entity entity)
    {
        Transform alreadyInstantiatedChild = selectedUnitsListParent.Find(entity.Code);
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
            g.name = entity.Code;
            Transform t = g.transform;
            t.Find("Count").GetComponent<TextMeshProUGUI>().text = "1";
            t.Find("Name").GetComponent<TextMeshProUGUI>().text = entity.Data.unitName;
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
