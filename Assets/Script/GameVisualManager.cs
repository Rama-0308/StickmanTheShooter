using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameVisualManager : NetworkBehaviour
{
    

    [SerializeField] private Transform facePrefab;
    [SerializeField] private Transform bodyPrefab;
    [SerializeField] private Transform pistolPrefab;
    [SerializeField] private Transform bulletPrefab; 
    [SerializeField] private Transform shootPrefab; 
    [SerializeField] private Transform deadPrefab;

    private NetworkObject[] spawnedAtIndex = new NetworkObject[8];

 

    private void Start()
    {
        GameManager.Instance.OnClickedOnRow += GameManager_OnClickedOnRow;
        GameManager.Instance.OnRematch += GameManager_OnRematch;

    }

    private void GameManager_OnRematch(object sender, System.EventArgs e)
    {
        if (!IsServer) return;

        for (int i = 0; i < spawnedAtIndex.Length; i++)
        {
            if (spawnedAtIndex[i] != null)
            {
                spawnedAtIndex[i].Despawn(true);
                spawnedAtIndex[i] = null;
            }
        }
    }

    private void GameManager_OnClickedOnRow(object sender, GameManager.OnClickedOnRowEventArgs e)
    {
        
        SpawnObject(e.x, e.y, e.player, e.rowState);
        
    }

    
    private void SpawnObject(int x, int y, GameManager.Player player, GameManager.RowState rowState)
    {
        if (!IsServer) return;

        int index = (x - 1) * 4 + (y - 1);

        if (spawnedAtIndex[index] != null)
        {
            spawnedAtIndex[index].Despawn(true); 
            spawnedAtIndex[index] = null;
        }
        Transform prefab = rowState switch
        {
            GameManager.RowState.Face => facePrefab,
            GameManager.RowState.Body => bodyPrefab,
            GameManager.RowState.Pistol => pistolPrefab,
            GameManager.RowState.Bullet => bulletPrefab,
            GameManager.RowState.Shoot => shootPrefab,
            GameManager.RowState.Dead => deadPrefab,
            _ => null,
        };
        if (prefab == null) return;

        Transform spawnedPrefab = Instantiate(prefab, GetWorldPosition(x, y), Quaternion.identity);

        // flip if x is 2
        if (x == 2)
        {
            Vector3 s = spawnedPrefab.localScale;
            spawnedPrefab.localScale = new Vector3(-Mathf.Abs(s.x), s.y, s.z);
        }

        //spawn in network
        //spawnedPrefab.GetComponent<NetworkObject>().Spawn(true);
        NetworkObject netObj = spawnedPrefab.GetComponent<NetworkObject>();
        netObj.Spawn(true);
        spawnedAtIndex[index] = netObj;

    }

    private Vector2 GetWorldPosition(int x, int y)
    {
        //return the world grid position
        float worldX = x == 1 ? -1.38f : 1.38f;
        float worldY = 2.5f - (y - 1) * 2.16f;

        return new Vector2(worldX, worldY);

    }
}
