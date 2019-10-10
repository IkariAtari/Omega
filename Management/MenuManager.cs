using System;
using System.Collections;
using System.Collections.Generic;
using UdpKit;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class MenuManager : Bolt.GlobalEventListener {

    public Scene SceneToLoad;

    [SerializeField]
    private LoadoutManager loadoutManager;

    [SerializeField]
    private RankManager rankManager;

    [SerializeField]
    private InputField Username;

    [SerializeField]
    private InputField Password;

    [SerializeField]
    private PlayerAuth Auth;

    [SerializeField]
    private Text Messages;

    [SerializeField]
    private GameObject LoginPanel;

    [SerializeField]
    private GameObject MainMenuPanel;

    [SerializeField]
    private GameObject CreateMatchPanel;
    [SerializeField]
    private Dropdown GameMode;
    [SerializeField]
    private Dropdown Time;
    [SerializeField]
    private GameObject Rounds;

    [SerializeField]
    private Text ClassName;

    [SerializeField]
    private Text CurrentWeapon;

    [SerializeField]
    private GameObject LoadOutPanel;

    [SerializeField]
    private GameObject PrimaryWeapons;

    [SerializeField]
    private GameObject SecondaryWeapons;

    [SerializeField]
    private GameObject Lethal;

    [SerializeField]
    private Text WeaponChosenPrimary;

    [SerializeField]
    private Text WeaponChosenSecondary;

    [SerializeField]
    private Text DamagePrimary;

    [SerializeField]
    private Text FireRatePrimary;

    [SerializeField]
    private Text RecoilPrimary;

    [SerializeField]
    private Text DamageSecondary;

    [SerializeField]
    private Text FireRateSecondary;

    [SerializeField]
    private Text RecoilSecondary;

    [SerializeField]
    private GameObject NonLethal;

    [SerializeField]
    private Text UsernameText;

    [SerializeField]
    private Image RankImage;

    [SerializeField]
    private Text RankName;

    [SerializeField]
    private Text XP;

    private int CurrentLoadout;

    private void Start()
    {
        CurrentLoadout = 0;
        SetLoadoutScreen(0);
    }

    public void TryLogin()
    {
        string _username = Username.text;
        string _password = Password.text;

        StartCoroutine(Login(_username, _password));
    }

    public void TryRegister()
    {
        string _username = Username.text;
        string _password = Password.text;

        StartCoroutine(Register(_username, _password));
    }

    private IEnumerator Login(string _username, string _password)
    {
        UnityWebRequest www = UnityWebRequest.Get("http://omega.ikariatari.com/PlayerFunctions.php?function=login&username="+_username+"&password="+_password);
        Debug.Log("http://omega.ikariatari.com/PlayerFunctions.php?function=login&username=" + _username + "&password=" + _password);

        yield return www.SendWebRequest();

        if(www.isNetworkError || www.isHttpError)
        {
            MessageHandler(www.error);
        }
        else
        {
            string[] _outcomes = www.downloadHandler.text.Split(':');

            MessageHandler(_outcomes[0]);

            if (_outcomes[0] == "success")
            {
                SuccessFullLogin(_outcomes);
            }
        }
    }

    private IEnumerator Register(string _username, string _password)
    {
        UnityWebRequest www = UnityWebRequest.Get("http://omega.ikariatari.com/PlayerFunctions.php?function=register&username=" + _username + "&password=" + _password);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            MessageHandler(www.error);
        }
        else
        {
            MessageHandler(www.downloadHandler.text);

            if (www.downloadHandler.text == "success")
            {
                StartCoroutine(Login(_username, _password));
            }
        }
    }

    public void OnGameModeChanged()
    {
        if(GameMode.value != 2)
        {
            Rounds.SetActive(true);
        }
        else
        {
            Rounds.SetActive(false);
        }
    }

    public void ShowMakeMatchPanel()
    {
        if(CreateMatchPanel.activeSelf == false)
        {
            CreateMatchPanel.SetActive(true);
        }
        else
        {
            CreateMatchPanel.SetActive(false);
        }
    }

    private void SuccessFullLogin(string[] _outcomes)
    {
        Auth.CreateSession(Username.text);
        Auth.Rank = Int32.Parse(_outcomes[1]);
        Auth.XP = Int32.Parse(_outcomes[2]);

        LoginPanel.SetActive(false);

        MainMenuPanel.SetActive(true);

        UsernameText.text = Auth.Name;
        UpdateRank();
    }

    public void UpdateRank()
    {
       Rank[] _ranks = rankManager.CalculateRank(Auth.XP);
       RankImage.sprite = _ranks[0].RankIcon;
       RankName.text = _ranks[0].RankName;
       XP.text = Auth.XP.ToString()+" xp";
    }

    public void SetLoadoutScreen(int _index)
    {
        ClassName.text = LoadOutPanel.transform.GetChild(_index).GetChild(0).GetComponent<Text>().text;
        CurrentLoadout = _index;
        Debug.Log(loadoutManager.Loadouts[CurrentLoadout].Primary.GetComponent<Weapon>().Name);
        loadoutManager.LoadoutChosen = _index;
        SetStats();
    }

    private void SetStats()
    {
        WeaponChosenPrimary.text = loadoutManager.Loadouts[CurrentLoadout].Primary.GetComponent<Weapon>().Name;
        WeaponChosenSecondary.text = loadoutManager.Loadouts[CurrentLoadout].Secondary.GetComponent<Weapon>().Name;
        DamagePrimary.text = loadoutManager.Loadouts[CurrentLoadout].Primary.GetComponent<Weapon>().Damage.ToString();
        FireRatePrimary.text = (200 / loadoutManager.Loadouts[CurrentLoadout].Primary.GetComponent<Weapon>().ResetTime).ToString();
        DamageSecondary.text = loadoutManager.Loadouts[CurrentLoadout].Secondary.GetComponent<Weapon>().Damage.ToString();
        FireRateSecondary.text = (200 / loadoutManager.Loadouts[CurrentLoadout].Secondary.GetComponent<Weapon>().ResetTime).ToString();
        Weapon PrimaryWeapon = loadoutManager.Loadouts[CurrentLoadout].Primary.GetComponent<Weapon>();
        Weapon SecondaryWeapon = loadoutManager.Loadouts[CurrentLoadout].Secondary.GetComponent<Weapon>();
        float recoilPrimary = (PrimaryWeapon.minRecoil + PrimaryWeapon.maxRecoil + PrimaryWeapon.minRecoilADS + PrimaryWeapon.maxRecoilADS) / 4;
        float recoilSecondary = (SecondaryWeapon.minRecoil + SecondaryWeapon.maxRecoil + SecondaryWeapon.minRecoilADS + SecondaryWeapon.maxRecoilADS) / 4;
        RecoilPrimary.text = recoilPrimary.ToString();
        RecoilSecondary.text = recoilSecondary.ToString();

    }

    public void ShowLoadoutPanel()
    {
        if(LoadOutPanel.activeSelf == false)
        {
            LoadOutPanel.SetActive(true);
        }
        else
        {
            LoadOutPanel.SetActive(false);
        }
    }
    public void GetAvailableWeapons(string _type)
    {
        switch(_type)
        {
            case "Primary":          
                PrimaryWeapons.SetActive(true);
                SecondaryWeapons.SetActive(false);
                Lethal.SetActive(false);
                NonLethal.SetActive(false);
                break;

            case "Secondary":
                PrimaryWeapons.SetActive(false);
                SecondaryWeapons.SetActive(true);
                Lethal.SetActive(false);
                NonLethal.SetActive(false);
                break;

            case "Lethal":      
                PrimaryWeapons.SetActive(false);
                SecondaryWeapons.SetActive(false);
                Lethal.SetActive(true);
                NonLethal.SetActive(false);
                break;
            case "NonLethal":
                PrimaryWeapons.SetActive(false);
                SecondaryWeapons.SetActive(false);
                Lethal.SetActive(false);
                NonLethal.SetActive(true);
                break;
        }
        
    }

    public void SetPrimaryWeapon(int _weaponID)
    {      
        loadoutManager.Loadouts[CurrentLoadout].Primary = loadoutManager.weapons[_weaponID];
        SetStats();
    }

    public void SetSecondaryWeapon(int _weaponID)
    {        
        loadoutManager.Loadouts[CurrentLoadout].Secondary = loadoutManager.weapons[_weaponID];
        SetStats();
    }

    public void SetLethal(int _weaponID)
    {    
        loadoutManager.Loadouts[CurrentLoadout].Lethal = loadoutManager.weapons[_weaponID];
        SetStats();
    }

    public void SetNonLethal(int _weaponID)
    {
        loadoutManager.Loadouts[CurrentLoadout].Lethal = loadoutManager.weapons[_weaponID];
        SetStats();
    }

    public void StartServer()
    {       
        BoltLauncher.StartServer();
    }

    public void StartClient()
    {
        BoltLauncher.StartClient();      
    }

    public override void BoltStartDone()
    {
        if(BoltNetwork.IsServer)
        {          
            BoltNetwork.SetServerInfo("Test", null);
            BoltNetwork.LoadScene("Clash");
        }
    }

    public override void SessionListUpdated(Map<Guid, UdpSession> sessionList)
    {
        Debug.LogFormat("Session list updated: {0} total sessions", sessionList.Count);

        foreach (var session in sessionList)
        {
            UdpSession photonSession = session.Value as UdpSession;

            if (photonSession.Source == UdpSessionSource.Photon)
            {
                BoltNetwork.Connect(photonSession);
            }
        }
    }

    private void MessageHandler(string Message)
    {
        switch(Message)
        {
            case "username_password_wrong":
                Messages.text = "Username or Password is wrong!";
                break;
            case "missing_username_password":
                Messages.text = "Username or Password not filled in!";
                break;
            case "user_already_exists":
                Messages.text = "Username already exists!";
                break;
            case "no_function_specified":
                Messages.text = "Something went wrong.";
                break;
            case "success;":
                Messages.text = "Successfull login.";
                break;
            case "success":
                Messages.text = "Successfull register.";
                break;
        }
    }
}
