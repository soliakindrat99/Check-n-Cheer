using System;
using System.Collections.Generic;
using Check_n_Cheer.Models;

namespace Check_n_Cheer.Interfaces
{
    public interface IOptionResultRepository
    {
        void AddOptionResult(OptionResult result);
        OptionResult GetOptionResult(Guid id);
        OptionResult GetOptionResult(Guid taskResultId, Guid optionId);
        void UpdateOptionResult(Guid id, OptionResult updatedResult);
        void RemoveOptionResult(Guid id);
    }
}
