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

    private Ball currentBall;
    private int availableBalls;

    public static Level Current { get; private set; }

    private void Awake()
    {
        settings = settings ?? LevelSettings.CreateDefaultInstance();
        Current = this;
    }

    private async void Start()
    {
        if (cameraController == null)
        {
            cameraController = FindObjectOfType<CameraController>();
        }

        SetupLevel();

        // TODO: Start an animation and wait for it to end
        await TaskUtils.WaitForSecondsRealtime(this, 1);
        StartLevel();
    }

    public void OnClick(Vector2 point)
    {
        if (currentBall == null)
        {
            return;
        }

        Ray ray = cameraController.gameCamera.ScreenPointToRay(point);
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
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

    private void SetupLevel()
    {
        if (tower != null)
        {
            Destroy(tower);
        }

        ChooseRandomBlockColors(settings.blockColorsNumber);

        tower = Instantiate(towerPrefab, transform).GetComponentInChildren<Tower>();
        tower.levels = settings.towerLevels;
        tower.blocksPerLevel = settings.blocksPerTowerLevel;
        tower.blockColorIds = blockColorIds;

        availableBalls = settings.availableBalls;
    }

    private void StartLevel()
    {
        for (int i = settings.towerLevels - settings.towerUnlockedLevels - 1; i >= 0; i--)
        {
            tower.SetLevelLocked(i, true);
        }

        SetupBall();
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
        }
        else
        {
            currentBall = null;
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
