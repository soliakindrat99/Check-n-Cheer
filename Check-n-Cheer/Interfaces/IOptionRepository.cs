using System;
using System.Collections.Generic;
using Check_n_Cheer.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Check_n_Cheer.Interfaces
{
    public interface IOptionRepository
    {
        void AddOption(Option option);
        Option GetOption(Guid id);
        List<Option> GetOptions();
        List<Option> GetOptions(Guid taskId);
        void UpdateOption(Guid id, Option updatedOption);
        void RemoveOption(Guid id);
    }
}
