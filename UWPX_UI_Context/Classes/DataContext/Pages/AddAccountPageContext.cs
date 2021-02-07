﻿using System.Threading.Tasks;
using Manager.Classes.Chat;
using Storage.Classes.Contexts;
using Storage.Classes.Models.Account;
using UWPX_UI_Context.Classes.DataTemplates.Dialogs;
using UWPX_UI_Context.Classes.DataTemplates.Pages;
using XMPP_API.Classes;
using XMPP_API.Classes.Network;

namespace UWPX_UI_Context.Classes.DataContext.Pages
{
    public sealed class AddAccountPageContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly AddAccountPageDataTemplate MODEL = new AddAccountPageDataTemplate();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async Task CreateAccountAsync()
        {
            await Task.Run(async () =>
            {
                JidModel jid = new JidModel
                {
                    localPart = Utils.getJidLocalPart(MODEL.BareJidText),
                    domainPart = Utils.getJidDomainPart(MODEL.BareJidText),
                    resourcePart = Utils.getRandomResourcePart()
                };
                AccountModel account = new AccountModel(jid, UiUtils.GenRandomHexColor());
                account.omemoInfo.GenerateOmemoKeys();

                // Look up the DNS SRV record:
                SRVLookupResult result = await XMPPAccount.dnsSrvLookupAsync(jid.domainPart);
                if (result.SUCCESS)
                {
                    account.server.address = result.SERVER_ADDRESS;
                    account.server.port = result.PORT;
                }
                MODEL.Account = account;
            });
        }

        public async Task SaveAccountAsync()
        {
            await Task.Run(() =>
            {
                using (MainDbContext ctx = new MainDbContext())
                {
                    // New account add it to the DB:
                    if (MODEL.OldAccount is null)
                    {
                        ctx.Add(MODEL.Account.fullJid);
                        ctx.Add(MODEL.Account.mamRequest);
                        ctx.Add(MODEL.Account.omemoInfo.deviceListSubscription);
                        ctx.AddRange(MODEL.Account.omemoInfo.devices);
                        ctx.Add(MODEL.Account.omemoInfo.identityKey);
                        ctx.AddRange(MODEL.Account.omemoInfo.preKeys);
                        ctx.Add(MODEL.Account.omemoInfo.signedPreKey);
                        ctx.Add(MODEL.Account.omemoInfo);
                        ctx.Add(MODEL.Account.server);
                        ctx.Add(MODEL.Account);
                    }
                    // Update the old account:
                    else
                    {
                        ctx.Update(MODEL.Account.fullJid);
                        ctx.Update(MODEL.Account.server);
                        ctx.Update(MODEL.Account);
                    }
                }
            });
        }

        public void ColorSelected(ColorPickerDialogDataTemplate dataTemplate)
        {
            if (dataTemplate.Confirmed)
            {
                MODEL.Account.color = UiUtils.ColorToHexString(dataTemplate.SelectedColor);
            }
        }

        public async Task DeleteAccountAsync(DeleteAccountConfirmDialogDataTemplate dataTemplate)
        {
            if (dataTemplate.Confirmed)
            {
                await DataCache.INSTANCE.DeleteAccountAsync(MODEL.Account);
            }
        }

        public void ChangeCertRequirements(CertificateRequirementsDialogDataTemplate dataTemplate)
        {
            if (dataTemplate.Confirmed)
            {
                MODEL.Account.server.ignoredCertificateErrors.Clear();
                foreach (CertificateRequirementDataTemplate item in dataTemplate.ITEMS)
                {
                    if (!item.Required)
                    {
                        MODEL.Account.server.ignoredCertificateErrors.Add(item.CertError);
                    }
                }
            }
        }

        public async Task OnWhatIsAJidAsync()
        {
            await UiUtils.LaunchUriAsync(new System.Uri("https://uwpx.org/support/"));
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
