using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUImanager : Bolt.EntityBehaviour<IPlayer> {

    private Text ToolTip;
    private Text Points;
    private Text Ammunition;
    private GameObject CrossHair;
    private Text Rounds;
    private Text Health;
    [SerializeField]
    private GameObject canvas;

    private GameObject GameManager;
    private GameObject ui;
    private Player Player;
    private WeaponManager WeaponManager;

    [SerializeField]
    private GameObject KillFeedObject;

    private GameObject KillFeedPanel;
    private GameObject DeathScreen;
    private GameObject FlashedScreen;

    private GameObject ScoreBoard;

    public override void Attached()
    {
        Instantiate(canvas);
        Player = GetComponent<Player>();
        WeaponManager = GetComponent<WeaponManager>();
        GameManager = GameObject.Find("GameManager");
        ui = GameObject.FindGameObjectWithTag("UI");

        ToolTip = ui.transform.GetChild(0).GetComponent<Text>();
        Points = ui.transform.GetChild(1).GetChild(0).GetComponent<Text>();
        Ammunition = ui.transform.GetChild(2).GetChild(1).GetComponent<Text>();
        Health = ui.transform.GetChild(3).GetChild(1).GetComponent<Text>();
        CrossHair = ui.transform.GetChild(4).gameObject;
        KillFeedPanel = ui.transform.GetChild(5).gameObject;
        DeathScreen = ui.transform.GetChild(6).gameObject;
        ui.transform.GetChild(6).GetChild(2).GetComponent<Button>().onClick.AddListener(delegate { SetLoadout(0);  });
        ui.transform.GetChild(6).GetChild(3).GetComponent<Button>().onClick.AddListener(delegate { SetLoadout(1); });
        ui.transform.GetChild(6).GetChild(4).GetComponent<Button>().onClick.AddListener(delegate { SetLoadout(2); });
        ui.transform.GetChild(6).GetChild(5).GetComponent<Button>().onClick.AddListener(delegate { SetLoadout(3); });
        ui.transform.GetChild(6).GetChild(6).GetComponent<Button>().onClick.AddListener(delegate { SetLoadout(4); });
        ScoreBoard = ui.transform.GetChild(7).gameObject;
        FlashedScreen = ui.transform.GetChild(9).gameObject;
    }

    private void SetLoadout(int _loadout)
    {
        WeaponManager.SwitchLoadout(_loadout);
    }

    public void Flashed()
    {
        StartCoroutine(FlashFunction());
    }

    private IEnumerator FlashFunction()
    {
        FlashedScreen.SetActive(true);

        yield return new WaitForSeconds(3);

        FlashedScreen.SetActive(false);
    }

    private void Update()
    {
        Health.text = Player.state.Health.ToString();
        Points.text = Player.Points.ToString();
        if (WeaponManager.CurrentWeapon.Type != "projectile")
        {
            Ammunition.text = WeaponManager.CurrentWeapon.AmmunitionInMagazine.ToString() + "|" + WeaponManager.CurrentWeapon.AmmunitionCount.ToString();
        }
        else
        {
            Ammunition.text = WeaponManager.CurrentWeapon.AmmunitionCount.ToString();
        }

        if(entity.IsOwner)
        {
            if (Input.GetKey(KeyCode.Tab))
            {
                ShowScoreBoard();
            }

            if(Input.GetKeyUp(KeyCode.Tab))
            {
                HideScoreBoard();
            }
        }
    }

    public void SetToolTip(string ToolTip_str)
    {
        ToolTip.text = ToolTip_str;
    }

    public void UnsetToolTip()
    {
        ToolTip.text = "";
    }

    public void ShowScoreBoard()
    {
        ScoreBoard.SetActive(true);
    }

    public void HideScoreBoard()
    {
        if(entity.IsOwner)
        {
            if(state.IsDead == false)
            {
                ScoreBoard.SetActive(false);
            }
        }
    }

    public void UnsetCrosshair()
    {
        CrossHair.SetActive(false);
    }

    public void SetCrosshair()
    {
        CrossHair.SetActive(true);
    }

    public void ShowDeathScreen(string Killer)
    {
        DeathScreen.SetActive(true);
        DeathScreen.transform.GetChild(0).GetComponent<Text>().text = Killer;      
    }

    public void HideDeathScreen()
    {
        DeathScreen.SetActive(false);
    }

    public void KilFeedAdd(string Killed, string Killer)
    {
        if (entity.IsOwner)
        {
            GameObject killfeedObject = Instantiate(KillFeedObject, transform.position, Quaternion.identity);
            killfeedObject.transform.SetParent(KillFeedPanel.transform);
            killfeedObject.GetComponent<Text>().text = Killer + " Killed " + Killed;
        }
    }
}
