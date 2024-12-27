using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public readonly struct Match
{
    public readonly int Length { get; }
    public readonly int X { get; }
    public readonly int Y { get; }
    public readonly bool IsVertical { get; }

    public Match(int length, int x, int y, bool isVertical)
    {
        Length = length;
        X = x;
        Y = y;
        IsVertical = isVertical;
    }
}

public class Grid : MonoBehaviour
{
    private const int COLUMNS = 6;
    private const int ROWS = 8;
    private const int MIN_MATCH_LENGTH = 3;

    [SerializeField] private int difficulty = 3;
    [SerializeField] private GameObject[] icons;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private RuntimeAnimatorController[] controllers;

    private readonly int[,] map = new int[COLUMNS, ROWS];
    private readonly List<Match> matches = new List<Match>();

    private void Start()
    {
        DOTween.SetTweensCapacity(1250, 50);
        InitializeGrid();
        CheckMap();
    }

    private void InitializeGrid()
    {
        for (int y = 0; y < ROWS; y++)
        {
            for (int x = 0; x < COLUMNS; x++)
            {
                int index = x + (y * COLUMNS);
                int iconType = UnityEngine.Random.Range(0, difficulty);

                UpdateIcon(index, iconType);
                map[x, y] = iconType;
            }
        }
    }

    private void UpdateIcon(int index, int iconType)
    {
        if (index < 0 || index >= icons.Length)
        {
            Debug.LogError($"Invalid icon index: {index}");
            return;
        }

        var icon = icons[index];
        var spriteRenderer = icon.GetComponent<SpriteRenderer>();
        var animator = icon.GetComponent<Animator>();

        animator.runtimeAnimatorController = controllers[iconType];
        spriteRenderer.sprite = sprites[iconType];
    }

    private void UpdateMap()
    {
        for (int y = 0; y < ROWS; y++)
        {
            for (int x = 0; x < COLUMNS; x++)
            {
                int index = x + (y * COLUMNS);
                UpdateIcon(index, map[x, y]);
            }
        }
    }

    private void CheckMap()
    {
        matches.Clear();
        FindHorizontalMatches();
        FindVerticalMatches();

        if (matches.Count > 0)
        {
            matches.Sort((a, b) => b.Length.CompareTo(a.Length));
            StartCoroutine(ProcessMatchesWithPause());
        }
    }

    private void FindHorizontalMatches()
    {
        for (int y = 0; y < ROWS; y++)
        {
            int currentType = -1;
            int matchLength = 1;

            for (int x = 0; x < COLUMNS; x++)
            {
                if (map[x, y] == currentType)
                {
                    matchLength++;
                }
                else
                {
                    if (matchLength >= MIN_MATCH_LENGTH)
                    {
                        matches.Add(new Match(matchLength, x - 1, y, false));
                    }
                    currentType = map[x, y];
                    matchLength = 1;
                }
            }

            if (matchLength >= MIN_MATCH_LENGTH)
            {
                matches.Add(new Match(matchLength, COLUMNS - 1, y, false));
            }
        }
    }

    private void FindVerticalMatches()
    {
        for (int x = 0; x < COLUMNS; x++)
        {
            int currentType = -1;
            int matchLength = 1;

            for (int y = 0; y < ROWS; y++)
            {
                if (map[x, y] == currentType)
                {
                    matchLength++;
                }
                else
                {
                    if (matchLength >= MIN_MATCH_LENGTH)
                    {
                        matches.Add(new Match(matchLength, x, y - 1, true));
                    }
                    currentType = map[x, y];
                    matchLength = 1;
                }
            }

            if (matchLength >= MIN_MATCH_LENGTH)
            {
                matches.Add(new Match(matchLength, x, ROWS - 1, true));
            }
        }
    }

    private IEnumerator ProcessMatchesWithPause()
    {
        Time.timeScale = 0;  // Pause the game

        while (matches.Count > 0)
        {
            Match currentMatch = matches[0];
            matches.RemoveAt(0);

            if (currentMatch.IsVertical)
            {
                TriggerVerticalAnimation(currentMatch);
            }
            else
            {
                TriggerHorizontalAnimation(currentMatch);
            }

            yield return new WaitForSecondsRealtime(0.75f);

            Time.timeScale = 1;  // Temporarily unpause for tile dropping

            if (currentMatch.IsVertical)
            {
                ClearVerticalMatch(currentMatch);
            }
            else
            {
                ClearHorizontalMatch(currentMatch);
            }

            UpdateMap();

            yield return new WaitForSecondsRealtime(0.1f);
            Time.timeScale = 0;  // Pause again for next match
        }

        Time.timeScale = 1;  // Resume normal time
        CheckMap();
    }

    private void TriggerVerticalAnimation(Match match)
    {
        int x = match.X;
        for (int y = match.Y; y > match.Y - match.Length; y--)
        {
            int index = x + (y * COLUMNS);
            if (index >= 0 && index < icons.Length)
            {
                Animator animator = icons[index].GetComponent<Animator>();
                animator.SetTrigger("pop");
            }
        }
    }

    private void TriggerHorizontalAnimation(Match match)
    {
        int y = match.Y;
        for (int x = match.X; x > match.X - match.Length; x--)
        {
            int index = x + (y * COLUMNS);
            if (index >= 0 && index < icons.Length)
            {
                Animator animator = icons[index].GetComponent<Animator>();
                animator.SetTrigger("pop");
            }
        }
    }

    private void ClearVerticalMatch(Match match)
    {
        if (match.X < 0 || match.X >= COLUMNS || match.Y < 0 || match.Y >= ROWS)
        {
            Debug.LogError($"Invalid match coordinates: X={match.X}, Y={match.Y}");
            return;
        }

        for (int y = match.Y; y >= 0; y--)
        {
            int sourceY = y - match.Length;

            if (sourceY >= 0)
            {
                map[match.X, y] = map[match.X, sourceY];
            }
            else
            {
                map[match.X, y] = UnityEngine.Random.Range(0, difficulty);
            }
        }
    }

    private void ClearHorizontalMatch(Match match)
    {
        if (match.X < 0 || match.X >= COLUMNS || match.Y < 0 || match.Y >= ROWS)
        {
            Debug.LogError($"Invalid match coordinates: X={match.X}, Y={match.Y}");
            return;
        }

        for (int x = match.X; x > match.X - match.Length && x >= 0; x--)
        {
            for (int y = match.Y; y > 0; y--)
            {
                map[x, y] = map[x, y - 1];
            }
            map[x, 0] = UnityEngine.Random.Range(0, difficulty);
        }
    }

    public void Swap(int startX, int startY, int targetX, int targetY)
    {
        if (!IsValidPosition(startX, startY) || !IsValidPosition(targetX, targetY))
        {
            Debug.LogError($"Invalid swap coordinates: ({startX},{startY}) to ({targetX},{targetY})");
            return;
        }

        (map[startX, startY], map[targetX, targetY]) = (map[targetX, targetY], map[startX, startY]);
        CheckMap();
        UpdateMap();
    }

    private bool IsValidPosition(int x, int y)
    {
        return x >= 0 && x < COLUMNS && y >= 0 && y < ROWS;
    }
}