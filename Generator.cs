using System;
using System.Collections.Generic;
using System.Linq;

namespace WordGameLogic
{
    public class Generator
    {
        private char[,] _grid;
        private int _width, _height;
        private Random _random = new Random();
        private int _iterations;
        public int MaxIterations = 10000;

        private readonly int[] dx = { 0, 0, 1, -1 };
        private readonly int[] dy = { 1, -1, 0, 0 };

        public bool TryGenerate(int width, int height, List<string> dictionary, out char[,] grid, out List<WordData> placedWords)
        {
            _width = width;
            _height = height;
            _grid = new char[width, height];
            _iterations = 0;
            placedWords = new List<WordData>();
            grid = null;

            if (Fill(dictionary.OrderByDescending(w => w.Length).ToList(), placedWords))
            {
                grid = _grid;
                return true;
            }
            return false;
        }

        private bool Fill(List<string> availableDict, List<WordData> placedWords)
        {
            _iterations++;
            if (_iterations > MaxIterations || !FindFirstEmpty(out int x, out int y))
                return _iterations <= MaxIterations;

            if (HasDeadEnds()) return false;

            var pool = availableDict.OrderBy(a => _random.Next()).ToList();
            foreach (var word in pool)
            {
                var path = new List<Point>();
                if (TryPlaceSnake(word, 0, x, y, path))
                {
                    placedWords.Add(new WordData { Word = word, Path = new List<Point>(path) });
                    if (Fill(availableDict.Where(w => w != word).ToList(), placedWords)) return true;

                    // Backtrack
                    foreach (var p in path) _grid[p.X, p.Y] = (placedWords.Any(w => w != placedWords.Last() && w.Path.Any(pp => pp.X == p.X && pp.Y == p.Y))) ? _grid[p.X, p.Y] : '\0';
                    placedWords.RemoveAt(placedWords.Count - 1);
                }
            }
            return false;
        }

        private bool TryPlaceSnake(string word, int index, int x, int y, List<Point> path)
        {
            if (index == word.Length) return true;
            if (x < 0 || x >= _width || y < 0 || y >= _height) return false;
            if (_grid[x, y] != '\0' && _grid[x, y] != word[index]) return false;
            if (path.Any(p => p.X == x && p.Y == y)) return false;

            char oldChar = _grid[x, y];
            _grid[x, y] = word[index];
            path.Add(new Point(x, y));

            var dirs = Enumerable.Range(0, 4).OrderBy(a => _random.Next()).ToArray();
            foreach (var d in dirs)
                if (TryPlaceSnake(word, index + 1, x + dx[d], y + dy[d], path)) return true;

            _grid[x, y] = oldChar;
            path.RemoveAt(path.Count - 1);
            return false;
        }

        private bool HasDeadEnds()
        {
            for (int y = 0; y < _height; y++)
                for (int x = 0; x < _width; x++)
                    if (_grid[x, y] == '\0')
                    {
                        int empty = 0;
                        for (int i = 0; i < 4; i++)
                        {
                            int nx = x + dx[i], ny = y + dy[i];
                            if (nx >= 0 && nx < _width && ny >= 0 && ny < _height && _grid[nx, ny] == '\0') empty++;
                        }
                        if (empty == 0) return true;
                    }
            return false;
        }

        private bool FindFirstEmpty(out int x, out int y)
        {
            for (y = 0; y < _height; y++)
                for (x = 0; x < _width; x++)
                    if (_grid[x, y] == '\0') return true;
            x = -1; y = -1; return false;
        }
    }
}
