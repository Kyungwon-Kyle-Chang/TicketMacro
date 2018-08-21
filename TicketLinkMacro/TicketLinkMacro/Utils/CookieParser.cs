using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TicketLinkMacro.Utils
{
    public class CookieParser
    {
        private static char[] cookieCollectionDelim = { ' ', ';' };
        private static char[] cookieDelim = { '=' };

        public static Cookie MakeCookie(string text)
        {
            if (text == null)
                return null;

            string[] splited = text.Split(cookieDelim, 2);

            Cookie cookie = new Cookie();
            cookie.Name = splited[0];
            cookie.Value = splited[1]; 

            return cookie;
        }

        public static CookieCollection MakeCookieCollection(string text)
        {
            if (text == null)
                return null;

            string[] splited = text.Split(cookieCollectionDelim, StringSplitOptions.RemoveEmptyEntries);

            CookieCollection cc = new CookieCollection();
            for (int i=0; i<splited.Length; i++)
            {
                cc.Add(MakeCookie(splited[i]));
            }

            return cc;
        }
    }
}
