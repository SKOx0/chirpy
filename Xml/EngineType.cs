using System.ComponentModel;

namespace Zippy.Chirp.Xml
{
    public enum EngineType
    {
         /// <summary>
        /// Not set
        /// </summary>
        Unspecified,

        /// <summary>
        /// Less engine
        /// </summary>
        [Description("Less")]
        Less
    }
}
