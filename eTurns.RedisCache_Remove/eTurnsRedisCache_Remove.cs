using System;
using System.ServiceProcess;
using System.Timers;
namespace eTurns.RedisCache_Remove
{
    public partial class eTurnsRedisCache_Remove : ServiceBase
    {
        #region [Global Declaration]        

        Timer TimerRemoveCache;
        public bool IsExecutionRunning = false;
        #endregion

        #region [Class constructor]
        public eTurnsRedisCache_Remove()
        {
            InitializeComponent();
        }
        #endregion

        #region [Service OnstartStop]
        protected override void OnStart(string[] args)
        {
            try
            {
                //Debugger.Launch();
                IsExecutionRunning = false;
                CommonFunctions.SaveLogInTextFile("eTurns.RedisCache_Remove >> On Start: " + System.DateTime.UtcNow, GeneralHelper.DoSendLogsInMail);
                TimerRemoveCache = new Timer(GeneralHelper.TimerIntervalMSFTPDownloadFiles);
                TimerRemoveCache.Elapsed += TimerFTPDownloadFiles_Elapsed;
                TimerRemoveCache.AutoReset = true;

                TimerRemoveCache.Start();
                System.Threading.Thread.Sleep(10000);
            }
            catch (Exception ex)
            {
                CommonFunctions.SaveExceptionInTextFile("eTurns.RedisCache_Remove.OnStart >>", ex, GeneralHelper.DoSendLogsInMail);
            }
        }

        protected override void OnStop()
        {
            // TODO: Add code here to perform any tear-down necessary to stop your service.
            CommonFunctions.SaveLogInTextFile("eTurns.RedisCache_Remove.OnStop >>", GeneralHelper.DoSendLogsInMail);
        }
        #endregion

        #region [Timer Methods]
        private void TimerFTPDownloadFiles_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {

                CommonFunctions.SaveLogInTextFile("eTurns.RedisCache_Remove >> TimerFTPDownloadFiles_Elapsed => Started: " + System.DateTime.UtcNow, GeneralHelper.DoSendLogsInMail);
                TimerRemoveCache.Enabled = false;
                RedisCacheRemoveHelper objFtpServerHelper = new RedisCacheRemoveHelper();
                objFtpServerHelper.ProcessRemoveRedisCache();

                TimerRemoveCache.Enabled = true;
                CommonFunctions.SaveLogInTextFile("eTurns.RedisCache_Remove >> TimerFTPDownloadFiles_Elapsed => Completed: " + System.DateTime.UtcNow, GeneralHelper.DoSendLogsInMail);

            }
            catch (Exception ex)
            {
                TimerRemoveCache.Enabled = true;
                CommonFunctions.SaveExceptionInTextFile("TimerFTPDownloadFiles_Elapsed", ex, GeneralHelper.DoSendLogsInMail);
            }
            finally
            {
                if (!TimerRemoveCache.Enabled)
                {
                    TimerRemoveCache.Enabled = true;
                }
            }
        }
        #endregion

    }
}
