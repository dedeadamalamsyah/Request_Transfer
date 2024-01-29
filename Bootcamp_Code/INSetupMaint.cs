using PX.Data;

namespace Bootcamp
{
    // Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
    public class INSetupMaint_Extension : PXGraphExtension<PX.Objects.IN.INSetupMaint>
    {
        public PXSelect<INSetupApproval> SetupApproval;
    }
}