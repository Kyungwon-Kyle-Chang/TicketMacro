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
        private string _productId = "";
        public string ProductID
        {
            get { return _productId; }
            set { SetProperty(ref _productId, value); }
        }

        private DateTime _selectedDate = DateTime.Today;
        public DateTime SelectedDate
        {
            get { return _selectedDate; }
            set { SetProperty(ref _selectedDate, value); }
        }

        private string _cookieText;
        public string CookieText
        {
            get { return _cookieText; }
            set { SetProperty(ref _cookieText, value); }
        }

        private string _scheduleID = "";
        public string ScheduleID
        {
            get { return _scheduleID; }
            set { SetProperty(ref _scheduleID, value); }
        }

        private bool _isInProgress = false;
        public bool IsInProgress
        {
            get { return _isInProgress; }
            set { SetProperty(ref _isInProgress, value); }
        }

        private string _statusText = "";
        public string StatusText
        {
            get { return _statusText; }
            set { SetProperty(ref _statusText, value); }
        }

        public CommandBase StartCheckButton { get; private set; }
        public CommandBase StopButton { get; private set; }
        public CommandBase ClearButton { get; private set; }
        public ObservableCollection<RemainSeatData> CurrentRemainSeats { get; set; }
        public ObservableCollection<RemainSeatData> RemainSeats { get; set; }

        private ProductRounds _productRound;
        private WebConnector _webConnector, _webConnector2;

        public MainWindowViewModel()
        {
            StartCheckButton = new CommandBase(StartCheckExecute, StartCheckCanExecute);
            StopButton = new CommandBase(StopExecute);
            ClearButton = new CommandBase(ClearExecute);
            CurrentRemainSeats = new ObservableCollection<RemainSeatData>();
            RemainSeats = new ObservableCollection<RemainSeatData>();

            Messenger.Instance.Register<string>(this, (s) => { StatusText = s; }, Context.PROGRESS_DESC);
        }

        private void StartCheckExecute(object obj)
        {
            _webConnector = new WebConnector();
            _productRound = _webConnector.CallPostAPI<ProductInfo, ProductRounds>(Configs.webAPI, Configs.uriRound, new ProductInfo(ProductID, string.Format("{0:yyyy.MM.dd}", SelectedDate)));

            if(_productRound == default(ProductRounds))
            {
                StatusText = "Connection Failed";
                return;
            }
            else if(_productRound.result.message != "success")
            {
                StatusText = "Failed Receiving Response";
                return;
            }
            else if(_productRound.data.Length == 0)
            {
                StatusText = "No Product Round Data";
                return;
            }


            ScheduleID = _productRound.data[0].scheduleId;
            //string reservePage = _webConnector.CallAPI<object>(Configs.webAPI, Configs.uriGetReservePage(ScheduleID), CookieText).ToString();
            //ExtractInitData(reservePage);
            //_webConnector2 = new WebConnector();
            //_webConnector.CallAPIAsync<Blocks>(Configs.webAPI, Configs.uriGetBlocks(_productRound.data[0].scheduleId), 1000, AddRemainSeatData, CookieText);
            _webConnector.CallAPIAsync<Grades>(Configs.webAPI, Configs.uriGetGrades(_productRound.data[0].scheduleId), 1000, AddRemainSeatData, CookieText);
            IsInProgress = true;
        }

        private bool StartCheckCanExecute(object obj)
        {
            return !IsInProgress && RegexManager.IsUnsignedInt(ProductID) && RegexManager.IsNotBlank(ProductID);
        }

        private void StopExecute(object obj)
        {
            _webConnector.CancelAsyncCall();
            //_webConnector2.CancelAsyncCall();
        }

        private void ClearExecute(object obj)
        {
            RemainSeats.Clear();
        }

        private void ExtractInitData(string s)
        {
            string[] delim = { "var initData = {};", "initData.globalLocale ="};
            string[] delim2 = { "initData." };
            string[] delim3 = { " = ", ";"};

            string s2 = s.Split(delim, StringSplitOptions.None)[1];
            string[] datum = s2.Split(delim2, StringSplitOptions.RemoveEmptyEntries);
            
            string product = datum[0].Split(delim3, StringSplitOptions.None)[1];
            string meta = datum[1].Split(delim3, StringSplitOptions.None)[1];
            string grade = datum[2].Split(delim3, StringSplitOptions.None)[1];
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
            CurrentRemainSeats.Clear();

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
                data.gradeName = item.name;
                data.registerTime = DateTime.Now;

                RemainSeats.Add(data);
                CurrentRemainSeats.Add(data);
            }
            Console.WriteLine(grades.result.errorMessage);
        }

        
    }
}
