namespace AutomationLib;

/// <summary>
/// 将临界区的任务包在 AwaitForStart 方法和 Done 方法之间
/// </summary>
public class TaskLock
{
	private TaskCompletionSource? _tcs = null;
	private readonly object _tcsLock = new();

	/// <summary>
	/// 等待上一个任务的完成，等待完成后会返回，然后就可以执行临界区的任务了。
	/// 执行完成了别忘记调用 Done，否则下次再调用这个函数会永远卡住
	/// </summary>
	/// <returns></returns>
	public async ValueTask AwaitForStart()
	{
		while (true)
		{
			// 如果上一个任务没完成，先等待
			if (_tcs != null)
			{
				await _tcs.Task;
			}

			lock (_tcsLock)
			{
				if (_tcs == null || _tcs.Task.IsCompleted)
				{
					// 进入这里说明获得执行权力了
					// 创建一个 TaskCompletionSource 然后退出循环
					_tcs = new TaskCompletionSource();
					break;
				}

				// 如果没有退出循环，说明没有获得执行权力，继续下一个循环，在下一个循环中继续等待
			}
		}
	}

	/// <summary>
	/// 完成临界区中的任务
	/// </summary>
	public void Done()
	{
		lock (_tcsLock)
		{
			_tcs?.TrySetResult();
		}
	}
}
