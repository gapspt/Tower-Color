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

    public static Level Current { get; private set; }

    private void Awake()
    {
        settings = settings ?? LevelSettings.CreateDefaultInstance();
        Current = this;
    }

    private void Start()
    {
        if (cameraController == null)
        {
            cameraController = FindObjectOfType<CameraController>();
        }

        Tower tower = Instantiate(towerPrefab, transform).GetComponentInChildren<Tower>();
        tower.levels = settings.towerLevels;
        tower.blocksPerLevel = settings.blocksPerTowerLevel;
        tower.blockColorIds = ChooseRandomBlockColorIds(settings.blockColorsNumber);
    }

    public void OnClick(Vector2 point)
    {
        Ray ray = cameraController.gameCamera.ScreenPointToRay(point);
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            Block block = hitInfo.transform.GetComponentInParent<Block>();
            if (block != null)
            {
                ThrowBall(block, hitInfo.point);
            }
        }
    }

    public void OnDrag(Vector2 delta)
    {
        cameraController.RotateByRelativeAmount(delta.x / Screen.width);
    }

    private int[] ChooseRandomBlockColorIds(int blockColorsNumber)
    {
        if (blockColorsNumber <= 0)
        {
            return null;
        }

        int availableColorsLength = LevelSettings.BlockColors.Length;
        int[] availableNumbers = new int[availableColorsLength];
        for (int i = 0; i < availableColorsLength; i++)
        {
            availableNumbers[i] = i;
        }

        int[] result = new int[blockColorsNumber];
        for (int i = 0; i < blockColorsNumber; i++)
        {
            int j = Random.Range(i, availableColorsLength);
            result[i] = availableNumbers[j];
            availableNumbers[j] = availableNumbers[i];
        }
        return result;
    }

    private async void ThrowBall(Block block, Vector3 hitPosition)
    {
        float gravity = Physics.gravity.y * throwGravityMultiplier;

        Vector3 initialBallPosition = cameraController.gameCamera.transform.TransformPoint(throwCameraPositionOffset);
        GameObject ball = Instantiate(ballPrefab, initialBallPosition, Quaternion.identity);

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

        Destroy(ball);
        block.Explode();
    }
}
