﻿using System;
using System.ComponentModel.DataAnnotations;
using Storage.Classes.Models.Account;
using XMPP_API.Classes.Network.XML.Messages.XEP_0384;

namespace Storage.Classes.Models.Omemo
{
    public class OmemoDeviceListSubscription: AbstractAccountModel
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        [Key]
        public string id { get; set; }
        /// <summary>
        /// The bare JID e.g. 'coven@chat.shakespeare.lit' we received the update from.
        /// </summary>
        [Required]
        public Jid bareJid { get; set; }
        /// <summary>
        /// The <see cref="DateTime"/> we received the last update for this entry.
        /// </summary>
        [Required]
        public DateTime lastUpdateReceived { get; set; }
        /// <summary>
        /// The current state of the subscription.
        /// </summary>
        public OmemoDeviceListSubscriptionState state { get; set; }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


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
