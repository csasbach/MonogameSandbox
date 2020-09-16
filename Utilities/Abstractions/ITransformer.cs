using Microsoft.Xna.Framework;

namespace Utilities.Abstractions
{
    /// <summary>
    /// Provides a transform for rendering
    /// </summary>
    public interface ITransformer
    {
        Matrix Transform { get; }
    }
}
