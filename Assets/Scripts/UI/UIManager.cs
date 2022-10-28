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

    public GameObject gameResourcesCostPrefab;
    public GameObject infoPanel;
    private TextMeshProUGUI _infoPanelTitleText;
    private TextMeshProUGUI _infoPanelDescriptionText;
    private Transform _infoPanelResourcesCostParent;

    private Dictionary<string, TextMeshProUGUI> _resourceTexts;
    private Dictionary<string, Button> _buildingButtons;

    private void Awake()
    {
        _buildingPlacer = GetComponent<BuildingPlacer>();

        _resourceTexts = new Dictionary<string, TextMeshProUGUI>();

        foreach (KeyValuePair<string, GameResource> pair in Globals.GAME_RESOURCES)
        {
            GameObject display = Instantiate(gameResourcesDisplayPrefab, resourcesUIParent);
            display.name = pair.Key;
            _resourceTexts[pair.Key] = display.transform.Find("Text").GetComponent<TextMeshProUGUI>();
            _SetResourceText(pair.Key, pair.Value.Amount);
            display.transform.Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>(
                    $"Textures/GameResources/{pair.Key}");
        }

        _buildingButtons = new Dictionary<string, Button>();

        for (int i = 0; i < Globals.BUILDING_DATA.Length; i++)
        {
            BuildingData data = Globals.BUILDING_DATA[i];
            GameObject button = Instantiate(buildingButtonPrefab, buildingMenu);
            Button b = button.GetComponent<Button>();

            button.name = data.unitName;
            button.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = data.unitName;
            _buildingButtons[data.code] = b;

            _AddBuildingButtonListener(b, i);

            if (!Globals.BUILDING_DATA[i].CanBuy())
            {
                b.interactable = false;
            }

            button.GetComponent<BuildingButton>().Initialize(Globals.BUILDING_DATA[i]);
            Debug.Log(button);
            Debug.Log(Globals.BUILDING_DATA[i]);
        }

        Transform infoPanelTransform = infoPanel.transform;
        _infoPanelTitleText = infoPanelTransform
            .Find("Content/Title").GetComponent<TextMeshProUGUI>();
        _infoPanelDescriptionText = infoPanelTransform
            .Find("Content/Description").GetComponent<TextMeshProUGUI>();
        _infoPanelResourcesCostParent = infoPanelTransform
            .Find("Content/ResourcesCost");
        ShowInfoPanel(false);
    }

    private void _SetResourceText(string resource, int value)
    {
        _resourceTexts[resource].text = value.ToString();
    }

    private void _AddBuildingButtonListener(Button b, int i)
    {
        b.onClick.AddListener(() => _buildingPlacer.SelectPlacedBuilding(i));
    }

    private void OnEnable()
    {
        EventManager.AddListener("UpdateResourceTexts", _OnUpdateResourceTexts);
        EventManager.AddListener("CheckBuildingButtons", _OnCheckBuildingButtons);
        EventManager.AddTypedListener("HoverBuildingButton", _OnHoverBuildingButton);
        EventManager.AddListener("UnhoverBuildingButton", _OnUnhoverBuildingButton);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener("UpdateResourceTexts", _OnUpdateResourceTexts);
        EventManager.RemoveListener("CheckBuildingButtons", _OnCheckBuildingButtons);
        EventManager.RemoveTypedListener("HoverBuildingButton", _OnHoverBuildingButton);
        EventManager.RemoveListener("UnhoverBuildingButton", _OnUnhoverBuildingButton);
    }

    private void _OnUpdateResourceTexts()
    {
        foreach (KeyValuePair<string, GameResource> pair in Globals.GAME_RESOURCES)
            _SetResourceText(pair.Key, pair.Value.Amount);
    }

    private void _OnCheckBuildingButtons()
    {
        foreach (BuildingData data in Globals.BUILDING_DATA)
            _buildingButtons[data.code].interactable = data.CanBuy();
    }

    public void UpdateResourceTexts()
    {
        foreach(KeyValuePair<string, GameResource> pair in Globals.GAME_RESOURCES)
        {
            _SetResourceText(pair.Key, pair.Value.Amount);
        }
    }

    public void CheckBuildingButtons()
    {
        foreach(BuildingData data in Globals.BUILDING_DATA)
        {
            _buildingButtons[data.code].interactable = data.CanBuy();
        }
    }

    private void _OnHoverBuildingButton(CustomEventData data)
    {
        SetInfoPanel(data.buildingData);
        ShowInfoPanel(true);
    }

    private void _OnUnhoverBuildingButton()
    {
        ShowInfoPanel(false);
    }

    public void SetInfoPanel(BuildingData data)
    {
        if (data.code != "")
            _infoPanelTitleText.text = data.code;
        if (data.description != "")
            _infoPanelDescriptionText.text = data.description;

        foreach (Transform child in _infoPanelResourcesCostParent)
            Destroy(child.gameObject);

        if (data.cost.Count > 0)
        {
            GameObject g; Transform t;
            foreach (ResourceValue resource in data.cost)
            {
                g = Instantiate(gameResourcesCostPrefab, _infoPanelResourcesCostParent);
                t = g.transform;
                t.Find("Text").GetComponent<TextMeshProUGUI>().text = resource.amount.ToString();
                t.Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>(
                    $"Textures/GameResources/{resource.code}");
            }
        }
    }

    public void ShowInfoPanel(bool show)
    {
        infoPanel.SetActive(show);
    }
}
