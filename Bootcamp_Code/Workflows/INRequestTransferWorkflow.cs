using PX.Data;
using PX.Data.WorkflowAPI;


namespace Bootcamp
{
    using static BoundedTo<INRequestTransferEntry, INRequestTransfer>;
    using static INRequestTransfer;
    using State = INRequestTransferStatus;

    // Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
    public class INRequestTransferWorkflow : PXGraphExtension<INRequestTransferEntry>
    {
        public static class CategoryNames
        {
            public const string Processing = "Processing";

            public const string Approval = "Approval";
            public const string View = "View";
        }

        public static class CategoryID
        {
            public const string Processing = "ProcessingID";

            public const string Approval = "ApprovalID";
            public const string View = "View";
        }

        /*#region Constants
        public static class States
        {
            public const string Hold = RequestTransfersStatusConstants.Hold;
            public const string Balanced = RequestTransfersStatusConstants.Balanced;
            public const string Open = RequestTransfersStatusConstants.Open;
            public const string PendingApproval = RequestTransfersStatusConstants.PendingApproval;
            public const string Rejected = RequestTransfersStatusConstants.Rejected;

            public class hold : PX.Data.BQL.BqlString.Constant<hold>
            {
                public hold() : base(Hold) { }
            }

            public class balanced : PX.Data.BQL.BqlString.Constant<balanced>
            {
                public balanced() : base(Balanced) { }
            }

            public class open : PX.Data.BQL.BqlString.Constant<open>
            {
                public open() : base(Open) { }
            }

            public class pendingApproval : PX.Data.BQL.BqlString.Constant<pendingApproval>
            {
                public pendingApproval() : base(PendingApproval) { }
            }

            public class rejected : PX.Data.BQL.BqlString.Constant<rejected>
            {
                public rejected() : base(Rejected) { }
            }
        }
        #endregion*/

        public class Conditions : Condition.Pack
        {
            public Condition IsOnHold => GetOrCreate(c => c.FromBql<
                    hold.IsEqual<True>
                >());

            public Condition IsBalanced => GetOrCreate(c => c.FromBql<
                approved.IsEqual<True>.And<hold.IsEqual<False>>
            >());

            public Condition DisableCreateTransfer => GetOrCreate(b => b.FromBql<
                Where<transferNbr.IsNotNull>>());
        }

        public sealed override void Configure(PXScreenConfiguration config)
        {
            Configure(config.GetScreenConfigurationContext<INRequestTransferEntry, INRequestTransfer>());
        }

