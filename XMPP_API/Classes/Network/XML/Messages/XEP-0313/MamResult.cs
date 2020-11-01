﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0313
{
    public class MamResult
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly string FIRST;
        public readonly string LAST;
        public readonly uint INDEX;
        public readonly uint COUNT;
        public readonly bool COMPLETE;
        public readonly List<QueryArchiveResultMessage> RESULTS;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public MamResult(QueryArchiveFinishMessage msg, List<QueryArchiveResultMessage> results)
        {
            if (!msg.COMPLETE)
            {
                Debug.Assert(!string.IsNullOrEmpty(msg.RESULT_SET.FIRST));
                Debug.Assert(!string.IsNullOrEmpty(msg.RESULT_SET.LAST));

                FIRST = msg.RESULT_SET.FIRST;
                LAST = msg.RESULT_SET.LAST;
                INDEX = msg.RESULT_SET.FIRST_INDEX is null ? 0 : (uint)msg.RESULT_SET.FIRST_INDEX;
            }
            else
            {
                COMPLETE = true;
            }
            COUNT = msg.RESULT_SET.COUNT is null ? 0 : (uint)msg.RESULT_SET.COUNT;

            RESULTS = results;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("First: ");
            sb.Append(FIRST);
            sb.Append(", Last: ");
            sb.Append(LAST);
            sb.Append(", Index: ");
            sb.Append(INDEX);
            sb.Append(", Count: ");
            sb.Append(COUNT);
            sb.Append(", Complete: ");
            sb.Append(COMPLETE);
            return sb.ToString();
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
