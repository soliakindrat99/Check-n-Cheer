using System;
using System.Collections.Generic;
using Check_n_Cheer.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Check_n_Cheer.Interfaces
{
    public interface IOptionRepository
    {
        Option GetOption(Guid id);
        List<Option> GetOptions(Guid taskId);
        void RemoveOption(Guid id);
        void AddOption(Option option);
        void UpdateOption(Guid id, Option updatedOption);
    }
}
