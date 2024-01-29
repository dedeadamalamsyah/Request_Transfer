using PX.Data;
using PX.SM;
using PX.TM;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PX.Common;
using PX.Objects;
using PX.Objects.EP;

namespace Bootcamp
{
    // Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
    public class EPApprovalMapMaint_Extension : PXGraphExtension<PX.Objects.EP.EPApprovalMapMaint>
    {
        #region Event Handlers
        public delegate IEnumerable<String> GetEntityTypeScreensDelegate();
        [PXOverride]
        public IEnumerable<String> GetEntityTypeScreens(GetEntityTypeScreensDelegate baseMethod)
        {
            return new string[]
                  {
                "AR301000",//Invoice and Memos
				"AP301000",//Bills and Adjustments
				"AP302000",//Checks and Payments
				"AP304000",//Cash Purchases
				"AR302000",//Payments and Applications
				"AR304000",//Cash Sales
				"CA304000",//Cash Transactions
				"EP305000",//Employee Time Card
				"EP308000",//Equipment Time Card
				"EP301000",//Expense Claim
				"EP301020",//Expense Receipt
				"PM301000",//Projects
				"PM307000",//Proforma
				"PM308000",//Change Order
				"PM308500",//Change Request
				"PO301000",//Purchase Order
				"RQ301000",//Purchase Request
				"RQ302000",//Purchase Requisition
				"SO301000",//Sales Order
				"CR304500",//Sales Quote
				"PM304500",//Project Quote
				"PM305000",//Cost Projection
				"GL301000",//Journal Transactions
				"IN304030",//Request Transfer
                  };
        }


        #endregion
    }
}