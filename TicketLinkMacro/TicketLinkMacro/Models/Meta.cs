using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketLinkMacro.Models
{
    public class Meta
    {
        public Draw draw;
        public string productId;
        public string productName;
        public string productNameEng;
        public string placeName;
        public string placeNameEng;
        public Schedule schedule;
        public bool isSports;
        public bool isRemainExposure;
        public Limit limit;
        public string reserveCommissionContent;
        public bool isPriceExposure;

        public class Draw
        {
            public Logical logical;
            public Physical physical;
            public BlockInfo[] blockInfo;

            public class Logical
            {
                public string logicalPlanId;
                public bool autoSelect;
            }
            public class Physical
            {
                public PhysicalPlan physicalPlan;
                public BGInfo bgInfo;
                public BGInfo blockBgInfo;
                public SeatMark seatMark;
                public VirtualVector2<string>[] pagingInfo;

                public class PhysicalPlan
                {
                    public string physicalPlanId;
                    public string physicalPlanName;
                    public string placeName;
                    public string hallName;
                    public string exposureMinValue;
                    public bool planGateCode;
                    public bool planLinked;
                    public bool planApiUse;
                    public bool planExposure;
                    public string areaSeatMarkStandard;
                    public bool blockPhysicalPlan;
                    public bool minimapExposure;
                }

                public class BGInfo
                {
                    public Vector2 center;
                    public string mapSize;
                    public string imageUrl;
                    public string imageurlThumnail;
                    public string imageTileUrl;
                    public string imageTileSize;
                    public bool empty;
                }
                
                public class SeatMark
                {
                    public string[] markName;
                    public string[] markCode;
                }


            }
            public class BlockInfo
            {
                public int blockId { get; set; }
                public string blockName { get; set; }
                public Vector2[] cornerPoints;
                public Vector2 linkedPoint;
                public Vector2 vectorPoint;
                public SimpleGrade[] grades;

                public class SimpleGrade
                {
                    public string gradeId;
                    public string remainCnt;
                }
            }
        }
        public class Schedule
        {
            public string scheduleId;
            public string date;
            public string displayProductDate;
        }
        public class Limit
        {
            public string totalCnt;
            public LimitGrade[] grades;

            public class LimitGrade
            {
                public string productGradeId;
                public string cnt;
                public bool check;
            }
        }
    }

    public class Vector2
    {
        public double x;
        public double y;
    }

    public class VirtualVector2<T>
    {
        public T virtualX;
        public T virtualY;

        public VirtualVector2(T x, T y)
        {
            virtualX = x;
            virtualY = y;
        }
    }
}
