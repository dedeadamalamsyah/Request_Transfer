using PX.Data;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.EP;
using PX.SM;
using System;

namespace Bootcamp
{
    [Serializable]
    [PXCacheName(Messages.INSetupApproval)]
    public partial class INSetupApproval : PX.Data.IBqlTable, IAssignedMap
    {
        #region Keys
        public class PK : PrimaryKeyOf<INSetupApproval>.By<approvalID>
        {
            public static INSetupApproval Find(PXGraph graph, int? approvalID, PKFindOptions options = PKFindOptions.None) => FindBy(graph, approvalID, options);
        }
        public static class FK
        {
            public class ApprovalMap : EPAssignmentMap.PK.ForeignKeyOf<INSetupApproval>.By<assignmentMapID> { }
            public class PendingApprovalNotification : PX.SM.Notification.PK.ForeignKeyOf<INSetupApproval>.By<assignmentNotificationID> { }
        }
        #endregion

        #region ApprovalID
        [PXDBIdentity(IsKey = true)]
        public virtual int? ApprovalID { get; set; }
        public abstract class approvalID : PX.Data.BQL.BqlInt.Field<approvalID> { }
        #endregion

        #region AssignmentMapID
        [PXDefault]
        [PXDBInt]
        [PXSelector(typeof(Search<EPAssignmentMap.assignmentMapID, Where<EPAssignmentMap.mapType, NotEqual<EPMapType.assignment>>>), DescriptionField = typeof(EPAssignmentMap.name))]
        [PXUIField(DisplayName = "Approval Map")]
        public virtual int? AssignmentMapID { get; set; }
        public abstract class assignmentMapID : PX.Data.BQL.BqlInt.Field<assignmentMapID> { }
        #endregion

        #region AssignmentNotificationID
        [PXDBInt]
        [PXSelector(typeof(Notification.notificationID), DescriptionField = typeof(Notification.name))]
        [PXUIField(DisplayName = "Pending Approval Notification")]
        public virtual int? AssignmentNotificationID { get; set; }
        public abstract class assignmentNotificationID : PX.Data.BQL.BqlInt.Field<assignmentNotificationID> { }
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

        #region CreatedDateTime
        [PXDBCreatedDateTime()]
        public virtual DateTime? CreatedDateTime { get; set; }
        public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }
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

        #region LastModifiedDateTime
        [PXDBLastModifiedDateTime()]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }
        #endregion

        #region Tstamp
        [PXDBTimestamp()]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion

        #region IsActive
        [PXDBBool]
        [PXDefault(typeof(Search<INSetupExt.usrInventoryRequestApproval>), PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual bool? IsActive { get; set; }
        public abstract class isActive : PX.Data.BQL.BqlBool.Field<isActive> { }
        #endregion
    }
}