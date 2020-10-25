using System.Threading.Tasks;
using middler.Common.SharedModels.Models;

namespace middler.Common.SharedModels.Interfaces
{
    public interface IVariablesRepository
    {
        ITreeNode GetVariable(string parent, string name);
        Task<ITreeNode> GetFolderTree();
    }
}