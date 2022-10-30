using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BuildingButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private BuildingData _buildingData;

    public GameObject infoPanel;

    [SerializeField] 
    private Color invalidTextColor;
    public GameObject gameResourcesCostPrefab;
    private TextMeshProUGUI _infoPanelTitleText;
    private TextMeshProUGUI _infoPanelDescriptionText;
    private Transform _infoPanelResourcesCostParent;

    private void Awake()
    {
        Transform infoPanelTransform = infoPanel.transform;
        _infoPanelTitleText = infoPanelTransform
            .Find("Content/Title").GetComponent<TextMeshProUGUI>();
        _infoPanelDescriptionText = infoPanelTransform
            .Find("Content/Description").GetComponent<TextMeshProUGUI>();
        _infoPanelResourcesCostParent = infoPanelTransform
            .Find("Content/ResourcesCost");
    }

    public void Initialize(BuildingData buildingData)
    {
        _buildingData = buildingData;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Enter");
        if (!infoPanel.activeSelf)
        {
            ShowInfoPanel(true);
            SetInfoPanel(_buildingData);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (infoPanel.activeSelf)
        {
            ShowInfoPanel(false);
        }
    }

    public void SetInfoPanel(BuildingData data)
    {
        if (data.code != "")
            _infoPanelTitleText.text = data.unitName;
        if (data.description != "")
            _infoPanelDescriptionText.text = data.description;

        foreach (Transform child in _infoPanelResourcesCostParent)
            Destroy(child.gameObject);

        if (data.cost.Count >= 0)
        {
            GameObject g; Transform t;
            foreach (ResourceValue resource in data.cost)
            {
                g = Instantiate(gameResourcesCostPrefab, _infoPanelResourcesCostParent);
                t = g.transform;
                t.Find("Text").GetComponent<TextMeshProUGUI>().text = resource.amount.ToString();
                t.Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>(
                    $"Textures/GameResources/{resource.code}");
                if (Globals.GAME_RESOURCES[resource.code].Amount < resource.amount)
                    t.Find("Text").GetComponent<TextMeshProUGUI>().color = invalidTextColor;
            }
        }
    }

    public void ShowInfoPanel(bool show)
    {
        infoPanel.SetActive(show);
    }
}
