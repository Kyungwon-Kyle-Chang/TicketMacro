using TicketLinkMacro.Models;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System;
using TicketLinkMacro.Utils;
using Newtonsoft.Json;
using TicketLinkMacro.Test;

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
            set {
                SetProperty(ref _selectedDate, value);
                GetProductRounds(value);
            }
        }

        public ObservableCollection<ProductRound> ProductRoundList { get; set; }

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
        private Product _product;
        private Meta _meta;
        private Grade[] _grades;
        private WebConnector _webConnectorContinuous, _webConnectorTemporary;

        private const int ASYNC_API_CALL_INTERVAL = 500;

        public MainWindowViewModel()
        {
            ProductRoundList = new ObservableCollection<ProductRound>();
            StartCheckButton = new CommandBase(StartCheckExecute, StartCheckCanExecute);
            StopButton = new CommandBase(StopExecute);
            ClearButton = new CommandBase(ClearExecute);
            CurrentRemainSeats = new ObservableCollection<RemainSeatData>();
            RemainSeats = new ObservableCollection<RemainSeatData>();
            _webConnectorTemporary = new WebConnector();

            Messenger.Instance.Register<string>(this, (s) => { StatusText = s; }, Context.PROGRESS_DESC);
        }

        private void GetProductRounds(DateTime date)
        {
            _productRound = _webConnectorTemporary.CallPostAPI<ProductInfo, ProductRounds>(Configs.webAPI, Configs.uriRound, new ProductInfo(ProductID, string.Format("{0:yyyy.MM.dd}", SelectedDate)));
            if (_productRound == default(ProductRounds))
            {
                StatusText = "Connection Failed";
                return;
            }
            else if (_productRound.result.message != "success")
            {
                StatusText = "Failed Receiving Response";
                return;
            }
            else if (_productRound.data.Length == 0)
            {
                StatusText = "No Product Round Data";
                return;
            }


            foreach (ProductRound item in _productRound.data)
            {
                ProductRoundList.Add(item);
            }
        }

        private void StartCheckExecute(object obj)
        {
            _webConnectorContinuous = new WebConnector();
            
            string reservePage = _webConnectorTemporary.CallHtmlAPI(Configs.webAPI, Configs.uriGetReservePage(ScheduleID), CookieText).ToString();
            ExtractInitData(reservePage);

            Blocks blocks = _webConnectorTemporary.CallAPI<Blocks>(Configs.webAPI, Configs.uriGetBlocks(ScheduleID), CookieText);
            if(blocks.data.Length > 0)
            {
                _webConnectorContinuous.CallAPIAsync<Blocks>(Configs.webAPI, Configs.uriGetBlocks(ScheduleID), ASYNC_API_CALL_INTERVAL, AddRemainSeatData, CookieText);
            }
            else
            {
                _webConnectorContinuous.CallAPIAsync<Grades>(Configs.webAPI, Configs.uriGetGrades(ScheduleID), ASYNC_API_CALL_INTERVAL, AddRemainSeatData, CookieText);
            }
            
            IsInProgress = true;
        }

        private bool StartCheckCanExecute(object obj)
        {
            return !IsInProgress
                && RegexManager.IsUnsignedInt(ProductID)
                && RegexManager.IsNotBlank(ProductID)
                && ScheduleID != null
                && CookieText != null;
        }

        private void StopExecute(object obj)
        {
            _webConnectorContinuous.CancelAsyncCall();
        }

        private void ClearExecute(object obj)
        {
            RemainSeats.Clear();
        }

        private void ExtractInitData(string s)
        {
            string[] delim = { "var initData = {};", "initData.globalLocale ="};
            string[] delim2 = { "initData." };
            string[] delim3 = { " = "};
            char[] delim4 = { ' ', ';', '\n' };

            string s2 = s.Split(delim, StringSplitOptions.None)[1];
            string[] datum = s2.Split(delim2, StringSplitOptions.RemoveEmptyEntries);
            
            string product = datum[1].Split(delim3, StringSplitOptions.None)[1];
            string meta = datum[2].Split(delim3, StringSplitOptions.None)[1];
            string grade = datum[3].Split(delim3, StringSplitOptions.None)[1];

            product = product.TrimEnd(delim4);
            meta = meta.TrimEnd(delim4);
            grade = grade.TrimEnd(delim4);

            _product = JsonConvert.DeserializeObject<Product>(product);
            _meta = JsonConvert.DeserializeObject<Meta>(meta);
            _grades = JsonConvert.DeserializeObject<Grade[]>(grade);
        }

        private void AddRemainSeatData(Blocks blocks)
        {
            CurrentRemainSeats.Clear();

            if (blocks == null || blocks.data == null)
            {
                IsInProgress = false;
                return;
            }

            var remainBlocks = blocks.data.Where(block => block.remainCnt > 0);
            foreach(Block item in remainBlocks)
            {
                RemainSeatData data = new RemainSeatData();
                data.blockId = item.blockId;
                data.gradeId = item.gradeId;
                data.blockName = _meta.draw.blockInfo.Where(x => x.blockId == item.blockId.ToString()).Select(x => x.blockName).First();
                data.gradeName = _grades.Where(x => x.gradeId == item.gradeId)?.Select(x => x.name).FirstOrDefault();
                data.remainCnt = item.remainCnt;
                data.registerTime = DateTime.Now;

                string[] remainSeats = GetRemainSeatsID(data);
                data.remainSeatsID = remainSeats.Length > 0 ? remainSeats[0] : null;
                for (int i = 1; i < remainSeats.Length; i++)
                {
                    data.remainSeatsID += $", {remainSeats[i]}";
                }

                RemainSeats.Add(data);
                CurrentRemainSeats.Add(data);
            }
            Console.WriteLine(blocks.result.errorMessage);
        }

        private void AddRemainSeatData(Grades grades)
        {
            CurrentRemainSeats.Clear();

            if(grades == null || grades.data == null)
            {
                IsInProgress = false;
                return;
            }

            var remainGrades = grades.data.Where(grade => grade.remainCnt > 0);
            foreach (Grade item in remainGrades)
            {
                RemainSeatData data = new RemainSeatData();
                data.gradeId = item.gradeId;
                data.remainCnt = item.remainCnt;
                data.gradeName = item.name;
                data.registerTime = DateTime.Now;

                string[] remainSeats = GetRemainSeatsID(data);
                data.remainSeatsID = remainSeats[0];
                for(int i=1; i<remainSeats.Length; i++)
                {
                    data.remainSeatsID += $", {remainSeats[i]}";
                }
                
                RemainSeats.Add(data);
                CurrentRemainSeats.Add(data);
            }
            Console.WriteLine(grades.result.errorMessage);
        }

        private SoldoutSeats GetSoldoutSeats(int blockId)
        {
            SoldoutSeats result;

            if (blockId == 0)
            {
                VirtualVector2<string>[] postData = _meta.draw.physical.pagingInfo;
                result = _webConnectorTemporary.CallPostAPI<VirtualVector2<string>[], SoldoutSeats>(Configs.webAPI, Configs.uriGetSoldoutAreas(ScheduleID), postData, CookieText);
            }
            else
            {
                result = _webConnectorTemporary.CallPostAPI<string, SoldoutSeats>(Configs.webAPI, Configs.uriGetSoldoutBlocks(ScheduleID), $"[{blockId}]", CookieText);
            }

            return result;
        }

        private AllSeatData GetAllSeatData(int blockId)
        {
            AllSeatData result;

            if(blockId == 0)
            {
                VirtualVector2<string>[] postData = _meta.draw.physical.pagingInfo;
                result = _webConnectorTemporary.CallPostAPI<VirtualVector2<string>[], AllSeatData>(Configs.webAPI, Configs.uriGetAllSeatsInArea(ScheduleID), postData, CookieText);
            }
            else
            {
                result = _webConnectorTemporary.CallPostAPI<string, AllSeatData>(Configs.webAPI, Configs.uriGetAllSeatsInBlock(ScheduleID), $"[{blockId}]", CookieText);
            }

            return result;
        }

        private string[] GetRemainSeatsID(RemainSeatData data)
        {
            List<string> result = new List<string>();

            SoldoutSeats soldouts = GetSoldoutSeats(data.blockId);
            result.AddRange(soldouts.data.Where(x => x.Value == false).Select(x => x.Key));

            return result.ToArray();
        }

        private PreOccupancy SetPreOccupancyInfo(int count, RemainSeatData data)
        {
            string[] remainSeatIds = data.remainSeatsID.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
            if (remainSeatIds.Length < count)
                return null;

            AllSeatData allSeatData = GetAllSeatData(data.blockId);
            List<AllSeatData.Data.Seat> remainSeatsInfo = new List<AllSeatData.Data.Seat>();
            for (int i=0; i<remainSeatIds.Length; i++)
            {
                foreach(AllSeatData.Data.Seat[] seats in allSeatData.data.able.Values)
                {
                    var seat = seats.Where(x => string.Equals(x.logicalSeatid, remainSeatIds[i]));
                    if (seat.Count() > 0)
                        remainSeatsInfo.Add(seat.First());
                }
            }

            PreOccupancy.Seat[] selectedSeats = new PreOccupancy.Seat[count];
            for(int i=0; i<count; i++)
            {
                selectedSeats[i].allotmentCode = remainSeatsInfo[i].allotmentCode;
                selectedSeats[i].area = remainSeatsInfo[i].area;
                selectedSeats[i].blockId = int.Parse(remainSeatsInfo[i].blockId);
                selectedSeats[i].logicalSeatId = uint.Parse(remainSeatsInfo[i].logicalSeatid);
                selectedSeats[i].orderNum = int.Parse(remainSeatsInfo[i].orderNum);
                selectedSeats[i].productGradeId = int.Parse(remainSeatsInfo[i].gradeId);
                selectedSeats[i].productGradeName = data.gradeName;
                selectedSeats[i].seatAttribute = remainSeatsInfo[i].mapInfo;
                selectedSeats[i].sortSeatAttribute = remainSeatsInfo[i].sortMapInfo;
            }

            PreOccupancy result = new PreOccupancy();
            result.code = remainSeatsInfo.First().allotmentCode;
            result.memberNo = 0;
            result.scheduleId = uint.Parse(ScheduleID);
            result.seats = selectedSeats;
            result.totalCnt = selectedSeats.Length;
            result.zones = new string[0];

            return result;
        }
    }
}
