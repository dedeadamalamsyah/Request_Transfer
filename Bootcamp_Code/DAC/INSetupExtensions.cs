using PX.Data;
using PX.Objects.CS;
using PX.Objects.EP;
using PX.Objects.IN;

namespace Bootcamp
{
    // Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
    public sealed class INSetupExt : PXCacheExtension<INSetup>
    {
        #region UsrRTransferNumberingID
        // Acuminator disable once PX1030 PXDefaultIncorrectUse [Justification]
        [PXDBString(10, IsUnicode = true)]
        [PXDefault("RTRANSFER")]
        [PXSelector(typeof(Numbering.numberingID), DescriptionField = typeof(Numbering.descr))]
        [PXUIField(DisplayName = "Request Transfer Numbering Sequence", Visibility = PXUIVisibility.Visible)]
        public string UsrRTransferNumberingID { get; set; }
        public abstract class usrRTransferNumberingID : PX.Data.BQL.BqlString.Field<usrRTransferNumberingID> { }
        #endregion

        #region UsrInventoryRequestApproval
        // Acuminator disable once PX1030 PXDefaultIncorrectUse [Justification]
        [EPRequireApproval]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Null)]
        [PXUIField(DisplayName = "Require Approval")]
        public bool? UsrInventoryRequestApproval { get; set; }
        public abstract class usrInventoryRequestApproval : PX.Data.BQL.BqlBool.Field<usrInventoryRequestApproval> { }
        #endregion
    }
}