using System;
using System.IO;

namespace Fireman.Core.SortStrategies;

/// <summary>
/// Strategy that sorts by file name alphabetically
/// </summary>
public class NameSortStrategy : SortStrategy {
    /// <inheritdoc />
    public override FileSystemInfo[] SortItems(FileSystemInfo[] items, SortContext context) {
        Array.Sort(items, (x, y) => CompareWithContext(x, y, context, (a, b) =>
            string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase)));

        return items;
    }
}
