using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileController : MonoBehaviour
{
    public Vector2 Boyut;

    [SerializeField] Tilemap baseTilemap;
    [SerializeField] Tilemap buildTilemap;
    [SerializeField] Tilemap highlightTilemap;
    [SerializeField] Tile highlightTile, Red, collidertile;
    [SerializeField] GameObject FarmTileObj, HouseObj;

    private List<Vector3Int> lastPositions = new List<Vector3Int>();
    public bool CantBuild;
    private Gamemanager manger;

    int FarmTileId = 9, HouseId = 0;
    GameObject ClonePrefab;
    Vector3 poss;
    Vector2 pos;

    private void Start()
    {
        if (PlayerPrefs.GetInt("FarmTileCount") > 0)
        {
            for (int i = 0; i < PlayerPrefs.GetInt("FarmTileCount"); i++)
            {
                poss = new Vector3(PlayerPrefs.GetFloat("FarmTilePosX" + FarmTileId), PlayerPrefs.GetFloat("FarmTilePosY" + FarmTileId), PlayerPrefs.GetFloat("FarmTilePosZ" + FarmTileId));
                //print(poss);
                Farmtile ObjFarmTile = Instantiate(FarmTileObj, new Vector2(poss.x, poss.y), Quaternion.identity).GetComponent<Farmtile>();
                ObjFarmTile.id = FarmTileId;
                PlaceTilesForAllChildren(ObjFarmTile.ColliderParent);
                FarmTileId++;
            }
        }

        if (PlayerPrefs.GetInt("HouseCount") > 0)
        {
            for (int i = 0; i < PlayerPrefs.GetInt("HouseCount"); i++)
            {
                print(HouseId);
                HouseId++;
                poss = new Vector3(PlayerPrefs.GetFloat("HousePosX" + HouseId), PlayerPrefs.GetFloat("HousePosY" + HouseId), PlayerPrefs.GetFloat("HousePosZ" + HouseId));
                print(poss);
                print("Positions : " + PlayerPrefs.GetFloat("HousePosX" + HouseId) + " , " + PlayerPrefs.GetFloat("HousePosY" + HouseId) + " , " + PlayerPrefs.GetFloat("HousePosZ" + HouseId));
                Houses ObjHouse = Instantiate(HouseObj, new Vector2(poss.x, poss.y), Quaternion.identity).GetComponent<Houses>();
                ObjHouse.Houseid = HouseId - 1;
                PlaceTilesForAllChildren(ObjHouse.ColliderParent);
            }
        }

        manger = Gamemanager.manger;
    }

    public void Build(GameObject Prefab)
    {
        manger.Kamsc.FieldCantMove = true;
        pos = manger.mainkam.ScreenToWorldPoint(Input.mousePosition);
        ClonePrefab = Instantiate(Prefab, new Vector2(pos.x, pos.y), Quaternion.identity);
        ClonePrefab.GetComponent<Houses>().Houseid = HouseId;
        PlayerPrefs.SetInt("HouseCount", HouseId);
        HouseId++;
        manger.BuildB = true;
    }

    public void BuildCancel(bool DecreaseValue = true)
    {
        foreach (var pos in lastPositions)
            highlightTilemap.SetTile(pos, null);

        manger.Kamsc.FieldCantMove = false;
        Destroy(ClonePrefab);

        if(DecreaseValue)
            HouseId--;

        PlayerPrefs.SetInt("HouseCount", HouseId);
        print(HouseId + "Houses");
        manger.BuildB = false;
    }

    public void PlaceTilesForAllChildren(Transform parent)
    {
        if (buildTilemap == null || parent == null || collidertile == null) return;

        foreach (Transform child in parent)
        {
            Vector3 worldPosition = child.position;
            Vector3Int cellPosition = buildTilemap.WorldToCell(worldPosition);

            buildTilemap.SetTile(cellPosition, collidertile);

            //Debug.Log("Tile yerleştirildi: " + cellPosition);
        }
    }

    void Update()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;

        Vector3Int cellPosition = baseTilemap.WorldToCell(mouseWorldPos);

        if (Input.GetButtonDown("Fire1") && manger.HoeToolB)
        {
            Vector3 worldPosition = baseTilemap.GetCellCenterWorld(cellPosition);
            Farmtile ObjFarmTile = Instantiate(FarmTileObj, worldPosition, Quaternion.identity).GetComponent<Farmtile>();
            ObjFarmTile.id = FarmTileId;
            PlayerPrefs.SetFloat("FarmTilePosX" + FarmTileId, ObjFarmTile.transform.position.x);
            PlayerPrefs.SetFloat("FarmTilePosY" + FarmTileId, ObjFarmTile.transform.position.y);
            PlayerPrefs.SetFloat("FarmTilePosZ" + FarmTileId, ObjFarmTile.transform.position.z);
            PlayerPrefs.Save();
            //print(new Vector3(PlayerPrefs.GetFloat("FarmTilePosX" + FarmTileId), PlayerPrefs.GetFloat("FarmTilePosY" + FarmTileId), PlayerPrefs.GetFloat("FarmTilePosZ" + FarmTileId)));
            PlayerPrefs.SetInt("FarmTileCount", FarmTileId - 8);
            FarmTileId++;
        }

        if (!manger.BuildB)
            return;

        List<Vector3Int> positions = GetSurroundingTiles(cellPosition, Boyut);

        foreach (var pos in lastPositions)
        {
            CantBuild = false;
            highlightTilemap.SetTile(pos, null);
        }

        foreach (var pos in positions)
        {
            if (buildTilemap.HasTile(pos))
            {
                highlightTilemap.SetTile(pos, Red);
                CantBuild = true;
            }
            else
                highlightTilemap.SetTile(pos, highlightTile);
        }

        lastPositions = positions;

        if (ClonePrefab != null)
        {
            ClonePrefab.transform.position = mouseWorldPos;

            if (Input.GetButtonUp("Fire1"))
            {
                if (!CantBuild)
                {
                    Vector3 worldPosition2 = baseTilemap.GetCellCenterWorld(cellPosition);
                    ClonePrefab.transform.position = worldPosition2;
                    PlayerPrefs.SetFloat("HousePosX" + HouseId, ClonePrefab.transform.position.x);
                    PlayerPrefs.SetFloat("HousePosY" + HouseId, ClonePrefab.transform.position.y);
                    PlayerPrefs.SetFloat("HousePosZ" + HouseId, ClonePrefab.transform.position.z);
                    PlayerPrefs.Save();
                    print(HouseId + " birinci");
                    PlaceTilesForAllChildren(ClonePrefab.GetComponent<Houses>().ColliderParent);
                    ClonePrefab = null;
                    BuildCancel(false);
                    print(HouseId + " ikinci");
                }
                else
                    BuildCancel();
            }
        }
    }

    List<Vector3Int> GetSurroundingTiles(Vector3Int cellPosition, Vector2 objectSize)
    {
        List<Vector3Int> positions = new List<Vector3Int>();

        int halfWidth = Mathf.FloorToInt(objectSize.x / 2);
        int halfHeight = Mathf.FloorToInt(objectSize.y / 2);

        int startX = cellPosition.x - halfWidth;
        int startY = cellPosition.y - halfHeight;

        int endX = startX + Mathf.CeilToInt(objectSize.x) - 1;
        int endY = startY + Mathf.CeilToInt(objectSize.y) - 1;

        for (int x = startX; x <= endX; x++)
        {
            for (int y = startY; y <= endY; y++)
            {
                positions.Add(new Vector3Int(x, y, 0));
            }
        }

        HashSet<Vector3Int> surroundingTiles = new HashSet<Vector3Int>();

        for (int y = startY; y <= endY; y++)
        {
            surroundingTiles.Add(new Vector3Int(startX - 1, y, 0)); // Sol taraf
            surroundingTiles.Add(new Vector3Int(endX + 1, y, 0));   // Sağ taraf
        }

        for (int x = startX - 1; x <= endX + 1; x++)
        {
            surroundingTiles.Add(new Vector3Int(x, startY - 1, 0)); // Alt taraf
            surroundingTiles.Add(new Vector3Int(x, endY + 1, 0));   // Üst taraf
        }

        positions.AddRange(surroundingTiles);
        return positions;
    }
}
