using UnityEngine;
using static GameManager;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private Transform placeSfxPrefab;
    [SerializeField] private Transform gunClickSfxPrefab;
    [SerializeField] private Transform shootSfxPrefab;



    private void Start()
    {
        GameManager.Instance.OnPlacedObject += GameManager_OnPlacedObject;
    }

    private void GameManager_OnPlacedObject(object sender, GameManager.OnPlacedObjectEventArgs e)
    {
        Transform prefab = e.rowState switch
        {
            GameManager.RowState.Face => placeSfxPrefab,
            GameManager.RowState.Body => placeSfxPrefab,
            GameManager.RowState.Pistol => placeSfxPrefab,
            GameManager.RowState.Bullet => placeSfxPrefab,
            GameManager.RowState.Shoot => gunClickSfxPrefab,
            GameManager.RowState.Dead => shootSfxPrefab,
            _ => null,
        };
        if (prefab == null) return;

        Transform prefabTranform = Instantiate(prefab);
        Destroy(prefabTranform.gameObject, 3f);
    }
}
