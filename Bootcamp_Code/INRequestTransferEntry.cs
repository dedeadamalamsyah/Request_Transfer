using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Data.WorkflowAPI;
using PX.Objects.EP;
using PX.Objects.IN;
using System.Collections;
using System.Collections.Generic;

namespace Bootcamp
{
    public class INRequestTransferEntry : PXGraph<INRequestTransferEntry, INRequestTransfer>
    {
        public PXSelect<INSetupApproval> SetupApproval;

        [PXViewName(Messages.Approval)]
        public EPApprovalAutomation<INRequestTransfer, INRequestTransfer.approved, INRequestTransfer.rejected, INRequestTransfer.hold, INSetupApproval> Approval;

        #region Views
        //The primary view
        [PXViewName(Messages.INRequestTransfer)]
        public SelectFrom<INRequestTransfer>.View RequestTransfer;

        //The view for the Repair Items tab
        [PXViewName(Messages.INRequestTransferDetail)]
        public SelectFrom<INRequestTransferDetail>.
        Where<INRequestTransferDetail.requestNbr.
        IsEqual<INRequestTransfer.requestNbr.FromCurrent>>.View
        RequestTransferDetail;

        //The view for the auto-numbering of records
        public PXSetup<INSetup> AutoNum;
        #endregion

        #region Graph constructor
        public INRequestTransferEntry()
        {
            INSetup setup = AutoNum.Current;
        }
        #endregion

        #region Event Handlers
        //Validate that Quantity is greater than or equal to 0 and
        //correct the value to the default if the value is less than the default.
        protected virtual void _(Events.FieldVerifying<INRequestTransferDetail,
        INRequestTransferDetail.quantity> e)
        {
            if (e.Row == null || e.NewValue == null) return;
            if ((decimal)e.NewValue < 0)
            {
                //Throwing an exception to cancel the assignment
                //of the new value to the field
                throw new PXSetPropertyException(
                Messages.QuantityCannotBeNegative);
            }
        }
        #endregion

        #region Actions
        public PXInitializeState<INRequestTransfer> initializeState;

        public PXAction<INRequestTransfer> ReleaseFromHold;
        [PXButton(), PXUIField(DisplayName = "Remove Hold",
        MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable releaseFromHold(PXAdapter adapter) => adapter.Get();

        public PXAction<INRequestTransfer> PutOnHold;
        [PXButton, PXUIField(DisplayName = "Hold",
        MapEnableRights = PXCacheRights.Select,
        MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable putOnHold(PXAdapter adapter) => adapter.Get();

        public PXAction<INRequestTransfer> Open;
        [PXButton]
        [PXUIField(DisplayName = "Open", Enabled = false)]
        protected virtual IEnumerable open(PXAdapter adapter) => adapter.Get();

        private static void CreateTransfer(INRequestTransfer requestTransfer)
        {
            using (var ts = new PXTransactionScope())
            {
                var transferEntry = PXGraph.CreateInstance<INTransferEntry>();

                var doc = new INRegister()
                {
                    DocType = INDocType.Transfer,
                };
                doc = transferEntry.CurrentDocument.Insert(doc);
                doc.SiteID = requestTransfer.SiteID;
                doc.ToSiteID = requestTransfer.ToSiteID;
                doc.TranDesc = requestTransfer.Description;
                transferEntry.CurrentDocument.Update(doc);

                var requestTransferEntry = PXGraph.CreateInstance<INRequestTransferEntry>();
                requestTransferEntry.RequestTransfer.Current = requestTransfer;

                foreach (INRequestTransferDetail line in requestTransferEntry.RequestTransferDetail.Select())
                {
                    var transferTran = transferEntry.transactions.Insert();
                    transferTran.InventoryID = line.InventoryID;
                    transferTran.Qty = line.Quantity;
                    transferTran.UOM = line.UOM;
                    transferEntry.transactions.Update(transferTran);
                }

                transferEntry.Actions.PressSave();

                requestTransfer.TransferNbr = transferEntry.CurrentDocument.Current.RefNbr;
                requestTransferEntry.RequestTransfer.Update(requestTransfer);
                requestTransferEntry.Actions.PressSave();

                ts.Complete();
            }
        }

        public PXAction<INRequestTransfer> CreateTransferAction;
        [PXButton]
        [PXUIField(DisplayName = "Release", Enabled = true)]
        protected virtual IEnumerable createTransferAction(PXAdapter adapter)
        {
            // Populate a local list variable.
            List<INRequestTransfer> list = new List<INRequestTransfer>();
            foreach (INRequestTransfer order in adapter.Get<INRequestTransfer>())
            {
                list.Add(order);
            }

            // Trigger the Save action to save changes in the database.
            Actions.PressSave();

            var requestTransfer = RequestTransfer.Current;
            PXLongOperation.StartOperation(this, delegate ()
            {
                CreateTransfer(requestTransfer);
            });

            // Return the local list variable.
            return list;
        }
        #endregion
    }
}