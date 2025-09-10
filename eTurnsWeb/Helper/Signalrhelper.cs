namespace eTurnsWeb.Helper
{
    //public class BackgroundServerTimeTimer : IRegisteredObject
    //{
    //    private Timer taskTimer;
    //    private IHubContext hub;

    //    public BackgroundServerTimeTimer()
    //    {
    //        HostingEnvironment.RegisterObject(this);
    //        hub = GlobalHost.ConnectionManager.GetHubContext<eTurnsHub>();
    //        taskTimer = new Timer(OnTimerElapsed, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10));
    //    }

    //    private void OnTimerElapsed(object sender)
    //    {
    //        hub.Clients.All.UpdateRedCircleCountInClients();
    //        //HttpContext.Current.Session["RefressReplenishMenuCounts"] = false;
    //    }

    //    public void Stop(bool immediate)
    //    {
    //        taskTimer.Dispose();
    //        HostingEnvironment.UnregisterObject(this);
    //    }
    //}
}