using System;

namespace Utilities.Attributes
{
    public sealed class SceneAttribute : Attribute
    {
        public string DisplayName { get; }
        public SceneAttribute(string displayName)
        {
            DisplayName = displayName;
        }
    }
}
