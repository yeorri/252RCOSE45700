using UnityEngine;

public class TowerBuilder : MonoBehaviour
{
    // [중요] 이 부분이 있어야 다른 스크립트에서 TowerBuilder.Instance 로 부를 수 있습니다!
    public static TowerBuilder Instance; 

    [Header("설정")]
    public GameObject gridTilePrefab; // 타일 프리팹
    public BuildMenu buildMenu;       // 메뉴 UI

    [Header("맵 생성 정보")]
    public int width = 20;
    public int height = 20;
    public float offset = 10f;

    [Header("참조")]
    public Transform towerContainer;
    public Transform tileContainer; // 타일들을 담을 폴더
    public Transform spawnPoint;
    public Transform goalPoint;

    void Awake()
    {
        // 게임 시작하면 "내가 바로 그 TowerBuilder다!" 라고 등록하는 과정
        Instance = this;
    }

    void Start()
    {
        GenerateGrid();
    }

    // 1. 타일 깔기
    void GenerateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                float worldX = x - offset + 0.5f;
                float worldZ = z - offset + 0.5f;
                Vector3 pos = new Vector3(worldX, 0.05f, worldZ); 

                GameObject tileObj = Instantiate(gridTilePrefab, pos, Quaternion.Euler(90, 0, 0), tileContainer);
                
                GridTile tile = tileObj.GetComponent<GridTile>();
                tile.myPosition = pos;
            }
        }
    }

    // 2. 메뉴 열기 (타일이 호출함)
    public void ShowBuildMenu(GridTile tile)
    {
        if (buildMenu != null)
        {
            buildMenu.OpenMenu(tile);
        }
    }

    // 3. 건설 시도 (메뉴가 호출함)
    public void TryBuildTower(GridTile tile, GameObject towerPrefab)
    {
        Tower towerScript = towerPrefab.GetComponent<Tower>();

        // A. 돈 검사
        if (GameManager.Instance.money < towerScript.baseCost)
        {
            Debug.Log("돈 부족!");
            return;
        }

        // B. 길 막힘 검사
        bool isPathSafe = MazeSolver.Instance.IsPathPossible(
            spawnPoint.position, 
            goalPoint.position, 
            tile.myPosition
        );

        if (isPathSafe)
        {
            GameManager.Instance.SpendMoney(towerScript.baseCost);
            
            Vector3 buildPos = tile.myPosition;
            buildPos.y = 0.5f; 
            
            // 타워 생성
            GameObject newTowerObj = Instantiate(towerPrefab, buildPos, Quaternion.identity, towerContainer);
            
            // [추가] 생성된 타워에게 "너의 주인 타일은 얘야"라고 알려줌
            Tower newTowerScript = newTowerObj.GetComponent<Tower>();
            newTowerScript.ownedTile = tile; 

            MazeSolver.Instance.BlockNode(buildPos);
            tile.isOccupied = true;
        }
        else
        {
            Debug.Log("길을 막으면 안 됩니다!");
        }
    }
    // [추가] 타일이 "여기 지어도 안전한가요?" 물어볼 때 대답해주는 함수
    public bool CheckPathSafety(Vector3 testPos)
    {
        // MazeSolver야, 지금 위치(testPos) 막히면 길 있니?
        return MazeSolver.Instance.IsPathPossible(
            spawnPoint.position, 
            goalPoint.position, 
            testPos
        );
    }
}