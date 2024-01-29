using PX.Data;
using PX.Data.BQL;

namespace Bootcamp
{
    public class INRequestTransferStatus
    {
        public class ListAttribute : PXStringListAttribute
        {
            public ListAttribute()
                : base(PXStringListAttribute.Pair("HO", "Hold"), PXStringListAttribute.Pair("PA", "Pending Approval"), PXStringListAttribute.Pair("RE", "Rejected"), PXStringListAttribute.Pair("OP", "Open"), PXStringListAttribute.Pair("BA", "Balanced"))
            {
            }
        }

        public class hold : BqlType<IBqlString, string>.Constant<hold>
        {
            public hold()
                : base("HO")
            {
            }
        }

        public class pendingApproval : BqlType<IBqlString, string>.Constant<pendingApproval>
        {
            public pendingApproval()
                : base("PA")
            {
            }
        }

        public class rejected : BqlType<IBqlString, string>.Constant<rejected>
        {
            public rejected()
                : base("RE")
            {
            }
        }

        public class open : BqlType<IBqlString, string>.Constant<open>
        {
            public open()
                : base("OP")
            {
            }
        }

        public class balanced : BqlType<IBqlString, string>.Constant<balanced>
        {
            public balanced()
                : base("BA")
            {
            }
        }

        public const string Initial = "__";

        public const string Hold = "HO";

        public const string PendingApproval = "PA";

        public const string Rejected = "RE";

        public const string Open = "OP";

        public const string Balanced = "BA";
    }
}
