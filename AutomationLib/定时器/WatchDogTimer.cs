using System.Timers;

namespace AutomationLib.定时器;
public class WatchDogTimer : IDisposable
{
	public WatchDogTimer(double timeout)
	{
		_timer = new()
		{
			Interval = timeout,
			AutoReset = false,
		};
		_timer.Elapsed += OnTimeOut;
	}

	~WatchDogTimer()
	{
		Dispose(false);
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	private bool _disposed;
	protected virtual void Dispose(bool disposing)
	{
		if (_disposed)
		{
			return;
		}

		_disposed = true;

		if (disposing)
		{
			_timer.Dispose();
		}
	}

	/// <summary>
	/// 启动看门狗定时器
	/// </summary>
	public void Start()
	{
		_timer.Start();
	}

	/// <summary>
	/// 停止看门狗定时器。停止后可以调用 Start 方法重新启动
	/// </summary>
	public void Stop()
	{
		_timer.Stop();
	}

	/// <summary>
	/// 喂狗
	/// </summary>
	public void Reset()
	{
		// 将定时器停止后重新启动能够重置计数器
		_timer.Stop();
		_timer.Start();
	}

	private void OnTimeOut(object? source, ElapsedEventArgs e)
	{
		WatchDogTimeOut?.Invoke();
	}

	/// <summary>
	/// 看门狗定时器超时事件
	/// </summary>
	public event Action? WatchDogTimeOut;

	private System.Timers.Timer _timer;
}
