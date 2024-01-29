using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Data.EP;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.CS;
using PX.Objects.EP;
using PX.Objects.IN;
using PX.TM;
using System;

namespace Bootcamp
{
    [Serializable]
    [PXPrimaryGraph(typeof(INRequestTransferEntry))]
    [PXCacheName(Messages.INRequestTransfer)]
    public class INRequestTransfer : IBqlTable, IAssign
    {
        #region Keys
        public class PK : PrimaryKeyOf<INRequestTransfer>.By<requestNbr>
        {
            public static INRequestTransfer Find(PXGraph graph, string requestNbr, PKFindOptions options = PKFindOptions.None) => FindBy(graph, requestNbr, options);
        }
        public static class FK
        {
            public class Site : INSite.PK.ForeignKeyOf<INRegister>.By<siteID> { }
            public class ToSite : INSite.PK.ForeignKeyOf<INRegister>.By<toSiteID> { }
        }
        #endregion

        #region RequestNbr
        [PXDBString(15, IsUnicode = true, IsKey = true, InputMask = ">CCCCCCCCCCCCCCC")]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXUIField(DisplayName = "Request Nbr.", Visibility = PXUIVisibility.SelectorVisible)]
        [AutoNumber(typeof(INSetupExt.usrRTransferNumberingID), typeof(requestDate))]
        [PXSelector(typeof(Search<requestNbr>))]
        public virtual string RequestNbr { get; set; }
        public abstract class requestNbr : PX.Data.BQL.BqlString.Field<requestNbr> { }
        #endregion

        #region RequestDate
        [PXDBDate()]
        [PXDefault(typeof(AccessInfo.businessDate))]
        [PXUIField(DisplayName = "Request Date")]
        public virtual DateTime? RequestDate { get; set; }
        public abstract class requestDate : PX.Data.BQL.BqlDateTime.Field<requestDate> { }
        #endregion

        #region SiteID
        [Site(DisplayName = "Warehouse ID", DescriptionField = typeof(INSite.descr), Visibility = PXUIVisibility.SelectorVisible)]
        [PXForeignReference(typeof(FK.Site))]
        [PXDefault]
        public virtual int? SiteID { get; set; }
        public abstract class siteID : PX.Data.BQL.BqlInt.Field<siteID> { }
        #endregion

        #region ToSiteID
        [Site(DisplayName = "To Warehouse ID", DescriptionField = typeof(INSite.descr), Visibility = PXUIVisibility.SelectorVisible)]
        [PXForeignReference(typeof(FK.ToSite))]
        [PXDefault]
        public virtual int? ToSiteID { get; set; }
        public abstract class toSiteID : PX.Data.BQL.BqlInt.Field<toSiteID> { }
        #endregion

        #region Description
        [PXDBString(60, IsUnicode = true)]
        [PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual string Description { get; set; }
        public abstract class description : PX.Data.BQL.BqlString.Field<description> { }
        #endregion

        #region Status
        [PXDBString(2, IsFixed = true)]
        [PXDefault(RequestTransfersStatusConstants.Hold)]
        [PXUIField(DisplayName = "Status", Enabled = false)]
        [PXStringList(
            new string[]
            {
                INRequestTransferStatus.Hold,
                RequestTransfersStatusConstants.Balanced,
                RequestTransfersStatusConstants.Open,
                RequestTransfersStatusConstants.Rejected,
                RequestTransfersStatusConstants.PendingApproval,
            },
            new string[]
            {
                Messages.Hold,
                Messages.Balanced,
                Messages.Open,
                Messages.Rejected,
                Messages.PendingApproval
            })]
        public virtual string Status { get; set; }
        public abstract class status : PX.Data.BQL.BqlString.Field<status> { }
        #endregion

        #region DetailLineCntr
        [PXDBInt()]
        [PXDefault(0)]
        public virtual int? DetailLineCntr { get; set; }
        public abstract class detailLineCntr : PX.Data.BQL.BqlInt.Field<detailLineCntr> { }
        #endregion

        #region TransferNbr
        [PXDBString(15, IsUnicode = true)]
        [PXUIField(DisplayName = "Transfer Nbr.", Enabled = false)]
        [PXSelector(typeof(SearchFor<INTransfer.refNbr>.
            Where<INTransfer.docType.IsEqual<INDocType.transfer>>))]
        public virtual string TransferNbr { get; set; }
        public abstract class transferNbr : PX.Data.BQL.BqlString.Field<transferNbr> { }
        #endregion

        #region Hold
        [PXDBBool()]
        [PXDefault(true)]
        public virtual bool? Hold { get; set; }
        public abstract class hold : PX.Data.BQL.BqlBool.Field<hold> { }
        #endregion

        #region CreatedDateTime
        [PXDBCreatedDateTime()]
        public virtual DateTime? CreatedDateTime { get; set; }
        public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }
        #endregion

