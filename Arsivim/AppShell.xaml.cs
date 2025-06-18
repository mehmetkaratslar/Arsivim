using Arsivim.Views;

namespace Arsivim;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		
		// Route registrations for programmatic navigation
		Routing.RegisterRoute("BelgeDetay", typeof(BelgeDetay));
	}
}
