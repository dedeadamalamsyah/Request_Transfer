using PX.Data;
using PX.Data.WorkflowAPI;
using System.Collections;

namespace Bootcamp
{
    using static BoundedTo<INRequestTransferEntry, INRequestTransfer>;
    using static INRequestTransfer;
    using State = INRequestTransferStatus;

    // Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
    public class INRequestTransferApprovalWorkflow : PXGraphExtension<INRequestTransferWorkflow, INRequestTransferEntry>
    {
        private class SetupApproval : IPrefetchable
        {
            private bool RequestApproval;

            void IPrefetchable.Prefetch()
            {
                using (PXDataRecord CASetup =
                    PXDatabase.SelectSingle<INSetupApproval>(new PXDataField<INSetupApproval.isActive>()))
                {
                    if (CASetup != null)
                        RequestApproval = (bool)CASetup.GetBoolean(0);
                }
            }
            private static SetupApproval Slot => PXDatabase
                .GetSlot<SetupApproval>(typeof(SetupApproval).FullName, typeof(INSetupApproval));
            public static bool IsRequestApproval =>
                PXAccess.FeatureInstalled<PX.Objects.CS.FeaturesSet.approvalWorkflow>() &&
                Slot.RequestApproval;
        }

        private static bool ApprovalIsActive => PXAccess.FeatureInstalled<PX.Objects.CS.FeaturesSet.approvalWorkflow>();


        [PXWorkflowDependsOnType(typeof(INSetupApproval))]
        public override void Configure(PXScreenConfiguration config)
        {
            Configure(config.GetScreenConfigurationContext<INRequestTransferEntry,
                INRequestTransfer>());
        }

        public class Conditions : Condition.Pack
        {
            public Condition IsNotOnHoldAndIsApproved => GetOrCreate(c => c.FromBql<
                hold.IsEqual<False>.And<approved.IsEqual<True>>
            >());

            public Condition IsNotOnHoldAndIsNotApproved => GetOrCreate(c => c.FromBql<
                hold.IsEqual<False>.And<approved.IsEqual<False>>
            >());

            public Condition IsRejected => GetOrCreate(c => c.FromBql<
                rejected.IsEqual<True>
                     >());
            public Condition IsApproved => GetOrCreate(c => c.FromBql<
                approved.IsEqual<True>
            >());

            public Condition IsNotApproved => GetOrCreate(c => c.FromBql<
                approved.IsEqual<False>.And<rejected.IsEqual<False>>
            >());

            public Condition IsApprovalDisabled => GetOrCreate(c =>
                SetupApproval.IsRequestApproval
                ? c.FromBql<True.IsEqual<False>>()
                : c.FromBql<status.IsNotIn<State.pendingApproval, State.rejected>>()
                );
        }

