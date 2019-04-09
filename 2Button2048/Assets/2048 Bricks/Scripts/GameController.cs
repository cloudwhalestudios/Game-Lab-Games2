using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;



public class GameController : MonoBehaviour
{
    public Vector2Int bricksCount;      // Move to: Game Option for grid size (experimental: no resacling applied yet)
    public RectTransform fieldTransform;
    public RectTransform placementTransform;
    public RectTransform placementIndicator;
    public int placementStart;
    public float autoMoveInterval;      // Move to: Assistance Option for selection speeds
    public float fallSpeed;             // Move to: Game Option for falling speed when playing brick
    public Transform nextBrickPoint;
    public Brick brickPrefab;

    public PlaySfx landingSfx;
    public PlaySfx mergingSfx;

    Brick[,] field;
    Brick[] placement;
    Brick previewBrick;

    Vector2Int currentPosition;
    int lastPlacementPosition;
    Brick currentBrickInstance;

    bool isFalling;
    float timeSinceMoveDown;

    bool isAnimating;
    bool isPlaced;

    float elapsedTime = 0;              // Move to: Assistance Option Script to keep track of time inbetween intervals

    public GameObject pauseMenu;                // Move to: Menu Handler
    public GameObject pauseButtonParent;        // Move to: Menu Handler
    public RectTransform menuSelectIndicator;   // Move to: Menu Handler
    public int startingIndex;                   // Move to: Menu Handler
    public bool startPaused;                    // Move to: Menu Handler
    int selectedButtonIndex;                    // Move to: Menu Handler
    List<Button> buttons;                       // Move to: Menu Handler
    Coroutine menuSelector;                     // Move to: Menu Handler

    class BrickPath
    {
        public Brick brick;
        public List<Vector2Int> path;
    }

    int GetRandomNumber()
    {
        return (int) Mathf.Pow(2, Random.Range(1, 5));
    }

    void Start()
    {
        InputController.ActiveInputMode = InputController.InputMode.Game;

        SpawnPreviewBrick();

        placement = new Brick[bricksCount.x];
        field = new Brick[bricksCount.x, bricksCount.y];

        if (!LoadGame())
        {
            UserProgress.Current.Score = 0;
            lastPlacementPosition = placementStart;
            currentPosition = new Vector2Int(lastPlacementPosition, 0);
            SpawnPlacement(lastPlacementPosition, GetRandomNumber());
            previewBrick.Number = GetRandomNumber();
        }

        SetupPauseMenu();
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }

    public void OnPrimaryGame()
    {
        // Move brick down
        if (isAnimating || isFalling)
            return;

        isFalling = true;
        timeSinceMoveDown = 0f;
        MoveDown();
    }

    public void OnSecondaryGame()
    {
        Pause();
    }

    public void OnPrimaryPause()
    {
        // perform select button action
        buttons[selectedButtonIndex].onClick.Invoke();
    }

    public void OnSecondaryPause()
    {
        //Quit();
    }

    private void SetupPauseMenu()
    {
        pauseMenu.SetActive(false);

        buttons = new List<Button>(pauseButtonParent.GetComponentsInChildren<Button>());

        if (startPaused)
        {
            Pause();
        }
    }

    // Move to: Menu Handler
    public void Pause(bool pause = true)
    {
        Time.timeScale = 1 - Time.timeScale;
        pauseMenu.SetActive(!pauseMenu.activeInHierarchy);

        if (Time.timeScale == 1)
        {
            InputController.ActiveInputMode = InputController.InputMode.Game;

            if (menuSelector != null)
            {
                StopCoroutine(menuSelector);
                menuSelector = null;
            }
        }
        else
        {
            InputController.ActiveInputMode = InputController.InputMode.Pause;
            menuSelector = StartCoroutine(MenuSelection());
            
        }
    }


    // Move to: Menu Handler
    IEnumerator MenuSelection()
    {
        selectedButtonIndex = startingIndex;
        yield return null;
        while (true)
        {
            Debug.Log("Selection " + selectedButtonIndex);
            IndicateMenuButton(selectedButtonIndex);
            yield return new WaitForSecondsRealtime(autoMoveInterval);

            selectedButtonIndex = (selectedButtonIndex + 1) % buttons.Count;
        }
    }

