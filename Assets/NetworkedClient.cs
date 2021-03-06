using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class NetworkedClient : MonoBehaviour
{

    int connectionID;
    int maxConnections = 1000;
    int reliableChannelID;
    int unreliableChannelID;
    int hostID;
    int socketPort = 5491;
    byte error;
    bool isConnected = false;
    int ourClientID;
    string opponentsSymbol;
    GameObject gameSystemManager;

    void Start()
    {
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();

        foreach (GameObject go in allObjects)
        {
            if (go.name == "GameManager")
                gameSystemManager = go;
        }
            Connect();
    }

    void Update()
    {
        UpdateNetworkConnection();
    }

    private void UpdateNetworkConnection()
    {
        if (isConnected)
        {
            int recHostID;
            int recConnectionID;
            int recChannelID;
            byte[] recBuffer = new byte[1024];
            int bufferSize = 1024;
            int dataSize;
            NetworkEventType recNetworkEvent = NetworkTransport.Receive(out recHostID, out recConnectionID, out recChannelID, recBuffer, bufferSize, out dataSize, out error);

            switch (recNetworkEvent)
            {
                case NetworkEventType.ConnectEvent:
                    Debug.Log("connected.  " + recConnectionID);
                    ourClientID = recConnectionID;
                    break;
                case NetworkEventType.DataEvent:
                    string msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                    ProcessRecievedMsg(msg, recConnectionID);
                    break;
                case NetworkEventType.DisconnectEvent:
                    isConnected = false;
                    Debug.Log("disconnected.  " + recConnectionID);
                    break;
            }
        }
    }
    
    private void Connect()
    {

        if (!isConnected)
        {
            Debug.Log("Attempting to create connection");

            NetworkTransport.Init();

            ConnectionConfig config = new ConnectionConfig();
            reliableChannelID = config.AddChannel(QosType.Reliable);
            unreliableChannelID = config.AddChannel(QosType.Unreliable);
            HostTopology topology = new HostTopology(config, maxConnections);
            hostID = NetworkTransport.AddHost(topology, 0);
            Debug.Log("Socket open.  Host ID = " + hostID);

            connectionID = NetworkTransport.Connect(hostID, "192.168.2.26", socketPort, 0, out error); // server is local on network

            if (error == 0)
            {
                isConnected = true;

                Debug.Log("Connected, id = " + connectionID);

            }
        }
    }
    
    public void Disconnect()
    {
        NetworkTransport.Disconnect(hostID, connectionID, out error);
    }
    
    public void SendMessageToHost(string msg)
    {
        byte[] buffer = Encoding.Unicode.GetBytes(msg);
        NetworkTransport.Send(hostID, connectionID, reliableChannelID, buffer, msg.Length * sizeof(char), out error);
    }

    private void ProcessRecievedMsg(string msg, int id) 
    {
        Debug.Log("msg recieved = " + msg + ".  connection id = " + id);

        string[] csv = msg.Split(',');

        int signifier = int.Parse(csv[0]);

        if(signifier == ServerToClientSignifiers.LoginResponse)
        {
            int loginResultSignifier = int.Parse(csv[1]);

            if (loginResultSignifier == LoginResponses.Success)
                gameSystemManager.GetComponent<GameSystemManager>().ChangeGameState(GameStates.MainMenu);
        }
        else if (signifier == ServerToClientSignifiers.GameSessionStarted)
        {
            gameSystemManager.GetComponent<GameSystemManager>().ChangeGameState(GameStates.PlayingTicTacToe);

            opponentsSymbol = (csv[1] == "X") ? "O" : "X";

            bool myTurn = (int.Parse(csv[2]) == 1) ? true : false;

            gameSystemManager.GetComponent<GameSystemManager>().InitGameSymbolsSetCurrentTurn(csv[1], opponentsSymbol, myTurn);
        }
        else if (signifier == ServerToClientSignifiers.OpponentTicTacToePlay)
        {
        
        }
        else if (signifier == ServerToClientSignifiers.OpponentChoice)
        {
            int cellNumberOfMovePlayed = int.Parse(csv[1]);

            gameSystemManager.GetComponent<GameSystemManager>().UpdateTicTacToeGridAfterMove(cellNumberOfMovePlayed);

            gameSystemManager.GetComponent<GameSystemManager>().MoveP = true;

            gameSystemManager.GetComponent<GameSystemManager>().UpdateWhosTurnText(true);

        }
        else if (signifier == ServerToClientSignifiers.OpponentWon)
        {
            gameSystemManager.GetComponent<GameSystemManager>().Changerturn(opponentsSymbol + " You win!");

            gameSystemManager.GetComponent<GameSystemManager>().MoveP = false;
        }
        else if (signifier == ServerToClientSignifiers.Tie)
        {
            gameSystemManager.GetComponent<GameSystemManager>().Changerturn("Tie");

            gameSystemManager.GetComponent<GameSystemManager>().MoveP = false;
        }

    }

    public bool IsConnected()
    {
        return isConnected;
    }


}

public static class ClientToServerSignifiers
{
    public const int Login = 1;

    public const int CreateAccount = 2;

    public const int AddToGameSessionQueue = 3;

    public const int TicTacToePlay = 4;

    public const int AnyMove = 5;

    public const int GameOver = 6;

    public const int Tie = 7;
}


public static class ServerToClientSignifiers
{
    public const int LoginResponse = 1;

    public const int GameSessionStarted = 2;

    public const int OpponentTicTacToePlay = 3;

    public const int OpponentChoice = 4;

    public const int OpponentWon = 5;

    public const int Tie = 6;
}

public static class LoginResponses
{
    public const int Success = 1;

    public const int FailureNameInUse = 2;

    public const int FailureNameNotFound = 3;

    public const int FailureIncorrectPassword = 4;
}


