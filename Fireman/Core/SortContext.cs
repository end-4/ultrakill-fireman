using Fireman.Interface;

namespace Fireman.Core;

/// <summary>
/// Context for sorting
/// </summary>
public class SortContext {
    /// <summary>
    /// Whether to reverse the sort
    /// </summary>
    public bool Reverse;

    /// <summary>
    /// Whether folders go before files
    /// </summary>
    public bool FoldersFirst;

    /// <summary>
    /// Constructor for a sort context
    /// </summary>
    public SortContext(bool reverse = false) {
        Reverse = reverse;
        FoldersFirst = Config.SortFoldersBeforeFiles.Value;
    }
}
