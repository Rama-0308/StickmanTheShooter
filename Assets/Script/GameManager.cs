using System;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using static GameManager;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance {  get; private set; }


    public event EventHandler OnGameStarted;
    public event EventHandler<OnClickedOnRowEventArgs> OnClickedOnRow;
    public class OnClickedOnRowEventArgs : EventArgs
    {
        public int x;
        public int y;
        public Player player;
        public RowState rowState;
    }



    public enum Player
    {
        None,
        Player1,
        Player2,
    }

    public enum PlayerChoice
    {
        Empty,
        Rock,
        Paper,
        Scissors
    }

    private PlayerChoice hostChoice = PlayerChoice.Empty;
    private PlayerChoice clientChoice = PlayerChoice.Empty;


    public event EventHandler<OnRpsResolvedEventArgs> OnRpsResolved;
    public class OnRpsResolvedEventArgs : EventArgs
    {
        public PlayerChoice hostChoice, clientChoice;
        public Player winner; 
    }

    public event EventHandler OnTurnEnd;

    public event EventHandler<OnGameWinEventArgs> OnGameWin;
    public class OnGameWinEventArgs : EventArgs
    {
        public Player winPlayer;
    }

    public event EventHandler OnRematch;
    public event EventHandler<OnPlacedObjectEventArgs> OnPlacedObject;
    public class OnPlacedObjectEventArgs : EventArgs
    {
        public RowState rowState;

    }


    //currently running player instance
    private Player localPlayerType;

    private Player currentPlayablePlayer;

    private int? pendingShooterIndex = null;
    private int pendingShooterX; 
    private int pendingShooterY;



    //array foe rowstate
    private RowState[] rowStates = new RowState[8];

    //x,y coordinates to array index
    private int IndexOf(int x, int y) => (x - 1) * 4 + (y - 1);
    public enum RowState
    {
        Empty,
        Face, 
        Body, 
        Pistol, 
        Bullet, 
        Shoot,
        Dead,
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("More than one GameManager Instance!");
        }
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        Debug.Log("onnetworkspawn: " + NetworkManager.Singleton.LocalClientId);
        if (NetworkManager.Singleton.LocalClientId == 0)
        {
            localPlayerType = Player.Player1;
        }
        else
        {
            localPlayerType = Player.Player2;
        }

        OnGameStarted?.Invoke(this, EventArgs.Empty);
        /*
        if (IsServer)
        {
            currentPlayablePlayer = Player.Player1;
        }*/

    }

    public void PlayerChoices(PlayerChoice choice)
    {
        PlayerChoicesRpc(choice);
    }

    [Rpc(SendTo.Server)]
    public void PlayerChoicesRpc(PlayerChoice choice, RpcParams rpcParams = default)
    {
        Player player = GetPlayerFromClientId(rpcParams.Receive.SenderClientId);
        if (player == Player.None) return;

        if (player == Player.Player1)
        {
            if (hostChoice != PlayerChoice.Empty) return; // already chose this round, ignore
            hostChoice = choice;
        }
           

        if (player == Player.Player2)
        {
            if (clientChoice != PlayerChoice.Empty) return;
            clientChoice = choice;
        }

        if (hostChoice != PlayerChoice.Empty && clientChoice != PlayerChoice.Empty)
        {
            ResolveRps();
        }

    }
    public void ClickedOnRow(int x, int y)
    {
        ClickedOnRowRpc(x, y);
    }

    [Rpc(SendTo.Server)]
    public void ClickedOnRowRpc(int x, int y, RpcParams rpcParams = default)
    {
        int index = IndexOf(x, y);


        //get current player
        Player actingPlayer = GetPlayerFromClientId(rpcParams.Receive.SenderClientId);

        //valid player check
        if (actingPlayer == Player.None) return;

        //player turn check
        if (actingPlayer != currentPlayablePlayer) return;



        if (pendingShooterIndex.HasValue)
        {
            Player targetOwner = GetPlayerFromIndex(index);
            RowState targetState = rowStates[index];

            if (targetOwner == actingPlayer) return;
            if (targetState == RowState.Empty || targetState == RowState.Dead) return;

            rowStates[index] = RowState.Dead;
            TriggerOnPlacedObjectRpc(RowState.Dead);
            OnClickedOnRowRpc(x, y, targetOwner, RowState.Dead);

            if (IsPlayerDead(0))
            {
                //Debug.Log("Player2Wins"!); 
                OnGameWinRpc(Player.Player2);
                return;
            }
            if (IsPlayerDead(4))
            {
                //Debug.Log("Player1Wins"!);
                OnGameWinRpc(Player.Player1);
                return; 
            }

            int shooterIndex = pendingShooterIndex.Value;
            rowStates[shooterIndex] = RowState.Pistol;

            OnClickedOnRowRpc(pendingShooterX, pendingShooterY, actingPlayer, RowState.Pistol);
            pendingShooterIndex = null;
            
            OnTurnEndRpc();

            return;
        }

        //get rowowner from clicked row
        Player rowOwner = GetPlayerFromIndex(index);

        //valid row check
        if (rowOwner != actingPlayer) return;

        RowState current = rowStates[index];
        

        //row state check
        if (current == RowState.Dead) return;

        //change rowstate
        RowState next = GetNextStage(current);
        rowStates[index] = next;
        TriggerOnPlacedObjectRpc(next);

        //raise event
        OnClickedOnRowRpc(x, y, actingPlayer, next);

        if (next == RowState.Shoot)
        {
            pendingShooterIndex = index;
            pendingShooterX = x;
            pendingShooterY = y;
        } 
        else 
        {
            
            OnTurnEndRpc();
        }

    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnPlacedObjectRpc(RowState next)
    {
        //OnPlacedObject?.Invoke(this, EventArgs.Empty);
        OnPlacedObject?.Invoke(this, new OnPlacedObjectEventArgs { rowState = next});

    }


    private void ResolveRps()
    {
        Player turnWinner = CompareChoices(hostChoice, clientChoice);
        if (turnWinner == Player.None)
        {
            OnRpsResolvedRpc(hostChoice, clientChoice, turnWinner);

            hostChoice = PlayerChoice.Empty;
            clientChoice = PlayerChoice.Empty;
            
            return;
        }
        currentPlayablePlayer = turnWinner; 

        OnRpsResolvedRpc(hostChoice, clientChoice, turnWinner);
        hostChoice = PlayerChoice.Empty;
        clientChoice = PlayerChoice.Empty;
        
    }

    private Player CompareChoices(PlayerChoice a, PlayerChoice b)
    {
        if (a == b)
        {
            return Player.None;
        }

        bool aWins = (a == PlayerChoice.Rock && b == PlayerChoice.Scissors)
                  || (a == PlayerChoice.Paper && b == PlayerChoice.Rock)
                  || (a == PlayerChoice.Scissors && b == PlayerChoice.Paper);
        return aWins ? Player.Player1 : Player.Player2;
    }

    [Rpc(SendTo.Everyone)]
    private void OnRpsResolvedRpc(PlayerChoice host, PlayerChoice client, Player winner)
    {
        OnRpsResolved?.Invoke(this, new OnRpsResolvedEventArgs { hostChoice = host, clientChoice = client, winner = winner });
    }

    [Rpc(SendTo.Everyone)]
    private void OnTurnEndRpc()
    {

        currentPlayablePlayer = Player.None;
        OnTurnEnd?.Invoke(this, EventArgs.Empty);
    }

    /*player turn switch
    private void SwitchTurn()
    {
        currentPlayablePlayer = currentPlayablePlayer == Player.Player1 ? Player.Player2 : Player.Player1;
    }*/

    [Rpc(SendTo.Everyone)]
    private void OnGameWinRpc(Player winPlayer)
    {
        OnGameWin?.Invoke(this, new OnGameWinEventArgs
        {
            winPlayer = winPlayer
        });
    }


    private Player GetPlayerFromClientId(ulong clientId)
    {
        if (clientId == 0) return Player.Player1;
        if (clientId == 1) return Player.Player2;
        return Player.None;
    }

    private RowState GetNextStage(RowState currentState)
    {
        switch (currentState)
        {
            case RowState.Empty:
                return RowState.Face;

            case RowState.Face:
                return RowState.Body;

            case RowState.Body:
                return RowState.Pistol;

            case RowState.Pistol:
                return RowState.Bullet;

            case RowState.Bullet:
                return RowState.Shoot;

            default:
                return currentState;

        }
    }

    private Player GetPlayerFromIndex(int index)
    {
        return index < 4 ? Player.Player1 : Player.Player2;
    }

    [Rpc(SendTo.Everyone)]
    private void OnClickedOnRowRpc(int x, int y, Player player, RowState newState)
    {
        OnClickedOnRow?.Invoke(this, new OnClickedOnRowEventArgs
        {
            x = x,
            y = y,
            player = player,
            rowState = newState,
        });
    }

    private bool IsPlayerDead(int startIndex)
    {
        for (int i = startIndex; i < startIndex + 4; i++)
        {
            if (rowStates[i] != RowState.Dead) return false;
        }
        return true;
    }

    [Rpc(SendTo.Server)]
    public void RematchRpc()
    {
        for (int i = 0; i < 8; i++)
        {
            rowStates[i] = RowState.Empty;
        }
        currentPlayablePlayer = Player.None;
        pendingShooterIndex = null;
        TriggerOnRematchRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnRematchRpc()
    {
        OnRematch?.Invoke(this, EventArgs.Empty);
    }
        

    //expose the localPlayerType for visualManager to read
    public Player GetLocalPlayer()
    {
        return localPlayerType;
    }

}
