using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Check_n_Cheer.Interfaces;
using Check_n_Cheer.Models;

namespace Check_n_Cheer.Repositories
{
    public class OptionRepository : IOptionRepository
    {
        private readonly CheckCheerContext _context;
        public OptionRepository(CheckCheerContext context)
        {
            _context = context;
        }

        public void AddOption(Option option)
        {
            _context.Add(option);
            _context.SaveChanges();
        }

        public Option GetOption(Guid id)
        {
            var option = _context.Options.FirstOrDefault(u => u.Id == id);
            return option;
        }

        public List<Option> GetOptions()
        {
            var options = _context.Options.Include(x => x.Task).ToList();
            return options;
        }

        public void RemoveOption(Guid id)
        {
            var option = _context.Options.FirstOrDefault(u => u.Id == id);
            _context.Options.Remove(option);
            _context.SaveChanges();
        }
    }
}
