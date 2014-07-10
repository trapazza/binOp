namespace BinOp.PEG
{
    using System;

    [Flags]
    public enum AstNodeFlags
    {
        /// <summary>
        /// Default
        /// </summary>
        None = 0,
        
        /// <summary>
        /// Ignores this node in case it has just a single child
        /// </summary>
        IgnoreIfSingleChild = 1 << 0,

        /// <summary>
        /// Makes this node invisible
        /// </summary>
        Ignore = 1 << 1,
    }
}