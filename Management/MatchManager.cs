using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[BoltGlobalBehaviour]
public class MatchManager : Bolt.EntityEventListener<IGameManager>
{
    public void RegisterPlayer(BoltEntity player)
    {
        int Count = 0;

        if (player != null)
        {
            foreach (BoltEntity _player in state.Players)
            {
                if (_player != null)
                {
                    Count++;
                }
            }
            state.Players[Count] = player;
        }
    }
}
