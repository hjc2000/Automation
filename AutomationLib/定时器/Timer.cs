namespace AutomationLib.定时器;
public class Timer
{
	/// <summary>
	/// 以一定的间隔执行一个任务一段时间
	/// </summary>
	/// <param name="action"></param>
	/// <param name="duration_in_milliseconds">持续时间</param>
	/// <param name="interval_in_milliseconds">间隔</param>
	public static void ExecuteForAPeriodOfTime(Action action, int duration_in_milliseconds, int interval_in_milliseconds)
	{
		System.Timers.Timer timer = new()
		{
			Interval = interval_in_milliseconds,
			AutoReset = true,
		};

		DateTime startTime = DateTime.Now;

		timer.Elapsed += (source, e) =>
		{
			try
			{
				if ((e.SignalTime - startTime).TotalMilliseconds >= duration_in_milliseconds)
				{
					timer.Stop();
					timer.Dispose();
				}
				else
				{
					action.Invoke();
				}
			}
			catch
			{
				timer.Stop();
				timer.Dispose();
			}
		};
		timer.Start();
	}

	public static void SetTimeOut(Action action, int timeout_milliseconds)
	{
		System.Timers.Timer timer = new()
		{
			Interval = timeout_milliseconds,
			AutoReset = false,
		};

		timer.Elapsed += (source, e) =>
		{
			action.Invoke();
			timer.Dispose();
		};

		timer.Start();
	}

	/// <summary>
	/// 以一定的时间间隔回调一个函数，直到取消为止
	/// </summary>
	/// <param name="action">时间到达后的回调函数</param>
	/// <param name="interval_in_milliseconds">定时间隔，单位 ms</param>
	/// <param name="token">取消定时任务</param>
	public static void SetInterval(Action action, int interval_in_milliseconds, CancellationToken token)
	{
		System.Timers.Timer timer = new()
		{
			Interval = interval_in_milliseconds,
			AutoReset = true,
		};

		timer.Elapsed += (source, e) =>
		{
			try
			{
				if (token.IsCancellationRequested)
				{
					timer.Stop();
					timer.Dispose();
				}
				else
				{
					action.Invoke();
				}
			}
			catch
			{
				timer.Stop();
				timer.Dispose();
			}
		};

		timer.Start();
	}
}