        protected virtual void Configure(WorkflowContext<INRequestTransferEntry, INRequestTransfer> context)
        {
            var approvalCategory = context.Categories.Get(INRequestTransferWorkflow.CategoryID.Approval);
            var conditions = context.Conditions.GetPack<Conditions>();

            var approve = context.ActionDefinitions
                .CreateExisting<INRequestTransferApprovalWorkflow>(g => g.approve, a => a
                .InFolder(approvalCategory, g => g.ReleaseFromHold)
                .PlaceAfter(g => g.ReleaseFromHold)
                .IsHiddenWhen(conditions.IsApprovalDisabled)
                .WithFieldAssignments(fa => fa.Add<approved>(e => e.SetFromValue(true))));

            var reject = context.ActionDefinitions
                .CreateExisting<INRequestTransferApprovalWorkflow>(g => g.reject, a => a
                .InFolder(approvalCategory, approve)
                .PlaceAfter(approve)
                .IsHiddenWhen(conditions.IsApprovalDisabled)
                .WithFieldAssignments(fa => fa.Add<rejected>(e => e.SetFromValue(true))));

            Workflow.ConfiguratorFlow InjectApprovalWorkflow(Workflow.ConfiguratorFlow flow)
            {
                return flow
                    .WithFlowStates(states =>
                    {
                        states.Add<State.pendingApproval>(flowState =>
                        {
                            return flowState
                                .WithActions(actions =>
                                {
                                    actions.Add(approve, a => a.IsDuplicatedInToolbar());
                                    actions.Add(reject, a => a.IsDuplicatedInToolbar());
                                    actions.Add(g => g.PutOnHold);
                                })
                                .WithFieldStates(fields =>
                                {
                                    fields.AddAllFields<INRequestTransfer>(table => table.IsDisabled());
                                    fields.AddField<requestNbr>();
                                    fields.AddTable<INRequestTransferDetail>(table => table.IsDisabled());
                                    fields.AddField<INRequestTransfer.workgroupID>();
                                    fields.AddField<INRequestTransfer.ownerID>();
                                });
                        });
                        states.Add<State.rejected>(flowState =>
                        {
                            return flowState
                                .WithActions(actions =>
                                {
                                    actions.Add(g => g.PutOnHold, a => a.IsDuplicatedInToolbar());
                                })
                                .WithFieldStates(fields =>
                                {
                                    fields.AddAllFields<INRequestTransfer>(table => table.IsDisabled());
                                    fields.AddField<requestNbr>();
                                    fields.AddTable<INRequestTransferDetail>(table => table.IsDisabled());
                                });
                        });
                    })
                    .WithTransitions(transitions =>
                    {
                        transitions.UpdateGroupFrom(State.Initial, ts =>
                        {
                            ts.Add(t => t // New Pending Approval
                                .To<State.pendingApproval>()
                                .IsTriggeredOn(g => g.initializeState)
                                .When(conditions.IsNotOnHoldAndIsNotApproved)
                                );
                            ts.Update(t => t
                                .To<State.balanced>()
                                .IsTriggeredOn(g => g.initializeState), t => t
                                .When(conditions.IsNotOnHoldAndIsApproved)); // IsNotOnHold -> IsNotOnHoldAndIsApproved
                        });

                        transitions.UpdateGroupFrom<State.hold>(ts =>
                        {
                            ts.Update(
                                t => t
                                .To<State.balanced>()
                                .IsTriggeredOn(g => g.ReleaseFromHold), t => t
                                .When(conditions.IsApproved));
                            ts.Add(t => t
                                .To<State.pendingApproval>()
                                .IsTriggeredOn(g => g.ReleaseFromHold)
                                .DoesNotPersist()
                                .When(conditions.IsNotApproved));
                            ts.Add(t => t
                                .To<State.rejected>()
                                .IsTriggeredOn(g => g.ReleaseFromHold)
                                .DoesNotPersist()
                                .When(conditions.IsRejected));
                            /*ts.Add(t => t
                                .To<State.pendingApproval>()
                                .IsTriggeredOn(g => g.OnUpdateStatus)
                                .DoesNotPersist()
                                .When(conditions.IsNotApproved));
                            ts.Add(t => t
                                .To<State.rejected>()
                                .IsTriggeredOn(g => g.OnUpdateStatus)
                                .DoesNotPersist()
                                .When(conditions.IsRejected));*/
                        });


                        transitions.AddGroupFrom<State.pendingApproval>(ts =>
                        {
                            ts.Add(t => t
                                .To<State.balanced>()
                                .IsTriggeredOn(approve)
                                .When(conditions.IsNotApproved));
                            ts.Add(t => t
                                .To<State.rejected>()
                                .IsTriggeredOn(reject)
                                .When(conditions.IsRejected));
                            ts.Add(t => t
                                .To<State.hold>()
                                .IsTriggeredOn(g => g.PutOnHold)
                                .DoesNotPersist());
                        });

                        transitions.AddGroupFrom<State.rejected>(ts =>
                        {
                            ts.Add(t => t
                                .To<State.hold>()
                                .IsTriggeredOn(g => g.PutOnHold)
                                .DoesNotPersist());
                        });
                    });
            }

            context.UpdateScreenConfigurationFor(screen =>
            {
                return screen
                    .UpdateDefaultFlow(InjectApprovalWorkflow)
                    .WithActions(actions =>
                    {
                        actions.Add(approve);
                        actions.Add(reject);
                        actions.Update(
                            g => g.PutOnHold,
                            a => a.WithFieldAssignments(fas =>
                            {
                                fas.Add<approved>(f => f.SetFromValue(false));
                                fas.Add<rejected>(f => f.SetFromValue(false));
                            }));
                    });
            });
        }

        public PXAction<INRequestTransfer> approve;
        [PXButton(CommitChanges = true), PXUIField(DisplayName = "Approve", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable Approve(PXAdapter adapter) => adapter.Get();

        public PXAction<INRequestTransfer> reject;
        [PXButton(CommitChanges = true), PXUIField(DisplayName = "Reject", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable Reject(PXAdapter adapter) => adapter.Get();
    }
}