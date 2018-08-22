using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketLinkMacro.Models
{
    [Serializable]
    public class ProductInfo
    {
        public string productId;
        public string productDate;

        public ProductInfo(string productId, string productDate)
        {
            this.productId = productId;
            this.productDate = productDate;
        }
    }
}