    // Move to: Menu Handler
    void IndicateMenuButton(int index)
    {
        var btnRect = buttons[index].GetComponent<RectTransform>();
        var pos = new Vector2(btnRect.localPosition.x, btnRect.localPosition.y);
        menuSelectIndicator.anchoredPosition = pos;
    }

    public void Restart()
    {
        if (Time.timeScale == 0)
        {
            Pause();
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        UserProgress.Current.SetField(new int[0]);
        return;
    }

    public void ClearSave()
    {
        PlayerPrefs.DeleteAll();
        Restart();
    }

    public void Quit()
    {
        // Exit Game
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBGL
        Application.OpenURL("google.com");
#else
        Application.Quit();
#endif
    }


    void Update()
    {
        if (isAnimating)
            return;

        timeSinceMoveDown += Time.deltaTime;

        if (isFalling && timeSinceMoveDown >= 1f / fallSpeed)
        {
            timeSinceMoveDown -= 1f / fallSpeed;
            MoveDown();
        }

        // Auto move
        if (!isFalling && !isPlaced)
        { 
            if (elapsedTime >= autoMoveInterval)
            {
                elapsedTime = 0;
                // Move right
                MoveHorizontaly(1);
            }
            elapsedTime += Time.deltaTime;
        }
    }

    bool LoadGame()
    {
        int[] numbers = UserProgress.Current.GetField();
        if (numbers == null || numbers.Length != bricksCount.x * bricksCount.y)
            return false;

        for (int x = 0; x < bricksCount.x; x++)
        {
            for (int y = 0; y < bricksCount.y; y++)
            {
                if (numbers[x * bricksCount.y + y] > 0)
                    InstantiateBrick(new Vector2Int(x, y), numbers[x * bricksCount.y + y]);
            }
        }

        lastPlacementPosition = placementStart;
        currentPosition = new Vector2Int(lastPlacementPosition, 0); // UserProgress.Current.CurrentBrick;
        previewBrick.Number = UserProgress.Current.NextBrick;
        SpawnPlacement(lastPlacementPosition, UserProgress.Current.CurrentBrickValue);
        return true;
    }

    void SaveGame()
    {
        int[] numbers = new int[bricksCount.x * bricksCount.y];
        for (int x = 0; x < bricksCount.x; x++)
        {
            for (int y = 0; y < bricksCount.y; y++)
            {
                numbers[x * bricksCount.y + y] = field[x, y] != null ? field[x, y].Number : 0;
            }
        }

        UserProgress.Current.SetField(numbers);
        UserProgress.Current.CurrentBrickValue = currentBrickInstance.Number;
        UserProgress.Current.NextBrick = previewBrick.Number;
        UserProgress.Current.Save();
    }

    void SpawnPlacement(int horizontal, int number)
    {
        isPlaced = false;
        currentBrickInstance = Instantiate(brickPrefab, placementTransform);
        currentBrickInstance.Number = number;

        currentBrickInstance.transform.SetParent(placementTransform, false);

        currentBrickInstance.GetComponent<RectTransform>().anchorMin = Vector2.zero;
        currentBrickInstance.GetComponent<RectTransform>().anchorMax = Vector2.zero;
        currentBrickInstance.GetComponent<RectTransform>().anchoredPosition 
            = GetBrickPosition(new Vector2(horizontal, 0));

        placement[horizontal] = currentBrickInstance;

        MoveIndicator();
        elapsedTime = 0;
    }

    void MoveToField(Vector2Int coords)
    {
        // remove from placement
        Brick brick = placement[coords.x];

        placement[coords.x] = null;

        currentPosition = new Vector2Int(coords.x, bricksCount.y - 1);

        if (field[currentPosition.x, currentPosition.y] != null)
        {
            Restart();
        }

        Spawn(currentPosition, brick);
    }

    void Spawn(Vector2Int coords, Brick brick = null)
    {
        if (brick == null)
        {
            brick = currentBrickInstance;
        }

        //Debug.Log(brick + " " + coords.ToString());

        brick.transform.SetParent(fieldTransform, false);
        brick.GetComponent<RectTransform>().anchorMin = Vector2.zero;
        brick.GetComponent<RectTransform>().anchorMax = Vector2.zero;
        brick.GetComponent<RectTransform>().anchoredPosition
            = GetBrickPosition(new Vector2(coords.x, coords.y));

        field[coords.x, coords.y] = brick;
    }

    private void InstantiateBrick(Vector2Int position, int number)
    {
        Brick brick = Instantiate(brickPrefab, fieldTransform);
        brick.Number = number;

        Spawn(position, brick);
    }

    void SpawnPreviewBrick()
    {
        previewBrick = Instantiate(brickPrefab, nextBrickPoint);
        previewBrick.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
        previewBrick.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
        previewBrick.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }

    Vector2 GetBrickPosition(Vector2 coords)
    {
        Vector2 brickSize = new Vector2
        {
            x = fieldTransform.rect.width / bricksCount.x,
            y = fieldTransform.rect.height / bricksCount.y
        };

        RectTransform brickTransform = brickPrefab.GetComponent<RectTransform>();

        Vector2 brickPosition = Vector2.Scale(coords, brickSize);
        brickPosition += Vector2.Scale(brickSize, brickTransform.pivot);

        return brickPosition;
    }

    void MoveDown()
    {

        // Move brick into field
        if (!isPlaced)
        {
            isPlaced = true;
            MoveToField(currentPosition);
        }
        else
        {
            Brick brick = field[currentPosition.x, currentPosition.y];

            // Move down if brick is not at the bottom or ontop of another
            if (currentPosition.y > 0 && field[currentPosition.x, currentPosition.y - 1] == null)
            {
                field[currentPosition.x, currentPosition.y] = null;
                currentPosition.y--;
                field[currentPosition.x, currentPosition.y] = brick;

                brick.GetComponent<RectTransform>().anchoredPosition =
                    GetBrickPosition(new Vector2(currentPosition.x, currentPosition.y));

                SaveGame();
            }
            else
            {
                isAnimating = true;
                landingSfx.Play();
                brick.DoLandingAnimation(
                    () =>
                    {
                        isAnimating = false;
                        Merge(
                            new List<Vector2Int> { currentPosition },
                            () =>
                            {
                                isFalling = false;

                                lastPlacementPosition = currentPosition.x;
                                currentPosition = new Vector2Int(lastPlacementPosition, 0);
                                SpawnPlacement(currentPosition.x, previewBrick.Number);

                                previewBrick.Number = GetRandomNumber();

                                SaveGame();
                            }
                        );
                    }
                );
            }
        }
    }

    void MoveHorizontaly(int value)
    {
        int x = (currentPosition.x + value) % placement.Length;

        Brick brick = placement[currentPosition.x];

        placement[currentPosition.x] = null;
        currentPosition.x = x;
        placement[currentPosition.x] = brick;

        brick.gameObject.GetComponent<RectTransform>().anchoredPosition =
            GetBrickPosition(new Vector2(currentPosition.x, 0));

        MoveIndicator();
    }

    void MoveIndicator()
    {
        // Draw debug indicator
        Debug.DrawLine(currentBrickInstance.transform.position - Vector3.left * 10,
            currentBrickInstance.transform.position + Vector3.left * 10, Color.green, autoMoveInterval);
        Debug.DrawLine(currentBrickInstance.transform.position - Vector3.up * 10,
            currentBrickInstance.transform.position + Vector3.up * 10, Color.green, autoMoveInterval);

        // Move placement indicator
        //Debug.Log(currentPosition);
        placementIndicator.anchoredPosition = GetBrickPosition(new Vector2(currentPosition.x, 1));
    }

    void Merge(List<Vector2Int> toMerge, Action onComplete)
    {
        isAnimating = true;

        List<Vector2Int> newCoords = new List<Vector2Int>();

        int animationsLeft = 0;
        foreach (Vector2Int coords in toMerge)
        {
            if (field[coords.x, coords.y] == null)
                continue;

            Brick brick = field[coords.x, coords.y];
            List<Vector2Int> area = WaveAlgorithm.GetArea(
                field,
                coords,
                b => b != null && b.Number == brick.Number
            );


            if (area.Count < 2)
                continue;

            newCoords.AddRange(area);

            List<BrickPath> paths = new List<BrickPath>();
            foreach (Vector2Int toMove in area)
            {
                if (toMove == coords)
                {
                    continue;
                }

                BrickPath brickPath = new BrickPath
                {
                    brick = field[toMove.x, toMove.y],
                    path = WaveAlgorithm.GetPath(
                        field,
                        toMove,
                        coords,
                        b => b != null && b.Number == brick.Number
                    )
                };
                brickPath.path.RemoveAt(0);
                paths.Add(brickPath);
            }

            foreach (Vector2Int toMove in area)
                if (toMove != coords)
                    field[toMove.x, toMove.y] = null;

            animationsLeft++;

            int areaSize = area.Count;
            AnimateMerge(
                paths,
                () =>
                {
                    animationsLeft--;

                    if (animationsLeft > 0)
                        return;

                    mergingSfx.Play();

                    brick.Number *= Mathf.ClosestPowerOfTwo(areaSize);
                    brick.DoMergingAnimation(
                        () =>
                        {
                            if (newCoords.Count > 0)
                                Normalize(
                                    normalized =>
                                    {
                                        newCoords.AddRange(normalized);
                                        Merge(newCoords, onComplete);
                                    }
                                );
                        }
                    );

                    UserProgress.Current.Score += brick.Number;
                }
            );
        }

        if (newCoords.Count > 0)
            return;

        isAnimating = false;
        onComplete.Invoke();
    }

    void AnimateMerge(List<BrickPath> brickPaths, Action onComplete)
    {
        brickPaths.Sort((p0, p1) => p1.path.Count.CompareTo(p0.path.Count));

        int pathLength = brickPaths[0].path.Count;

        if (pathLength == 0)
        {
            brickPaths.ForEach(p => Destroy(p.brick.gameObject));
            onComplete.Invoke();
            return;
        }

        int animationsLeft = 0;
        foreach (BrickPath brickPath in brickPaths)
        {
            if (brickPath.path.Count < pathLength)
                break;

            Vector2 position = GetBrickPosition(brickPath.path[0]);

            brickPath.path.RemoveAt(0);

            animationsLeft++;
            brickPath.brick.DoLocalMove(
                position,
                () =>
                {
                    animationsLeft--;
                    if (animationsLeft == 0)
                        AnimateMerge(brickPaths, onComplete);
                }
            );
        }
    }

    void Normalize(Action<List<Vector2Int>> onComplete)
    {
        List<Vector2Int> normalized = new List<Vector2Int>();
        for (int x = 0; x < field.GetLength(0); x++)
        {
            for (int y = 0; y < field.GetLength(1); y++)
            {
                Brick brick = field[x, y];

                if (brick == null)
                    continue;

                int yEmpty = y;
                while (yEmpty > 0 && field[x, yEmpty - 1] == null)
                    yEmpty--;

                if (yEmpty == y)
                    continue;

                field[x, y] = null;
                field[x, yEmpty] = brick;
                Vector2Int brickCoords = new Vector2Int(x, yEmpty);

                normalized.Add(brickCoords);

                bool isFirst = normalized.Count == 1;
                brick.DoLocalMove(
                    GetBrickPosition(brickCoords),
                    () =>
                    {
                        if (isFirst)
                        {
                            brick.DoLandingAnimation(() => onComplete.Invoke(normalized));
                            landingSfx.Play();
                        }
                        else
                            brick.DoLandingAnimation(null);
                    }
                );
            }
        }

        if (normalized.Count == 0)
            onComplete.Invoke(normalized);
    }
}