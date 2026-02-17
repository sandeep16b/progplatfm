using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.Relational.Services;

namespace Abim.Platform.Program.App.Services
{
    /// <summary>
    /// ISourceService interface.
    /// </summary>
    public interface ISourceService :
        IService<Source>
    {
        /// <summary>
        /// There is one Source whose code is "ABIM". Finds and returns that Source.
        /// </summary>
        /// <returns></returns>
        Source GetAbimSource();
    }
}
