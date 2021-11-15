using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class GameSystemManager : MonoBehaviour
{

    GameObject inputFieldUserName, inputFieldPassword, buttonSubmit, toggleLogin, toggleCreate, ticTacToeSystem, gameStatusText;

    GameObject networkedClient;

    GameObject findGameSessionButton, placeHolderGameButton;

    GameObject infoStuff1, infoStuff2, tictactoeScene;

    public Button[] ticTacToeCells;
    string playersTurn, opponentsTurn;
    public bool MoveP;
    int numberOfTotalMovesMade = 0;

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
            else if (go.name == "TicTacToeBoard")
                ticTacToeSystem = go;
            else if (go.name == "GameStatusText")
                gameStatusText = go;


        }

        buttonSubmit.GetComponent<Button>().onClick.AddListener(SubmitButtonPressed);
        toggleCreate.GetComponent<Toggle>().onValueChanged.AddListener(ToggleCreateValueChanged);
        toggleLogin.GetComponent<Toggle>().onValueChanged.AddListener(ToggleLoginValueChanged);

        ticTacToeCells = ticTacToeSystem.GetComponentsInChildren<Button>();
        AddListenersToButtonCellArray();

        findGameSessionButton.GetComponent<Button>().onClick.AddListener(FindGameSessionButtonPressed);
        placeHolderGameButton.GetComponent<Button>().onClick.AddListener(PlaceHolderGameButtonPressed);

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

    private void AddListenersToButtonCellArray()
    {
        foreach (Button button in ticTacToeCells)
        {
            button.onClick.AddListener(ButtonCellPressed);
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

    private void ButtonCellPressed()
    {
        Button button = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();

        for (int i = 0; i < ticTacToeCells.Length; i++)
        {
            if (button == ticTacToeCells[i] && buttonText.text == "" && MoveP == true)
            {
                numberOfTotalMovesMade++;
                Debug.Log("Number of moves made: " + numberOfTotalMovesMade);
                MoveP = false;
                UpdatePlayersCurrentTurnText(MoveP);
                buttonText.text = playersTurn;
                networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.AnyMove + "," + i);
                if (CheckIfGameOver())
                {
                    Debug.Log("Printing Symbols");
                    for (int j = 0; j < 7; j += 3)
                    {
                        Debug.Log(ticTacToeCells[j].GetComponentInChildren<TextMeshProUGUI>().text + "," + ticTacToeCells[j + 1].GetComponentInChildren<TextMeshProUGUI>().text + "," + ticTacToeCells[j + 2].GetComponentInChildren<TextMeshProUGUI>().text);

                    }
                }
                return;
            }
        }
    }

    private void ToggleCreateValueChanged(bool newValue)
    {
        toggleLogin.GetComponent<Toggle>().SetIsOnWithoutNotify(!newValue);
    }

    private void ToggleLoginValueChanged(bool newValue)
    {
        toggleCreate.GetComponent<Toggle>().SetIsOnWithoutNotify(!newValue);
    }

    private void FindGameSessionButtonPressed()
    {
        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.AddToGameSessionQueue + "" );
        ChangeGameState(GameStates.WaitingForMatch);
    }

    private void PlaceHolderGameButtonPressed()
    {
        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.TicTacToePlay + "");
        Debug.Log("playing game");
    }

    public void InitGameSymbolsSetCurrentTurn(string playerSymbol, string opponentSymbol, bool myTurn)
    {
        playersTurn = playerSymbol;
        opponentsTurn = opponentSymbol;
        MoveP = myTurn;
        UpdatePlayersCurrentTurnText(MoveP);
    }

    public void UpdateTicTacToeGridAfterMove(int cellNumber)
    {
        numberOfTotalMovesMade++;
        ticTacToeCells[cellNumber].GetComponentInChildren<TextMeshProUGUI>().text = opponentsTurn;
    }

    public void UpdatePlayersCurrentTurnText(bool myTurn)
    {
        gameStatusText.GetComponent<TextMeshProUGUI>().text = (myTurn == true) ? "Your Move" : "Opponents Move";
    }

    private void ResetAllCellButtonTextValues()
    {
        foreach (Button button in ticTacToeCells)
        {
            button.GetComponentInChildren<TextMeshProUGUI>().text = "";
        }
    }

    public bool CheckIfGameOver()
    {
        //Earliest a game can be over is 5 moves so only start checking after the 5th move
        if (numberOfTotalMovesMade >= 5)
        {
            if (CheckIfGameWon())
            {
                gameStatusText.GetComponent<TextMeshProUGUI>().text = playersTurn + " Won!";
                networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.GameOver.ToString());
                return true;
            }
            else if (numberOfTotalMovesMade == 9)
            {
                gameStatusText.GetComponent<TextMeshProUGUI>().text = "Game Drawn";
                networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.GameDrawn.ToString());
                return true;
            }
        }
        return false;
    }

    bool CheckIfGameWon()
    {
        //Checks for rows having same symbol
        for (int i = 0; i < 7; i += 3)
        {
            string leftCell = ticTacToeCells[i].GetComponentInChildren<TextMeshProUGUI>().text;
            string middleCell = ticTacToeCells[i + 1].GetComponentInChildren<TextMeshProUGUI>().text;
            string rightCell = ticTacToeCells[i + 2].GetComponentInChildren<TextMeshProUGUI>().text;

            if (leftCell != "" && leftCell == middleCell && leftCell == rightCell)
                return true;
        }
        //Checks for columns having same symbol
        for (int i = 0; i < 3; i++)
        {
            string topCell = ticTacToeCells[i].GetComponentInChildren<TextMeshProUGUI>().text;
            string middleCell = ticTacToeCells[i + 3].GetComponentInChildren<TextMeshProUGUI>().text;
            string bottomCell = ticTacToeCells[i + 6].GetComponentInChildren<TextMeshProUGUI>().text;

            if (topCell != "" && topCell == middleCell && topCell == bottomCell)
                return true;
        }
        //Checks for diagonals
        string topLeftCorner = ticTacToeCells[0].GetComponentInChildren<TextMeshProUGUI>().text;
        string middleGridCell = ticTacToeCells[4].GetComponentInChildren<TextMeshProUGUI>().text;
        string topRightCorner = ticTacToeCells[2].GetComponentInChildren<TextMeshProUGUI>().text;

        if (topLeftCorner != "" && topLeftCorner == middleGridCell & topLeftCorner == ticTacToeCells[8].GetComponentInChildren<TextMeshProUGUI>().text)
            return true;
        if (topRightCorner != "" && topRightCorner == middleGridCell && topRightCorner == ticTacToeCells[6].GetComponentInChildren<TextMeshProUGUI>().text)
            return true;

        return false;
    }

    public void UpdateGameStatusText(string gameText)
    {
        gameStatusText.GetComponent<TextMeshProUGUI>().text = gameText;
    }

    public void ChangeGameState(int newState)
    {
        //inputFieldUserName, inputFieldPassword, buttonSubmit, toggleLogin, toggleCreate
        // findGameSessionButton, placeHolderGameButton

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
        gameStatusText.SetActive(false);

        if (newState == GameStates.Login)
        {
            inputFieldUserName.SetActive(true);
            inputFieldPassword.SetActive(true);
            buttonSubmit.SetActive(true);
            toggleLogin.SetActive(true);
            toggleCreate.SetActive(true);
            infoStuff1.SetActive(true);
            infoStuff2.SetActive(true);

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
            placeHolderGameButton.SetActive(true);
            ticTacToeSystem.SetActive(true);
            numberOfTotalMovesMade = 0;
            gameStatusText.SetActive(true);
            ResetAllCellButtonTextValues();

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

