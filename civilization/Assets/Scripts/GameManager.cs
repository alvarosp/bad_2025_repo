using UnityEngine;
using UnityEngine.InputSystem;
using System;
using TMPro;
using UnityEngine.UI;
using System.IO;

public class GameManager : MonoBehaviour
{
    public readonly string GAME_VERSION = "0.2.3/2025.04.21";
    public static GameManager Instance { get; private set; }
    public GameObject endGamePanel;
    public TMP_Text tmp_roundCounter;
    public TMP_Text tmp_name;
    public TMP_Text tmp_unitData;
    public Button buttonNextTurn;
    public GameObject GOSelect;

    private TMP_Text endGameMessage;
    private MapManager mapManager;
    private Unit selectedUnit;
    private Mouse mouse;
    private int round;
    private GameObject selectCapsule;

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Running game version " + GAME_VERSION);
        endGameMessage = endGamePanel.GetComponentInChildren<TMP_Text>();
        mapManager = MapManager.Instance;
        mouse = Mouse.current;
        buttonNextTurn.onClick.AddListener(AdvanceTurn);
        round = 1;
        tmp_roundCounter.text = "Round: " + round;
        selectCapsule = Instantiate(GOSelect, Vector3.zero, Quaternion.identity);
        selectCapsule.SetActive(false);
        UpdateUnitInfo();
    }

    public void OnSelect(InputValue cc)
    {
        Debug.Log("Trying to select");
        Vector3 mousePosition = mouse.position.ReadValue();
        Vector3 mousePositionInWorld = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector3Int cellPosition = mapManager.GetWorldToCellPosition(mousePositionInWorld);
        Debug.Log("Click at " + cellPosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePositionInWorld, Vector2.zero);
        if (hit.collider != null)
        {
            selectedUnit = (hit.collider.gameObject).GetComponent<Unit>();
            selectCapsule.transform.position = selectedUnit.transform.position;
            selectCapsule.SetActive(true);
            UpdateUnitInfo();
        }
        else
        {
            selectedUnit = null;
            selectCapsule.SetActive(false);
            UpdateUnitInfo();
        }
    }

    public void OnMoveUnit(InputValue cc)
    {
        if (!selectedUnit || selectedUnit.Owner != mapManager.player)
        {
            return;
        }
        Vector3 mousePosition = mouse.position.ReadValue();
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector3Int cell = mapManager.GetWorldToCellPosition(worldPoint);
        mapManager.TryToMoveUnitTo(selectedUnit, cell);
        if (selectedUnit.Hp > 0)
        {
            selectCapsule.transform.position = selectedUnit.transform.position;
        } else
        {
            selectCapsule.SetActive(false);
            selectedUnit = null;
        }
        UpdateUnitInfo();
    }

    private void UpdateUnitInfo()
    {
        if (selectedUnit == null)
        {
            tmp_name.text = "";
            tmp_unitData.text = "";
        }
        else
        {
            tmp_name.text = selectedUnit.Type.Name;
            tmp_unitData.text = selectedUnit.PanelInfo();
        }
    }

    private void AdvanceTurn()
    {
        Debug.Log("Button clicked");
        mapManager.EnemyAI();
        if (mapManager.CheckVictoryCondition())
        {
            endGameMessage.text = "Victory!";
            endGamePanel.SetActive(true);
        }
        else
        {
            mapManager.AdvanceTurn();
            round++;
            tmp_roundCounter.text = "Round: " + round;
        }
    }

    public string ReadFromFile(string path)
    {
        StreamReader sr = new StreamReader(path);
        string fileContents = sr.ReadToEnd();
        sr.Close();
        return fileContents;
    }
}
