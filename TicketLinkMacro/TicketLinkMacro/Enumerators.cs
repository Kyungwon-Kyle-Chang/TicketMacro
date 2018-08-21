using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketLinkMacro
{
    public enum TradeType {
                    DEFAULT,
                    DEPOSIT,
                    WITHDRAWAL,
                    TRANSFER,
                    BUY,
                    SELL
    }

    public enum Context {
        CALL_NEWINVESTWINDOW,
        CALL_REVISEINFOWINDOW,
        CALL_CREDITSWINDOW,
        CALL_ASKSAVEWINDOW,
        LOAD_NEWINTERFACE,
        INVESTINFO_UPDATED,
        TRADETYPE_CHANGED,
        HOLDINGASSET_CHANGED,
        EVALUATE_CAPITAL,
        WRITE_LOG,
        PROGRESSBAR,
        PROGRESS_DESC,
        RESET
    }
}
