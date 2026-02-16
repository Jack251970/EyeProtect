using EyeProtect.Core.Service;
using U5BFA.Libraries;

namespace EyeProtect.Views
{
    public partial class MainTrayIconFlyout : TrayIconFlyout
    {
        private readonly MainService mainService;

        public MainTrayIconFlyout(MainService mainService) : base(new MainTrayIconFlyoutWindow())
        {
            this.mainService = mainService;
            InitializeComponent();
        }
    }
}
