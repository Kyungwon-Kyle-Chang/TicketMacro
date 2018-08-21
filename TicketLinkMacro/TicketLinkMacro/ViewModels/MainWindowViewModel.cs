using TicketLinkMacro.Models;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System;
using TicketLinkMacro.Utils;

namespace TicketLinkMacro.ViewModels
{
    class MainWindowViewModel : BindableBase
    {
        private string _scheduledId = "";
        public string ScheduledId
        {
            get { return _scheduledId; }
            set { SetProperty(ref _scheduledId, value); }
        }

        private string _cookieText;
        public string CookieText
        {
            get { return _cookieText; }
            set { SetProperty(ref _cookieText, value); }
        }

        private bool _isInProgress = false;
        public bool IsInProgress
        {
            get { return _isInProgress; }
            set { SetProperty(ref _isInProgress, value); }
        }

        public CommandBase StartCheckButton { get; private set; }
        public CommandBase StopButton { get; private set; }
        public ObservableCollection<RemainSeatData> RemainSeats { get; set; }
        
        private WebConnector _webConnector, _webConnector2;

        public MainWindowViewModel()
        {
            StartCheckButton = new CommandBase(StartCheckExecute, StartCheckCanExecute);
            StopButton = new CommandBase(StopExecute);
            RemainSeats = new ObservableCollection<RemainSeatData>();
        }

        private void StartCheckExecute(object obj)
        {
            _webConnector = new WebConnector();
            _webConnector2 = new WebConnector();
            _webConnector.CallAPIAsync<Blocks>(Configs.webAPI, Configs.uriGetBlocks(ScheduledId), 1000, AddRemainSeatData, CookieText);
            _webConnector2.CallAPIAsync<Grades>(Configs.webAPI, Configs.uriGetGrades(ScheduledId), 1000, AddRemainSeatData, CookieText);
            IsInProgress = true;
        }

        private bool StartCheckCanExecute(object obj)
        {
            return !IsInProgress && RegexManager.IsUnsignedInt(ScheduledId) && RegexManager.IsNotBlank(ScheduledId);
        }

        private void StopExecute(object obj)
        {
            _webConnector.CancelAsyncCall();
            _webConnector2.CancelAsyncCall();
        }

        private void AddRemainSeatData(Blocks blocks)
        {
            if(blocks == null)
            {
                IsInProgress = false;
                return;
            }
            else if (blocks.data == null)
                return;

            var remainBlocks = blocks.data.Where(block => block.remainCnt > 0);
            foreach(Block item in remainBlocks)
            {
                RemainSeatData data = new RemainSeatData();
                data.blockId = item.blockId;
                data.remainCnt = item.remainCnt;
                data.registerTime = DateTime.Now;

                RemainSeats.Add(data);
            }
            Console.WriteLine(blocks.result.errorMessage);
        }

        private void AddRemainSeatData(Grades grades)
        {
            if(grades == null)
            {
                IsInProgress = false;
                return;
            }
            else if (grades.data == null)
                return;

            var remainGrades = grades.data.Where(grade => grade.remainCnt > 0);
            foreach (Grade item in remainGrades)
            {
                RemainSeatData data = new RemainSeatData();
                data.gradeId = item.gradeId;
                data.remainCnt = item.remainCnt;
                data.blockName = item.name;
                data.registerTime = DateTime.Now;

                RemainSeats.Add(data);
            }
            Console.WriteLine(grades.result.errorMessage);
        }
    }
}
