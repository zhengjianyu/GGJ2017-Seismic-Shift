using UnityEngine;
using System.Collections;

public class ShowPanels : MonoBehaviour {

	public GameObject optionsPanel;							//Store a reference to the Game Object OptionsPanel 
	public GameObject optionsTint;							//Store a reference to the Game Object OptionsTint 
	public GameObject menuPanel;							//Store a reference to the Game Object MenuPanel 
	public GameObject pausePanel;							//Store a reference to the Game Object PausePanel 
    public GameObject losePanel;
    public GameObject creditsPanel;
    public GameObject ingameUI;
    public GameObject ScoreBoard;
    public bool inMenu = true;
    public int sc = 100;
    private ControlGUIScript ControlGUI;
    private changeScore Score;


    void Awake()
    {
        //Get a reference to ShowPanels attached to UI object
        ControlGUI = ingameUI.GetComponent<ControlGUIScript>();
        Score = ScoreBoard.GetComponent<changeScore>();
    }

    public void changeStates(bool TF)
    {
        ControlGUI.inmenu = TF;
        Score.inmenu = TF;
    }

    public void addScore(int x)
    {
        Score.addscore(x);
    }

    public void addtime(float x)
    {
        ControlGUI.AddTime(x);
    }

    public void losetime(float x)
    {
        ControlGUI.LoseTime(x);
    }

    //Call this function to activate and display the Options panel during the main menu
    public void ShowOptionsPanel()
	{
		optionsPanel.SetActive(true);
		optionsTint.SetActive(true);
	}

	//Call this function to deactivate and hide the Options panel during the main menu
	public void HideOptionsPanel()
	{
		optionsPanel.SetActive(false);
		optionsTint.SetActive(false);
	}

    public void ShowCreditsPanel()
    {
        creditsPanel.SetActive(true);
        //optionsTint.SetActive(true);
    }

    public void HideCreditsPanel()
    {
        creditsPanel.SetActive(false);
        //optionsTint.SetActive(false);
    }

    //Call this function to activate and display the main menu panel during the main menu
    public void ShowMenu()
	{
		menuPanel.SetActive (true);
	}

	//Call this function to deactivate and hide the main menu panel during the main menu
	public void HideMenu()
	{
		menuPanel.SetActive (false);
	}

    public void ShowGameUI()
    {
        ingameUI.SetActive(true);
        ScoreBoard.SetActive(true);
        ingameUI.GetComponent<ControlGUIScript>().Playing();
    }

    //Call this function to deactivate and hide the main menu panel during the main menu
    public void HideGameUI()
    {
        ingameUI.SetActive(false);
        ScoreBoard.SetActive(false);
        ingameUI.GetComponent<ControlGUIScript>().NotPlaying();
    }

    //Call this function to activate and display the Pause panel during game play
    public void ShowPausePanel()
	{
		pausePanel.SetActive (true);
		optionsTint.SetActive(true);
	}

	//Call this function to deactivate and hide the Pause panel during game play
	public void HidePausePanel()
	{
		pausePanel.SetActive (false);
		optionsTint.SetActive(false);
        
	}

    public void ShowLosePanel()
    {
        losePanel.SetActive(true);
        Score.setzero();
        sc = 100;
        //optionsTint.SetActive(true);
    }

    //Call this function to deactivate and hide the Lose panel during game play
    public void HideLosePanel()
    {
        losePanel.SetActive(false);
        //optionsTint.SetActive(false);

    }
}
