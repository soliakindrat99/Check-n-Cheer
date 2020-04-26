﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Check_n_Cheer.Interfaces;
using Check_n_Cheer.Models;
using Check_n_Cheer.DTO;
using Microsoft.Extensions.Logging;

namespace Check_n_Cheer.Controllers
{
    public class OptionController : Controller
    {
        private readonly ILogger<TestController> _logger;
        private IUserRepository _userRepository;
        private ITaskRepository _taskRepository;
        private IOptionRepository _optionRepository;
        public OptionController(ILogger<TestController> logger,IUserRepository userRepository, ITaskRepository taskRepository, IOptionRepository optionRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
            _taskRepository = taskRepository;
            _optionRepository = optionRepository;
        }
        public string Get(string key)
        {
            if (Request == null)
                return null;
            return Request.Cookies[key];
        }
        public ActionResult Index()
        {
            _logger.LogInformation("GET Option/Index");
            return View();
        }

        [HttpGet]
        public ActionResult Create(Guid taskId, Guid testId)
        {
            _logger.LogInformation("GET Option/Create");
            var option = new CreateOptionDTO { TaskId = taskId, TestId = testId };
            Guid id = Guid.Parse(Get("user"));
            User user = _userRepository.GetUser(id);
            if (user != null && user.Role == "Teacher")
            {
                ViewData["LoggedIn"] = "true";
                return View(option);
            }
            ViewData["LoggedIn"] = "false";
            return RedirectToAction("Error");
        }

        [HttpPost]
        public ActionResult CreateOption(Guid id, string optionName, string? optionIsCorrect)
        {
            _logger.LogInformation("POST Option/Create");
            bool correct = (optionIsCorrect != null);
            var task = _taskRepository.GetTask(id); 
            var option = new Option()
            {
                Id = Guid.NewGuid(),
                IsCorrect = correct,
                Name = optionName,
                Task = task
            };
            _optionRepository.AddOption(option);
            return RedirectToAction("ManageOptions", new { taskId = id});
            
        }

        [HttpGet]
        public ActionResult Delete(Guid id)
        {
            _logger.LogInformation("GET Option/Delete");
            return View();
        }

        [HttpGet]
        public ActionResult ManageOptions(Guid taskId)
        {
            _logger.LogInformation("GET Option/ManageOptions");
            var options = _optionRepository.GetOptions(taskId);
            ViewData["TaskId"] = taskId;
            return View(options);
        }

        [HttpPost]
        public ActionResult ChangeOption(Guid id, string optionName, string? optionIsCorrect)
        {
            Option option = _optionRepository.GetOption(id);

            bool correct = (optionIsCorrect!=null);
            if(option != null)
            {
                if(optionName != option.Name || option.IsCorrect != correct)
                {
                    option.Name = optionName;
                    option.IsCorrect = correct;
                    _optionRepository.UpdateOption(id, option);                    
                }
                return RedirectToAction("ManageOptions", new { taskId =  option.Task.Id});
            }
            return RedirectToAction("Error");
        }

        [HttpPost]
        public ActionResult DeleteOption(Guid id)
        {
            Option option = _optionRepository.GetOption(id);

            
            if (option != null)
            {
                _optionRepository.RemoveOption(id);
                return RedirectToAction("ManageOptions", new { taskId = option.Task.Id });
            }
            return RedirectToAction("Error");
        }
    }
}