        protected static void Configure(WorkflowContext<INRequestTransferEntry, INRequestTransfer> context)
        {
            var conditions = context.Conditions.GetPack<Conditions>();

            #region Categories
            /* var commonCategories = CommonActionCategories.Get(context);
             var processingCategory = commonCategories.Processing;*/

            var processingCategory = context.Categories.CreateNew(CategoryID.Processing,
                category => category.DisplayName(CategoryNames.Processing));

            var approvalCategory = context.Categories.CreateNew(CategoryID.Approval,
                category => category.DisplayName(CategoryNames.Approval));

            var viewCategory = context.Categories.CreateNew(CategoryID.View,
               category => category.DisplayName(CategoryNames.View));
            #endregion

            context.AddScreenConfigurationFor(screen => screen
            .StateIdentifierIs<INRequestTransfer.status>()
            .AddDefaultFlow(flow => flow
                .WithFlowStates(fss =>
                {
                    fss.Add(State.Initial, state => state.IsInitial(g => g.initializeState));
                    fss.Add<State.hold>(flowState =>
                    {
                        return flowState
                        .WithActions(actions =>
                        {
                            actions.Add(g => g.ReleaseFromHold, a => a
                            .IsDuplicatedInToolbar()
                            .WithConnotation(ActionConnotation.Success));
                        })
                        .WithFieldStates(fields =>
                        {
                            fields.AddAllFields<INRequestTransfer>();
                            fields.AddTable<INRequestTransferDetail>();
                            fields.AddField<INRequestTransfer.approved>(c => c.IsDisabled());
                            fields.AddField<INRequestTransfer.ownerID>();
                            fields.AddField<INRequestTransfer.workgroupID>(c => c.IsDisabled());
                        });
                    });

                    fss.Add<State.balanced>(flowState =>
                    {
                        return flowState
                        .WithFieldStates(states =>
                        {
                            states.AddField<INRequestTransfer.siteID>(state
                                => state.IsDisabled());
                            states.AddField<INRequestTransfer.toSiteID>(state
                                => state.IsDisabled());

                            states.AddTable<INRequestTransfer>(c => c.IsDisabled());
                            states.AddField<INRequestTransfer.transferNbr>();

                            states.AddTable<INRequestTransferDetail>(c => c.IsDisabled());

                        })
                        .WithActions(actions =>
                        {
                            actions.Add(g => g.PutOnHold, a =>
                            a.IsDuplicatedInToolbar());

                            actions.Add(g => g.Open, a => a
                             .IsDuplicatedInToolbar()
                             .WithConnotation(ActionConnotation.Success));
                        });
                    });

                    fss.Add<State.open>(flowState =>
                    {
                        return flowState
                        .WithFieldStates(states =>
                        {
                            states.AddField<INRequestTransfer.siteID>(state
                                => state.IsDisabled());
                            states.AddField<INRequestTransfer.toSiteID>(state
                            => state.IsDisabled());

                            states.AddTable<INRequestTransfer>(c => c.IsDisabled());
                            states.AddField<INRequestTransfer.transferNbr>();

                            states.AddTable<INRequestTransferDetail>(c => c.IsDisabled());
                        })
                        .WithActions(actions =>
                        {
                            actions.Add(g => g.CreateTransferAction, a => a
                            .IsDuplicatedInToolbar()
                            .WithConnotation(ActionConnotation.Success));
                        });

                    });
                })
                .WithTransitions(transitions =>
                {
                    transitions.AddGroupFrom(State.Initial, ts =>
                    {
                        ts.Add(t => t
                            .To<State.hold>()
                            .IsTriggeredOn(g => g.initializeState)
                            .When(conditions.IsOnHold));

                        ts.Add(t => t.To<State.open>()
                            .IsTriggeredOn(g => g.initializeState));
                    });

                    transitions.AddGroupFrom<State.hold>(ts =>
                    {
                        ts.Add(t => t.To<State.balanced>()
                        .IsTriggeredOn(g => g.ReleaseFromHold));
                        /*.WithFieldAssignments(fas => fas.Add<hold>(f => f.SetFromValue(false))));*/
                    });

                    transitions.AddGroupFrom<State.balanced>(ts =>
                    {
                        ts.Add(t => t.To<State.hold>().IsTriggeredOn(g => g.PutOnHold));
                        /*.WithFieldAssignments(fas => fas.Add<hold>(f => f.SetFromValue(true))));*/

                        ts.Add(t => t.To<State.open>().IsTriggeredOn(g => g.Open));
                    });
                }))
            .WithCategories(categories =>
            {
                categories.Add(processingCategory);
                categories.Add(approvalCategory);
                categories.Add(viewCategory);
            })
            .WithActions(actions =>
            {
                actions.Add(g => g.initializeState, a => a.IsHiddenAlways());

                actions.Add(g => g.ReleaseFromHold, c => c
                 .WithCategory(processingCategory));
                /*.WithPersistOptions(ActionPersistOptions.NoPersist)
                .WithFieldAssignments(fas => fas.Add<hold>(f => f.SetFromValue(false))));*/

                actions.Add(g => g.PutOnHold, c => c
                 .WithCategory(processingCategory));
                /*.WithPersistOptions(ActionPersistOptions.NoPersist)
                .WithFieldAssignments(fas => fas.Add<hold>(f => f.SetFromValue(true))));*/

                actions.Add(g => g.Open, c => c
                 .WithCategory(processingCategory, Placement.Last));

                actions.Add(g => g.CreateTransferAction, c => c
                 .WithCategory(processingCategory)
                 .IsDisabledWhen(conditions.DisableCreateTransfer));
            })
            );
        }
    }
}