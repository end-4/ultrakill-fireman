using System;
using System.IO;

namespace Fireman.Core.SortStrategies;

/// <summary>
/// Strategy that sorts by file extension alphabetically
/// </summary>
public class ExtensionSortStrategy : SortStrategy {
    /// <inheritdoc />
    public override FileSystemInfo[] SortItems(FileSystemInfo[] items, SortContext context) {
        Array.Sort(items, (x, y) => CompareWithContext(x, y, context, (a, b) => {
            string extA = Path.GetExtension(a.Name);
            string extB = Path.GetExtension(b.Name);

            int extCompare = string.Compare(extA, extB, StringComparison.OrdinalIgnoreCase);

            // Extensions different -> OK
            if (extCompare != 0) return extCompare;

            // Extensions same -> Compare by name
            return string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase);
        }));

        return items;
    }
}
