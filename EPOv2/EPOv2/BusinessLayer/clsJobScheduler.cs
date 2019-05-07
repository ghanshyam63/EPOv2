using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Quartz;
using Quartz.Impl;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace EPOv2.BusinessLayer
{
    public class ClsJobScheduler
    {
        public static async Task Start()
        {
            NameValueCollection props = new NameValueCollection
            {
                {"quartz.serializer.type", "binary"}
            };
            StdSchedulerFactory factory = new StdSchedulerFactory(props);
            IJobDetail job = JobBuilder.Create<ClsSubstitutionCorrection>()
                .WithIdentity("job1")
                .Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithDailyTimeIntervalSchedule
                (s =>
                    s.WithIntervalInSeconds(30)
                        .OnEveryDay()
                )
                .ForJob(job)
                .WithIdentity("trigger1")
                .StartNow()
                .WithCronSchedule("0 00 10 ? * MON-SUN")
                .Build();

            StdSchedulerFactory sf = new StdSchedulerFactory(props);
            IScheduler sc =await sf.GetScheduler();
            await sc.ScheduleJob(job, trigger);
            await sc.Start();
            await EpoReportRunnerScheduler("OutstandingInvoice", "0 00 05 ? * MON-FRI");
            await EpoReportRunnerScheduler("DeclinedVouchers", "0 30 04 ? * MON-SUN");
            //await EpoReportRunnerScheduler("UsersUpdate", "0 56 08 ? * MON-SUN");
            await EpoReportRunnerScheduler("UsersUpdate", "0 0 0/1 1/1 * ? *");
            await EpoReportRunnerScheduler("CCOwnerChecker", "0 20 09 ? * MON-FRI");
        }
        public static async Task EpoReportRunnerScheduler(String TaskToRun,string cronExpression)
        {
            // construct a scheduler factory
            NameValueCollection props = new NameValueCollection
            {
                {"quartz.serializer.type", "binary"}
            };
            StdSchedulerFactory factory = new StdSchedulerFactory(props);

            // get a scheduler
            IScheduler sched = await factory.GetScheduler();
            await sched.Start();


            // define the job and tie it to our  Job class
            IJobDetail job = JobBuilder.Create<EPOReportRunnerJob>()
                .WithIdentity($"RefreshJob{TaskToRun}")
                .UsingJobData("Parameter", TaskToRun)
               .Build();

           
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity($"{TaskToRun}TRIGGER")
               .ForJob(job)
                .StartNow()
                    .WithCronSchedule(cronExpression)
                    .Build();

            await sched.ScheduleJob(job, trigger);
        }

    }
}