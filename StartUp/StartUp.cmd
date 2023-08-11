rem 这个文件是被 StartUp.exe 读取的。会读取里面的每一行命令，调用 cmd 执行。

rem 如果要启动的程序是控制台程序，必须使用 start 命令启动另一个 shell 去运行这个程序，因为
rem 我在将 CMD 作为子进程的时候选择非 shell 方式启动，因为要重定向输入输出。这种方式下，CMD
rem 并不像在 shell 中使用那样，可以通过命令直接在 shell 中执行一个控制台程序。

rem 如果是图形界面程序，像下面启动的 lghub，可以直接用字符串启动。因为图形界面程序不会试图
rem 在 CMD 子进程中运行。执行下面的命令后，lghub 与 CMD 没有任何关联，CMD 可以继续执行其它
rem 命令。
"C:\Program Files\LGHUB\lghub.exe"
