using System;
using System.IO;

namespace Fireman.Core.User;

/// <summary>
/// Sorting strategy for file system items
/// </summary>
public abstract class SortStrategy {
    /// <summary>
    /// Sorts the given items
    /// </summary>
    /// <param name="items">The items array</param>
    /// <param name="context">Additional preferences for sorting</param>
    /// <returns>Sorted items array</returns>
    public abstract FileSystemInfo[] SortItems(FileSystemInfo[] items, SortContext context);

    /// <summary>
    /// Sorts items with context
    /// </summary>
    /// <param name="x">First item</param>
    /// <param name="y">Second item</param>
    /// <param name="context">The sorting context</param>
    /// <param name="primaryComparison">The main comparison that doesn't have to take the context into account</param>
    /// <returns>The comparison result</returns>
    protected static int CompareWithContext(
        FileSystemInfo x,
        FileSystemInfo y,
        SortContext context,
        Func<FileSystemInfo, FileSystemInfo, int> primaryComparison
    ) {
        // Context: folders first?
        if (context.FoldersFirst) {
            bool xIsDir = x is DirectoryInfo;
            bool yIsDir = y is DirectoryInfo;

            if (xIsDir && !yIsDir) return -1;
            if (!xIsDir && yIsDir) return 1;
        }

        // Primary sort logic
        int result = primaryComparison(x, y);

        // Context: reverse?
        return context.Reverse ? -result : result;
    }
}
