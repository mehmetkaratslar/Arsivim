using Arsivim.Views;

namespace Arsivim;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		
		// Route registrations for programmatic navigation
		Routing.RegisterRoute("BelgeDetay", typeof(BelgeDetay));
		Routing.RegisterRoute("BelgeEkle", typeof(BelgeEkle));
		Routing.RegisterRoute("BelgeDuzenle", typeof(BelgeEkle));
		Routing.RegisterRoute("KisiEkle", typeof(KisiEkle));
		Routing.RegisterRoute("KisiDetay", typeof(KisiEkle));
	}
}
