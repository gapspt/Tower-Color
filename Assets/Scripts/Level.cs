using UnityEngine;

public class Level : MonoBehaviour
{
    public LevelSettings settings;

    public CameraController cameraController;

    public GameObject towerPrefab;
    public GameObject ballPrefab;

    public float throwSpeed = 35;
    public float throwGravityMultiplier = 2;
    public Vector3 throwCameraPositionOffset;

    private int[] blockColorIds;
    private Tower tower;
    private int towerBlocksLayerMask;

    private Ball currentBall;
    private int availableBalls;
    private int lockedLevels;
    private int[] standingBlocksPerLevel;

    public static Level Current { get; private set; }

    private void Awake()
    {
        settings = settings ?? LevelSettings.CreateDefaultInstance();
        towerBlocksLayerMask = LayerMask.GetMask(LayerMask.LayerToName(LevelSettings.TowerBlocksLayer));
        Current = this;
    }

    private void Start()
    {
        if (cameraController == null)
        {
            cameraController = FindObjectOfType<CameraController>();
        }

        SetupLevel();

        Vector3 lookAtPosition = (tower.GetLevelLocalPosition(0) +
            tower.GetLevelLocalPosition(settings.towerUnlockedLevels)) / 2;
        cameraController.SetLookAtPosition(lookAtPosition);

        UIManager.Current?.SetStartScreenVisible(true);
    }

    public void OnClick(Vector2 point)
    {
        if (currentBall == null)
        {
            return;
        }

        Ray ray = cameraController.gameCamera.ScreenPointToRay(point);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, towerBlocksLayerMask))
        {
            Block block = hitInfo.transform.GetComponentInParent<Block>();
            if (block != null && !block.IsLocked && !block.IsExploding)
            {
                ThrowBall(block, hitInfo.point);
            }
        }
    }

    public void OnDrag(Vector2 delta)
    {
        cameraController.RotateByRelativeAmount(delta.x / Screen.width);
    }

    public void OnBlockFell(Block block)
    {
        int levelStandingBlocks = --standingBlocksPerLevel[block.TowerLevel];
        if (lockedLevels > 0 && levelStandingBlocks == 0)
        {
            tower.SetLevelLocked(--lockedLevels, false);
            Vector3 lookAtPosition = (tower.GetLevelLocalPosition(lockedLevels) +
                         tower.GetLevelLocalPosition(lockedLevels + settings.towerUnlockedLevels)) / 2;
            cameraController.MoveToLevelAtPosition(lookAtPosition);
        }
    }

    private void SetupLevel()
    {
        if (tower != null)
        {
            Destroy(tower);
        }

        int towerLevels = settings.towerLevels;
        int blocksPerTowerLevel = settings.blocksPerTowerLevel;

        ChooseRandomBlockColors(settings.blockColorsNumber);

        tower = Instantiate(towerPrefab, transform).GetComponentInChildren<Tower>();
        tower.levels = towerLevels;
        tower.blocksPerLevel = blocksPerTowerLevel;
        tower.blockColorIds = blockColorIds;

        availableBalls = settings.availableBalls;
        standingBlocksPerLevel = new int[towerLevels];
        for (int i = towerLevels - 1; i >= 0; i--)
        {
            standingBlocksPerLevel[i] = blocksPerTowerLevel;
        }
    }

    public async void StartLevel()
    {
        UIManager.Current?.HideAll();

        int levels = settings.towerLevels;
        int unlockedLevels = settings.towerUnlockedLevels;
        Vector3 lookAtPosition = (tower.GetLevelLocalPosition(levels) +
             tower.GetLevelLocalPosition(levels - unlockedLevels)) / 2;
        await cameraController.MoveAtLevelStart(lookAtPosition);

        lockedLevels = settings.towerLevels - settings.towerUnlockedLevels;
        for (int i = lockedLevels - 1; i >= 0; i--)
        {
            tower.SetLevelLocked(i, true);
        }

        SetupBall();

        UIManager.Current?.UpdateAvailableBalls(availableBalls);
        UIManager.Current?.SetHudVisible(true);
    }

    private void ChooseRandomBlockColors(int blockColorsNumber)
    {
        if (blockColorsNumber <= 0)
        {
            return;
        }

        int availableColorsLength = LevelSettings.BlockColors.Length;
        int[] availableNumbers = new int[availableColorsLength];
        for (int i = 0; i < availableColorsLength; i++)
        {
            availableNumbers[i] = i;
        }

        blockColorIds = new int[blockColorsNumber];
        for (int i = 0; i < blockColorsNumber; i++)
        {
            int j = Random.Range(i, availableColorsLength);
            blockColorIds[i] = availableNumbers[j];
            availableNumbers[j] = availableNumbers[i];
        }
    }

    private void SetupBall()
    {
        GameObject ballObject = Instantiate(ballPrefab, cameraController.gameCamera.transform);
        ballObject.transform.localPosition = throwCameraPositionOffset;
        currentBall = ballObject.GetComponentInChildren<Ball>();
        currentBall.Setup(blockColorIds[Random.Range(0, blockColorIds.Length)]);
    }

    private async void ThrowBall(Block block, Vector3 hitPosition)
    {
        Ball ball = currentBall;

        availableBalls--;
        if (availableBalls > 0)
        {
            SetupBall();
            UIManager.Current?.UpdateAvailableBalls(availableBalls);
        }
        else
        {
            currentBall = null;
            UIManager.Current?.SetHudVisible(false);
        }

        float gravity = Physics.gravity.y * throwGravityMultiplier;

        Vector3 initialBallPosition = ball.transform.position;
        Vector3 distance = hitPosition - initialBallPosition;
        Vector2 horizontalDistance = new Vector2(distance.x, distance.z);
        float totalTime = horizontalDistance.magnitude / throwSpeed;
        float verticalSpeed = distance.y / totalTime - gravity * totalTime * 0.5f;

        float startTime = Time.time;
        float endTime = startTime + totalTime;
        Vector2 horizontalDirection = horizontalDistance.normalized;

        await TaskUtils.WaitForNextUpdate(this);
        while (Time.time < endTime)
        {
            float t = Time.time - startTime;
            float verticalDiff = (gravity * t * 0.5f + verticalSpeed) * t;
            Vector2 horizontalDiff = horizontalDirection * (throwSpeed * t);

            ball.transform.position = initialBallPosition + new Vector3(horizontalDiff.x, verticalDiff, horizontalDiff.y);

            await TaskUtils.WaitForNextUpdate(this);
        }

        if (block.ColorId == ball.ColorId)
        {
            block.Explode();
        }
        Destroy(ball.gameObject);
    }
}
