using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class GameSystemManager : MonoBehaviour
{

    GameObject inputFieldUserName, inputFieldPassword, buttonSubmit, toggleLogin, toggleCreate, ticTacToeSystem, WhosTurn, toggleObserver, replayButton;

    GameObject networkedClient;

    GameObject findGameSessionButton, placeHolderGameButton, SendTextButton;

    GameObject infoStuff1, infoStuff2, tictactoeScene, messages;

    public Button[] GameSquare;
    string playersTurn, opponentsTurn;
    public bool MoveP;
    int HowManyTurns = 0;

    void Start()
    {
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
         
        foreach (GameObject go in allObjects)
        {
            if (go.name == "InputFieldUserName")
                inputFieldUserName = go;
            else if (go.name == "InputFieldPassword")
                inputFieldPassword = go;
            else if (go.name == "ButtonSubmit")
                buttonSubmit = go;
            else if (go.name == "ToggleLogin")
                toggleLogin = go;
            else if (go.name == "ToggleCreate")
                toggleCreate = go;
            else if (go.name == "NetworkedClient")
                networkedClient = go;
            else if (go.name == "FindGameSessionButton")
                findGameSessionButton = go;
            else if (go.name == "PlaceHolderGameButton")
                placeHolderGameButton = go;
            else if (go.name == "InfoText1")
                infoStuff1 = go;
            else if (go.name == "InfoText2")
                infoStuff2 = go;
            else if (go.name == "TicTacToe")
                tictactoeScene = go;
            else if (go.name == "TicTacToeSystem")
                ticTacToeSystem = go;
            else if (go.name == "WhosTurn")
                WhosTurn = go;
            else if (go.name == "SendTextButton")
                SendTextButton = go;
            else if (go.name == "Messages")
                messages = go;
            else if (go.name == "observerToggle")
                toggleObserver = go;
            else if (go.name == "ReplayButton")
                replayButton = go;


        }

        buttonSubmit.GetComponent<Button>().onClick.AddListener(SubmitButtonPressed);
        toggleCreate.GetComponent<Toggle>().onValueChanged.AddListener(ToggleCreateValueChanged);
        toggleLogin.GetComponent<Toggle>().onValueChanged.AddListener(ToggleLoginValueChanged);
        toggleObserver.GetComponent<Toggle>().onValueChanged.AddListener(ToggleLoginValueChanged);
        findGameSessionButton.GetComponent<Button>().onClick.AddListener(FindGameSessionButtonPressed);
        placeHolderGameButton.GetComponent<Button>().onClick.AddListener(PlaceHolderGameButtonPressed);
        SendTextButton.GetComponent<Button>().onClick.AddListener(SendTextButtonPressed);
        replayButton.GetComponent<Button>().onClick.AddListener(ReplayButtonPressed);

        GameSquare = ticTacToeSystem.GetComponentsInChildren<Button>();
        AddListenersOfTicTacToe();

        ChangeGameState(GameStates.Login);
    }

    // Update is called once per frame
    void Update()
    {


        /*  if (Input.GetKeyDown(KeyCode.A))
            ChangeGameState(GameStates.Login);

        if (Input.GetKeyDown(KeyCode.S))
            ChangeGameState(GameStates.MainMenu);

        if (Input.GetKeyDown(KeyCode.D))
            ChangeGameState(GameStates.WaitingForMatch);

        if (Input.GetKeyDown(KeyCode.F))
            ChangeGameState(GameStates.PlayingTicTacToe); */

    }

    private void AddListenersOfTicTacToe()
    {
        foreach (Button button in GameSquare)
        {
            button.onClick.AddListener(ButtonPressed);
        }
    }

    private void SubmitButtonPressed()
    {

        string n = inputFieldUserName.GetComponent<InputField>().text;

        string p = inputFieldPassword.GetComponent<InputField>().text;

        if (toggleLogin.GetComponent<Toggle>().isOn)
            networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.Login + "," + n + "," + p);
        else
            networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.CreateAccount + "," + n + "," + p);
        


    }

    private void ButtonPressed()
    {
        Button button = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();

        for (int i = 0; i < GameSquare.Length; i++)
        {
            if (button == GameSquare[i] && buttonText.text == "" && MoveP == true)
            {
                HowManyTurns++;

                MoveP = false;

                UpdateWhosTurnText(MoveP);

                buttonText.text = playersTurn;

                networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.AnyMove + "," + i);

                if (GameOver())
                {
                    Debug.Log("Printing Symbols");
                    for (int j = 0; j < 7; j += 3)
                    {
                        Debug.Log(GameSquare[j].GetComponentInChildren<TextMeshProUGUI>().text + "," + GameSquare[j + 1].GetComponentInChildren<TextMeshProUGUI>().text 
                        + "," + GameSquare[j + 2].GetComponentInChildren<TextMeshProUGUI>().text);

                    }
                }
                return;
            }
        }
    }

    bool GameWon()
    {
        for (int i = 0; i < 7; i += 3)
        {

            string left = GameSquare[i].GetComponentInChildren<TextMeshProUGUI>().text;

            string mid = GameSquare[i + 1].GetComponentInChildren<TextMeshProUGUI>().text;

            string right = GameSquare[i + 2].GetComponentInChildren<TextMeshProUGUI>().text;

            if (left != "" && left == mid && left == right)
                return true;
        }
        for (int i = 0; i < 3; i++)
        {

            string top = GameSquare[i].GetComponentInChildren<TextMeshProUGUI>().text;

            string mid = GameSquare[i + 3].GetComponentInChildren<TextMeshProUGUI>().text;

            string bottom = GameSquare[i + 6].GetComponentInChildren<TextMeshProUGUI>().text;

            if (top != "" && top == mid && top == bottom)
                return true;
        }
        string topleft = GameSquare[0].GetComponentInChildren<TextMeshProUGUI>().text;

        string midG = GameSquare[4].GetComponentInChildren<TextMeshProUGUI>().text;

        string topRight = GameSquare[2].GetComponentInChildren<TextMeshProUGUI>().text;

        if (topleft != "" && topleft == midG & topleft == GameSquare[8].GetComponentInChildren<TextMeshProUGUI>().text)
            return true;
        if (topRight != "" && topRight == midG && topRight == GameSquare[6].GetComponentInChildren<TextMeshProUGUI>().text)
            return true;

        return false;
    }

    private void FindGameSessionButtonPressed()
    {
        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.AddToGameSessionQueue + "");

        ChangeGameState(GameStates.WaitingForMatch);
    }

    private void PlaceHolderGameButtonPressed()
    {
        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.TicTacToePlay + "");

        Debug.Log("playing game");
    }

    private void SendTextButtonPressed()
    {
        messages.GetComponent<Text>().text = "GLHF";

        Debug.Log("GLHF");
    }

    private void ReplayButtonPressed()
    {
        //don't have any code for now, sorry.

        Debug.Log("ReplayButton Pressed");
    }


    public void UpdateTicTacToeGridAfterMove(int cellNumber)
    {
        HowManyTurns++;
        GameSquare[cellNumber].GetComponentInChildren<TextMeshProUGUI>().text = opponentsTurn;
    }

    public void UpdateWhosTurnText(bool myTurn)
    {
        WhosTurn.GetComponent<TextMeshProUGUI>().text = (myTurn == true) ? "Your Turn" : "Opponents Turn";
    }

    private void ToggleCreateValueChanged(bool newValue)
    {
        toggleLogin.GetComponent<Toggle>().SetIsOnWithoutNotify(!newValue);
    }

    private void ToggleLoginValueChanged(bool newValue)
    {
        toggleCreate.GetComponent<Toggle>().SetIsOnWithoutNotify(!newValue);
    }

    private void ToggleObserverValueChanged(bool newValue)
    {
        toggleObserver.GetComponent<Toggle>().SetIsOnWithoutNotify(!newValue);
        //Don't have any functions right now. sorry.
    }

    public bool GameOver()
    {
        if (HowManyTurns >= 5)
        {
            if (GameWon())
            {
                WhosTurn.GetComponent<TextMeshProUGUI>().text = playersTurn + " You win!";

                networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.GameOver.ToString());
                return true;
            }
            else if (HowManyTurns == 9)
            {
                WhosTurn.GetComponent<TextMeshProUGUI>().text = "Tie";

                networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.Tie.ToString());
                return true;
            }
        }
        return false;
    }


    public void Changerturn(string gameText)
    {
        WhosTurn.GetComponent<TextMeshProUGUI>().text = gameText;
    }

    public void InitGameSymbolsSetCurrentTurn(string playerSymbol, string opponentSymbol, bool myTurn)
    {
        playersTurn = playerSymbol; opponentsTurn = opponentSymbol; MoveP = myTurn;

        UpdateWhosTurnText(MoveP);
    }
    public void ChangeGameState(int newState)
    {
        inputFieldUserName.SetActive(false);
        inputFieldPassword.SetActive(false);
        buttonSubmit.SetActive(false);
        toggleLogin.SetActive(false);
        toggleCreate.SetActive(false);
        findGameSessionButton.SetActive(false);
        placeHolderGameButton.SetActive(false);
        infoStuff1.SetActive(false);
        infoStuff2.SetActive(false);
        ticTacToeSystem.SetActive(false);
        WhosTurn.SetActive(false);
        SendTextButton.SetActive(false);
        messages.SetActive(false);
        toggleObserver.SetActive(false);
        replayButton.SetActive(false);

        if (newState == GameStates.Login)
        {
            inputFieldUserName.SetActive(true);
            inputFieldPassword.SetActive(true);
            buttonSubmit.SetActive(true);
            toggleLogin.SetActive(true);
            toggleCreate.SetActive(true);
            infoStuff1.SetActive(true);
            infoStuff2.SetActive(true);
            toggleObserver.SetActive(true);
            replayButton.SetActive(true);
        }
        else if(newState == GameStates.MainMenu)
        {
            findGameSessionButton.SetActive(true);
        }
        else if (newState == GameStates.WaitingForMatch)
        {

        }
        else if (newState == GameStates.PlayingTicTacToe)
        {
            placeHolderGameButton.SetActive(false);
            ticTacToeSystem.SetActive(true);
            HowManyTurns = 0;
            WhosTurn.SetActive(true);
            SendTextButton.SetActive(true);
            messages.SetActive(true);

        }

    }
}
public static class GameStates
{
    public const int Login = 1;

    public const int MainMenu = 2;

    public const int WaitingForMatch = 3;

    public const int PressTicTacToe = 4;

    public const int PlayingTicTacToe = 5;
}

