using JCNET;
using System.Diagnostics;
using System.Text;

namespace AutomationLib;

public class CMD : IAsyncDisposable
{
	public CMD()
	{
		InnerProcess = new Process();

		#region 进程配置
		InnerProcess.StartInfo.FileName = @"CMD.exe";
		InnerProcess.StartInfo.Arguments = "/q /k @echo off";
		// 直接从可执行文件启动它，不使用shell启动它
		InnerProcess.StartInfo.UseShellExecute = false;
		// 不创建窗口
		InnerProcess.StartInfo.CreateNoWindow = true;
		// 重定向输入输出
		InnerProcess.StartInfo.RedirectStandardInput = true;
		InnerProcess.StartInfo.RedirectStandardOutput = true;
		InnerProcess.StartInfo.RedirectStandardError = true;
		// 订阅事件
		InnerProcess.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
		{
			string data = e.Data ?? string.Empty;
			ReceiveData(data);
		};
		#endregion

		InnerProcess.Start();
		InnerProcess.BeginOutputReadLine();
	}

	private bool _disposed = false;
	public async ValueTask DisposeAsync()
	{
		if (_disposed)
		{
			return;
		}

		_disposed = true;
		GC.SuppressFinalize(this);

		InnerProcess.Close();
		await InnerProcess.WaitForExitAsync();
		InnerProcess.Dispose();
	}

	private Process InnerProcess { get; set; }
	private readonly Queue<Action<string>> _callbackQueue = new();

	private StringBuilder _receiveStringBuilder = new();
	private int _flag = 0;
	/// <summary>
	/// 在回调函数中接收 CMD 进程传来的数据
	/// </summary>
	/// <param name="data"></param>
	private void ReceiveData(string data)
	{
		#region 检测帧开始、结尾
		if (data.StartsWith('{'))
		{
			// 开始接收
			_receiveStringBuilder.Clear();
			_flag = 1;
		}
		else if (data.StartsWith('}'))
		{
			// 结束接收
			_flag = 2;
		}
		#endregion

		switch (_flag)
		{
		case 1:
			{
				_receiveStringBuilder.AppendLine(data);
				break;
			}
		case 2:
			{
				_receiveStringBuilder.AppendLine(data);
				string result = _receiveStringBuilder.ToString();

				// 接收完成
				result = result.SliceMaxBetween('{', '}') ?? string.Empty;
				result = result.Trim();
				DistributeData(result);
				_flag = 0;
				break;
			}
		}
	}

	/// <summary>
	/// 回调委托队列中的委托，进行数据分发
	/// </summary>
	/// <param name="receiveStr"></param>
	private void DistributeData(string receiveStr)
	{
		lock (_callbackQueue)
		{
			Action<string> action = _callbackQueue.Dequeue();
			action.Invoke(receiveStr);
		}
	}

	/// <summary>
	/// 用来防止 SendCommandAsync 被多个线程同时执行
	/// </summary>
	private readonly SemaphoreSlim _sendCmdTaskLock = new(1, 1);

	/// <summary>
	/// 向 CMD 进程发送命令。这个函数只能同时有一个线程在执行，避免两个线程同时向 CMD
	/// 进行写入导致内容交织在一起。所以这个函数内部加锁了
	/// </summary>
	/// <param name="cmd"></param>
	/// <param name="callback"></param>
	/// <returns></returns>
	private async ValueTask SendCommandAsync(string cmd, Action<string> callback)
	{
		// 加锁，防止多线程同时向CMD发送命令，会串在一起
		await _sendCmdTaskLock.WaitAsync();
		lock (_callbackQueue)
		{
			_callbackQueue.Enqueue(callback);
		}

		await InnerProcess.StandardInput.WriteLineAsync("echo {");
		await InnerProcess.StandardInput.WriteLineAsync(cmd);
		await InnerProcess.StandardInput.WriteLineAsync("echo }");
		await InnerProcess.StandardInput.FlushAsync();
		_sendCmdTaskLock.Release();
	}

	public async Task<string> RunCommandAsync(string cmd)
	{
		TaskCompletionSource tcs = new();
		string result = string.Empty;
		await SendCommandAsync(cmd, (str) =>
		{
			result = str;
			tcs.SetResult();
		});
		await tcs.Task;
		return result;
	}

	public async ValueTask<string[]> RunCommandAsync(string[] cmds)
	{
		string[] results = new string[cmds.Length];
		for (int i = 0; i < cmds.Length; i++)
		{
			results[i] = await RunCommandAsync(cmds[i]);
		}

		return results;
	}
}