        #region CreatedByID
        [PXDBCreatedByID()]
        public virtual Guid? CreatedByID { get; set; }
        public abstract class createdByID : PX.Data.BQL.BqlGuid.Field<createdByID> { }
        #endregion

        #region CreatedByScreenID
        [PXDBCreatedByScreenID()]
        public virtual string CreatedByScreenID { get; set; }
        public abstract class createdByScreenID : PX.Data.BQL.BqlString.Field<createdByScreenID> { }
        #endregion

        #region LastModifiedDateTime
        [PXDBLastModifiedDateTime()]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }
        #endregion

        #region LastModifiedByID
        [PXDBLastModifiedByID()]
        public virtual Guid? LastModifiedByID { get; set; }
        public abstract class lastModifiedByID : PX.Data.BQL.BqlGuid.Field<lastModifiedByID> { }
        #endregion

        #region LastModifiedByScreenID
        [PXDBLastModifiedByScreenID()]
        public virtual string LastModifiedByScreenID { get; set; }
        public abstract class lastModifiedByScreenID : PX.Data.BQL.BqlString.Field<lastModifiedByScreenID> { }
        #endregion

        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion

        #region NoteID
        [PXNote()]
        public virtual Guid? NoteID { get; set; }
        public abstract class noteID : PX.Data.BQL.BqlGuid.Field<noteID> { }
        #endregion

        #region WorkgroupID
        [PXDBInt]
        /*[PXSelector(typeof(Search<EPCompanyTree.workGroupID>), SubstituteKey = typeof(EPCompanyTree.description))]*/
        [PXSelector(typeof(Search5<EPCompanyTree.workGroupID,
            InnerJoin<EPCompanyTreeMember, On<EPCompanyTreeMember.workGroupID, Equal<EPCompanyTree.workGroupID>>,
            InnerJoin<EPEmployee, On<EPCompanyTreeMember.contactID, Equal<EPEmployee.defContactID>>>>,
            Where<EPEmployee.defContactID, Equal<Current<ownerID>>>,
            Aggregate<GroupBy<EPCompanyTree.workGroupID, GroupBy<EPCompanyTree.description>>>>),
            SubstituteKey = typeof(EPCompanyTree.description))]
        [PXUIField(DisplayName = "Workgroup ID", Enabled = false)]
        public virtual int? WorkgroupID
        {
            get;
            set;
        }
        public abstract class workgroupID : BqlInt.Field<workgroupID> { }
        #endregion

        #region OwnerID
        [PXDefault(typeof(AccessInfo.contactID), PersistingCheck = PXPersistingCheck.Nothing)]
        [Owner(typeof(workgroupID), DisplayName = "Owner", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual int? OwnerID
        {
            get;
            set;
        }
        public abstract class ownerID : BqlInt.Field<ownerID> { }
        #endregion

        #region Approved
        [PXDBBool()]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual bool? Approved { get; set; }
        public abstract class approved : PX.Data.BQL.BqlBool.Field<approved> { }
        #endregion

        #region Rejected
        public abstract class rejected : PX.Data.BQL.BqlBool.Field<rejected> { }
        protected bool? _Rejected = false;
        [PXBool]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Reject", Visibility = PXUIVisibility.Visible, Enabled = false)]
        public bool? Rejected
        {
            get
            {
                return _Rejected;
            }
            set
            {
                _Rejected = value;
            }
        }
        #endregion

    }
}