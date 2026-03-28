
//AI
public class OctreeQuantizer
{
    private readonly OctreeNode _root = new();
    private readonly OctreeNode?[] _reducibleNodes = new OctreeNode?[7];
    private int _leafCount;
    private readonly int _maxColors;

    public OctreeQuantizer(int maxColors) => _maxColors = maxColors;

    public void AddColor(byte r, byte g, byte b)
    {
        _root.AddColor(r, g, b, 0, this);
        if (_leafCount > _maxColors * 2)
            while (_leafCount > _maxColors)
                Reduce();
    }

    public List<(byte R, byte G, byte B)> GetPalette()
    {
        while (_leafCount > _maxColors)
            Reduce();
        var colors = new List<(byte, byte, byte)>(_maxColors);
        _root.CollectLeaves(colors);
        return colors;
    }

    internal void TrackLeaf() => _leafCount++;

    internal void AddReducible(OctreeNode node, int level)
    {
        node.Next = _reducibleNodes[level];
        _reducibleNodes[level] = node;
    }

    private void Reduce()
    {
        int level = 6;
        while (level >= 0 && _reducibleNodes[level] == null) level--;
        if (level < 0) return;

        var node = _reducibleNodes[level]!;
        _reducibleNodes[level] = node.Next;
        int leavesRemoved = node.Reduce();
        _leafCount -= leavesRemoved;
        _leafCount++; // merged node becomes one leaf
    }

    public class OctreeNode
    {
        private readonly OctreeNode?[] _children = new OctreeNode[8];
        private long _rSum, _gSum, _bSum;
        private int _pixelCount;
        private bool _isLeaf;
        private bool _isTracked;
        public OctreeNode? Next;

        public void AddColor(byte r, byte g, byte b, int level, OctreeQuantizer tree)
        {
            if (_isLeaf || level >= 7)
            {
                if (!_isLeaf)
                {
                    _isLeaf = true;
                    tree.TrackLeaf();
                }
                _rSum += r;
                _gSum += g;
                _bSum += b;
                _pixelCount++;
                return;
            }

            int bit = 6 - level;
            int index = (((r >> bit) & 1) << 2) | (((g >> bit) & 1) << 1) | ((b >> bit) & 1);

            if (_children[index] == null)
            {
                _children[index] = new OctreeNode();
                if (level < 6 && !_children[index]!._isTracked)
                {
                    tree.AddReducible(_children[index]!, level);
                    _children[index]!._isTracked = true;
                }
            }

            _children[index]!.AddColor(r, g, b, level + 1, tree);
        }

        // Returns number of child leaves removed (caller adds 1 back for this node becoming a leaf)
        public int Reduce()
        {
            int removed = 0;
            for (int i = 0; i < 8; i++)
            {
                var child = _children[i];
                if (child == null) continue;
                _rSum += child._rSum;
                _gSum += child._gSum;
                _bSum += child._bSum;
                _pixelCount += child._pixelCount;
                removed++;
                _children[i] = null;
            }
            _isLeaf = true;
            return removed;
        }

        public void CollectLeaves(List<(byte R, byte G, byte B)> colors)
        {
            if (_isLeaf && _pixelCount > 0)
            {
                byte r = (byte)(_rSum / _pixelCount);
                byte g = (byte)(_gSum / _pixelCount);
                byte b = (byte)(_bSum / _pixelCount);
                colors.Add((r, g, b));
                return;
            }
            foreach (var child in _children)
                child?.CollectLeaves(colors);
        }
    }
}