using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Check_n_Cheer.Interfaces;
using Check_n_Cheer.Models;

namespace Check_n_Cheer.Repositories
{
    public class OptionResultRepository: IOptionResultRepository
    {
        private readonly CheckCheerContext _context;
        public OptionResultRepository(CheckCheerContext context)
        {
            _context = context;
        }

        public void AddOptionResult(OptionResult result)
        {
            _context.Add(result);
            _context.SaveChanges();
        }
        public OptionResult GetOptionResult(Guid id)
        {
            var result = _context.OptionResults
                .Include(x => x.TaskResult)
                .Include(x => x.Option)
                .FirstOrDefault(x => x.Id == id);
            return result;
        }
        public OptionResult GetOptionResult(Guid taskResultId, Guid optionId)
        {
            var result = _context.OptionResults
                .Include(x => x.TaskResult)
                .Include(x => x.Option)
                .FirstOrDefault(x => x.TaskResult.Id == taskResultId && x.Option.Id == optionId);
            return result;
        }

        public void UpdateOptionResult(Guid id, OptionResult updatedResult)
        {
            var result = _context.OptionResults.FirstOrDefault(u => u.Id == id);
            result.IsChecked = updatedResult.IsChecked;
            _context.SaveChanges();
        }
        public void RemoveOptionResult(Guid id)
        {
            var result = _context.OptionResults.FirstOrDefault(u => u.Id == id);
            _context.OptionResults.Remove(result);
            _context.SaveChanges();
        }

    }
}
