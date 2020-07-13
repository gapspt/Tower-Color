using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Level : MonoBehaviour
{
    private const int BaseRandomSeed = 1000000;

    public LevelSettings settings;

    public CameraController cameraController;

    public GameObject towerPrefab;
    public GameObject ballPrefab;

    public float throwSpeed = 35;
    public float throwGravityMultiplier = 2;
    public Vector3 throwCameraPositionOffset;

    private int[] blockColorIds;
    private int[] ballsColorIds;
    private Tower tower;
    private int towerBlocksLayerMask;

    private Ball currentBall;
    private bool levelRunning = false;
    private bool levelWon;
    private int availableBalls;
    private int lockedLevels;
    private int winRequiredBlocks;
    private int winRemainingBlocks;
    private int[] standingBlocksPerLevel;
    private IDictionary<Power, int> availablePowers;

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

        LoadLevel(false);
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
        if (!levelRunning)
        {
            return;
        }

        winRemainingBlocks--;
        UIManager.Current?.UpdateLevelProgress((winRequiredBlocks - winRemainingBlocks) / (float)winRequiredBlocks);

        if (winRemainingBlocks == 0)
        {
            FinishGame(true);
            return;
        }

        int levelStandingBlocks = --standingBlocksPerLevel[block.TowerLevel];
        if (lockedLevels > 0 && levelStandingBlocks == 0)
        {
            tower.SetLevelLocked(--lockedLevels, false);
            Vector3 lookAtPosition = (tower.GetLevelLocalPosition(lockedLevels) +
                         tower.GetLevelLocalPosition(lockedLevels + settings.towerUnlockedLevels)) / 2;
            cameraController.MoveToLevelAtPosition(lookAtPosition);
        }
    }

    public async void LoadLevel(bool showTransitionOverlay = true)
    {
        if (showTransitionOverlay)
        {
            await UIManager.Current?.SetTransitionOverlayVisible(true);
        }
        UIManager.Current?.HideAll();

        if (tower != null)
        {
            Destroy(tower.gameObject);
        }

        int towerLevels = settings.towerLevels;
        int blocksPerTowerLevel = settings.blocksPerTowerLevel;

        // Initialize the level random state
        Random.InitState(BaseRandomSeed + SavedData.GamesWon);
        Random.Range(0f, 1f); // Ignore the first value

        ChooseRandomBlockColors(settings.blockColorsNumber);

        tower = Instantiate(towerPrefab, transform).GetComponentInChildren<Tower>();
        tower.levels = towerLevels;
        tower.blocksPerLevel = blocksPerTowerLevel;
        tower.blockColorIds = blockColorIds;
        tower.Setup();

        availableBalls = settings.availableBalls;
        ChooseRandomBallColors(availableBalls);

        levelWon = false;
        winRequiredBlocks = towerLevels * blocksPerTowerLevel - settings.winMaxStandingBlocks;
        winRemainingBlocks = winRequiredBlocks;
        standingBlocksPerLevel = new int[towerLevels];
        for (int i = towerLevels - 1; i >= 0; i--)
        {
            standingBlocksPerLevel[i] = blocksPerTowerLevel;
        }
        availablePowers = settings.availablePowers
            .ToDictionary(entry => entry.power, entry => entry.count);

        Vector3 lookAtPosition = (tower.GetLevelLocalPosition(0) +
            tower.GetLevelLocalPosition(settings.towerUnlockedLevels)) / 2;
        cameraController.SetLookAtPosition(lookAtPosition);

        UIManager.Current?.UpdateLevelNumber(SavedData.GamesWon + 1);
        if (showTransitionOverlay)
        {
            await UIManager.Current?.SetTransitionOverlayVisible(false);
        }
        UIManager.Current?.SetStartScreenVisible(true);
    }

    public async void StartLevel()
    {
        UIManager.Current?.SetStartScreenVisible(false);

        int levels = settings.towerLevels;
        int unlockedLevels = settings.towerUnlockedLevels;
        Vector3 lookAtPosition = (tower.GetLevelLocalPosition(levels) +
             tower.GetLevelLocalPosition(levels - unlockedLevels)) / 2;
        await cameraController.MoveAtLevelStart(lookAtPosition);

        lockedLevels = levels - settings.towerUnlockedLevels;
        for (int i = levels - 1; i >= 0; i--)
        {
            bool locked = i < lockedLevels;
            tower.SetLevelLocked(i, locked);
        }

        SetupBall();

        UIManager.Current?.UpdateAvailableBalls(availableBalls);
        UIManager.Current?.UpdateAvailablePowers(availablePowers);
        UIManager.Current?.SetHudVisible(true);
        UIManager.Current?.UpdateLevelProgress(0);
        UIManager.Current?.SetLevelProgressVisible(true);

        levelRunning = true;
    }

    public void ActivatePower(Power power)
    {
        availablePowers.TryGetValue(Power.Rainbow, out int count);
        if (count > 0 && currentBall != null)
        {
            availablePowers[power] = --count;
            currentBall.SetPower(power);
            UIManager.Current?.UpdateAvailablePowers(availablePowers);
        }
    }

    private void ChooseRandomBlockColors(int blockColorsNumber)
    {
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

    private void ChooseRandomBallColors(int availableBalls)
    {
        int colorsNumber = blockColorIds.Length;

        ballsColorIds = new int[availableBalls];
        for (int i = 0; i < availableBalls; i++)
        {
            ballsColorIds[i] = blockColorIds[i % colorsNumber];
        }

        for (int i = 0; i < availableBalls; i++)
        {
            int j = Random.Range(i, availableBalls);
            int temp = ballsColorIds[i];
            ballsColorIds[i] = ballsColorIds[j];
            ballsColorIds[j] = temp;
        }
    }

    private void SetupBall()
    {
        GameObject ballObject = Instantiate(ballPrefab, cameraController.gameCamera.transform);
        ballObject.transform.localPosition = throwCameraPositionOffset;
        currentBall = ballObject.GetComponentInChildren<Ball>();
        currentBall.Setup(ballsColorIds[availableBalls - 1]);
    }

    private async void ThrowBall(Block block, Vector3 hitPosition)
    {
        Ball ball = currentBall;

        availableBalls--;
        bool lastBall = availableBalls == 0;
        if (!lastBall)
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

        if (block.ColorId == ball.ColorId || ball.power == Power.Rainbow)
        {
            block.Explode();
        }
        Destroy(ball.gameObject);


        if (lastBall)
        {
            UIManager.Current?.UpdateLastBallTimer(1);
            UIManager.Current?.SetLastBallTimerVisible(true);

            float loseDelay = settings.loseDelay;
            float loseTime = Time.time + loseDelay;

            await TaskUtils.WaitForNextUpdate(this);
            for (float time = Time.time; time < loseTime && !levelWon; time = Time.time)
            {
                UIManager.Current?.UpdateLastBallTimer((loseTime - time) / loseDelay);
                await TaskUtils.WaitForNextUpdate(this);
            }

            UIManager.Current?.UpdateLastBallTimer(0);
            UIManager.Current?.SetLastBallTimerVisible(false);

            if (!levelWon)
            {
                FinishGame(false);
            }
        }
    }

    private void FinishGame(bool won)
    {
        levelRunning = false;
        levelWon = won;
        if (won)
        {
            SavedData.GamesWon++;
        }
        if (currentBall != null)
        {
            Destroy(currentBall.gameObject);
            currentBall = null;
        }
        UIManager.Current?.SetHudVisible(false);
        UIManager.Current?.UpdateLevelWon(won);
        UIManager.Current?.SetEndScreenVisible(true);
    }
}
